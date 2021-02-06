using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Rozklad.V2.DataAccess;
using Rozklad.V2.Google;
using Rozklad.V2.Helpers;
using Rozklad.V2.Scheduler;
using Rozklad.V2.Services;
using Rozklad.V2.Telegram;
using Rozklad.V2.Telegram.Commands;

namespace Rozklad.V2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            services.AddEntityFrameworkNpgsql();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("Connection"));
            });

            services.AddScoped<IUserSerice, UserSerivce>();
            services.AddControllersWithViews();
            var appSettings = appSettingsSection.Get<AppSettings>();
            services.AddSingleton<AppSettings>(_ => appSettings);
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserSerice>();
                            var userId = Guid.Parse(context.Principal.Identity.Name ?? string.Empty);
                            var user = userService.GetById(userId);
                            if (user == null)
                            {
                                // return unauthorized if user no longer exists
                                context.Fail("Unauthorized");
                            }

                            return Task.CompletedTask;
                        }
                    };
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            services.AddControllersWithViews().AddNewtonsoftJson();
            // services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IRozkladRepository, RozkladRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            // services.AddScoped<IJobManager, JobManager>();
            services.AddSingleton<TelegramValidationService>(s => new TelegramValidationService(appSettings.BotToken));
            // In production, the React files will be served from this director
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Rozklad",
                    Version = "v1",
                    Description = "Simple API"
                });
            });

            // HANGFIRE 
            // Add Hangfire services.
            // services.AddHangfire(configuration => configuration
            //     .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            //     .UseFilter(new AutomaticRetryAttribute
            //         {Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete})
            //     .UseSimpleAssemblyNameTypeSerializer()
            //     .UseRecommendedSerializerSettings()
            //     //.UsePostgreSqlStorage(Configuration.GetConnectionString("HangfireConnection")
            //     .UseMemoryStorage()
            // );

            // Add the processing server as IHostedService
            // services.AddHangfireServer();

            // Add framework services.
            services.AddMvc()
                .AddNewtonsoftJson(s => s.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
            // services.AddScoped<ISchedulerService, SchedulerService>();
            // services.AddScoped<INotificationJob, NotificationJob>();
            // services.AddScoped<ITelegramNotificationService, TelegramNotificationService>();
            // Telegram Bot start 
            services.AddScoped<StartCommand>();
            services.AddScoped<DisableNotificationsCommand>();
            services.AddScoped<EnableNotificationsCommand>();
            services.AddScoped<ICommandFactory, CommandFactory>();
            // Google setup 
            services.AddScoped<IGoogleCalendarService>(_ => new GoogleCalendarService(appSettings.GoogleApiKey));
            services.AddScoped<GoogleTokenService>(_ => new GoogleTokenService(
            new GoogleCredentials(
                Configuration["GoogleClientId"], 
                Configuration["GoogleClientSecret"])));
            // services.AddScoped<IGoogleCalendarService, GoogleCalendarService>();
            Bot.GetBotClientAsync(appSettings).Wait();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            }

            // it needs for create bot service and setWebhook
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                // endpoints.MapHangfireDashboard();
            });
            app.UseSwagger()
                .UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });
            // app.UseHangfireDashboard();
            // app.UseSpa(spa =>
            // {
            //     spa.Options.SourcePath = "ClientApp";
            //
            //     if (env.IsDevelopment())
            //     {
            //         spa.UseReactDevelopmentServer(npmScript: "start");
            //     }
            // });

            // schedulerService.InitialalizeJobs();
        }
    }
}
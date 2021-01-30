using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Rozklad.V2.Entities;
using Rozklad.V2.Helpers;
using Serilog;
using Serilog.Core;


namespace Rozklad.V2.Google
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        private const string credStr = "token.json";
        private UserCredential _credential;
        private readonly CalendarService _calendarService;

        private string[] Scopes =
        {
            CalendarService.Scope.CalendarEvents
        };

        private readonly string _applicationName = "Rozklad";
        
        public GoogleCalendarService(ClientSecrets secrets)
        {
            _credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credStr, true)
            ).Result;
            _calendarService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer= _credential,
                ApplicationName = this._applicationName
            });
            
            Log.Information("Google setup done!");
        }

        public async Task SetNotificationsInCalendar(CalendarSettings settings)
        {
            // todo for all lessons from settings calculate firetime
            // todo for all firetimes set event in calendar 
                // for firetime in firetimes 
                //  todo create method to set events on certain weeks 
                throw new NotImplementedException();
        }

    }
}
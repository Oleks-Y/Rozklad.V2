using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Rozklad.V2.Helpers;
using Rozklad.V2.Telegram.Commands;
using Telegram.Bot;

namespace Rozklad.V2.Telegram
{
    
    // public class Bot
    // {
    //     private TelegramBotClient _botClient;
    //     private List<Command> _commands;
    //
    //     private readonly AppSettings _settings;
    //     public IEnumerable<Command> Commands => _commands.AsReadOnly();
    //
    //     public TelegramBotClient BotClient => _botClient;
    //     
    //     public Bot(AppSettings appSettings)
    //     {
    //         _settings = appSettings;
    //         GetBotClient().Wait();
    //         _commands = new List<Command> {new StartCommand()};
    //         
    //     }
    //
    //     private async Task GetBotClient()
    //     {
    //         _botClient = new TelegramBotClient(_settings.BotToken);
    //         var hook = string.Format(_settings.Url, "api/message/update");
    //         await _botClient.SetWebhookAsync(hook);
    //         // var client = new HttpClient();
    //         // var requestData = JsonConvert.SerializeObject(new {url = hook});
    //         // var responseMessage = await client.PostAsync($"https://api.telegram.org/bot{_settings.BotToken}/",
    //         //     new StringContent(requestData, Encoding.UTF8, "application/json"));
    //         
    //     }
        
    
    public class Bot
    {
        private static TelegramBotClient _botClient;
        // private static List<Command> _commandsList;
        public static TelegramBotClient BotClient => _botClient;
        // public static IReadOnlyList<Command> Commands => _commandsList.AsReadOnly();
        
        // Todo add command to disable/enable notifications 

        public static TelegramBotClient GetBotClient() => _botClient;
        public static async Task<TelegramBotClient> GetBotClientAsync(AppSettings appSettings)
        {
            if (_botClient != null)
            {
                return _botClient;
            }
    

            _botClient = new TelegramBotClient(appSettings.BotToken);
            await _botClient.SetWebhookAsync(appSettings.Url+"/api/message/update");
            return _botClient;
        }
    }

    
}
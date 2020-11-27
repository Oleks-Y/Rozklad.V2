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
    
    public class Bot
    {
        private static TelegramBotClient _botClient;
        // private static List<Command> _commandsList;
        public static TelegramBotClient BotClient => _botClient;
        // public static IReadOnlyList<Command> Commands => _commandsList.AsReadOnly();
        

        public static TelegramBotClient GetBotClient() => _botClient;
        public static async Task<TelegramBotClient> GetBotClientAsync(AppSettings appSettings)
        {
            if (_botClient != null)
            {
                return _botClient;
            }
    
            // todo БОТ : додати команду для зміни часу сповіщення  
            _botClient = new TelegramBotClient(appSettings.BotToken);
            await _botClient.SetWebhookAsync(appSettings.Url+"/api/message/update");
            return _botClient;
        }
    }

    
}
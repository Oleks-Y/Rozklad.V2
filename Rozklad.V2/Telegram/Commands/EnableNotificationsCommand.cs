using System.Threading.Tasks;
using Rozklad.V2.Entities;
using Rozklad.V2.Scheduler;
using Rozklad.V2.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Rozklad.V2.Telegram.Commands
{
    public class EnableNotificationsCommand : ICommand
    {
        private readonly IRozkladRepository _repository;
        private readonly INotificationRepository _notificationRepositor;

        public EnableNotificationsCommand(IRozkladRepository repository, INotificationRepository notificationRepositor)
        {
            _repository = repository;
            _notificationRepositor = notificationRepositor;
        }
        
        public static string Name => @"/enable";
        public  async Task Execute(Message message, TelegramBotClient client)
        {
            var student = await _repository.GetUserByTelegramId(message.Chat.Id);
            if (student == null)
            {
                await Bot.BotClient.SendTextMessageAsync(message.Chat.Id, "Тут текст з проханням зарєєструватись на сайті");
            }

            // await _notificationRepositor.EnableNotifications(student.Id);
            await Bot.BotClient.SendTextMessageAsync(message.Chat.Id, "Сповіщення ввімкнено!");
        }

        public  bool Contains(Message message)
        {
            return message.Type == MessageType.Text 
                   && message.Text.Contains(Name);
        }
    }
}
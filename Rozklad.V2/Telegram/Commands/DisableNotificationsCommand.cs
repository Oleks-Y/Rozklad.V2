using System;
using System.Threading.Tasks;
using Rozklad.V2.Entities;
using Rozklad.V2.Scheduler;
using Rozklad.V2.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Rozklad.V2.Telegram.Commands
{
    public class DisableNotificationsCommand : ICommand
    {
        private readonly IRozkladRepository _repository;
        private readonly INotificationRepository _notificationRepository;
        public DisableNotificationsCommand(IRozkladRepository repository, INotificationRepository notificationRepository)
        {
            _repository = repository;
            _notificationRepository = notificationRepository;
        }
        public static string Name => @"/disable";
        public  async Task Execute(Message message, TelegramBotClient client)
        {
            
            var student = await _repository.GetUserByTelegramId(message.Chat.Id);
            if (student == null)
            {
                await Bot.BotClient.SendTextMessageAsync(message.Chat.Id, "Тут текст з проханням зарєєструватись на сайті");
            }

            // await _notificationRepository.DisableNotifications(student.Id);
            await Bot.BotClient.SendTextMessageAsync(message.Chat.Id, "Сповіщення вимкнено!");
        }

        public  bool Contains(Message message)
        {
            return message.Type == MessageType.Text 
                   && message.Text.Contains(Name);
        }
    }
}
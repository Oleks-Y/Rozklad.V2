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
        private readonly JobsManager _jobsManager;

        public EnableNotificationsCommand(IRozkladRepository repository, JobsManager jobsManager)
        {
            _repository = repository;
            _jobsManager = jobsManager;
        }
        
        public static string Name => @"/enable";
        public  async Task Execute(Message message, TelegramBotClient client)
        {
            // todo перевірка наявності сповіщень 
            var student = await _repository.GetUserByTelegramId(message.From.Id);
            if (student == null)
            {
                // todo Bot : винести текст команд в статичний класс 
                await Bot.BotClient.SendTextMessageAsync(message.Chat.Id, "Щоб користуватись сповіщеннями, їх потрібно увімкнути на сайті <domain> ");
            }
            var notificationEntity = new NotificationsSettings
            {
                StudentId = student.Id,
                IsNotificationsOn = true
            };
            await _repository.UpdateNotification(notificationEntity);
            await _repository.SaveAsync();
            await _jobsManager.RefreshJobs();
            await Bot.BotClient.SendTextMessageAsync(message.Chat.Id, "Сповіщення увімкнено !");
        }

        public  bool Contains(Message message)
        {
            return message.Type == MessageType.Text 
                   && message.Text.Contains(Name);
        }
    }
}
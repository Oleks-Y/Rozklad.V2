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
        private readonly JobsManager _jobsManager;

        public DisableNotificationsCommand(IRozkladRepository repository, JobsManager jobsManager)
        {
            _repository = repository;
            _jobsManager = jobsManager;
        }
        public static string Name => @"/disable";
        public  async Task Execute(Message message, TelegramBotClient client)
        {
            // todo перевірка наяності свовіщень 
            Console.WriteLine("We are here !");
            var student = await _repository.GetUserByTelegramId(message.From.Id);
            var notificationEntity = new NotificationsSettings
            {
                StudentId = student.Id,
                IsNotificationsOn = false
            };
            await _repository.UpdateNotification(notificationEntity);
            await _repository.SaveAsync();
            await _jobsManager.RefreshJobs();
            await Bot.BotClient.SendTextMessageAsync(message.Chat.Id, "Сповіщення вимкнено !");
        }

        public  bool Contains(Message message)
        {
            return message.Type == MessageType.Text 
                   && message.Text.Contains(Name);
        }
    }
}
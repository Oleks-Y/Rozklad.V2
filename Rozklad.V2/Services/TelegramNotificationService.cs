using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Rozklad.V2.Entities;
using Rozklad.V2.Telegram.Actions;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Rozklad.V2.Services
{
    public class TelegramNotificationService : ITelegramNotificationService
    {
        private readonly IRozkladRepository _repository;

        private readonly ILogger<TelegramNotificationService> _logger;
        public TelegramNotificationService(IRozkladRepository repository, ILogger<TelegramNotificationService> logger)
        {
            _repository = repository;
            _logger = logger;
        }


        public async Task SendNotifications(IEnumerable<Notification> notifications)
        {
            // Todo check is correct this logic 
            
            
            
            // check if chat id exists 
            // get all students 
            var telegramData = (await _repository.GetUserTelegramData(notifications.Select(s => s.StudentId))).ToArray();
            if (telegramData.Length==0)
            {
                // Todo add user to telegram data table when adds a user 
                // No student data is in table 
                return;
            }
            foreach(var notification in notifications.AsParallel().ToArray())
            {
                var studentId = notification.StudentId;
                var chatId = telegramData.FirstOrDefault(s => s.StudentId == studentId).TelegramChatId;
                if (chatId == null)
                {
                    // chat Id not exists
                    _logger.LogWarning($"User {studentId} not have chatId");
                }
                await NotificationsAction.Send(notification, chatId.Value);
            }
        }
    }
}
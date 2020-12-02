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


        public void SendNotifications(IEnumerable<Notification> notifications)
        {
            // check if chat id exists 
            // get all students 
            
            var studentIds = notifications.Select(s => s.StudentId).ToList();
            var telegramData = _repository.GetUserTelegramData(studentIds).ToList();
            if (telegramData.Count==0)
            {
                // No students data is in table 
                return;
            }
            foreach(var notification in notifications)
            {
                var studentId = notification.StudentId;
                var first = telegramData.FirstOrDefault(s => s.StudentId == studentId);
                if (first == null)
                {
                    // it happens, when can`t find user data 
                    _logger.LogError($"{notification.StudentId} don`t have telegram info, but uses telegram notification");
                    continue;
                }

                var chatId = first.TelegramChatId;
                if (chatId == null)
                {
                    // chat Id not exists
                    _logger.LogWarning($"User {studentId} not have chatId");
                }
                NotificationsAction.Send(notification, chatId.Value);
            }
        }
    }
}
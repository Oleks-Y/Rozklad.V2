using System;
using System.Threading.Tasks;
using Rozklad.V2.Entities;

namespace Rozklad.V2.Telegram.Actions
{
    public class NotificationsAction
    {
        public static void Send(Notification notification, long chat_id)
        {
            var lessonName = notification.Lesson.Subject.Name;
            var link = "";
            var accesCode = "";
            switch (notification.Lesson.Type)
            {
                case "Лек":
                    link = notification.Lesson.Subject.LessonsZoom;
                    accesCode = notification.Lesson.Subject.LessonsAccessCode;
                    break;
                default:
                    link = notification.Lesson.Subject.LabsZoom;
                    accesCode = notification.Lesson.Subject.LabsAccessCode;
                    break;
            }

            var message = "";
            message = accesCode != string.Empty ? 
                $"Скоро пара ! {lessonName}\n Посилання: {link} \n Код доступу {accesCode}" 
                : $"Скоро пара ! {lessonName}\n Посилання: {link} ";
            Bot.BotClient.SendTextMessageAsync(chat_id, message).Wait();
        }
    }
}
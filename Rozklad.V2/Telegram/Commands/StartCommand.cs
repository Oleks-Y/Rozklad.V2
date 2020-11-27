using System;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Rozklad.V2.Entities;
using Rozklad.V2.Services;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

namespace Rozklad.V2.Telegram.Commands
{
    public class StartCommand : ICommand
    {
        private readonly IRozkladRepository _repository;

        public StartCommand(IRozkladRepository repository)
        {
            _repository = repository;
        }

        public static string Name => @"/start";
        public  async Task Execute(Message message, TelegramBotClient client)
        {
            // if not exist 
            // Add student chat id
            var isExist = await _repository.UserDataExistsAsync(message.From.Id);
            if (!isExist)
            {
                // user not in database 
                // check if it student 
                var student = await _repository.GetUserByTelegramId(message.Chat.Id);
                if (student==null)
                {
                    // якщо користувач авторизований не через телеграм, то він отримає це повідомлення 
                    // student not in students
                    await client.SendTextMessageAsync(message.Chat.Id, "Привіт! Цей бот буде надсилати тобі сповіщення про пари з сайту <domain>. Спочатку тобі потрібно зарєструватись тут з допомогою телеграму", parseMode: ParseMode.Markdown);
                }
                await _repository.AddUserTelegramChatInfoAsync(new TelegramData
                {
                    Id = Guid.NewGuid(),
                    StudentId = student.Id,
                    TelegramId = student.Telegram_Id,
                    TelegramChatId = message.Chat.Id
                });
            }
            var chatId = message.Chat.Id;
            await client.SendTextMessageAsync(chatId, "Привіт! Цей бот буде надсилати тобі сповіщення про пари з сайту <domain>", parseMode: ParseMode.Markdown);
        }

        public  bool Contains(Message message)
        {
            return message.Type == MessageType.Text 
                   && message.Text.Contains(Name);
        }
    }
}
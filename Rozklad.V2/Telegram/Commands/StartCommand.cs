﻿using System;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Rozklad.V2.Entities;
using Rozklad.V2.Services;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

namespace Rozklad.V2.Telegram.Commands
{
    public class StartCommand : Command
    {
        public override string Name => @"/start";
        public override async Task Execute(Message message, TelegramBotClient client, IRozkladRepository repository)
        {
            // if not exist 
            // Add student chat id
            var isExist = await repository.UserDataExistsAsync(message.From.Id);
            if (!isExist)
            {
                // user not in database 
                // check if it student 
                var student = await repository.GetUserByTelegramId(message.Chat.Id);
                if (student==null)
                {
                    // Todo debug database errors 
                    // Todo debug bot functionality 
                    // Todo add telegram notifications 
                    // student not in students
                    await client.SendTextMessageAsync(message.Chat.Id, "Привіт! Цей бот буде надсилати тобі сповіщення про пари з сайту <domain>. Спочатку тобі потрібно зарєструватись тут", parseMode: ParseMode.Markdown);
                }
                await repository.AddUserTelegramChatInfoAsync(new TelegramData
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

        public override bool Contains(Message message)
        {
            return message.Type == MessageType.Text 
                   && message.Text.Contains(Name);
        }
    }
}
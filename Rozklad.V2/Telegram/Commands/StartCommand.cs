using System.Reflection.Metadata;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

namespace Rozklad.V2.Telegram.Commands
{
    public class StartCommand : Command
    {
        public override string Name => @"/start";
        public override async Task Execute(Message message, TelegramBotClient client)
        {
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
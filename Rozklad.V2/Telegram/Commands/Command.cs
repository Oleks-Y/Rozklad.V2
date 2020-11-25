using System.Threading.Tasks;
using Rozklad.V2.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Rozklad.V2.Telegram.Commands
{
    public  interface ICommand
    {
        public static string Name { get; }

        public abstract Task Execute(Message message, TelegramBotClient client);

        public abstract bool Contains(Message message);
    }
}
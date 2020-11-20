using System.Threading.Tasks;
using Rozklad.V2.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Rozklad.V2.Telegram.Commands
{
    public  abstract class Command
    {
        public abstract string Name { get; }

        public abstract Task Execute(Message message, TelegramBotClient client,IRozkladRepository repository);

        public abstract bool Contains(Message message);
    }
}
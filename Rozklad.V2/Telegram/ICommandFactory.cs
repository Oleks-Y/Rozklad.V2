using Rozklad.V2.Telegram.Commands;
using Telegram.Bot.Types;

namespace Rozklad.V2.Telegram
{
    public interface ICommandFactory
    {
        public ICommand GetCommand(Message message);
    }
}
using Rozklad.V2.Telegram.Commands;
using Telegram.Bot.Types;

namespace Rozklad.V2.Telegram
{
    public interface ICommandFactory
    {
        public Command GetCommand(Message message);
    }
}
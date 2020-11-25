using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rozklad.V2.Services;
using Rozklad.V2.Telegram;
using Telegram.Bot.Types;

namespace Rozklad.V2.Controllers
{
    [Route("api/message/update")]
    public class TelegramMessageController : ControllerBase
    {
        private readonly IRozkladRepository _repository;

        private readonly ICommandFactory _commandFactory;
        public TelegramMessageController(IRozkladRepository repository, ICommandFactory commandFactory)
        {
            _repository = repository;
            _commandFactory = commandFactory;
        }

        [HttpPost]
        public async Task<IActionResult> OnUpdate([FromBody] Update update)
        {
            if (update == null) return Ok();

            var message = update.Message;
            var command = _commandFactory.GetCommand(message);
            if(command!=null)
                await command.Execute(message, Bot.BotClient);
            await _repository.SaveAsync();
            return Ok();
        }
    }
}
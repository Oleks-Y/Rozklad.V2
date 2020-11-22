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

        public TelegramMessageController(IRozkladRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> OnUpdate([FromBody] Update update)
        {
            if (update == null) return Ok();

            var commands = Bot.Commands;
            var message = update.Message;
            var botClient = Bot.BotClient;

            foreach (var command in commands)
            {
                if (command.Contains(message))
                {
                    await command.Execute(message, botClient, _repository);
                    await _repository.SaveAsync();
                    break;
                }
            }
            return Ok();
        }
    }
}
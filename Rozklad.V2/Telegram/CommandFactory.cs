using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Rozklad.V2.Telegram.Commands;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Rozklad.V2.Telegram
{
    public class CommandFactory : ICommandFactory
    {

        private readonly IServiceProvider _serviceProvider;

        public CommandFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICommand GetCommand(Message message)
        {
            if(message.Type != MessageType.Text)
            {
                return null;
            }
            Console.WriteLine(message.Text);
            var commandString = message.Text;
            if (StartCommand.Name.Contains(commandString))
            {
                return _serviceProvider.GetRequiredService<StartCommand>();
            }
            if (commandString.Contains(EnableNotificationsCommand.Name))
            {
                return _serviceProvider.GetRequiredService<EnableNotificationsCommand>();
            }
            if (DisableNotificationsCommand.Name.Contains(commandString))
            {
                return _serviceProvider.GetRequiredService<DisableNotificationsCommand>();
            }

            return null;
        }
    }
}
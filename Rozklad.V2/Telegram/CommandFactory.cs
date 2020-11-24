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
        private List<Command> _commandsList;

        private readonly IServiceProvider _serviceProvider;

        public CommandFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Command GetCommand(Message message)
        {
            if(message.Type != MessageType.Text)
            {
                return null;
            }
            var commandString = message.Text;
            if (commandString==StartCommand.Name)
            {
                return _serviceProvider.GetService<StartCommand>();
            }
            if (commandString==EnableNotificationsCommand.Name)
            {
                return _serviceProvider.GetService<EnableNotificationsCommand>();
            }
            if (commandString==DisableNotificationsCommand.Name)
            {
                return _serviceProvider.GetService<DisableNotificationsCommand>();
            }

            return null;
        }
    }
}
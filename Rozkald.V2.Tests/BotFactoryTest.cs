using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Rozklad.V2.Scheduler;
using Rozklad.V2.Services;
using Rozklad.V2.Telegram;
using Rozklad.V2.Telegram.Commands;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Xunit;

namespace Rozkald.V2.Tests
{
    public class BotFactoryTest
    {
        [Fact]
        public void CanCreateCommand()
        {
            //Arrange 
            
            var repositoryMock = new Mock<IRozkladRepository>();
            var jobManagerMock = new Mock<JobsManager>();
            var mock = new Mock<IServiceProvider>();
            mock.Setup(s => s.GetRequiredService<StartCommand>())
                .Returns(new StartCommand(repositoryMock.Object));
            mock.Setup(s => s.GetRequiredService<EnableNotificationsCommand>())
                .Returns(new EnableNotificationsCommand(repositoryMock.Object, jobManagerMock.Object));
            mock.Setup(s => s.GetRequiredService<DisableNotificationsCommand>())
                .Returns(new DisableNotificationsCommand(repositoryMock.Object, jobManagerMock.Object));
            
            var Factory = new CommandFactory(mock.Object);
            var messageStartMock = new Mock<Message>();
            messageStartMock.Setup(m => m.Type).Returns(MessageType.Text);
            messageStartMock.Setup(m => m.Text).Returns("/start");
            // Act 

            var result = Factory.GetCommand(messageStartMock.Object);
            
            // Assert 
            
            Assert.IsType<StartCommand>(result);
        }
    }
}
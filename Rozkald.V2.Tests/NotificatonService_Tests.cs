using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Rozklad.V2.DataAccess;
using Rozklad.V2.Entities;
using Rozklad.V2.Services;
using Rozklad.V2.Telegram;
using Xunit;

namespace Rozkald.V2.Tests
{
    public class NotificatonService_Tests
    {
        // [Fact]
        // public async void CanSendNotification()
        // {
        //     // Arrange 
        //     var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        //         .UseInMemoryDatabase(databaseName: "Rozklad2")
        //         .Options;
        //     var studentId1 = Guid.NewGuid();
        //     var studentId2 = Guid.NewGuid();
        //     using (var context = new ApplicationDbContext(options))
        //     {
        //         context.TelegramData.Add(new TelegramData
        //         {
        //             Id = Guid.NewGuid(),
        //             StudentId = studentId1,
        //             TelegramId = 456955082,
        //             TelegramChatId = 456955082
        //         });
        //         context.TelegramData.Add(new TelegramData
        //         {
        //             Id = Guid.NewGuid(),
        //             StudentId = studentId2,
        //             TelegramId = 456955082,
        //             TelegramChatId = 456955082
        //         });
        //
        //         context.SaveChanges();
        //     }
        //     // mock bot 
        //     // mock action 
        //     var mock = new Mock<Bot>();
        //     mock.Setup(b=>b.)
        //     var logger = Mock.Of<ILogger<TelegramNotificationService>>();
        //     // Act 
        //     using (var context = new ApplicationDbContext(options))
        //     {
        //         var repository = new RozkladRepository(context);
        //         var notifserive = new TelegramNotificationService(repository, logger);
        //     }
        //     // Assert
        // }
    }
}
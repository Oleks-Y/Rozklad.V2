using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using Rozklad.V2.DataAccess;
using Rozklad.V2.Entities;
using Rozklad.V2.Helpers;
using Rozklad.V2.Services;
using Xunit;

namespace Rozkald.V2.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async void CanGetTelegramData()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Rozklad2")
                .Options;
            var studentId1 = Guid.NewGuid();
            var studentId2 = Guid.NewGuid();
            using (var context = new ApplicationDbContext(options))
            {
                context.TelegramData.Add(new TelegramData
                {
                    Id = Guid.NewGuid(),
                    StudentId = studentId1,
                    TelegramId = 456955082,
                    TelegramChatId = 456955082
                });
                context.TelegramData.Add(new TelegramData
                {
                    Id = Guid.NewGuid(),
                    StudentId = studentId2,
                    TelegramId = 456955082,
                    TelegramChatId = 456955082
                });

                context.SaveChanges();
            }
            // Act 
            var result = new List<TelegramData>();
            using (var context = new ApplicationDbContext(options))
            {
                var Guids = new List<Guid>{ studentId1, studentId2};
                var repository = new RozkladRepository(context);
                result = (repository.GetUserTelegramData(Guids)).ToList();
            }
            // Assert 
            Assert.True(result.Count>0);
            Assert.True(result[0].TelegramId==456955082);
        }
        
        [Fact]
        public async void CanGetNotifications()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Rozklad2")
                .Options;
            using (var context = new ApplicationDbContext(options))
            {
                context.Groups.Add(new Group
                {
                    Id = Guid.Parse("3bb26431-a7ba-4b55-970a-c8544cb920c8"),
                    Group_Name = "Test"
                });
                context.Students.Add(new Student
                {
                    Id = Guid.Parse("fb484eee-dea4-409b-b6df-778cfd82d6a1"),
                    GroupId = Guid.Parse("3bb26431-a7ba-4b55-970a-c8544cb920c8"),
                });
                context.NotificationsSettings.Add(new NotificationsSettings
                {
                    Id = Guid.Parse("3c1ea52f-3610-4ddc-a8b7-9b87e167eeff"),
                    NotificationType = "Telegram",
                    StudentId = Guid.Parse("fb484eee-dea4-409b-b6df-778cfd82d6a1"),
                    IsNotificationsOn = true,
                    TimeBeforeLesson = 15
                });
                context.Subjects.Add(new Subject
                {
                    Id = Guid.Parse("e4d55a2a-cb04-4a0d-a355-d99130ff5986"),
                    Name = "First",
                    Teachers = "some teacher",
                    GroupId = Guid.Parse("3bb26431-a7ba-4b55-970a-c8544cb920c8"),
                    LabsZoom = "",
                    LessonsZoom = "",
                    LabsAccessCode = "",
                    LessonsAccessCode = ""
                });
                context.Subjects.Add(new Subject
                {
                    Id = Guid.Parse("f30c42db-7bee-4d3d-beec-2bc39e797e6c"),
                    Name = "First",
                    Teachers = "some teacher",
                    GroupId = Guid.Parse("3bb26431-a7ba-4b55-970a-c8544cb920c8"),
                    LabsZoom = "",
                    LessonsZoom = "",
                    LabsAccessCode = "",
                    LessonsAccessCode = ""
                });

                context.Lessons.Add(new Lesson
                {
                    Id = Guid.Parse("3692e169-1ea6-45fa-8dc3-9738eb0a3a8b"),
                    SubjectId = Guid.Parse("e4d55a2a-cb04-4a0d-a355-d99130ff5986"),
                    Type = "Лек",
                    Week = 2,
                    TimeStart = "10:25:00",
                    DayOfWeek = 1
                });
                
                context.Lessons.Add(new Lesson
                {
                    Id = Guid.Parse("1ca4899a-01c5-43c6-bc4f-6c2cce53c8de"),
                    SubjectId = Guid.Parse("e4d55a2a-cb04-4a0d-a355-d99130ff5986"),
                    Type = "Лек",
                    Week = 2,
                    TimeStart = "12:20:00",
                    DayOfWeek = 1
                });
                context.Lessons.Add(new Lesson
                {
                    Id = Guid.Parse("e6aa81c4-d7c7-4d54-acf5-c372884fd6da"),
                    SubjectId = Guid.Parse("f30c42db-7bee-4d3d-beec-2bc39e797e6c"),
                    Type = "Лек",
                    Week = 1,
                    TimeStart = "12:20:00",
                    DayOfWeek = 1
                });
                context.SaveChanges();
            }
            var result1 = new List<Notification>();
            // Act 
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new RozkladRepository(context);
                var fireTime1 = new FireTime
                {
                    Time = new TimeSpan(10,10,00),
                    LessonTime = new TimeSpan(10,25,00),
                    NumberOfDay = 1,
                    NumberOfWeek = 2
                };
                result1 = (repository.GetAllNotificationsByThisTime(fireTime1)).ToList();
            }
            
            //Assert 
            
            Assert.True(result1.Count()==1);
            Assert.True(result1[0].StudentId ==Guid.Parse("fb484eee-dea4-409b-b6df-778cfd82d6a1"));
            
        }
    }
}
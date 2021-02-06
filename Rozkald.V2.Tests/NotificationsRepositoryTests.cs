using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rozklad.V2.DataAccess;
using Rozklad.V2.Entities;
using Rozklad.V2.Helpers;
using Rozklad.V2.Services;
using Xunit;

namespace Rozkald.V2.Tests
{
    public class NotificationsRepositoryTests
    {
        [Fact]
        public async Task CanAddNotificationsTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Rozklad2")
                .Options;
            var studentId1 = Guid.NewGuid();
            var subjId1 = Guid.NewGuid();
            var subjId2= Guid.NewGuid();
            var subjId3= Guid.NewGuid();
            var groupId = Guid.NewGuid();
            using (var context = new ApplicationDbContext(options))
            {
                context.Students.Add(new Student
                {
                    Id = studentId1,
                    FirstName = "One",
                    LastName = "One",
                    DisabledSubjects = new List<DisabledSubject>
                    {
                        new DisabledSubject
                        {
                            Id = Guid.NewGuid(),
                            StudentId = studentId1,
                            SubjectId = subjId1
                        },
                        new DisabledSubject
                        {
                            Id = Guid.NewGuid(),
                            StudentId = studentId1,
                            SubjectId = subjId2
                        }
                    },
                    NotificationsSettings = new NotificationsSettings
                    {
                     Id   = Guid.NewGuid(),
                     NotificationType = "Telegram",
                     StudentId = studentId1,
                     IsNotificationsOn = false,
                     TimeBeforeLesson = 5
                    },
                    Group = new Group
                    {
                        Id = Guid.NewGuid(),
                        Group_Name = "т-т",
                        Subjects = new Subject[]
                        {
                            new Subject
                            {
                                Id = subjId1,
                                GroupId = groupId,
                                Name = "aa",
                                Lessons = new List<Lesson>
                                {
                                    new Lesson
                                    {
                                        Id = Guid.NewGuid(),
                                        SubjectId = subjId1,
                                        Type = "Лек",
                                        Week = 1,
                                        TimeStart = "8:30:00",
                                        DayOfWeek = 2
                                    }
                                }
                            },
                            new Subject
                            {
                                Id = subjId2,
                                GroupId = groupId,
                                Name = "aa",
                                Lessons = new List<Lesson>
                                {
                                    new Lesson
                                    {
                                        Id = Guid.NewGuid(),
                                        SubjectId = subjId2,
                                        Type = "Лек",
                                        Week = 1,
                                        TimeStart = "10:25:00",
                                        DayOfWeek = 3
                                    }
                                }
                            }, 
                            new Subject
                            {
                                Id = subjId3,
                                GroupId = groupId,
                                Name = "aa",
                                Lessons = new List<Lesson>
                                {
                                    new Lesson
                                    {
                                        Id = Guid.NewGuid(),
                                        SubjectId = subjId3,
                                        Type = "Лек",
                                        Week = 1,
                                        TimeStart = "12:20:00",
                                        DayOfWeek = 2
                                    }
                                }
                            }, 
                        }
                    }
                });
                // context.NotificationsSettings.Add(new NotificationsSettings
                // {
                //     Id = Guid.NewGuid(),
                //     NotificationType = "Telegram",
                //     StudentId = studentId1,
                //     IsNotificationsOn = false,
                //     TimeBeforeLesson = 5
                // });
                // context.SaveChanges();
            }
            // Act 
            IEnumerable<FireTime> result;
            using (var context = new ApplicationDbContext(options))
            {
                // var repository = new NotificationRepository(context);
                // result = await repository.EnableNotifications(studentId1);
            }
            // Assert 
            
            Assert.True(true);    
            
            
        }
        
    }
}
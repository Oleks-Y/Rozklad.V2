using System;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Rozklad.V2.Entities;
using Rozklad.V2.Google;
using Rozklad.V2.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Rozkald.V2.Tests
{
    public class GoogleCalendarTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public GoogleCalendarTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async void CanGetListCalendars()
        {
            //Arrange
            var service = new GoogleCalendarService("AIzaSyDNruSkc8Hnu41rzHEHix-7gPNcQ3oILzQ");
            // Act
            var calendarId = await service.GetCalendarIdForUser("ya29.a0AfH6SMCyCqDILG3VYxVXGGzyotBBS_qsJo8ILoMOSAMeAb8cc09JyZFfnkEXUg5pCCkiUyGcpT4cz5qK3y3HohBd2Qr6tpyNG6TsCIyQL7qgLPgpERVOpunm5ZubyrXULGIl90iJzQL0n0M3ftIKsAWyPkt-NONvCSmzvuGjt9Zc");
            // Assert
            _testOutputHelper.WriteLine(calendarId);
        }

        [Fact]
        public async void CanSetNotifications()
        {
            const string apiKey= "AIzaSyDNruSkc8Hnu41rzHEHix-7gPNcQ3oILzQ";
            const string accessToken = "ya29.a0AfH6SMAAPOUuV5HTwH_aENYOeNtPANgeuszT_ezxMI49_A-vpKN3O-zati20rfwQxORFnzl2r41YRCjB0jBwbuwCR4Nh7XeQaJSkUbpvSRFQtJTdC51_Yr-JnR66WEyYD70Lg-WrqZw2mYxitKv3AOtrbrauIkKBRfLvsfqCxMGS";
            // Arrange
            var service = new GoogleCalendarService(apiKey);
            // Act 
            var settings = new CalendarSettings
            {
                Lessons = new[]
                {
                    new Lesson
                    {
                        Id = Guid.NewGuid(),
                        Subject = new Subject
                        {
                            Id = Guid.NewGuid(),
                            Name = "Matematika",
                            Teachers = "Tutu",
                        },
                        Type = "Lek",
                        Week = 2,
                        TimeStart = "12:20:00",
                        DayOfWeek = 2
                    },


                },
                CalendarId = await service.GetCalendarIdForUser(accessToken),
                NotificationsSettings = new NotificationsSettings
                {
                    Id = Guid.NewGuid(),
                    NotificationType = "Google",
                    StudentId = Guid.NewGuid(),
                    IsNotificationsOn = true,
                    TimeBeforeLesson = 5
                }
            };
            await service.SetNotificationsInCalendar(settings,accessToken );
            // Assert 
        }
    }
}
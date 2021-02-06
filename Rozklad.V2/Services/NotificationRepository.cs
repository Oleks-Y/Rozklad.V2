using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Calendar.v3.Data;
using Microsoft.EntityFrameworkCore;
using Rozklad.V2.DataAccess;
using Rozklad.V2.Entities;
using Rozklad.V2.Google;
using Rozklad.V2.Helpers;
using Rozklad.V2.Scheduler;

namespace Rozklad.V2.Services
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IGoogleCalendarService _calendar;
        private readonly GoogleTokenService _tokenService;
        public NotificationRepository(ApplicationDbContext context, IGoogleCalendarService calendar, GoogleTokenService tokenService)
        {
            _context = context;
            _calendar = calendar;
            _tokenService = tokenService;
        }

        
        public async Task SetNotificationSettings(NotificationsSettings settings)
        {
            // update calendar notifications
            if (settings.NotificationType == "Google")
            {
                // get all created events 
                var evetns =await setGoogleNotifications(settings);
                //todo set events in database
            }
            await this.changeNottificationsSettingsInDb(settings);
            
        }

        /// <summary>
        /// Method to update entifiy in database
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private async Task<IEnumerable<Event>> setGoogleNotifications(NotificationsSettings newSettings)
        {
            // todo here check if notifications have already set, and then update
            // todo if isOn==false  then deleteNotifications 
            // todo check calendar updates somewhere 
            
            // we don`t srote accessToken, because it`s easier to just reuqest new one 
            var googleDataForUser= await _context.GoogleData.FirstOrDefaultAsync(s=>s.StudentId == newSettings.StudentId);
            var accessToken = await _tokenService.GetActualAccessToken(googleDataForUser.RefreshToken);
            
            var currentSettings =
                await _context.NotificationsSettings.FirstOrDefaultAsync(ns => ns.StudentId == newSettings.StudentId);
            
            switch (newSettings.IsNotificationsOn)
            {
                case true:
                    return await setNotificationsInCalendar(newSettings, accessToken);
                // just retunrings empty list, if no new tasks was set 
                case false when !currentSettings.IsNotificationsOn:
                    return new List<Event>();
                case false:
                    return await setNotificationsInCalendar(newSettings, accessToken);
            }
        }
        /// <summary>
        /// method with database and calendar related logic 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        // todo need to split up db and calendar
        private async Task<IEnumerable<Event>> setNotificationsInCalendar(NotificationsSettings settings, string accessToken)
        {
            var lessons = getAllLessonsForStudent(settings.StudentId);
            var googleCalendarId = getCalendarIdForStudent(settings.StudentId);
            var calendarSettings = new CalendarSettings
            {
                Lessons   = await lessons,
                CalendarId = await googleCalendarId,
                NotificationsSettings = settings,
                AccessToken = accessToken
            };
            var events =  await _calendar.SetNotificationsInCalendar(calendarSettings);
            return events;
        } 
        private async Task changeNottificationsSettingsInDb(NotificationsSettings settings)
        {
            var student = await _context.Students.Include(s => s.NotificationsSettings)
                .FirstOrDefaultAsync(s => s.Id == settings.StudentId);

            // check if notifications entity exists 
            if (student.NotificationsSettingsId == null || student.NotificationsSettings == null)
            {
                // if not exists - create
                settings.Id = Guid.NewGuid();
                await _context.NotificationsSettings.AddAsync(settings);
                student.NotificationsSettingsId = settings.Id;
                return;
            }

            // if exists - update 
            student.NotificationsSettings.IsNotificationsOn = settings.IsNotificationsOn;
            student.NotificationsSettings.TimeBeforeLesson = settings.TimeBeforeLesson;
            student.NotificationsSettings.NotificationType = settings.NotificationType;
        }


        // todo need to seprate database logic and notification logic
        private async Task<IEnumerable<Lesson>> getAllLessonsForStudent(Guid studentId)
        {
            // get all lessons except disbled and muted 
            var student = _context.Students
                .Include(s=>s.DisabledSubjects)
                .Include(s=>s.MutedSubjects)
                .Include(s=>s.Group)
                .FirstOrDefault(s => s.Id == studentId);
            if (student == null)
            {
                throw new ArgumentNullException("student can`t be null while setting nottifications");
            }
            var subjectsAll = student.Group.Subjects;
            // get all subjects, that not disabled or muted
            var subjectsAvailable = subjectsAll.Where(s => student.DisabledSubjects.All(ds => ds.SubjectId != s.Id))
                .Where(s => student.MutedSubjects.All(ms => ms.SubjectId != s.Id));
            var allLessonsAvailable = _context.Lessons.Where(l => subjectsAvailable.Any(s => s.Id == l.SubjectId));

            var lessons = await allLessonsAvailable.ToListAsync();
            return lessons;
        }

        private async Task<string> getCalendarIdForStudent(Guid studentId)
        {
            var googleSettings = await _context.GoogleData.FirstOrDefaultAsync(gd => gd.StudentId == studentId);
            return googleSettings.CalendarId;
        }
        private async Task setNotifications(Guid studentId, bool isOn)
        {
            var student =
                await _context.Students.Include("NotificationsSettings").FirstOrDefaultAsync(s => s.Id == studentId);
            student.NotificationsSettings.IsNotificationsOn = isOn;
        }

        private async Task<IEnumerable<FireTime>> getAllFiretimes()
        {
            var students = _context.Students.Include("NotificationsSettings").Include("DisabledSubjects");
            var studentsWithNotificationsOn = students.Where(s => s.NotificationsSettings.IsNotificationsOn == true);
            // all unique lessons 
            var firetimes = await getFiretimesFromStudents(studentsWithNotificationsOn);
            return firetimes;
        }

        private async Task<IEnumerable<FireTime>> getAllFireTimesForStudent(Guid studentId)
        {
            // Actucally, it always will be array of single element, 
            // but now, i don`t know another way to run it in single query 
            var students = _context.Students.Include("NotificationsSettings")
                .Include("DisabledSubjects").Where(s=>s.Id==studentId);
            // todo check if notifications is on 

            var fireTimes = await getFiretimesFromStudents(students);

            return fireTimes;
        }

        private async Task<IEnumerable<FireTime>> getFiretimesFromStudents(IQueryable<Student> studentsWithNotificationsOn)
        {
            var lessons = await 
                studentsWithNotificationsOn
                    .SelectMany(s =>
                        s.Group.Subjects.Where(subj =>
                                !s.DisabledSubjects.Select(i => i.SubjectId).Contains(subj.Id))
                            .SelectMany(subj => subj.Lessons.Select(less => new {lesson = less, student = s})))
                    .ToListAsync();
            var firetimes = lessons.Select(obj =>
                new FireTime
                {
                    Time = DateTime.Parse(obj.lesson.TimeStart)
                        .AddMinutes(-obj.student.NotificationsSettings.TimeBeforeLesson).TimeOfDay,
                    NumberOfDay = obj.lesson.DayOfWeek,
                    NumberOfWeek = obj.lesson.Week,
                    LessonTime = TimeSpan.Parse(obj.lesson.TimeStart)
                }
            );
            return firetimes.Distinct();
        }
    }
}
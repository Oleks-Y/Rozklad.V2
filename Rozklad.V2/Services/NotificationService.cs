using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rozklad.V2.DataAccess;
using Rozklad.V2.Helpers;

namespace Rozklad.V2.Services
{
    public class NotificationService : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FireTime>> EnableNotifications(Guid stuedntId)
        {
            // set notificaions on 
            var notificationsSettings =
                await _context.NotificationsSettings.FirstOrDefaultAsync(s => s.StudentId == stuedntId);
            notificationsSettings.IsNotificationsOn = true;
            // get firetimes, specific for this student  
                // get all firetimes 
                // get firetimes for this student
                // calculate (all firetimes) / (firetimes for this student)
            var firetimesAll = await getAllFiretimes();
            var firetimesSpecificForStudent = await getAllFireTimesForStudent(stuedntId);
            var firetimesToAdd = firetimesSpecificForStudent.Except(firetimesAll);

            return await firetimesToAdd.ToListAsync();
        }

        public Task<IEnumerable<FireTime>> DisableNotifications(Guid stuedntId)
        {
            throw new NotImplementedException();
        }

        private async Task<IQueryable<FireTime>> getAllFiretimes()
        {
            var students = _context.Students.Include("NotificationsSettings").Include("DisabledSubjects");
            var studentsWithNotificationsOn = students.Where(s => s.NotificationsSettings.IsNotificationsOn == true);
            // all unique lessons 
            var lessons =
                studentsWithNotificationsOn
                    .SelectMany(s =>
                        s.Group.Subjects.Where(subj =>
                                !s.DisabledSubjects.Select(i => i.SubjectId).Contains(subj.Id))
                            .SelectMany(subj => subj.Lessons.Select(less => new {lesson = less, student = s})))
                    .Distinct();
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

        private async Task<IQueryable<FireTime>> getAllFireTimesForStudent(Guid studentId)
        {
            // Actucally, it always will be array of single element, 
            // but now, i don`t know another way to run it in single query 
            var students = _context.Students.Include("NotificationsSettings")
                .Include("DisabledSubjects").Where(s=>s.Id==studentId);
            if (students.Any(s => !s.NotificationsSettings.IsNotificationsOn))
            {
                return null;
            }
            var lessons =
                students
                    .SelectMany(s =>
                        s.Group.Subjects.Where(subj =>
                                !s.DisabledSubjects.Select(i => i.SubjectId).Contains(subj.Id))
                            .SelectMany(subj => subj.Lessons.Select(less => new {lesson = less, student = s})))
                    .Distinct();
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
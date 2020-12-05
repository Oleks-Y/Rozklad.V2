using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rozklad.V2.DataAccess;
using Rozklad.V2.Entities;
using Rozklad.V2.Helpers;
using Rozklad.V2.Scheduler;

namespace Rozklad.V2.Services
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        private readonly IJobManager _jobManager;

        public NotificationRepository(ApplicationDbContext context, IJobManager jobManager)
        {
            _context = context;
            _jobManager = jobManager;
        }

        public async Task<IEnumerable<FireTime>> EnableNotifications(Guid stuedntId)
        {
            await setNotifications(stuedntId, true);
            var firetimesAll = getAllFiretimes();
            var firetimesSpecificForStudent = getAllFireTimesForStudent(stuedntId);
            var firetimesToAdd = firetimesSpecificForStudent.Except(firetimesAll);

             return await firetimesToAdd.ToListAsync();
        }

        public async Task<IEnumerable<FireTime>> DisableNotifications(Guid stuedntId)
        {
            await setNotifications(stuedntId, false);
            var firetimesAll = getAllFiretimes();
            var firetimesSpecificForStudent = getAllFireTimesForStudent(stuedntId);
            var firetimesToRemove = firetimesSpecificForStudent.Except(firetimesAll);

            return await firetimesToRemove.ToListAsync();
        }

        private async Task setNotifications(Guid studentId, bool isOn)
        {
            var notificationsSettings =
                await _context.NotificationsSettings.FirstOrDefaultAsync(s => s.StudentId == studentId);
            notificationsSettings.IsNotificationsOn = isOn;
        }

        private IQueryable<FireTime> getAllFiretimes()
        {
            var students = _context.Students.Include("NotificationsSettings").Include("DisabledSubjects");
            var studentsWithNotificationsOn = students.Where(s => s.NotificationsSettings.IsNotificationsOn == true);
            // all unique lessons 
            var firetimes = getFiretimesFromStudents(studentsWithNotificationsOn);
            return firetimes;
        }

        private IQueryable<FireTime> getAllFireTimesForStudent(Guid studentId)
        {
            // Actucally, it always will be array of single element, 
            // but now, i don`t know another way to run it in single query 
            var students = _context.Students.Include("NotificationsSettings")
                .Include("DisabledSubjects").Where(s=>s.Id==studentId);
            if (students.Any(s => !s.NotificationsSettings.IsNotificationsOn))
            {
                return null;
            }

            var fireTimes = getFiretimesFromStudents(students);

            return fireTimes;
        }

        private IQueryable<FireTime> getFiretimesFromStudents(IQueryable<Student> studentsWithNotificationsOn)
        {
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
    }
}
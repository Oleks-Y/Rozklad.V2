﻿using System;
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


        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task EnableNotifications(Guid stuedntId)
        {
            await setNotifications(stuedntId, true);
        }

        public async Task DisableNotifications(Guid stuedntId)
        {
            await setNotifications(stuedntId, false);
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
﻿using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rozklad.V2.DataAccess;
using Rozklad.V2.Entities;
using Rozklad.V2.Exceptions;
using Rozklad.V2.Helpers;
using Rozklad.V2.Models;
using Rozklad.V2.Pages;

namespace Rozklad.V2.Services
{
    public class RozkladRepository : IRozkladRepository
    {
        private readonly ApplicationDbContext _context;

        public RozkladRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Subject> GetSubjectAsync(Guid subjectId)
        {
            return await _context.Subjects.FirstOrDefaultAsync(s => s.Id == subjectId);
        }

        public async Task<IEnumerable<Subject>> GetDisabledSubjectsAsync(Guid studentId)
        {
            // get disabled subjects by studentId
            var disabledSubjects = _context.DisabledSubjects.Where(s => s.StudentId == studentId);
            var subjects = new List<Subject>();
            subjects.AddRange(await _context.Subjects.Where(s =>
                disabledSubjects.Select(subject => subject.SubjectId).Contains(s.Id)).ToListAsync());
            return subjects;
        }


        public async Task UnmuteSubject(Guid studentId, Guid subjectId)
        {
             var subjectsToRemove = await _context.MutedSubjects.Where(s=>s.StudentId==studentId && s.SubjectId==subjectId).ToListAsync();
             _context.RemoveRange(subjectsToRemove);
        }

        public Task<Student> GetStudentWithNotification(Guid studentId)
        {
            return _context.Students.Include(s => s.NotificationsSettings).FirstOrDefaultAsync(s => s.Id == studentId);
        }

        public async Task<IEnumerable<Lesson>> GetLessonsForStudent(Guid studentId)
        {
            var student = _context.Students.Include("Group").FirstOrDefault(s => s.Id == studentId);

            var lessons = await _context.Lessons.Include("Subject")
                .Where(l => l.Subject.GroupId == student.Group.Id).ToListAsync();

            var disabledSubjects = await _context.DisabledSubjects.Where(s => s.StudentId == student.Id).ToListAsync();
            // todo refactor 
            var lessonsCopy = new Lesson[lessons.Count];
            lessons.CopyTo(lessonsCopy);
            foreach (var lesson in lessonsCopy.Where(lesson =>
                disabledSubjects.Select(s => s.SubjectId).Contains(lesson.SubjectId)))
            {
                lessons.Remove(lesson);
            }

            return lessons.OrderBy(l => l.Week)
                .ThenBy(l => l.DayOfWeek)
                .ThenBy(l => l.TimeStart)
                .ToList();
        }

        public Group GetGroupByName(string groupName)
        {
            return _context.Groups.FirstOrDefault(g => g.Group_Name.Replace(" ", "") == groupName);
        }

        public Task<Student> GetStudentAsync(Guid studentId)
        {
            return _context.Students.Include("Group").Include("Group.Subjects").Include("DisabledSubjects")
                .FirstOrDefaultAsync(s => s.Id == studentId);
        }

        public async Task MuteSubject(Guid studentId, Guid subjectId)
        {
            await _context.AddAsync(new MutedSubject
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                SubjectId = subjectId
            });
        }

        public async Task<IEnumerable<Subject>> GetSubjectsForStudentAsync(Guid studentId)
        {
            // Get all subject for student group 
            // Remove disabled subjects 
            // Return list to user 
            var student = await _context.Students.Include("Group").Include("Group.Subjects").Include("DisabledSubjects")
                .FirstOrDefaultAsync(s => s.Id == studentId);
            var disabledSubejcts = student.DisabledSubjects;
            // var mutedSubjects = student.MutedSubjects;
            return student.Group.Subjects
                .Where(s => disabledSubejcts.All(ds => ds.SubjectId != s.Id));
                // .Where(s=> mutedSubjects.Any(ms=>ms.SubjectId != s.Id));
        }

        public async Task<IEnumerable<Lesson>> GetLessonsForGroupAsync(Guid groupId)
        {
            var group = await _context.Groups.Include("Subjects").FirstOrDefaultAsync(g => g.Id == groupId);
            var lessons = await _context.Lessons.Include("Subject")
                .Where(l => l.Subject.GroupId == group.Id)
                .OrderBy(l => l.Week)
                .ThenBy(l => l.DayOfWeek)
                .ThenBy(l => l.TimeStart)
                .ToListAsync();

            return lessons;
        }

        public async Task DisableSubjectAsync(Guid studentId, Guid subjectId)
        {
            // add to disabledSubjects new line with subject id 
            await _context.DisabledSubjects.AddAsync(new DisabledSubject()
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                SubjectId = subjectId
            });
            await _context.MutedSubjects.AddAsync(new MutedSubject
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                SubjectId = subjectId
            });
        }

        public async  Task EnableSubject(Guid studentId, Guid subjectId)
        {
            var toRemove =
                _context.DisabledSubjects.Where(s => s.StudentId == studentId && s.SubjectId == subjectId);
            var toUnmute = _context.MutedSubjects.Where(s => s.StudentId == studentId && s.SubjectId == subjectId);
            _context.DisabledSubjects.RemoveRange(toRemove);
            _context.MutedSubjects.RemoveRange(toUnmute);
        }


        public void UpdateSubject(Subject subject)
        {
        }

        public async Task<IEnumerable<Group>> GetAllGroupsAsync()
        {
            return await _context.Groups.ToListAsync();
        }


        public bool StudentExists(Guid studentId)
        {
            return _context.Students.Any(s => s.Id == studentId);
        }

        public async Task UpdateNotification(NotificationsSettings notificationsSettings)
        {
            var student = await _context.Students.Include(s => s.NotificationsSettings)
                .FirstOrDefaultAsync(s => s.Id == notificationsSettings.StudentId);

            // check if notifications entity exists 
            if (student.NotificationsSettingsId == null || student.NotificationsSettings == null)
            {
                // if not exists - create
                notificationsSettings.Id = Guid.NewGuid();
                await _context.NotificationsSettings.AddAsync(notificationsSettings);
                student.NotificationsSettingsId = notificationsSettings.Id;
                return;
            }

            // if exists - update 
            student.NotificationsSettings.IsNotificationsOn = notificationsSettings.IsNotificationsOn;
            student.NotificationsSettings.TimeBeforeLesson = notificationsSettings.TimeBeforeLesson;
            student.NotificationsSettings.NotificationType = notificationsSettings.NotificationType;
        }

        // public async Task<IEnumerable<NotificationsSettings>> GetAllNotificationsSettings()
        // {
        //     return await _context.NotificationsSettings.ToListAsync();
        // }

        public async Task<IEnumerable<FireTime>> GetFireTimesForStudent(Guid studentId)
        {
            var notificationsSettings =
                (await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId)).NotificationsSettings;
            var firetimes = new List<FireTime>();
            var lessons = await GetLessonsForStudent(notificationsSettings.StudentId);
            foreach (var lesson in lessons)
            {
                var notificationTime = DateTime.Parse(lesson.TimeStart)
                    .AddMinutes(-notificationsSettings.TimeBeforeLesson).TimeOfDay;
                var firetime = new FireTime
                {
                    Time = notificationTime,
                    NumberOfDay = lesson.DayOfWeek,
                    NumberOfWeek = lesson.Week,
                    LessonTime = TimeSpan.Parse(lesson.TimeStart)
                };
                if (!firetimes.Contains(firetime))
                {
                    firetimes.Add(firetime);
                }
            }

            return firetimes;
        }


        public async Task<IEnumerable<Notification>> GetAllNotificationsByThisTime(FireTime fireTime)
        {
            // it makes only one big request to database 
            var students = await _context.Students
                .Include(s => s.NotificationsSettings)
                .Include(s => s.Group)
                .ThenInclude(g => g.Subjects)
                .ThenInclude(s => s.Lessons)
                .ThenInclude(l => l.Subject)
                .Include(s => s.DisabledSubjects)
                .Include(s => s.MutedSubjects)
                .Where(s => s.NotificationsSettings.IsNotificationsOn).ToListAsync();
            var lessons = students.SelectMany(s => s.Group.Subjects).SelectMany(s => s.Lessons).ToList();
            var notifications = new List<Notification>();
            foreach (var student in students)
            {
                var groupId = student.Group.Id;
                var studentLessons = lessons
                    .Where(l => l.Subject.GroupId == groupId)
                    .Where(l => l.Week == fireTime.NumberOfWeek && l.DayOfWeek == fireTime.NumberOfDay &&
                                l.TimeStart == fireTime.LessonTime.ToString())
                    // remove all subjects, disabled by this student 
                    .Where(l => student.DisabledSubjects.All(ds => ds.SubjectId != l.SubjectId))
                    .Where(l => student.MutedSubjects.All(ms => ms.SubjectId != l.SubjectId));
                notifications.AddRange(studentLessons.Select(l => new Notification
                {
                    Lesson = l,
                    Type = student.NotificationsSettings.NotificationType,
                    StudentId = student.Id
                }));
            }

            return notifications;
        }

        public async Task AddUserTelegramChatInfoAsync(TelegramData data)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == data.StudentId);
            if (student == null)
            {
                throw new ArgumentException(nameof(TelegramData));
            }

            await _context.TelegramData.AddAsync(data);
        }

        public async Task<bool> UserDataExistsAsync(long telegramId)
        {
            var isExist = await _context.TelegramData.AnyAsync(d => d.TelegramId == telegramId);

            return isExist;
        }

        public async Task AddUserChatId(long telegramId, long chatId)
        {
            var data = await _context.TelegramData.FirstOrDefaultAsync(s => s.TelegramId == telegramId);
            if (data == null)
            {
                throw new ArgumentNullException(nameof(telegramId));
            }

            if (data.TelegramChatId != null)
            {
                throw new TelegramChatIdExistsException();
            }

            data.TelegramChatId = chatId;
        }

        public IEnumerable<TelegramData> GetUserTelegramData(IEnumerable<Guid> studentsIds)
        {
            var telegramDatas = _context.TelegramData.Where(s => studentsIds.Contains(s.StudentId)).ToList();
            return telegramDatas;
        }

        public bool UserTelegramDataExists(Guid studentId)
        {
            return _context.TelegramData.Any(s => s.StudentId == studentId);
        }

        public Task<Student> GetUserByTelegramId(long telegramId)
        {
            return _context.Students.Include("Group").FirstOrDefaultAsync(s => s.Telegram_Id == telegramId);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
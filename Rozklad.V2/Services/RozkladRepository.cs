using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rozklad.V2.DataAccess;
using Rozklad.V2.Entities;
using Rozklad.V2.Exceptions;
using Rozklad.V2.Helpers;
using Rozklad.V2.Models;

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
            return _context.Subjects.FirstOrDefault(s => s.Id == subjectId);
        }

        public async Task<IEnumerable<Subject>> GetDisabledSubjectsAsync(Guid studentId)
        {
            // get disabled subjects by studentId
            var disabledSubjects = await _context.DisabledSubjects.Where(s => s.StudentId == studentId).ToListAsync();
            var subjects = new List<Subject>();
            subjects.AddRange(_context.Subjects.Where(s =>
                disabledSubjects.Select(subject => subject.SubjectId).Contains(s.Id)));
            return subjects;
        }

        // public async Task<IEnumerable<Lesson>> GetLessonsForStudent(Guid studentId)
        // {
        //     var student = _context.Students.Include("Group").FirstOrDefault(s => s.Id == studentId);
        //     var lessons = await _context.Lessons.Include("Subject")
        //         .Where(l => l.Subject.GroupId == student.Group.Id)
        //         .OrderBy(l=>l.Week)
        //         .ThenBy(l=>l.DayOfWeek)
        //         .ThenBy(l=>l.TimeStart)
        //         .ToListAsync();
        //     // 
        //     return lessons;
        // }

        public async Task UpdateNotification(NotificationsSettings notificationsInfo)
        {
            // if notification not exist in table create it and set data 
            var notificationsInfoFromDb =
                await _context.NotificationsSettings.FirstOrDefaultAsync(
                    n => n.StudentId == notificationsInfo.StudentId);
            if (notificationsInfoFromDb == null)
            {
                await _context.NotificationsSettings.AddAsync(notificationsInfo);
                return;
            }

            notificationsInfoFromDb.IsNotificationsOn = notificationsInfo.IsNotificationsOn;
            notificationsInfoFromDb.TimeBeforeLesson = notificationsInfo.TimeBeforeLesson;
        }

        public async Task<IEnumerable<Lesson>> GetLessonsForStudent(Guid studentId)
        {
            var student = _context.Students.Include("Group").FirstOrDefault(s => s.Id == studentId);
            
            var lessons = await _context.Lessons.Include("Subject")
                .Where(l => l.Subject.GroupId == student.Group.Id).ToListAsync();
            
            var disabledSubjects = _context.DisabledSubjects.Where(s => s.StudentId == student.Id);
            
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
        private IEnumerable<Lesson> GetLessonsForStudentSync(Guid studentId)
        {
            var student = _context.Students.Include("Group").FirstOrDefault(s => s.Id == studentId);
            
            var lessons = _context.Lessons.Include("Subject")
                .Where(l => l.Subject.GroupId == student.Group.Id).ToList();
            
            var disabledSubjects = _context.DisabledSubjects.Where(s => s.StudentId == student.Id);
            
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

        public async Task<IEnumerable<Subject>> GetSubjectsForStudentAsync(Guid studentId)
        {
            // Get all subject for student group 
            // Remove disabled subjects 
            // Return list to user 
            var student = await _context.Students.Include("Group").Include("Group.Subjects").Include("DisabledSubjects")
                .FirstOrDefaultAsync(s => s.Id == studentId);
            var disabledSubejcts = student.DisabledSubjects;

            return student.Group.Subjects.Where(s => disabledSubejcts.All(ds => ds.SubjectId != s.Id));
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
        }

        public void EnableSubject(Guid studentId, Guid subjectId)
        {
            var toRemove =
                _context.DisabledSubjects.Where(s => s.StudentId == studentId && s.SubjectId == subjectId);
            _context.DisabledSubjects.RemoveRange(toRemove);
        }

        public void AddStudent(Student student)
        {
            throw new NotImplementedException();
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

        public async Task<IEnumerable<NotificationsSettings>> GetAllNotificationsSettings()
        {
            return await _context.NotificationsSettings.ToListAsync();
        }

        public async Task<IEnumerable<FireTime>> GetAllNotificationsFireTimes()
        {
            // get all notifications 
            // foreach student in notifications get all lessons 
            // for timetable calculate notificationsTime 
            // select all unique notification times  
            var notificationsSettings = await _context.NotificationsSettings.ToListAsync();
            var fireTimes = new List<FireTime>();
            foreach (var notificationsSetting in 
                notificationsSettings.Where(notificationsSetting => notificationsSetting.IsNotificationsOn))
            {
                var lessons = await GetLessonsForStudent(notificationsSetting.StudentId);
                foreach (var lesson in lessons)
                {
                    var notificationTime = DateTime.Parse(lesson.TimeStart)
                        .AddMinutes(-notificationsSetting.TimeBeforeLesson).TimeOfDay;
                    var firetime = new FireTime
                    {
                        Time = notificationTime,
                        NumberOfDay = lesson.DayOfWeek,
                        NumberOfWeek = lesson.Week,
                        LessonTime = TimeSpan.Parse(lesson.TimeStart)
                    };
                    // get notification time 
                    if (!fireTimes.Contains(firetime))
                    {
                        fireTimes.Add(firetime);
                    }
                }
            }

            return fireTimes;
        }

        // time of lesson in format "8:30:00"
        public IEnumerable<Notification> GetAllNotificationsByThisTime( FireTime fireTime){
            // get all notifications 
            // foreach student in notifications get all lessons  
            // if lesson is next lesson, return it 
            var notificationsSettings = _context.NotificationsSettings.ToList();
            var notifications = new List<Notification>();
            foreach (var notificationsSetting in notificationsSettings)
            {
                if (!notificationsSetting.IsNotificationsOn)
                {
                    continue;
                }
                var lessons = this.GetLessonsForStudentSync(notificationsSetting.StudentId);
                notifications.AddRange(from lesson in lessons
                    where lesson.Week == fireTime.NumberOfWeek && lesson.DayOfWeek == fireTime.NumberOfDay &&
                          // check if time format is ok 
                          lesson.TimeStart == fireTime.LessonTime.ToString()
                    select new Notification
                    {
                        Lesson = lesson,
                        StudentId = notificationsSetting.StudentId,
                        Type = notificationsSetting.NotificationType
                    });
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
            var telegramDatas =  _context.TelegramData.Where(s => studentsIds.Contains(s.StudentId)).ToList();
            return telegramDatas;
        }

        public bool UserTelegramDataExists(Guid studentId)
        {
            return _context.TelegramData.Any(s => s.StudentId == studentId);
        }

        public Task<Student> GetUserByTelegramId(long telegramId)
        {
            return _context.Students.FirstOrDefaultAsync(s => s.Telegram_Id == telegramId);
        }
        
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
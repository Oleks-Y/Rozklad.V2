﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rozklad.V2.Entities;
using Rozklad.V2.Helpers;
using Rozklad.V2.Models;

namespace Rozklad.V2.Services
{
    public interface IRozkladRepository
    {
        Task<Subject> GetSubjectAsync(Guid subjectId);

        Task<IEnumerable<Subject>> GetDisabledSubjectsAsync(Guid studentId);
        Group GetGroupByName(string groupName);

        Task<Student> GetStudentAsync(Guid studentId);

        Task UpdateNotification(NotificationsSettings notificationsInfo);

        public Task<IEnumerable<Lesson>> GetLessonsForStudent(Guid studentId);

        Task<IEnumerable<Subject>> GetSubjectsForStudentAsync(Guid studentId);
        Task<IEnumerable<Lesson>> GetLessonsForGroupAsync(Guid groupId);
        Task DisableSubjectAsync(Guid studentId, Guid subjectId);
        void EnableSubject(Guid studentId, Guid subjectId);

        void UpdateSubject(Subject subject);
        Task<IEnumerable<Group>> GetAllGroupsAsync();

        bool StudentExists(Guid studentId);

        Task<IEnumerable<NotificationsSettings>> GetAllNotificationsSettings();

        Task<IEnumerable<FireTime>> GetAllNotificationsFireTimes();

        Task<IEnumerable<Notification>> GetAllNotificationsByThisTime(int lessonWeek, int dayOfWeek,
            TimeSpan timeOfLesson);

        Task SaveAsync();
    }
}
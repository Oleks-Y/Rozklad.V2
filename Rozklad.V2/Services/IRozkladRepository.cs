using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rozklad.V2.Entities;

namespace Rozklad.V2.Services
{
    public interface IRozkladRepository
    {
        Task<Subject> GetSubjectAsync(Guid  subjectId);
        

        Group GetGroupByName(string groupName);

        Task<Student> GetStudentAsync(Guid studentId);

        public Task<IEnumerable<Lesson>> GetLessonsForStudent(Guid studentId);
        
        Task<IEnumerable<Subject>> GetSubjectsForStudentAsync(Guid studentId);

        Task DisableSubjectAsync(Guid studentId, Guid subjectId);
        void EnableSubject(Guid studentId, Guid subjectId);
        
        void UpdateSubject(Subject subject);

        
        bool StudentExists(Guid studentId);
        

        Task SaveAsync();

    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rozklad.V2.Entities;

namespace Rozklad.V2.Services
{
    public interface IRozkladRepository
    {
        Subject GetSubject(string subjectId);
        
        IEnumerable<Subject> GetAvailableSubjectsForStudent(string studentId);
        IEnumerable<Lesson> GetLessons();
        IEnumerable<Subject> GetSubjectsForStudent(string studentId, bool withRequired);

        Group GetGroup(string groupId);
        void UpdateGroup(Group group);

        Group GetGroupByName(string groupName);
        void AddSubjectToStudent(string studentId, string subjectId);
        void DeleteSubjectFromStudent(string studentId, string subjectId);
        
        Student GetStudent(string studentId);
        Student GetStudentByLastname(string lastname, string group);

        void AddStudent(Student student);
        
        void UpdateSubject(string subjectId,Subject subject);
        void UpdateStudent(string studentId,Student student);

        
        void DeleteStudent(Student student);

        bool StudentExists(string studentId);
        public bool SubjectExists(string subjectId);
        public LessonWithSubject GetLessonWithSubject(string lessonId);

        public IEnumerable<LessonWithSubject> GetLessonsWithSubjectsForStudent(string studentId);
        public Task<IEnumerable<LessonWithSubject>> GetLessonsWithSubjectsForStudentAsync(string studentId);
        void Save();
    }
}
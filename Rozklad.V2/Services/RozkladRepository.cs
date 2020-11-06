using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rozklad.V2.DataAccess;
using Rozklad.V2.Entities;
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
        
        public Subject GetSubject(string subjectId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Subject> GetAvailableSubjectsForStudent(string studentId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Lesson> GetLessons()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Subject> GetSubjectsForStudent(string studentId, bool withRequired)
        {
            throw new NotImplementedException();
        }

        public Group GetGroup(string groupId)
        {
            return _context.Groups.FirstOrDefault(g => g.Id.ToString() == groupId);
        }

        public void UpdateGroup(Group group)
        {
            
        }

        public Group GetGroupByName(string groupName)
        {
            return _context.Groups.FirstOrDefault(g => g.Group_Name.Replace(" ", "") == groupName);
        }
        public void AddSubjectToStudent(string studentId, string subjectId)
        {
            throw new NotImplementedException();
        }

        public void DeleteSubjectFromStudent(string studentId, string subjectId)
        {
            throw new NotImplementedException();
        }

        public Student GetStudent(string studentId)
        {
            throw new NotImplementedException();
        }

        public Student GetStudentByLastname(string lastname, string @group)
        {
            throw new NotImplementedException();
        }

        public void AddStudent(Student student)
        {
            throw new NotImplementedException();
        }

        public void UpdateSubject(string subjectId, Subject subject)
        {
            throw new NotImplementedException();
        }

        public void UpdateStudent(string studentId, Student student)
        {
            throw new NotImplementedException();
        }

        public void DeleteStudent(Student student)
        {
            throw new NotImplementedException();
        }

        public bool StudentExists(string studentId)
        {
            throw new NotImplementedException();
        }

        public bool SubjectExists(string subjectId)
        {
            throw new NotImplementedException();
        }

        public LessonWithSubject GetLessonWithSubject(string lessonId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LessonWithSubject> GetLessonsWithSubjectsForStudent(string studentId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LessonWithSubject>> GetLessonsWithSubjectsForStudentAsync(string studentId)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Subject> GetSubjectAsync(Guid subjectId)
        {
            return _context.Subjects.FirstOrDefault(s => s.Id == subjectId);
        }

        public async Task<IEnumerable<Lesson>> GetLessonsForStudent(Guid studentId)
        {
            var student = _context.Students.Include("Group").FirstOrDefault(s => s.Id == studentId);
            var lessons = await _context.Lessons.Include("Subject")
                .Where(l => l.Subject.GroupId == student.Group.Id)
                .OrderBy(l=>l.Week)
                .ThenBy(l=>l.DayOfWeek)
                .ThenBy(l=>l.TimeStart)
                .ToListAsync();

            return lessons;
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

        public  async  Task<IEnumerable<Group>> GetAllGroupsAsync()
        {
            return  await _context.Groups.ToListAsync();
        }


        public bool StudentExists(Guid studentId)
        {
            return _context.Students.Any(s => s.Id == studentId);
        }
        
        
        

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
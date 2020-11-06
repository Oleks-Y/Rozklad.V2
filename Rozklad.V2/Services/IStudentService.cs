using System.Collections.Generic;
using Rozklad.V2.Entities;

namespace Rozklad.V2.Services
{
    public interface IStudentService
    {
        Student Authentificate(string username, string password);

        Student GetById(int id);

        Student Create(Student student, string passrord);
    }
}
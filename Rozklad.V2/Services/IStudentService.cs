using System;
using System.Collections.Generic;
using Rozklad.V2.Entities;

namespace Rozklad.V2.Services
{
    public interface IStudentService
    {
        Student Authentificate(string username, string password);

        Student GetById(Guid id);

        Student Create(Student student, string passrord);
    }
}
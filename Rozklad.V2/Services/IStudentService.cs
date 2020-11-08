using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rozklad.V2.Entities;

namespace Rozklad.V2.Services
{
    public interface IStudentService
    {
        Student Authentificate(string username, string password);

        Student GetById(Guid id);
        Student GetStudentByUsernameAsync(string username);

        Student Create(Student student, string passrord);

        Student CreateFromTelegram(Student student, long telegramId);
    }
}
using System;
using Rozklad.V2.Entities;

namespace Rozklad.V2.Services
{
    public interface IUserSerice
    {
        // AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
        AuthenticateResponse RefreshToken(string token, string ipAddress);

        AuthenticateResponse AuthenticateWithTelegram(AuthenticateRequestTelegram model, string ipAddress);

        Student GetById(Guid studentId);
        bool RevokeToken(string token, string ipAddress);
    }
}
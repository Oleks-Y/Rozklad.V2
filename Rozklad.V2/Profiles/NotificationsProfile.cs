using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Rozklad.V2.Entities;
using Rozklad.V2.Models;

namespace Rozklad.V2.Profiles
{
    public class NotificationsProfile : Profile
    {
        public NotificationsProfile()
        {
            CreateMap<NotificationsModel, NotificationsSettings>();
        }
    }
}
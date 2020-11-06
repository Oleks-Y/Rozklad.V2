using AutoMapper;
using Rozklad.V2;

namespace Rozklad.V2.Profiles
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<Models.RegisterModel, Entities.Student>();
            CreateMap<Entities.Student, Models.AuthentificateDto>();
        }
    }
}
using AutoMapper;
using Rozklad.V2;


namespace Rozklad.V2.Profiles
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<Models.RegisterModel, Entities.Student>();
            CreateMap<Entities.Student, Models.AuthentificateDto>()
                .ForMember(dest => dest.Group,
                    opt => opt.MapFrom(src=>src.Group.Group_Name));

            CreateMap<Entities.Student, Models.StudentForUpdateDto>();
            // .ForMember(
            //     dest => dest.Group,
            //     opt => 
            //         opt.MapFrom(src => src.Group.Group_Name));
            CreateMap<Models.StudentForUpdateDto, Entities.Student>();
        }
    }
}
using System;
using System.Collections.Generic;
using AutoMapper;
using Rozklad.V2;
using Rozklad.V2.Entities;


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
            // CreateMap<Models.StudentForUpdateDto, Entities.Student>()
            //     .ForMember(dest=>dest.DisabledSubjects,
            //         opt=>opt.MapFrom(src =>
            //         {
            //             var disabledSubjects = new List<Entities.DisabledSubject>();
            //             foreach (var subjectId in src.DisabledSubjects)
            //             {
            //                 disabledSubjects.Add(new DisabledSubject
            //                 {
            //                     Id = Guid.NewGuid(),
            //                     SubjectId = subjectId,
            //                     
            //                 });
            //             }
            //         }));
            CreateMap<Entities.Student, Models.StudentForUpdateDto>();
        }
    }
}
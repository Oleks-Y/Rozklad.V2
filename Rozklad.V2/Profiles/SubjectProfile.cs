using AutoMapper;

namespace Rozklad.V2.Profiles
{
    public class SubjectProfile : Profile
    {
        public SubjectProfile()
        {
            CreateMap<Entities.Subject, Models.SubjectDto>();
            CreateMap<Entities.Subject, Models.SubjectToUpdate>();
            CreateMap<Models.SubjectToUpdate, Entities.Subject>();
        }
        
    }
}
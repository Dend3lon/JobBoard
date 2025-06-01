using DomainData.Entities;
using DtoModels;
using BusinessModels;
using AutoMapper;

namespace AutoMapperProfiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Resume, ResumeBusinessModel>().ReverseMap();
            CreateMap<ResumeBusinessModel, ResumeDto>().ReverseMap();

            CreateMap<Vacancy, VacancyBusinessModel>().ReverseMap();
            CreateMap<VacancyBusinessModel, VacancyDto>().ReverseMap();

            CreateMap<Application, ApplicationBusinessModel>().ReverseMap();
            CreateMap<ApplicationBusinessModel, ApplicationDto>().ReverseMap();

        }
    }
}

//Update-Database
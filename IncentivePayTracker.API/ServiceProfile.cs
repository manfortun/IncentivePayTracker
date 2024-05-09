using AutoMapper;

namespace IncentivePayTracker.API;

public class ServiceProfile : Profile
{
    public ServiceProfile()
    {
        CreateMap<Models.Employee, DTO.Employee>();
        CreateMap<Models.Infraction, DTO.Infraction>();
        CreateMap<Models.EmployeeInfraction, DTO.EmployeeInfraction>();
        CreateMap<Models.EmploymentDate, DTO.EmploymentDate>();
        CreateMap<Models.EmployeeTimeIn, DTO.EmployeeTimeIn>();

        CreateMap<DTO.Employee, Models.Employee>();
        CreateMap<DTO.Infraction, Models.Infraction>();
        CreateMap<DTO.EmployeeInfraction, Models.EmployeeInfraction>();
        CreateMap<DTO.EmploymentDate, Models.EmploymentDate>();
        CreateMap<DTO.EmployeeTimeIn, Models.EmployeeTimeIn>();
    }
}

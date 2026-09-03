using AutoMapper;
using MicroERP.Application.Features.Payrolls.DTOs;
using MicroERP.Domin.Entities.Payrolls;

namespace MicroERP.Application.Common.Mappings;

public class EmployeeSalaryComponentProfile : Profile
{
    public EmployeeSalaryComponentProfile()
    {
        CreateMap<EmployeeSalaryComponent, EmployeeSalaryComponentDto>()

            .ForMember(
                dest => dest.EmployeeName,
                opt => opt.MapFrom(
                    src => src.Employee.User.FullName))

            .ForMember(
                dest => dest.ComponentName,
                opt => opt.MapFrom(
                    src => src.SalaryComponent.NameAr));
    }
}
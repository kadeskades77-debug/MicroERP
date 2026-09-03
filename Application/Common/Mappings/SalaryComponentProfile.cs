using AutoMapper;
using MicroERP.Application.Features.Payrolls.SalaryComponents.DTOs;
using MicroERP.Domin.Entities.Payrolls;

namespace MicroERP.Application.Common.Mappings;

public class SalaryComponentProfile : Profile
{
    public SalaryComponentProfile()
    {
        CreateMap<CreateSalaryComponentDto, SalaryComponent>();


        CreateMap<UpdateSalaryComponentDto, SalaryComponent>();


        CreateMap<SalaryComponent, SalaryComponentDto>();
    }
}
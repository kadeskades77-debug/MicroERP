using AutoMapper;
using MicroERP.Application.Features.Payrolls.Reports.DTOs;
using MicroERP.Domin.Entities.Payrolls;

namespace MicroERP.Application.Features.Payrolls.Reports.Mappers;

public class PayrollReportProfile : Profile
{
    public PayrollReportProfile()
    {
        CreateMap<Payroll, PayrollSummaryDto>()
            .ForMember(
                d => d.EmployeeName,
                o => o.MapFrom(s => s.Employee.User.FullName))


            .ForMember(
                d => d.Department,
                        o => o.MapFrom(s => s.Employee.Department.NameAr))

            .ForMember(
                dest => dest.PayrollId,
                opt => opt.MapFrom(src => src.Id));
        CreateMap<PayrollItem, PayrollItemDetailsDto>()
    .ForMember(
        d => d.ComponentName,
        o => o.MapFrom(s => s.SalaryComponent.NameAr))

    .ForMember(
        d => d.Type,
        o => o.MapFrom(s => s.SalaryComponent.Type));
    }
}
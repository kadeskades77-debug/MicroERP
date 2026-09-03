using AutoMapper;
using MicroERP.Application.Features.Payrolls.Adjustments.DTOs;


namespace MicroERP.Application.Common.Mappings;

public class PayrollAdjustmentProfile : Profile
{
    public PayrollAdjustmentProfile()
    {
        CreateMap<PayrollAdjustment, PayrollAdjustmentDto>()

            .ForMember(
                dest => dest.EmployeeName,
                opt => opt.MapFrom(
                    src => src.Employee.User.FullName))

            .ForMember(
                dest => dest.Period,
                opt => opt.MapFrom(
                    src => src.PayrollPeriod != null
                        ? $"{src.PayrollPeriod.Month}/{src.PayrollPeriod.Year}"
                        : null));



        CreateMap<CreatePayrollAdjustmentDto, PayrollAdjustment>();



        CreateMap<UpdatePayrollAdjustmentDto, PayrollAdjustment>();
    

    }

}
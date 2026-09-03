using AutoMapper;
using MicroERP.Application.Features.Payrolls.DTOs;
using MicroERP.Domin.Entities.Payrolls;

namespace MicroERP.Application.Common.Mappings;

public class PayrollProfile : Profile
{
    public PayrollProfile()
    {
        CreateMap<PayrollPeriod, PayrollPeriodDto>();


        //================ Payroll List =================

        CreateMap<Payroll, PayrollListDto>()

            .ForMember(
                dest => dest.EmployeeName,
                opt => opt.MapFrom(
                    src => src.Employee.User.FullName))


            .ForMember(
                dest => dest.Period,
                opt => opt.MapFrom(
                    src =>
                    $"{src.PayrollPeriod.Month}/{src.PayrollPeriod.Year}"))


            .ForMember(
                dest => dest.ApprovedByUserName,
                opt => opt.MapFrom(
                    src => src.ApprovedByUser != null
                        ? src.ApprovedByUser.FullName
                        : null))


            .ForMember(
                dest => dest.PaidByUserName,
                opt => opt.MapFrom(
                    src => src.PaidByUser != null
                        ? src.PaidByUser.FullName
                        : null));



        //================ Payroll Details =================

        CreateMap<Payroll, PayrollDetailsDto>()

            .ForMember(
                dest => dest.EmployeeName,
                opt => opt.MapFrom(
                    src => src.Employee.User.FullName))


            .ForMember(
                dest => dest.Period,
                opt => opt.MapFrom(
                    src =>
                    $"{src.PayrollPeriod.Month}/{src.PayrollPeriod.Year}"))


            .ForMember(
                dest => dest.ApprovedByUserName,
                opt => opt.MapFrom(
                    src => src.ApprovedByUser != null
                        ? src.ApprovedByUser.FullName
                        : null))


            .ForMember(
                dest => dest.PaidByUserName,
                opt => opt.MapFrom(
                    src => src.PaidByUser != null
                        ? src.PaidByUser.FullName
                        : null))


            .ForMember(
                dest => dest.Items,
                opt => opt.MapFrom(
                    src => src.PayrollItems));



        //================ Payroll Items =================

        CreateMap<PayrollItem, PayrollItemDto>()

            .ForMember(
                dest => dest.ComponentName,
                opt => opt.MapFrom(
                    src => src.SalaryComponent.NameAr))


            .ForMember(
                dest => dest.Type,
                opt => opt.MapFrom(
                    src => src.SalaryComponent.Type));

        //=============================

        CreateMap<Payroll, PayslipDto>()

    .ForMember(
        dest => dest.PayrollId,
        opt => opt.MapFrom(
            src => src.Id))


    .ForMember(
        dest => dest.EmployeeName,
        opt => opt.MapFrom(
            src => src.Employee.User.FullName))


    .ForMember(
        dest => dest.Period,
        opt => opt.MapFrom(
            src =>
            $"{src.PayrollPeriod.Month}/{src.PayrollPeriod.Year}"))


    .ForMember(
        dest => dest.Status,
        opt => opt.MapFrom(
            src => src.Status.ToString()))


    .ForMember(
        dest => dest.ApprovedByUserName,
        opt => opt.MapFrom(
            src => src.ApprovedByUser != null
                ? src.ApprovedByUser.FullName
                : null))


    .ForMember(
        dest => dest.PaidByUserName,
        opt => opt.MapFrom(
            src => src.PaidByUser != null
                ? src.PaidByUser.FullName
                : null))


    .ForMember(
        dest => dest.Items,
        opt => opt.MapFrom(
            src => src.PayrollItems));



        CreateMap<PayrollItem, PayslipItemDto>()

            .ForMember(
                dest => dest.ComponentName,
                opt => opt.MapFrom(
                    src => src.SalaryComponent.NameAr))


            .ForMember(
                dest => dest.Type,
                opt => opt.MapFrom(
                    src => src.SalaryComponent.Type));
    }
}
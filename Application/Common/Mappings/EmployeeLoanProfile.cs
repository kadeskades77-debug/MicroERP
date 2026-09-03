using AutoMapper;
using MicroERP.Application.Features.Payrolls.EmployeeLoans.DTOs;
using MicroERP.Domin.Entities.Payrolls;


namespace MicroERP.Application.Common.Mappings
{

    public class EmployeeLoanProfile : Profile
    {
        public EmployeeLoanProfile()
        {
            CreateMap<CreateEmployeeLoanDto, EmployeeLoan>();

            CreateMap<EmployeeLoan, EmployeeLoanDto>()
                .ForMember(
                    dest => dest.EmployeeName,
                    opt => opt.MapFrom(
                        src => src.Employee.User.FullName))

                .ForMember(
                    dest => dest.CancelledByUserName,
                    opt => opt.MapFrom(
                        src => src.CancelledByUser != null
                            ? src.CancelledByUser.FullName
                            : null))

                .ForMember(
                    dest => dest.SuspendedByUserName,
                    opt => opt.MapFrom(
                        src => src.SuspendedByUser != null
                            ? src.SuspendedByUser.FullName
                            : null));

            CreateMap<EmployeeLoanInstallment, EmployeeLoanInstallmentDto>();
        }
    }
}


using AutoMapper;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;
using MicroERP.Domin.Entities.EmployeeAttendance;

namespace MicroERP.Application.Common.Mappings
{
    public class AttendanceProfile : Profile
    {
        public AttendanceProfile()
        {
            CreateMap<AttendanceRecord, AttendanceDto>()
             .ForMember(
                 dest => dest.EmployeeName,
                 opt => opt.MapFrom(
                     src => src.Employee.User.FullName
                 )
             );

            CreateMap<CreateAttendanceDto, AttendanceRecord>();

        }
    }
}

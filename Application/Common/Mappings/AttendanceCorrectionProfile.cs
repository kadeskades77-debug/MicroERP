using AutoMapper;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;
using MicroERP.Domin.Entities.EmployeeAttendance;

namespace MicroERP.Application.Common.Mappings;

public class AttendanceCorrectionProfile : Profile
{
    public AttendanceCorrectionProfile()
    {
        CreateMap<AttendanceCorrection, AttendanceCorrectionDto>()
            .ForMember(
                d => d.EmployeeId,
                o => o.MapFrom(s => s.AttendanceRecord.EmployeeId))

            .ForMember(
                d => d.EmployeeName,
                o => o.MapFrom(s => s.AttendanceRecord.Employee.User.FullName))

            .ForMember(
                d => d.Date,
                o => o.MapFrom(s => s.AttendanceRecord.Date))

            .ForMember(
                d => d.OldCheckIn,
                o => o.MapFrom(s => s.OldCheckIn))

            .ForMember(
                d => d.NewCheckIn,
                o => o.MapFrom(s => s.NewCheckIn))

            .ForMember(
                d => d.OldCheckOut,
                o => o.MapFrom(s => s.OldCheckOut))

            .ForMember(
                d => d.NewCheckOut,
                o => o.MapFrom(s => s.NewCheckOut))

            .ForMember(
                d => d.OldStatus,
                o => o.MapFrom(s => s.OldStatus))

            .ForMember(
                d => d.NewStatus,
                o => o.MapFrom(s => s.NewStatus));
    }
}
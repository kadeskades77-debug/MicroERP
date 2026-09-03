using AutoMapper;
using MicroERP.Application.Features.EmployeeAttendance.WorkSchedules.DTOs;
using MicroERP.Domin.Entities.EmployeeAttendance;

namespace MicroERP.Application.Common.Mappings
{
    public class WorkScheduleProfile : Profile
    {
        public WorkScheduleProfile()
        {
            CreateMap<WorkSchedule, WorkScheduleDto>();

            CreateMap<CreateWorkScheduleDto, WorkSchedule>();

            CreateMap<UpdateWorkScheduleDto, WorkSchedule>();
        }
    }
}

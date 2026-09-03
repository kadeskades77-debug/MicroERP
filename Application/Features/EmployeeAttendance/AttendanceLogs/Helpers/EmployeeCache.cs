using MicroERP.Domin.Entities.Employees;

namespace MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Helpers;

public class EmployeeCache
{
    public Dictionary<(int DeviceId, string EmployeeCode), Employee> ByDeviceCode { get; set; }
        = new();


    public Dictionary<int, Employee> ById { get; set; }
        = new();
}
using MicroERP.Application.Features.Employees.DTOs;
using MicroERP.Domin.Entities.Employees;

namespace MicroERP.Application.Common.Mappings;

public static class EmployeeMappings
{
    public static EmployeeDto ToDto(this Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,

            FullName = employee.User.FullName,
            UserName = employee.User.UserName!,
            Email = employee.User.Email,

            Phone = employee.Phone,
            Salary = employee.Salary,

            DepartmentCode = employee.Department.Code,
            DepartmentName = employee.Department.NameEn,

            PositionId = employee.PositionId,
            PositionCode = employee.Position?.Code,
            PositionName = employee.Position?.NameEn,

            IsActive = employee.IsActive
        };
    }


    public static EmployeeListDto ToListDto(
        this Employee employee)
    {
        return new EmployeeListDto
        {
            Id = employee.Id,

            FullName = employee.User.FullName,
            UserName = employee.User.UserName!,
            Email = employee.User.Email,

            Phone = employee.Phone,
            Salary = employee.Salary,

            DepartmentCode = employee.Department.Code,
            DepartmentName = employee.Department.NameAr,

            PositionId = employee.PositionId,
            PositionCode = employee.Position?.Code,
            PositionName = employee.Position?.NameAr,

            IsActive = employee.IsActive
        };
    }
}
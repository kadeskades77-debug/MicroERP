using Domin.Entities;
using MicroERP.Application.Features.Departments.DTOs;

namespace MicroERP.Application.Common.Mappings;

public static class DepartmentMappings
{
    public static DepartmentDto ToDto(this Department department)
    {
        return new DepartmentDto
        {
            Id = department.Id,

            Code = department.Code,

            NameAr = department.NameAr,

            NameEn = department.NameEn,

            ManagerEmployeeId = department.ManagerEmployeeId,

            HasManager = department.HasManager,

            IsActive = department.IsActive
        };
    }


    public static DepartmentListDto ToListDto(this Department department)
    {
        return new DepartmentListDto
        {
            Id = department.Id,

            Code = department.Code,

            NameAr = department.NameAr,

            ManagerName = department.ManagerEmployee != null
                ? department.ManagerEmployee.User.FullName
                : null,

            IsActive = department.IsActive
        };
    }
}
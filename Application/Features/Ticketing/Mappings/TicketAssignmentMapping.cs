using MicroERP.Application.Features.Ticketing.DTOs;
using MicroERP.Domin.Entities.Ticketing;

namespace MicroERP.Application.Features.Ticketing.Mappings;

public static class TicketAssignmentMapping
{
    public static TicketAssignmentDto ToDto(
        this TicketAssignment assignment)
    {
        return new TicketAssignmentDto
        {
            // =====================================================
            // Basic Information
            // =====================================================

            Id =
                assignment.Id,

            TicketId =
                assignment.TicketId,


            // =====================================================
            // Employee
            // =====================================================

            EmployeeId =
                assignment.EmployeeId,

            EmployeeName =
                assignment.Employee?.User.FullName
                ?? string.Empty,


            // =====================================================
            // Department
            // =====================================================

            DepartmentId =
                assignment.DepartmentId,

            DepartmentName =
                assignment.Department?.NameEn,


            // =====================================================
            // Assignment Status
            // =====================================================

            IsCurrent =
                assignment.IsCurrent,


            // =====================================================
            // Dates
            // =====================================================

            CreatedOn =
                assignment.CreatedOn,

            UnassignedOn =
                assignment.UnassignedOn
        };
    }
}
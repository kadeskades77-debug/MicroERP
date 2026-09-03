using MicroERP.Domin.Common;
using MicroERP.Domin.Entities.Employees;
using MicroERP.Domin.Entities.Payrolls;
using MicroERP.Domin.Enums;
using MicroERP.Domin.Identity;

namespace MicroERP.Domin.Entities.EmployeeAttendance;


public class EmployeeOvertime : BaseEntity
{
    // =========================================================
    // Employee
    // =========================================================
    public int EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;


    // =========================================================
    // Overtime Period
    // =========================================================

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public int TotalMinutes { get; set; }


    // =========================================================
    // Calculation
    // =========================================================

    public decimal HourlyRate { get; set; }

    public decimal Multiplier { get; set; }

    public decimal Amount { get; set; }


    // =========================================================
    // Classification
    // =========================================================

    public OvertimeType Type { get; set; }

    public OvertimeStatus Status { get; set; }

    public OvertimeSource Source { get; set; }


    // =========================================================
    // Reason
    // =========================================================

    public string? Reason { get; set; }


    // =========================================================
    // Approval
    // =========================================================

    public string? ApprovedByUserId { get; set; }

    public ApplicationUser? ApprovedByUser { get; set; }

    public DateTime? ApprovedOn { get; set; }


    // =========================================================
    // Rejection
    // =========================================================

    public string? RejectedByUserId { get; set; }

    public ApplicationUser? RejectedByUser { get; set; }

    public DateTime? RejectedOn { get; set; }

    public string? RejectionReason { get; set; }


    // =========================================================
    // Cancellation
    // =========================================================

    public string? CancelledByUserId { get; set; }

    public ApplicationUser? CancelledByUser { get; set; }

    public DateTime? CancelledOn { get; set; }

    public string? CancellationReason { get; set; }


    // =========================================================
    // Payment
    // =========================================================

    public bool IsPaid { get; set; }


    // =========================================================
    // Payroll
    // =========================================================

    public int? PayrollItemId { get; set; }

    public PayrollItem? PayrollItem { get; set; }
}


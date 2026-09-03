using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.AttendanceTransactions.DTOs;

public class AttendanceTransactionDto
{
    public int Id { get; set; }


    public int EmployeeId { get; set; }


    public string EmployeeName { get; set; } = null!;


    public DateOnly Date { get; set; }


    public TimeOnly TransactionTime { get; set; }


    public AttendanceTransactionType Type { get; set; }


    public ShiftNumber ShiftNumber { get; set; }


    public string? Notes { get; set; }


    public DateTime CreatedOn { get; set; }
}
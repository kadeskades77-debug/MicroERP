using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.AttendanceTransactions.DTOs;

public class CreateAttendanceTransactionDto
{
    public int EmployeeId { get; set; }


    public TimeOnly TransactionTime { get; set; }


    public AttendanceTransactionType Type { get; set; }


    public string? Notes { get; set; }
}
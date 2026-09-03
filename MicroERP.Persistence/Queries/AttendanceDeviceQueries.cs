using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Queries;

public class AttendanceDeviceQueries : IAttendanceDeviceQueries
{
    private readonly IApplicationDbContext _context;


    public AttendanceDeviceQueries(
        IApplicationDbContext context)
    {
        _context = context;
    }



    public async Task<Result<List<AttendanceDeviceDto>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await _context.AttendanceDevices
            .AsNoTracking()
            .Select(x => new AttendanceDeviceDto
            {
                Id = x.Id,

                Name = x.Name,

                DeviceCode = x.DeviceCode,

                IpAddress = x.IpAddress,

                Location = x.Location,

                IsActiveDevice = x.IsActiveDevice,

                ConnectionType = x.ConnectionType
            })
            .ToListAsync(cancellationToken);


        return Result<List<AttendanceDeviceDto>>
            .Succeeded(result);
    }




    public async Task<Result<AttendanceDeviceDto>> GetByIdAsync(int id,
        CancellationToken cancellationToken = default)
    {
        var device = await _context.AttendanceDevices
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new AttendanceDeviceDto
            {
                Id = x.Id,

                Name = x.Name,

                DeviceCode = x.DeviceCode,

                IpAddress = x.IpAddress,

                Location = x.Location,

                IsActiveDevice = x.IsActiveDevice,

                ConnectionType = x.ConnectionType
            })
            .FirstOrDefaultAsync(cancellationToken);



        if (device == null)
        {
            return Result<AttendanceDeviceDto>
                .Failure("Attendance device not found");
        }


        return Result<AttendanceDeviceDto>
            .Succeeded(device);
    }
}
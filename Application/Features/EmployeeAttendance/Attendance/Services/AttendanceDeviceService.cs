using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Domin.Entities.EmployeeAttendance;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Services;

public class AttendanceDeviceService : IAttendanceDeviceService
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public AttendanceDeviceService(
        IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }



    public async Task<Result<AttendanceDeviceDto>> CreateAsync(
        CreateAttendanceDeviceDto dto,
        CancellationToken cancellationToken = default)
    {
        var exists = await _context.AttendanceDevices
            .AnyAsync(
                x => x.DeviceCode == dto.DeviceCode,
                cancellationToken);


        if (exists)
        {
            return Result<AttendanceDeviceDto>
                .Failure("Device code already exists");
        }



        var device = new AttendanceDevice
        {
            Name = dto.Name,

            DeviceCode = dto.DeviceCode,

            IpAddress = dto.IpAddress,

            Location = dto.Location,

            ConnectionType = dto.ConnectionType,

            IsActiveDevice = true
        };


        _context.AttendanceDevices.Add(device);


        await _context.SaveChangesAsync(cancellationToken);



        return Result<AttendanceDeviceDto>.Succeeded(
            new AttendanceDeviceDto
            {
                Id = device.Id,

                Name = device.Name,

                DeviceCode = device.DeviceCode,

                IpAddress = device.IpAddress,

                Location = device.Location,

                ConnectionType = device.ConnectionType,

                IsActiveDevice = device.IsActiveDevice
            });
    }


    public async Task<Result<AttendanceDeviceDto>> UpdateAsync(int id,
    UpdateAttendanceDeviceDto dto,
    CancellationToken cancellationToken = default)
    {
        var device =
            await _context.AttendanceDevices
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);


        if (device == null)
        {
            return Result<AttendanceDeviceDto>
                .Failure("Device not found");
        }



        var oldValues = new
        {
            device.Name,
            device.IpAddress,
            device.Location,
            device.ConnectionType,
            device.IsActiveDevice
        };



        if (!string.IsNullOrWhiteSpace(dto.Name))
            device.Name = dto.Name;


        if (!string.IsNullOrWhiteSpace(dto.IpAddress))
            device.IpAddress = dto.IpAddress;


        if (!string.IsNullOrWhiteSpace(dto.Location))
            device.Location = dto.Location;


        if (dto.IsActiveDevice.HasValue)
            device.IsActiveDevice =
                dto.IsActiveDevice.Value;


        if (dto.ConnectionType.HasValue)
            device.ConnectionType =
                dto.ConnectionType.Value;



        var newValues = new
        {
            device.Name,
            device.IpAddress,
            device.Location,
            device.ConnectionType,
            device.IsActiveDevice
        };


        await _auditService.LogAsync(
      "Update",
      "AttendanceDevice",
      device.Id.ToString(),
      oldValues,
      newValues);


        await _context.SaveChangesAsync(
            cancellationToken);




        return Result<AttendanceDeviceDto>
            .Succeeded(
            new AttendanceDeviceDto
            {
                Id = device.Id,
                Name = device.Name,
                DeviceCode = device.DeviceCode,
                IpAddress = device.IpAddress,
                Location = device.Location,
                ConnectionType = device.ConnectionType,
                IsActiveDevice = device.IsActiveDevice
            });
    }


    public async Task<Result<bool>> DeleteAsync(int id,
        CancellationToken cancellationToken = default)
    {
        var device =
            await _context.AttendanceDevices
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);


        if (device == null)
        {
            return Result<bool>
                .Failure("Device not found");
        }



        var oldValues = new
        {
            device.Name,
            device.DeviceCode,
            device.IpAddress,
            device.Location
        };



        device.IsDeleted = true;

        device.IsActiveDevice = false;

        await _auditService.LogAsync(
           "Delete",
           "AttendanceDevice",
           device.Id.ToString(),
           oldValues,
           null);

        await _context.SaveChangesAsync(
            cancellationToken);

      

        return Result<bool>
            .Succeeded(true);
    }

    public async Task<Result> ActivateAsync(int id,
    CancellationToken cancellationToken = default)
    {
        var device =
            await _context.AttendanceDevices
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     !x.IsDeleted,
                cancellationToken);


        if (device == null)
        {
            return Result.Failure(
                "Device not found.");
        }


        if (device.IsActiveDevice)
        {
            return Result.Failure(
                "Device is already active.");
        }


        var oldValues = new
        {
            device.IsActiveDevice
        };


        device.IsActiveDevice = true;


        var newValues = new
        {
            device.IsActiveDevice
        };


        await _auditService.LogAsync(
            "Activate",
            "AttendanceDevice",
            device.Id.ToString(),
            oldValues,
            newValues);


        await _context.SaveChangesAsync(
            cancellationToken);


        return Result.Succeeded(
            "Device activated successfully.");
    }

    public async Task<Result> DeactivateAsync(int id,
    CancellationToken cancellationToken = default)
    {
        var device =
            await _context.AttendanceDevices
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     !x.IsDeleted,
                cancellationToken);


        if (device == null)
        {
            return Result.Failure(
                "Device not found.");
        }


        if (!device.IsActiveDevice)
        {
            return Result.Failure(
                "Device is already inactive.");
        }


        var oldValues = new
        {
            device.IsActiveDevice
        };


        device.IsActiveDevice = false;


        var newValues = new
        {
            device.IsActiveDevice
        };


        await _auditService.LogAsync(
            "Deactivate",
            "AttendanceDevice",
            device.Id.ToString(),
            oldValues,
            newValues);


        await _context.SaveChangesAsync(
            cancellationToken);


        return Result.Succeeded(
            "Device deactivated successfully.");
    }
}
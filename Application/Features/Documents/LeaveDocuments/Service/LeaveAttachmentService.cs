using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Documents.LeaveDocuments.DTOS;
using MicroERP.Application.Features.Documents.LeaveDocuments.Interfaces;
using MicroERP.Domin.Entities;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MicroERP.Application.Features.Documents.LeaveDocuments.Service
{
    public class LeaveAttachmentService : ILeaveAttachmentService
    {
        private readonly IApplicationDbContext _context;
        private readonly IFileStorageService _fileStorage;


        public LeaveAttachmentService(
            IApplicationDbContext context,
            IFileStorageService fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }



        public async Task<Result<LeaveAttachmentDto>> UploadEmployeeLeaveAsync(int leaveId,UploadLeaveAttachmentDto dto,
            CancellationToken cancellationToken = default)
        {
            var leave = await _context.EmployeeLeaves
           .FirstOrDefaultAsync(
             x => x.Id == leaveId,
             cancellationToken);


            if (leave == null)
                return Result<LeaveAttachmentDto>.Failure(
                    "Leave not found");


            if (leave == null)
                return Result<LeaveAttachmentDto>.Failure(
                    "Leave not found");



            var filePath = await _fileStorage.SaveFileAsync(
                dto.File,
                "Leaves/EmployeeLeave",
                cancellationToken);



            var attachment = new LeaveAttachment
            {
                FileName = dto.File.FileName,
                FilePath = filePath,
                EmployeeLeaveId = leave.Id
            };


            _context.LeaveAttachments.Add(attachment);


            await _context.SaveChangesAsync(cancellationToken);



            return Result<LeaveAttachmentDto>.Succeeded(
                new LeaveAttachmentDto
                {
                    Id = attachment.Id,
                    FileName = attachment.FileName,
                    FilePath = attachment.FilePath,
                    EmployeeLeaveId = attachment.EmployeeLeaveId,
                    CreatedOn = attachment.CreatedOn
                });
        }




        public async Task<Result<LeaveAttachmentDto>> UploadEmployeeSpecialLeaveAsync(int leaveId,UploadLeaveAttachmentDto dto,
            CancellationToken cancellationToken = default)
        {
            var leave = await _context.EmployeeSpecialLeaves
                .FirstOrDefaultAsync(
                    x => x.Id == leaveId,
                    cancellationToken);


            if (leave == null)
                return Result<LeaveAttachmentDto>.Failure(
                    "Special leave not found");



            var filePath = await _fileStorage.SaveFileAsync(
                dto.File,
                "Leaves/SpecialLeave",
                cancellationToken);



            var attachment = new LeaveAttachment
            {
                FileName = dto.File.FileName,
                FilePath = filePath,
                EmployeeSpecialLeaveId = leave.Id
            };


            _context.LeaveAttachments.Add(attachment);


            await _context.SaveChangesAsync(cancellationToken);



            return Result<LeaveAttachmentDto>.Succeeded(
                new LeaveAttachmentDto
                {
                    Id = attachment.Id,
                    FileName = attachment.FileName,
                    FilePath = attachment.FilePath,
                    EmployeeSpecialLeaveId = attachment.EmployeeSpecialLeaveId,
                    CreatedOn = attachment.CreatedOn
                });
        }



        public async Task<Result<List<LeaveAttachmentDto>>> GetByLeaveAsync(int leaveId,LeaveCategory category,
      CancellationToken cancellationToken = default)
        {
            IQueryable<LeaveAttachment> query;

            if (category == LeaveCategory.Regular)
            {
                query = _context.LeaveAttachments
                    .Where(x => x.EmployeeLeaveId == leaveId);
            }
            else if (category == LeaveCategory.Special)
            {
                query = _context.LeaveAttachments
                    .Where(x => x.EmployeeSpecialLeaveId == leaveId);
            }
            else
            {
                return Result<List<LeaveAttachmentDto>>
                    .Failure("Invalid leave category.");
            }

            var attachments = await query
                .Select(x => new LeaveAttachmentDto
                {
                    Id = x.Id,
                    FileName = x.FileName,
                    FilePath = x.FilePath,
                    EmployeeLeaveId = x.EmployeeLeaveId,
                    EmployeeSpecialLeaveId = x.EmployeeSpecialLeaveId,
                    Category = x.EmployeeLeaveId != null
                        ? LeaveCategory.Regular
                        : LeaveCategory.Special,
                    CreatedOn = x.CreatedOn
                })
                .ToListAsync(cancellationToken);

            return Result<List<LeaveAttachmentDto>>
                .Succeeded(attachments);
        }


        public async Task<Result> DeleteAsync(int attachmentId,
             CancellationToken cancellationToken = default)
        {
            var attachment = await _context.LeaveAttachments
                .FirstOrDefaultAsync(
                    x => x.Id == attachmentId,
                    cancellationToken);


            if (attachment == null)
                return Result.Failure(
                    "Attachment not found");


            await _fileStorage.DeleteFileAsync(
                attachment.FilePath,
                cancellationToken);


            _context.LeaveAttachments.Remove(attachment);


            await _context.SaveChangesAsync(cancellationToken);


            return Result.Succeeded();
        }
    }
}

using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Authentication.Auth.DTOs;

namespace MicroERP.Application.Features.Authentication.Auth.Interfaces
{
    public interface IdentityService
    {
        Task<Result> RegisterAsync(RegisterDto dto);
        Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto);
        Task<Result<CreateEmployeeUserResultDto>>CreateEmployeeUserAsync(CreateEmployeeUserDto dto);
        Task<Result> DeleteUserAsync(string userId);
        Task<Result> ActivateUserAsync(string userId);
        Task<Result> DeactivateUserAsync(string userId);
        Task<Result> UpdateEmployeeUserAsync(string userId,string? fullName);
        Task<Result> ChangePasswordAsync(ChangePasswordDto dto);
        Task<Result<ResetPasswordResultDto>> ResetPasswordAsync(string userId);
        Task<Result> ChangeEmailAsync(ChangeEmailDto dto);
        Task<Result> LockUserAsync(string userId);
        Task<Result> UnlockUserAsync(string userId);
    }
}

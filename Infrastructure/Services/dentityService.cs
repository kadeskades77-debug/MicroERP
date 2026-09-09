using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Authorization.Auth.DTOs;
using MicroERP.Application.Features.Authorization.Auth.Interfaces;
using MicroERP.Domain.Audit;
using MicroERP.Domain.Identity;
using MicroERP.Domin.Identity;
using MicroERP.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace MicroERP.Infrastructure.Services;

public class dentityService : IdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ICredentialGenerator _credentialGenerator;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAuthorizationManager _authorizationManager;
    private readonly IAuditService _auditService;
    private readonly IApplicationDbContext _context;
    public dentityService(UserManager<ApplicationUser> userManager, IJwtService jwtService, RoleManager<ApplicationRole> roleManager, ICredentialGenerator credentialGenerator, SignInManager<ApplicationUser> signInManager, IAuthorizationManager authorizationManager, IAuditService auditService, IApplicationDbContext context)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _roleManager = roleManager;
        _credentialGenerator = credentialGenerator;
        _signInManager = signInManager;
        _authorizationManager = authorizationManager;
        _auditService = auditService;
        _context = context;
    }

    public async Task<Result> RegisterAsync(RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            FullName = dto.FullName,
            Email = dto.Email,
            UserName = dto.Email
        };

        var result = await _userManager.CreateAsync(
            user,
            dto.Password);

        if (!result.Succeeded)
        {
            return Result.Failure(
                string.Join(
                    ", ",
                    result.Errors.Select(x => x.Description)));
        }
        await _auditService.LogAsync(
             AuditActions.Register,
             nameof(ApplicationUser),
             user.Id,
             null,
             new
             {
                 user.UserName,
                 user.Email,
                 user.FullName
             });
        await _context.SaveChangesAsync();
        return Result.Succeeded(
            "User created successfully");
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.UserName);

        if (user is null)
        {
            return Result<AuthResponseDto>.Failure(
                "Invalid User Name or password.");
        }

        if (!user.IsActive)
        {
            return Result<AuthResponseDto>.Failure(
                "This account is inactive.");
        }

        var signInResult = await _signInManager.PasswordSignInAsync(
            user.UserName!,
            dto.Password,
            isPersistent: false,
            lockoutOnFailure: true);

        if (signInResult.IsLockedOut)
        {
            return Result<AuthResponseDto>.Failure(
                "Your account has been locked due to multiple failed login attempts. Please try again after 10 minutes.");
        }

        if (!signInResult.Succeeded)
        {
            return Result<AuthResponseDto>.Failure(
                "Invalid email or password.");
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        var roles = await _userManager.GetRolesAsync(user);

        var token = await _jwtService.GenerateTokenAsync(user, roles);

        var response = new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles.ToList()
        };
        await _auditService.LogAsync(
            AuditActions.Login,
            nameof(ApplicationUser),
            user.Id,
            null,
            new
            {
                user.UserName,
                user.Email,
                user.FullName
            });
        return Result<AuthResponseDto>.Succeeded(
            response,
            "Login successful.");
    }
    public async Task<Result<CurrentUserDto>> GetCurrentUserAsync(string userId)
{
    var user = await _userManager.FindByIdAsync(userId);

    if (user is null)
        return Result<CurrentUserDto>.Failure(
            "User not found.");

    if (!user.IsActive)
        return Result<CurrentUserDto>.Failure(
            "This account is inactive.");

    var roles = await _userManager.GetRolesAsync(user);

    var permissions =
        await _authorizationManager
            .GetPermissionsByUserAsync(userId);

    var response = new CurrentUserDto
    {
        Email = user.Email ?? string.Empty,
        FullName = user.FullName,
        Roles = roles.ToList(),
        Permissions = permissions.ToList()
    };

    return Result<CurrentUserDto>.Succeeded(response);
}
    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure("User not found.");

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return Result.Failure(
                string.Join(", ",
                    result.Errors.Select(x => x.Description)));
        }

        return Result.Succeeded();
    }
    public async Task<Result> UpdateEmployeeUserAsync(string userId,string? fullName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure("User not found.");

        if (!string.IsNullOrWhiteSpace(fullName))
            user.FullName = fullName.Trim();

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return Result.Failure(
                string.Join(", ",
                    result.Errors.Select(x => x.Description)));
        }

        return Result.Succeeded();
    }
    public async Task<Result> ActivateUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure("User not found.");

        user.IsActive = true;
        user.LockoutEnd = null;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return Result.Failure(
                string.Join(", ", result.Errors.Select(x => x.Description)));
        }
        await _auditService.LogAsync(
             AuditActions.Activate,
             nameof(ApplicationUser),
             user.Id,
             new
             {
                 IsActive = false
             },
             new
             {
                 IsActive = true
             });
        await _context.SaveChangesAsync();
        return Result.Succeeded();
    }
    public async Task<Result> DeactivateUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure("User not found.");

        user.IsActive = false;

        user.LockoutEnabled = true;
        user.LockoutEnd = DateTimeOffset.MaxValue;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return Result.Failure(
                string.Join(", ", result.Errors.Select(x => x.Description)));
        }
        await _auditService.LogAsync(
              AuditActions.Deactivate,
              nameof(ApplicationUser),
              user.Id,
              new
              {
                  IsActive = true
              },
              new
              {
                  IsActive = false
              });
        await _context.SaveChangesAsync();
        return Result.Succeeded();
    }
    public async Task<Result<CreateEmployeeUserResultDto>> CreateEmployeeUserAsync(CreateEmployeeUserDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var emailExists =
                await _userManager.FindByEmailAsync(dto.Email);

            if (emailExists != null)
            {
                return Result<CreateEmployeeUserResultDto>
                    .Failure("Email already exists.");
            }
        }

        var userName = await GetUniqueUserNameAsync(dto.FullName);

        var password = _credentialGenerator.GeneratePassword(userName);

        var user = new ApplicationUser
        {
            FullName = dto.FullName,
            UserName = userName,
            Email = dto.Email,
            IsActive = true
        };

        var result =  await _userManager.CreateAsync(user, password);
        var roleResult = await _userManager.AddToRoleAsync(user,SystemRoles.Employee);

        if (!roleResult.Succeeded)
        {
            return Result<CreateEmployeeUserResultDto>.Failure(
                string.Join(", ",
                    roleResult.Errors.Select(x => x.Description)));
        }

        if (!result.Succeeded)
        {
            return Result<CreateEmployeeUserResultDto>
                .Failure(
                    string.Join(
                        ", ",
                        result.Errors.Select(x => x.Description)));
        }

        return Result<CreateEmployeeUserResultDto>
            .Succeeded(
                new CreateEmployeeUserResultDto
                {
                    UserId = user.Id,
                    UserName = user.UserName!,
                    Password = password
                },
                "User created successfully");
    }
    public async Task<Result> ChangePasswordAsync(ChangePasswordDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);

        if (user is null)
            return Result.Failure("User not found.");

        var result = await _userManager.ChangePasswordAsync(
            user,
            dto.CurrentPassword,
            dto.NewPassword);

        if (!result.Succeeded)
        {
            return Result.Failure(
                string.Join(", ",
                    result.Errors.Select(x => x.Description)));
        }

        return Result.Succeeded("Password changed successfully.");
    }
    public async Task<Result<ResetPasswordResultDto>> ResetPasswordAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result<ResetPasswordResultDto>.Failure("User not found.");

        var password = _credentialGenerator.GeneratePassword(user.FullName);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(
            user,
            token,
            password);

        if (!result.Succeeded)
        {
            return Result<ResetPasswordResultDto>.Failure(
                string.Join(", ",
                    result.Errors.Select(x => x.Description)));
        }
          await _auditService.LogAsync(
            AuditActions.ResetPassword,
            nameof(ApplicationUser),
            user.Id);
        await _context.SaveChangesAsync();
        return Result<ResetPasswordResultDto>.Succeeded(
            new ResetPasswordResultDto
            {
                Password = password
            },
            "Password reset successfully.");

    }
    public async Task<Result> ChangeEmailAsync(ChangeEmailDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);

        if (user is null)
            return Result.Failure("User not found.");

        user.Email = dto.Email.Trim().ToLower();
        var oldEmail = user.Email;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return Result.Failure(
                string.Join(", ",
                    result.Errors.Select(x => x.Description)));
        }
         await _auditService.LogAsync(
             AuditActions.ChangeEmail,
             nameof(ApplicationUser),
             user.Id,
             new
             {
                 Email = oldEmail
             },
             new
             {
                 Email = user.Email
             });
        await _context.SaveChangesAsync();
        return Result.Succeeded("Email changed successfully.");
    }
    public async Task<Result> LockUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure("User not found.");

        user.LockoutEnabled = true;
        user.LockoutEnd = DateTimeOffset.MaxValue;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return Result.Failure(
                string.Join(", ",
                    result.Errors.Select(x => x.Description)));
        }
        await _auditService.LogAsync(
          AuditActions.Lock,
          nameof(ApplicationUser),
          user.Id,
          null,
          new
          {
              user.LockoutEnd
          });
        await _context.SaveChangesAsync();
        return Result.Succeeded("User locked successfully.");
    }
    public async Task<Result> UnlockUserAsync(string userId)
{
    var user = await _userManager.FindByIdAsync(userId);

    if (user is null)
        return Result.Failure("User not found.");
        user.LockoutEnabled = false;
        user.LockoutEnd = null;

    var result = await _userManager.UpdateAsync(user);

    if (!result.Succeeded)
    {
        return Result.Failure(
            string.Join(", ",
                result.Errors.Select(x => x.Description)));
    }
        await _auditService.LogAsync(
        AuditActions.Unlock,
        nameof(ApplicationUser),
        user.Id,
        null,
        new
        {
            user.LockoutEnd
        });
        await _context.SaveChangesAsync();
        return Result.Succeeded("User unlocked successfully.");
}
    private async Task<string> GetUniqueUserNameAsync(string fullName)
    {
        var baseUserName =_credentialGenerator.GenerateUserName(fullName);

        var userName = baseUserName;

        var counter = 1;

        while (await _userManager.FindByNameAsync(userName) != null)
        {
            userName = $"{baseUserName}{counter}";
            counter++;
        }

        return userName;
    }
}
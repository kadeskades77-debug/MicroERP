using System.Security.Claims;

namespace MicroERP.Application.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException(
                "User id not found");

        return userId;
    }


    public static int GetEmployeeId(this ClaimsPrincipal user)
    {
        var employeeId = user.FindFirstValue(
            "EmployeeId");

        if (string.IsNullOrEmpty(employeeId))
            throw new UnauthorizedAccessException(
                "Employee id not found");

        return int.Parse(employeeId);
    }
}
using MicroERP.Domin.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroERP.Application.Features.Authentication.Auth.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(
     ApplicationUser user,
     IList<string> roles);
    }
}

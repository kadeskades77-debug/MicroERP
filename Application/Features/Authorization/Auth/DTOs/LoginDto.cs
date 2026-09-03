using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroERP.Application.Features.Authentication.Auth.DTOs
{
    public class LoginDto
    {
        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}

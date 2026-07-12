using MicroERP.Application.Common.Models;

namespace MicroERP.Application.Common.Interfaces
{
    public interface ICredentialGenerator
    {
        public string GenerateUserName(string fullName);
        string GeneratePassword(string fullName);
    }
}

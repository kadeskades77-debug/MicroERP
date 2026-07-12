using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;

namespace MicroERP.Infrastructure.Security
{
    public class CredentialsGenerator : ICredentialGenerator
    {
        public  string GenerateUserName(string fullName)
        {
            var parts = fullName
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                throw new ArgumentException("Full name is required.");

            // إذا كان الاسم كلمة واحدة
            if (parts.Length == 1)
                return Capitalize(parts[0]);

            var firstLetter = char.ToUpper(parts[0][0]);

            var lastName = Capitalize(parts[^1]);

            return $"{firstLetter}{lastName}";
        }

        public string GeneratePassword(string fullName)
        {
            var name = fullName.Trim();

            var upperLetter = char.ToUpper(name[0]);

            var lowerLetter = char.ToLower(
                name.Length > 1
                    ? name[1]
                    : name[0]);

            var random = Random.Shared.Next(100000, 999999);

            return $"{upperLetter}{lowerLetter}@{random}";
        }

        private  string Capitalize(string value)
        {
            value = value.Trim().ToLower();

            return char.ToUpper(value[0]) + value[1..];
        }
    }
}
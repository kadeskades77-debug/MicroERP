

using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.SalaryComponents.DTOs
{
    public class UpdateSalaryComponentDto
    {
        public string? NameAr { get; set; }

        public string? NameEn { get; set; }

        public string? Code { get; set; }

        public SalaryComponentType? Type { get; set; }

        public CalculationType? CalculationType { get; set; }

        public bool? IsTaxable { get; set; }

        public bool? IsAttendanceRelated { get; set; }

        public bool? IsDefault { get; set; }
    }
}

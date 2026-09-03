

using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.SalaryComponents.DTOs
{
    public class SalaryComponentDto
    {
        public int Id { get; set; }

        public string NameAr { get; set; } = null!;

        public string NameEn { get; set; } = null!;


        public string Code { get; set; } = null!;


        public SalaryComponentType Type { get; set; }


        public CalculationType CalculationType { get; set; }


        public bool IsTaxable { get; set; }


        public bool IsAttendanceRelated { get; set; }


        public bool IsDefault { get; set; }
    }
}



namespace MicroERP.Application.Features.EmployeeEvaluations.DTOs.EmployeeEvaluationDto
{
    

    public class EmployeeEvaluationStatisticsDto
    {
        public int TotalEmployees { get; set; }

        public int EvaluatedEmployees { get; set; }

        public int DraftEvaluations { get; set; }

        public int InProgressEvaluations { get; set; }

        public int SubmittedEvaluations { get; set; }

        public int ApprovedEvaluations { get; set; }

        public int RejectedEvaluations { get; set; }

        public decimal AverageScore { get; set; }

        public decimal HighestScore { get; set; }

        public decimal LowestScore { get; set; }

        public int ExcellentEmployees { get; set; }

        public int VeryGoodEmployees { get; set; }

        public int GoodEmployees { get; set; }

        public int AcceptableEmployees { get; set; }

        public int PoorEmployees { get; set; }
    }
}

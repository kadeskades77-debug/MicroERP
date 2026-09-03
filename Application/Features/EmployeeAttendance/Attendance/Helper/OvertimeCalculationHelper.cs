namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Helper
{
    public static class OvertimeCalculationHelper
    {
        public static decimal CalculateAmount(
            int totalMinutes,
            decimal hourlyRate,
            decimal multiplier)
        {
            if (totalMinutes < 0)
                throw new ArgumentException(
                    "Total minutes cannot be negative.");

            if (hourlyRate < 0)
                throw new ArgumentException(
                    "Hourly rate cannot be negative.");

            if (multiplier <= 0)
                throw new ArgumentException(
                    "Multiplier must be greater than zero.");

            var amount =
                totalMinutes / 60m *
                hourlyRate *
                multiplier;

            return Round(amount);
        }


        public static decimal CalculateMultiplier(
            int totalMinutes,
            decimal hourlyRate,
            decimal amount)
        {
            if (totalMinutes <= 0)
                throw new ArgumentException(
                    "Total minutes must be greater than zero.");

            if (hourlyRate <= 0)
                throw new ArgumentException(
                    "Hourly rate must be greater than zero.");

            if (amount < 0)
                throw new ArgumentException(
                    "Amount cannot be negative.");

            var multiplier =
                amount /
                (totalMinutes / 60m * hourlyRate);

            return Round(multiplier, 4);
        }


        public static decimal CalculateHourlyRate(
            int totalMinutes,
            decimal multiplier,
            decimal amount)
        {
            if (totalMinutes <= 0)
                throw new ArgumentException(
                    "Total minutes must be greater than zero.");

            if (multiplier <= 0)
                throw new ArgumentException(
                    "Multiplier must be greater than zero.");

            if (amount < 0)
                throw new ArgumentException(
                    "Amount cannot be negative.");

            var hourlyRate =
                amount /
                (totalMinutes / 60m * multiplier);

            return Round(hourlyRate, 2);
        }


        private static decimal Round(
            decimal value,
            int decimals = 2)
        {
            return decimal.Round(
                value,
                decimals,
                MidpointRounding.AwayFromZero);
        }

    }
}

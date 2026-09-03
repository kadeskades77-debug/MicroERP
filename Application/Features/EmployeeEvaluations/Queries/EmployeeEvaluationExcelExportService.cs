using MicroERP.Application.Common.Excel.Interfaces;
using MicroERP.Application.Common.Excel.Models;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeEvaluations.DTOs.EmployeeEvaluationDto;
using MicroERP.Application.Features.EmployeeEvaluations.Queries.MicroERP.Application.Features.EmployeeEvaluations.Excel;
using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeEvaluations.Queries
{
    public class EmployeeEvaluationExcelExportService
     : IEmployeeEvaluationExcelExportService
    {
        private readonly IEmployeeEvaluationQueries _queries;
        private readonly IExcelExportService _excelExportService;

        public EmployeeEvaluationExcelExportService(
            IEmployeeEvaluationQueries queries,
            IExcelExportService excelExportService)
        {
            _queries = queries;
            _excelExportService = excelExportService;
        }


        // =========================================================
        // All Employees
        // =========================================================

        public async Task<Result<byte[]>>
        ExportAllEmployeesAsync(
        EmployeeEvaluationExcelFilterDto filter,
        CancellationToken ct = default)
        {
            var result =
                await _queries.GetForExcelAsync(
                    filter,
                    ct);

            if (!result.Success)
            {
                return Result<byte[]>.Failure(
                    result.Message);
            }

            if (result.Data == null ||
                result.Data.Count == 0)
            {
                return Result<byte[]>.Failure(
                    "No employee evaluations found for the specified filters.");
            }

            var sheet =
                BuildAllEmployeesSheet(
                    result.Data,
                    filter);

            var file =
                _excelExportService.Export(sheet);

            if (file == null || file.Length == 0)
            {
                return Result<byte[]>.Failure(
                    "Failed to generate Excel file.");
            }

            return Result<byte[]>.Succeeded(file);
        }


        // =========================================================
        // Employee History
        // =========================================================

        public async Task<Result<EmployeeEvaluationExcelFileResult>>
        ExportEmployeeHistoryAsync(
         EmployeeEvaluationHistoryExcelFilterDto filter,
         CancellationToken ct = default)
        {
            var result =
                await _queries.GetEmployeeHistoryForExcelAsync(
                    filter,
                    ct);

            if (!result.Success)
            {
                return Result<EmployeeEvaluationExcelFileResult>
                    .Failure(result.Message);
            }

            if (result.Data == null)
            {
                return Result<EmployeeEvaluationExcelFileResult>
                    .Failure(
                        "No employee evaluation history found.");
            }

            if (result.Data.Evaluations == null ||
                result.Data.Evaluations.Count == 0)
            {
                return Result<EmployeeEvaluationExcelFileResult>
                    .Failure(
                        "No employee evaluation history found for the specified filters.");
            }

            var sheet =
                BuildEmployeeHistorySheet(
                    result.Data);

            if (sheet == null)
            {
                return Result<EmployeeEvaluationExcelFileResult>
                    .Failure(
                        "Failed to build employee history Excel sheet.");
            }

            var file =
                _excelExportService.Export(sheet);

            if (file == null || file.Length == 0)
            {
                return Result<EmployeeEvaluationExcelFileResult>
                    .Failure(
                        "Failed to generate employee history Excel file.");
            }

            return Result<EmployeeEvaluationExcelFileResult>
                .Succeeded(
                    new EmployeeEvaluationExcelFileResult
                    {
                        File =
                            file,

                        EmployeeName =
                            result.Data.Header.EmployeeName
                    });
        }


        // =========================================================
        // EmployeeMonthly
        // =========================================================

        public async Task<Result<EmployeeEvaluationExcelFileResult>>
        ExportEmployeeMonthlyAsync(
        EmployeeMonthlyEvaluationExcelFilterDto filter,
        CancellationToken ct = default)
        {
            var result =
                await _queries.GetEmployeeMonthlyReportAsync(
                    filter,
                    ct);

            if (!result.Success)
            {
                return Result<EmployeeEvaluationExcelFileResult>
                    .Failure(result.Message);
            }

            if (result.Data == null)
            {
                return Result<EmployeeEvaluationExcelFileResult>
                    .Failure(
                        "No employee monthly evaluation found.");
            }

            var sheet =
                BuildEmployeeMonthlySheet(
                    result.Data);

            if (sheet == null)
            {
                return Result<EmployeeEvaluationExcelFileResult>
                    .Failure(
                        "Failed to build employee monthly evaluation Excel sheet.");
            }

            var file =
                _excelExportService.Export(sheet);

            if (file == null || file.Length == 0)
            {
                return Result<EmployeeEvaluationExcelFileResult>
                    .Failure(
                        "Failed to generate employee monthly evaluation Excel file.");
            }

            return Result<EmployeeEvaluationExcelFileResult>
                .Succeeded(
                    new EmployeeEvaluationExcelFileResult
                    {
                        File = file,

                        EmployeeName =
                            result.Data.Header.EmployeeName
                    });
        }

        // =========================================================
        // Department
        // =========================================================

        public async Task<Result<byte[]>>
         ExportDepartmentAsync(
         DepartmentEvaluationExcelFilterDto filter,
         CancellationToken ct = default)
        {
            var result =
                await _queries.GetDepartmentForExcelAsync(
                    filter,
                    ct);

            if (!result.Success)
            {
                return Result<byte[]>.Failure(
                    result.Message);
            }

            if (result.Data == null)
            {
                return Result<byte[]>.Failure(
                    "No department evaluation data found.");
            }

            var sheet =
                BuildDepartmentSheet(
                    result.Data);

            var file =
                _excelExportService.Export(sheet);

            if (file == null || file.Length == 0)
            {
                return Result<byte[]>.Failure(
                    "Failed to generate Excel file.");
            }

            return Result<byte[]>.Succeeded(file);
        }


        // =========================================================
        // All Employees Sheet
        // =========================================================

        private static ExcelSheetData BuildAllEmployeesSheet(
            IReadOnlyCollection<EmployeeEvaluationExcelDto> data,
            EmployeeEvaluationExcelFilterDto filter)
        {
            var topRows =
                BuildCommonPeriodRows(
                    filter.Year,
                    filter.Period,
                    filter.Month,
                    filter.Quarter);


            var rows =
                data
                    .Select(x =>
                        new ExcelSheetRow
                        {
                            Type =
                                ExcelRowType.Data,

                            Values =
                                new object?[]
                                {
                                x.EmployeeName,
                                x.DepartmentName,
                                x.PeriodLabel,
                                x.TotalScore,
                                x.FinalRate.ToString(),
                                x.Status.ToString()
                                }
                        })
                    .ToList();


            return new ExcelSheetData
            {
                SheetName =
                    "Employee Evaluations",

                SectionTitle =
                    "EMPLOYEE EVALUATIONS",

                TopRows =
                    topRows,

                Headers =
                    new[]
                    {
                    "Employee Name",
                    "Department",
                    "Period",
                    "Total Score",
                    "Final Rate",
                    "Status"
                    },

                Rows =
                    rows
            };
        }


        // =========================================================
        // Employee History Sheet
        // =========================================================

        private static ExcelSheetData BuildEmployeeHistorySheet(
        EmployeeEvaluationHistoryExcelResultDto data)
        {
            var header =
                data.Header;

            var topRows =
                new List<IReadOnlyList<object?>>
                {
            new object[]
            {
                "Employee",
                header.EmployeeName
            },

            new object[]
            {
                "Department",
                header.DepartmentName
            },

            new object[]
            {
                "Period",
                header.PeriodLabel
            },

            new object[]
            {
                "Period Type",
                header.PeriodType.ToString()
            },

            new object[]
            {
                "Total Score",
                header.TotalScore
            },

            new object[]
            {
                "Final Rate",
                header.FinalRate.ToString()
            }
                };

            var rows =
                data.Evaluations
                    .Select(x =>
                        new ExcelSheetRow
                        {
                            Type =
                                ExcelRowType.Data,

                            Values =
                                new object?[]
                                {
                            x.PeriodLabel,
                            x.TotalScore,
                            x.FinalRate.ToString()
                                }
                        })
                    .ToList();

            return new ExcelSheetData
            {
                SheetName =
                    "Employee Evaluation",

                SectionTitle =
                    "EMPLOYEE EVALUATION HISTORY",

                TopRows =
                    topRows,

                Headers =
                    new[]
                    {
                "Period",
                "Total Score",
                "Final Rate"
                    },

                Rows =
                    rows
            };
        }

        // =========================================================
        // EmployeeMonthly Sheet
        // =========================================================

        private static ExcelSheetData
        BuildEmployeeMonthlySheet(
        EmployeeMonthlyEvaluationExcelResultDto data)
        {
            var header =
                data.Header;

            var topRows =
                new List<IReadOnlyList<object?>>
                {
            new object[]
            {
                "Employee",
                header.EmployeeName
            },

            new object[]
            {
                "Department",
                header.DepartmentName
            },

            new object[]
            {
                "Period",
                header.PeriodName
            },

            new object[]
            {
                "Total Score",
                header.TotalScore
            },

            new object[]
            {
                "Final Rate",
                header.FinalRate.ToString()
            },

            new object[]
            {
                "Status",
                header.Status.ToString()
            }
                };

            var rows =
                data.Items
                    .Select(x =>
                        new ExcelSheetRow
                        {
                            Type =
                                ExcelRowType.Data,

                            Values =
                                new object?[]
                                {
                            x.CriterionName,
                            x.Score,
                            x.Notes
                                }
                        })
                    .ToList();

            return new ExcelSheetData
            {
                SheetName =
                    "Monthly Evaluation",

                SectionTitle =
                    "EMPLOYEE MONTHLY EVALUATION",

                TopRows =
                    topRows,

                Headers =
                    new[]
                    {
                "Criterion",
                "Score",
                "Notes"
                    },

                Rows =
                    rows
            };
        }

        // =========================================================
        // Department Sheet
        // =========================================================

        private static ExcelSheetData BuildDepartmentSheet(
            DepartmentEvaluationExcelResultDto data)
        {
            var header =
                data.Header;


            var topRows =
                new List<IReadOnlyList<object?>>
                {
                new object[]
                {
                    "Department",
                    header.DepartmentName
                },

                new object[]
                {
                    "Manager",
                    header.ManagerName ?? "-"
                },

                new object[]
                {
                    "Period",
                    header.PeriodLabel
                },

                new object[]
                {
                    "Period Type",
                    header.PeriodType.ToString()
                },

                new object[]
                {
                    "Average Score",
                    header.AverageScore
                },

                new object[]
                {
                    "Final Rate",
                    header.FinalRate.ToString()
                }
                };


            var rows =
                data.Employees
                    .Select(x =>
                        new ExcelSheetRow
                        {
                            Type =
                                ExcelRowType.Data,

                            Values =
                                new object?[]
                                {
                                x.EmployeeName,
                                x.TotalScore,
                                x.FinalRate.ToString(),
                                x.Status.ToString()
                                }
                        })
                    .ToList();


            return new ExcelSheetData
            {
                SheetName =
                    "Department Evaluation",

                SectionTitle =
                    "DEPARTMENT EVALUATION",

                TopRows =
                    topRows,

                Headers =
                    new[]
                    {
                    "Employee Name",
                    "Total Score",
                    "Final Rate",
                    "Status"
                    },

                Rows =
                    rows
            };
        }


        // =========================================================
        // Common Period Rows
        // =========================================================

        private static IReadOnlyList<IReadOnlyList<object?>>
            BuildCommonPeriodRows(
                int year,
                EvaluationReportPeriod period,
                int? month,
                int? quarter)
        {
            var rows =
                new List<IReadOnlyList<object?>>
                {
                new object[]
                {
                    "Year",
                    year
                },

                new object[]
                {
                    "Period",
                    period.ToString()
                }
                };


            if (month.HasValue)
            {
                rows.Add(
                    new object[]
                    {
                    "Month",
                    month.Value
                    });
            }


            if (quarter.HasValue)
            {
                rows.Add(
                    new object[]
                    {
                    "Quarter",
                    quarter.Value
                    });
            }


            return rows;
        }
    }
}

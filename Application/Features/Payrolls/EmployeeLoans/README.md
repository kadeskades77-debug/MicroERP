# EmployeeLoans

This feature is organized into DTOs, Interfaces, Services and Validators.

## Important design

- `EmployeeLoan` represents the loan itself.
- `EmployeeLoanInstallment` represents each scheduled installment.
- A loan installment can reference a `PayrollAdjustment`.
- `PayrollAdjustment` is the payroll transaction; the loan is the source/business aggregate.
- Installments should become `Applied` when a payroll adjustment is applied during payroll generation.
- Installments should become `Paid` only when the related payroll is marked as paid.
- Do not expose `IsApplied` for manual editing.

## Required domain types

The project is expected to already contain:

- `EmployeeLoan`
- `EmployeeLoanInstallment`
- `LoanStatus`
- `LoanInstallmentStatus`
- `PayrollAdjustment`
- `Result<T>`
- `PagedResult<T>`
- `PagedRequest`
- `ApplicationDbContext`

The service methods are intentionally left with `NotImplementedException` until the exact existing project conventions, audit service, AutoMapper profiles, payroll status flow, and domain namespaces are confirmed. This prevents silently inventing behavior that could conflict with the existing payroll implementation.

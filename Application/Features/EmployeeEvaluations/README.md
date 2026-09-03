# EmployeeEvaluations

Feature package for EvaluationPeriod, EvaluationTemplate, EvaluationCriterion, EmployeeEvaluation and EmployeeEvaluationItem.

Folders:
- DTOs
- Interfaces
- Services
- Validators

The service interfaces are separated by aggregate. Database-specific existence checks, workflow rules, audit handling, soft-delete behavior, and the project's Result/PagedResult conventions must be wired to the existing MicroERP infrastructure rather than guessed.

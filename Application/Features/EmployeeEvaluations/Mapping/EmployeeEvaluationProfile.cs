using AutoMapper;
using MicroERP.Domin.Entities.EmployeeEvaluations;

namespace MicroERP.Application.Features.EmployeeEvaluations.Mapping;

public class EmployeeEvaluationProfile : Profile
{
    public EmployeeEvaluationProfile()
    {
        // Employee Evaluation
        CreateMap<EmployeeEvaluation, EmployeeEvaluationDto>()
    .ForMember(
        dest => dest.EmployeeName,
        opt => opt.MapFrom(
            src => src.Employee.User.FullName))

    .ForMember(
        dest => dest.DepartmentName,
        opt => opt.MapFrom(
            src => src.Employee.Department.NameEn))

    .ForMember(
        dest => dest.EvaluatorName,
        opt => opt.MapFrom(
            src => src.Evaluator.FullName));


        // Employee Evaluation Item
        CreateMap<EmployeeEvaluationItem, EmployeeEvaluationItemDto>()
            .ForMember(
                dest => dest.CriterionName,
                opt => opt.MapFrom(
                    src => src.Criterion.Name))

            .ForMember(
                dest => dest.MaxScore,
                opt => opt.MapFrom(
                    src => src.Criterion.MaxScore))

            .ForMember(
                dest => dest.Weight,
                opt => opt.MapFrom(
                    src => src.Criterion.Weight))

            .ForMember(
                dest => dest.WeightedScore,
                opt => opt.MapFrom(
                    src =>
                        src.Criterion.MaxScore == 0
                            ? 0
                            : (src.Score / src.Criterion.MaxScore)
                              * src.Criterion.Weight));


        // Evaluation Template
        CreateMap<EvaluationTemplate, EvaluationTemplateDto>();


        // Evaluation Criterion
        CreateMap<EvaluationCriterion, EvaluationCriterionDto>();


        // Evaluation Period
        CreateMap<EvaluationPeriod, EvaluationPeriodDto>();
    }
}
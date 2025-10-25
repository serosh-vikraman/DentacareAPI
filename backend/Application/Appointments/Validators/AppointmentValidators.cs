using Application.Appointments.Dtos;
using FluentValidation;

namespace Application.Appointments.Validators;

public sealed class CreateAppointmentValidator : AbstractValidator<CreateAppointmentRequest>
{
    public CreateAppointmentValidator()
    {
        RuleFor(x => x.PatientName).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Department).NotEmpty().MaximumLength(128);
        RuleFor(x => x.DoctorName).NotEmpty().MaximumLength(256);
        RuleFor(x => x.ConsultMode).NotEmpty().MaximumLength(32);
        RuleFor(x => x.PaymentMode).MaximumLength(32).When(x => !string.IsNullOrWhiteSpace(x.PaymentMode));
    }
}

public sealed class UpdateAppointmentValidator : AbstractValidator<UpdateAppointmentRequest>
{
    public UpdateAppointmentValidator()
    {
        RuleFor(x => x.Status).MaximumLength(32);
        RuleFor(x => x.Reason).MaximumLength(4096).When(x => !string.IsNullOrWhiteSpace(x.Reason));
        RuleFor(x => x.Notes).MaximumLength(2048);
        RuleFor(x => x.InvestigationRvg).MaximumLength(256).When(x => !string.IsNullOrWhiteSpace(x.InvestigationRvg));
        RuleFor(x => x.InvestigationBlood).MaximumLength(256).When(x => !string.IsNullOrWhiteSpace(x.InvestigationBlood));
        RuleFor(x => x.InvestigationOthers).MaximumLength(4096).When(x => !string.IsNullOrWhiteSpace(x.InvestigationOthers));
        RuleFor(x => x.DifferentialDiagnosis).MaximumLength(4096).When(x => !string.IsNullOrWhiteSpace(x.DifferentialDiagnosis));
        RuleFor(x => x.Diagnosis).MaximumLength(4096).When(x => !string.IsNullOrWhiteSpace(x.Diagnosis));
        RuleFor(x => x.TreatmentPlan).MaximumLength(4096).When(x => !string.IsNullOrWhiteSpace(x.TreatmentPlan));
    }
}






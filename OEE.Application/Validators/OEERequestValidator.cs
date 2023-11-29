using FluentValidation;
using OEE.Application.Enums;
using OEE.Domain.Models;
using System;

namespace OEE.Application.Validators
{
    public class OEERequestValidator : AbstractValidator<OEERequestModel>
    {
        public OEERequestValidator()
        {
            base.RuleFor(x => x.StationRequest)
                .NotEmpty();

            base.When(x => x.Period != null, () =>
            {
                base.RuleFor(x => x.Period)
                    .Must(x => (Enum.IsDefined(typeof(PeriodEnum.Period), x)))
                    .WithMessage("The Value for Period is not within the valid options.");
            });
        }
    }
}

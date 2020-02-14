using API.Domains.Models;
using API.Domains.Models.Faults;
using API.Domains.Validators.Extensions;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Domains.Validators
{
    public class ContactValidator : AbstractValidator<Contact>
    {
        public ContactValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithErrorCode(((int)Validation.UserEmailNotInformed).ToString())
                .WithMessage("Email must be informed");

            RuleFor(x => x.Email)
                .Length(1, 80)
                .WithErrorCode(((int)Validation.UserEmailExceedsLimit).ToString())
                .WithMessage("The email length must be between 1 and 80 characters");

            RuleFor(x => x.Email)
                .ValidEmail()
                .WithErrorCode(((int)Validation.UserEmailNotValid).ToString())
                .WithMessage("Email is not valid");
        }

        protected override void EnsureInstanceNotNull(object email)
        {
            if (email == null)
            {
                var error = new ValidationFailure("Email", "Email must be informed", null)
                {
                    ErrorCode = ((int)Validation.UserEmailNotInformed).ToString()
                };

                throw new ValidationException("Something happened when our server was validating your email", new List<ValidationFailure> { error });
            }
        }

    }
}

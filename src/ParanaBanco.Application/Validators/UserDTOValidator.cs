using FluentValidation;
using ParanaBanco.Application.DTOs;
using System.Text.RegularExpressions;

namespace ParanaBanco.Application.Validators
{
    public class UserDTOValidator : AbstractValidator<UserDTO>
    {
        public UserDTOValidator()
        {
            RuleFor(p => p.FullName)
                .NotEmpty()
                .WithMessage("FullName field can't be empty.");

            RuleFor(p => p.FullName)
                .MaximumLength(100)
                .WithMessage("FullName field has size of 100 characters.");

            RuleFor(p => p.Email)
                .NotEmpty()
                .WithMessage("Email field can't be empty.");

            RuleFor(p => p.Email)
                .MaximumLength(100)
                .WithMessage("Email field has size of 100 characters.");

            RuleFor(p => p.Email)
                .Must(ValidEmail)
                .WithMessage("Wrong email format.");
        }

        private bool ValidEmail(string email)
        {
            var regex = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");

            return regex.IsMatch(email);
        }
    }
}

using BE_V2.DataDB;
using FluentValidation;
using System;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(user => user.Username)
            .NotEmpty().WithMessage("Username must not be blank")
            .Must(username => !username.StartsWith(" ")).WithMessage("Username first character cannot have space")
            .Must(username => !username.Any(ch => !char.IsLetterOrDigit(ch))).WithMessage("Username special characters are not allowed");

        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Password must not be blank");

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email must not be blank")
            .EmailAddress().WithMessage("Email must be in valid format");

        RuleFor(user => user.PhoneNumber)
            .NotEmpty().WithMessage("Phone number must not be blank")
            .Matches(@"^[0-9]*$").WithMessage("Phone special characters are not allowed");

        RuleFor(user => user.Address)
            .NotEmpty().WithMessage("Address must not be blank")
            .Must(address => !address.StartsWith(" ")).WithMessage("Address first character cannot have space");

        RuleFor(user => user.DateOfBirth)
            .NotEmpty().WithMessage("Date of Birth must not be blank")
            .Must(dob => dob <= DateTime.Now).WithMessage("Date of Birth must be in the past");
    }
}

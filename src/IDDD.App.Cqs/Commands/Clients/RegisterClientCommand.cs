using IDDD.Common.Cqs.Command;
using System.Collections.Generic;
using System.Linq;
using IDDD.Domain.Membership.Clients;
using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace IDDD.App.Cqs.Commands.Client
{
    public class RegisterClientCommand : ICommand, IValidatableObject
    {
        public ApplicationTypes ApplicationType { get; set; }
        public string Key { get; set; }
        public string LogoutRedirectUri { get; set; }
        public string Name { get; set; }
        public string Origin { get; set; }
        public string RedirectUri { get; set; }

        public string ConfirmationUri { get; set; }
        public IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var validator = new RegisterClientCommandValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));

        }
    }

        public class RegisterClientCommandValidator : AbstractValidator<RegisterClientCommand>
        {
            public RegisterClientCommandValidator()
            {
                RuleFor(command => command.Name).NotEmpty();
                RuleFor(command => command.Key).NotEmpty();
                RuleFor(command => command.ApplicationType).NotEmpty();
                RuleFor(command => command.Origin).NotEmpty();
                RuleFor(command => command.RedirectUri).NotEmpty();
                RuleFor(command => command.LogoutRedirectUri).NotEmpty();
            RuleFor(command => command.ConfirmationUri).NotEmpty();
            }
        }
    
}

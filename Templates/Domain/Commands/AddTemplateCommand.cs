using NetDevPack.Messaging;
using System;
using {{Namespace2}}.Domain.Validations;

namespace {{Namespace2}}.Domain.Commands
{
    public class Add{{Template}}Command : Command
    {
        public Add{{Template}}Command()
        {
        }
		
{{PROPERTIES}}        public override bool IsValid()
        {
            ValidationResult = new Add{{Template}}CommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
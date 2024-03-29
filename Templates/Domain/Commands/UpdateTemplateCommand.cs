using NetDevPack.Messaging;
using System;
using {{Namespace2}}.Domain.Validations;

namespace {{Namespace2}}.Domain.Commands
{
    public class Update{{Template}}Command : Command
    {
        public Update{{Template}}Command(Guid id)
        {
            Id = id;
        }
		
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; protected set; }

{{PROPERTIES}}        public override bool IsValid()
        {
            ValidationResult = new Update{{Template}}CommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
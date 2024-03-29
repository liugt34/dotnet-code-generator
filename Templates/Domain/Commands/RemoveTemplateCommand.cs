using NetDevPack.Messaging;
using System;

namespace {{Namespace2}}.Domain.Commands
{
    public class Remove{{Template}}Command : Command
    {
        public Remove{{Template}}Command(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; protected set; }

        public override bool IsValid()
        {
            return !Id.Equals(Guid.Empty);
        }
    }
}
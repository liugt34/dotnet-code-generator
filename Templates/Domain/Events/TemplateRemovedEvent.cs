using NetDevPack.Messaging;
using System;

namespace {{Namespace2}}.Domain.Events
{
    internal class {{Template}}RemovedEvent : Event
    {
        public {{Template}}RemovedEvent(Guid id)
        {
            this.Id = this.AggregateId = id;
        }

        public Guid Id { get; protected set; }
    }
}
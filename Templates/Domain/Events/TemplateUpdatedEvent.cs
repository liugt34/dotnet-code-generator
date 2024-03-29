using System;
using {{Namespace1}}.Core.Domain.Events;

namespace {{Namespace2}}.Domain.Events
{
    internal class {{Template}}UpdatedEvent : BaseUpdatedEvent
    {
        public {{Template}}UpdatedEvent(Guid id)
        {
            this.Id = this.AggregateId = id;
        }


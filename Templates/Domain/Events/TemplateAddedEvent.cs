using System;
using {{Namespace1}}.Core.Domain.Events;

namespace {{Namespace2}}.Domain.Events
{
    internal class {{Template}}AddedEvent : BaseAddedEvent
    {
        public {{Template}}AddedEvent(Guid id)
        {
            this.Id = this.AggregateId = id;
        }


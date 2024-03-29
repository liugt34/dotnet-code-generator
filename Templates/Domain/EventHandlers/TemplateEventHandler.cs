using MediatR;
using System.Threading;
using System.Threading.Tasks;
using {{Namespace2}}.Domain.Events;

namespace {{Namespace2}}.Domain.EventHandlers
{
    internal class {{Template}}EventHandler : INotificationHandler<{{Template}}AddedEvent>, INotificationHandler<{{Template}}UpdatedEvent>, INotificationHandler<{{Template}}RemovedEvent>
    {
        public Task Handle({{Template}}AddedEvent @event, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task Handle({{Template}}UpdatedEvent @event, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task Handle({{Template}}RemovedEvent @event, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
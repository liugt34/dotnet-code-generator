using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;
using {{Namespace1}}.Core.Domain.Interfaces;
using {{Namespace1}}.Core.Identity;
using {{Namespace2}}.Domain.Commands;
using {{Namespace2}}.Domain.Entities;
using {{Namespace2}}.Domain.Events;

namespace {{Namespace2}}.Domain.CommandHandlers
{
    public class {{Template}}CommandHandler : CommandHandler, IRequestHandler<Add{{Template}}Command, ValidationResult>, IRequestHandler<Update{{Template}}Command, ValidationResult>, IRequestHandler<Remove{{Template}}Command, ValidationResult>
    {
        private readonly IAuthorizedUser _user;
        private readonly ICrudRepository<{{Template}}> _{{template}}Repository;

        public {{Template}}CommandHandler(IAuthorizedUser user, ICrudRepository<{{Template}}> {{template}}Repository)
        {
            _user = user;
            _{{template}}Repository = {{template}}Repository;
        }

        public async Task<ValidationResult> Handle(Add{{Template}}Command cmd, CancellationToken cancellationToken)
        {
            if (!cmd.IsValid())
            {
                return cmd.ValidationResult;
            }

            var entity = new {{Template}}(Guid.NewGuid())
            {
{{ENTITY_ADD_1}}                Creator = _user.RealName,
                CreatorId = _user.Subject.Value,
            };

            entity.AddDomainEvent(new {{Template}}AddedEvent(entity.Id)
            {
{{ENTITY_ADD_1}}                Creator = entity.Creator,
                CreatorId = entity.CreatorId,
                CreationTime = entity.CreationTime,
            });

            _{{template}}Repository.Add(entity);

            return await Commit(_{{template}}Repository.UnitOfWork);
        }

        public async Task<ValidationResult> Handle(Update{{Template}}Command cmd, CancellationToken cancellationToken)
        {
            if (!cmd.IsValid()) return cmd.ValidationResult;

            var entity = await _{{template}}Repository.FindAsync(cmd.Id);
            if (entity == null)
            {
                AddError($"{{ENTITY_NAME_CN}}不存在!");
                return ValidationResult;
            }

            entity.Updater = _user.RealName;
            entity.UpdaterId = _user.Subject.Value;
            entity.UpdateTime = DateTime.Now;

{{ENTITY_UPDATE_1}}
            entity.AddDomainEvent(new {{Template}}UpdatedEvent(entity.Id)
            {
{{ENTITY_ADD_1}}                Updater = entity.Updater,
                UpdaterId = entity.UpdaterId,
                UpdateTime = entity.UpdateTime,
            });

            _{{template}}Repository.Update(entity);

            return await Commit(_{{template}}Repository.UnitOfWork);
        }

        public async Task<ValidationResult> Handle(Remove{{Template}}Command cmd, CancellationToken cancellationToken)
        {
            if (!cmd.IsValid()) return cmd.ValidationResult;

            var entity = await _{{template}}Repository.FindAsync(cmd.Id);

            if (entity == null)
            {
                AddError($"{{ENTITY_NAME_CN}}不存在!");
                return ValidationResult;
            }

            entity.AddDomainEvent(new {{Template}}RemovedEvent(cmd.Id));

            _{{template}}Repository.SoftDelete(entity);

            return await Commit(_{{template}}Repository.UnitOfWork);
        }

        #region Dispose

        public void Dispose()
        {
            _{{template}}Repository.Dispose();
        }

        #endregion Dispose
    }
}
using System;
using {{Namespace1}}.Core.Application.Interfaces;
using {{Namespace2}}.Application.Dtos;
using {{Namespace2}}.Application.Filters;
using {{Namespace2}}.Domain.Entities;

namespace {{Namespace2}}.Application.Interfaces
{
    /// <summary>
    /// {{ENTITY_NAME_CN}}应用服务接口
    /// </summary>
    public interface I{{Template}}AppService : IReadonlyAppService<{{Template}}Dto, {{Template}}, Guid, {{Template}}QueryFilter>
    {
    }
}
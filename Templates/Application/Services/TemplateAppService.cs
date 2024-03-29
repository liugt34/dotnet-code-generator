using AutoMapper;
using System;
using {{Namespace1}}.Core.Application.Services;
using {{Namespace1}}.Core.Domain.Interfaces;
using {{Namespace2}}.Application.Dtos;
using {{Namespace2}}.Application.Filters;
using {{Namespace2}}.Application.Interfaces;
using {{Namespace2}}.Domain.Entities;

namespace {{Namespace2}}.Application.Services
{
    /// <summary>
    /// {{ENTITY_NAME_CN}}应用服务实例
    /// </summary>
    internal class {{Template}}AppService : ReadonlyAppService<{{Template}}Dto, {{Template}}, Guid, {{Template}}QueryFilter>, I{{Template}}AppService
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="repository"></param>
        public {{Template}}AppService(IMapper mapper, IReadonlyRepository<{{Template}}> repository) : base(mapper, repository)
        {
        }
    }
}
using AutoMapper;
using NetDevPack.Messaging;
using System;
using {{Namespace1}}.Core.Application.Interfaces;
using {{Namespace2}}.Domain.Commands;

namespace {{Namespace2}}.Application.Dtos
{
    /// <summary>
    /// {{ENTITY_NAME_CN}}删除输入项
    /// </summary>
    public class {{Template}}RemoveInput : IInputToCommand
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="base64">Base64字符</param>
        public {{Template}}RemoveInput(string base64)
        {
            Id = Guid.Parse(base64.Base64Decode());
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">原始Id</param>
        public {{Template}}RemoveInput(Guid id)
        {
            this.Id = id;
        }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 映射为Command
        /// </summary>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public Command Map(IMapper mapper)
        {
            return mapper.Map<Remove{{Template}}Command>(this);
        }
    }
}
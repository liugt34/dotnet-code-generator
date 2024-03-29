using AutoMapper;
using NetDevPack.Messaging;
using System;
using System.ComponentModel.DataAnnotations;
using {{Namespace1}}.Core.Application.Interfaces;
using {{Namespace2}}.Domain.Commands;

namespace {{Namespace2}}.Application.Dtos
{
    /// <summary>
    /// {{ENTITY_NAME_CN}}更新输入项
    /// </summary>
    public class {{Template}}UpdateInput : IInputToCommand
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        [Required(ErrorMessage = "必填项")]
        public Guid? Id { get; set; }


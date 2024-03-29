using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using {{Namespace1}}.Core.Domain.Entities;

namespace {{Namespace2}}.Domain.Entities
{
    /// <summary>
    /// {{ENTITY_NAME_CN}}
    /// </summary>
    [Table("{{TableName}}", Schema = "public")]
    public class {{Template}} : AggregateRoot<Guid>
    {
        public {{Template}}()
        {
        }

        public {{Template}}(Guid id) : base(id)
        {
        }
		
		
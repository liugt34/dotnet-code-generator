using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using {{Namespace1}}.Core.Domain.Mappings;
using {{Namespace2}}.Domain.Entities;

namespace {{Namespace2}}.Infrastructure.Mappings
{
    public class {{Template}}Map : EntityTypeConfiguration<{{Template}}, Guid>
    {
        public override void Configure(EntityTypeBuilder<{{Template}}> builder)
        {
            base.Configure(builder);

            builder.ToTable("{{TableName}}").HasKey(t => t.Id);
        }
    }
}
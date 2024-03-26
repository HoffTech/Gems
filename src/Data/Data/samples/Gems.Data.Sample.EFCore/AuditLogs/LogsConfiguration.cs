using Gems.Data.Sample.EFCore.AuditLogs.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gems.Data.Sample.EFCore.AuditLogs
{
    public class LogsConfiguration : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.HasKey(x => x.LogId);

            builder
                .Property(p => p.UpdatedBy)
                .HasColumnName(nameof(Log.UpdatedBy))
                .HasComment("Кем обновлен");

            builder
                .Property(p => p.UpdatedAt)
                .HasColumnName(nameof(Log.UpdatedAt))
                .IsRequired()
                .HasComment("Время обновления");
        }
    }
}

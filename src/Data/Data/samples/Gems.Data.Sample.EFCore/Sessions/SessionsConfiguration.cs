using Gems.Data.Sample.EFCore.Sessions.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gems.Data.Sample.EFCore.Sessions
{
    public class SessionsConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.HasKey(x => x.SessionId);

            builder
                .Property(p => p.Quantity)
                .HasColumnName(nameof(Session.Quantity))
                .HasComment("Количество запросов в сессии");

            builder
                .Property(p => p.SubmittedAt)
                .HasColumnName(nameof(Session.SubmittedAt))
                .IsRequired()
                .HasComment("Дата и время старта сессии");
        }
    }
}

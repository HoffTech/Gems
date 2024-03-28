using Gems.Data.Sample.EFCore.Persons.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gems.Data.Sample.EFCore.Persons.EntityTypeConfigurations
{
    public class SessionsConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.HasKey(x => x.SessionId);
        }
    }
}

using Gems.Data.Sample.EFCore.Persons.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gems.Data.Sample.EFCore.Persons.EntityTypeConfigurations
{
    public class LogsConfiguration : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.HasKey(x => x.LogId);
        }
    }
}

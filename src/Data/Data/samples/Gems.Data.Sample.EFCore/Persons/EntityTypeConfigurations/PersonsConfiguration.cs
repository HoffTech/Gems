using Gems.Data.Sample.EFCore.Persons.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gems.Data.Sample.EFCore.Persons.EntityTypeConfigurations
{
    public class PersonsConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.HasKey(x => x.PersonId);
        }
    }
}

using Gems.Data.Sample.EFCore.Persons.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gems.Data.Sample.EFCore.Persons
{
    public class PersonsConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.HasKey(x => x.PersonId);

            builder.Property(p => p.PersonId)
                .HasColumnName("person_id")
                .HasComment("Идентификатор");

            builder
                .Property(p => p.FirstName)
                .HasColumnName("first_name")
                .HasComment("Имя");

            builder
                .Property(p => p.LastName)
                .HasColumnName("last_name")
                .IsRequired()
                .HasComment("Фамилия");

            builder
                .Property(p => p.Age)
                .HasColumnName("age")
                .IsRequired()
                .HasComment("Возраст");

            builder
                .Property(p => p.Gender)
                .HasColumnName("gender")
                .IsRequired()
                .HasComment("Пол");
        }
    }
}

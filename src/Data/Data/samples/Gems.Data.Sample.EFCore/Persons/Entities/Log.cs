using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gems.Data.Sample.EFCore.Persons.Entities
{
    [Table("log", Schema = "public")]
    public class Log
    {
        [Key]
        [Required]
        [Column("log_id")]
        public Guid LogId { get; set; }

        [Required]
        [Column("updated_by")]
        public string UpdatedBy { get; set; }

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}

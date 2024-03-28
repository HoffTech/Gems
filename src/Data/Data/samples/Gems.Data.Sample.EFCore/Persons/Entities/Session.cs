using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gems.Data.Sample.EFCore.Persons.Entities
{
    [Table("session", Schema = "public")]
    public class Session
    {
        [Key]
        [Required]
        [Column("session_id")]
        public Guid SessionId { get; set; }

        [Required]
        [Column("qty")]
        public int Quantity { get; set; }

        [Required]
        [Column("submitted_at")]
        public DateTime SubmittedAt { get; set; }
    }
}

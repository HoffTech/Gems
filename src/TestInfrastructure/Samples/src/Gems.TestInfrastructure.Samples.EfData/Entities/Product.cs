using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gems.TestInfrastructure.Samples.EfData.Entities
{
    [Table("products")]
    public class Product
    {
        [Key]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("product_category_id")]
        public int ProductCategoryId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gems.TestInfrastructure.Samples.EfData.Entities
{
    [Table("product_categories")]
    public class ProductCategory
    {
        [Key]
        [Column("product_category_id")]
        public int ProductCategoryId { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}

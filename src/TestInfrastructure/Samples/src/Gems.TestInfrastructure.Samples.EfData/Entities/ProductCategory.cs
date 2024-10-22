// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

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

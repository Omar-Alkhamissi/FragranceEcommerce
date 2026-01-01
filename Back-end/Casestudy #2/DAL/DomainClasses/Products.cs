using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FragEcom.DAL.DomainClasses
{
    public class Products
    {
        [Key]
        public string Id { get; set; } = null!;

        [Required]
        [ForeignKey("BrandId")]
        public int BrandId { get; set; }

        [Required]
        [StringLength(200)]
        public string? ProductName { get; set; }

        [StringLength(200)]
        public string? GraphicName { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal CostPrice { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal MSRP { get; set; }

        [Required]
        public int QtyOnHand { get; set; }

        [Required]
        public int QtyOnBackOrder { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        public Brands? Brand { get; set; }
    }
}
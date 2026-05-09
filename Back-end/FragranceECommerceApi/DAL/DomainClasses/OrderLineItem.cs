using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FragEcom.DAL.DomainClasses
{
    public class OrderLineItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("OrderId")]
        public int OrderId { get; set; }

        [Required]
        [ForeignKey("ProductId")]
        public string? ProductId { get; set; }

        [Required]
        public int QtyOrdered { get; set; }

        [Required]
        public int QtySold { get; set; }

        [Required]
        public int QtyBackOrdered { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal SellingPrice { get; set; }
    }
}
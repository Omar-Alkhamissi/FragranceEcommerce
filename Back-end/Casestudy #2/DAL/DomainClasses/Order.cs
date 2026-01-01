using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FragEcom.DAL.DomainClasses
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal OrderAmount { get; set; }

        [Required]
        [ForeignKey("CustomerId")]
        public int CustomerId { get; set; }
    }
}
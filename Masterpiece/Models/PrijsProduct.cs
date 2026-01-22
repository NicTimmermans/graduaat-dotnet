using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Models
{
    public class PrijsProduct
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DatumVanaf { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Prijs { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        // Navigation properties
        public virtual Product Product { get; set; }

    }
}

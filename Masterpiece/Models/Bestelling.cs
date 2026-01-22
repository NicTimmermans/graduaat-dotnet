using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Models
{
    public class Bestelling
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Reservatie")]
        public int ReservatieId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public int Aantal { get; set; }
        public string? Opmerking { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? TijdstipBestelling { get; set; }

        [ForeignKey("Status")]
        public int StatusId { get; set; }

        // Navigation properties
        public virtual Reservatie Reservatie { get; set; }
        public virtual Product Product { get; set; }
        public virtual Status Status { get; set; }
    }
}

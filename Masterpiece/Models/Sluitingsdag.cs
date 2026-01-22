using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Models
{
    public class Sluitingsdag
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "date")]
        [DataType(DataType.Date)]

        public DateTime? Datum { get; set; }

        public string Naam { get; set; }
    }
}

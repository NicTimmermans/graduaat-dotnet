

public class ReserverenViewModel
{
    public int AantalPersonen { get; set; }
    public DateTime? Datum { get; set; }
    public int? TijdslotId { get; set; }

    public List<TijdslotBeschikbaarheidHelperViewModel> Tijdsloten { get; set; } = new();
    public List<DateTime> BeschikbareDatums { get; set; } = new();
}

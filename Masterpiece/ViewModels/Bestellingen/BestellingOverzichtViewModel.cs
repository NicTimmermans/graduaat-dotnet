namespace Restaurant.ViewModels
{
    public class BestellingOverzichtViewModel
    {
       
            public int ReservatieId { get; set; }
            public List<BestellingItemViewModel> Gerechten { get; set; } = new();
            public List<BestellingItemViewModel> Dranken { get; set; } = new();
            

        public List<string> TafelNummers { get; set; } = new();            
        public string TafelDisplay => TafelNummers != null && TafelNummers.Any()
                                     ? string.Join(", ", TafelNummers)
                                     : ReservatieId.ToString();

        //nodig voor header oranje te maken
        public bool GerechtenAllemaalKlaar { get; set; }

        //nodig voor header groen te maken
        public bool DrankenAllemaalGeleverd { get; set; }
    }
}

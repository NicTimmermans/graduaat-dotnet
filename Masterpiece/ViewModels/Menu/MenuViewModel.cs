namespace Restaurant.ViewModels
{
    public class MenuViewModel
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public string Beschrijving { get; set; }
        public string AllergenenInfo { get; set; }
        public string AfbeeldingUrl { get; set; }
        public bool IsSuggestie { get; set; }
        public decimal? Prijs { get; set; }
        public string CategorieNaam { get; set; }
        public bool CategorieActief { get; set; }
        public string CategorieTypeNaam { get; set; }
        public bool CategorieTypeActief { get; set; }
    }
}

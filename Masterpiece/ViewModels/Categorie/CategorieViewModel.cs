namespace Restaurant.ViewModels
{
    public class CategorieViewModel
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public bool Actief {  get; set; }
        public int TypeId { get; set; }
        public CategorieType Type { get; set; }
    }
}

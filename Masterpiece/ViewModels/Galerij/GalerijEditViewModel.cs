namespace Restaurant.ViewModels
{
    public class GalerijEditViewModel
    {
        public List<string> Afbeeldingen = new List<string>();
        public IFormFile? NieuweAfbeelding { get; set; }
    }
}

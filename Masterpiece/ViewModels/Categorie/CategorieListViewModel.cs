namespace Restaurant.ViewModels
{
    public class CategorieListViewModel
    {
        public List<CategorieViewModel> Categorien { get; set; } = new();

        public CategorieCreateViewModel? CreateCategorie { get; set; } = new();

        public CategorieEditViewModel? EditCategorie { get; set; } = new();

        public string? FeedbackMessage { get; set; }
        public bool IsError { get; set; }

        public List<SelectListItem> Types { get; set; }
    }
}

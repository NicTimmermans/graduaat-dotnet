namespace Restaurant.ViewModels
{
    public class ParameterListViewModel
    {
        public List<ParameterViewModel> Parameters { get; set; } = new();

        public ParameterCreateViewModel? CreateParameter { get; set; } = new();

        public ParameterEditViewModel? EditParameter { get; set; } = new();

        public string? FeedbackMessage { get; set; }
        public bool IsError { get; set; }
    }
}

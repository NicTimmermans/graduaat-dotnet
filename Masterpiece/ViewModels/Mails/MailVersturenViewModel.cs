namespace Restaurant.ViewModels
{
    public class MailVersturenViewModel
    {

        public string? Naam { get; set; }
        public string? Onderwerp { get; set; }
        public string? Body { get; set; }
        public List<Mail> AvailableMails { get; set; } = new List<Mail>();
        public int SelectedMailId { get; set; }
        
    }
}

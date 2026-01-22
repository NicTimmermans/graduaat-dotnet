
    namespace Restaurant.ViewModels
    {
        public class TijdslotBeschikbaarheidViewModel
        {
        [DataType(DataType.Date)]
            public DateTime Datum { get; set; }
            public List<TijdslotBeschikbaarheidHelperViewModel> Tijdsloten { get; set; } = new();
        }

        
    }



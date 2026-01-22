using System.Collections.Generic;

namespace Restaurant.ViewModels
{
    public class TafelListViewModel
    {
        public List<TafelReservatiesViewModel> Tafels { get; set; } = new();

        /// <summary>
        /// "active" | "inactive" | "all"
        /// </summary>
        public string ActiveFilter { get; set; } = "active";
    }
}

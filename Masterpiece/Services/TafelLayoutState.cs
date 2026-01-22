using System.Collections.Generic;

namespace Restaurant.Services
{
    /// <summary>
    /// Houdt de huidige volgorde van tafels in geheugen.
    /// Geen DB, geen migraties – gewoon in-memory state.
    /// </summary>
    public class TafelLayoutState
    {
        /// <summary>
        /// De volgorde van Tafel.Id's in het grondplan.
        /// </summary>
        public List<int> Order { get; set; } = new();
    }
}

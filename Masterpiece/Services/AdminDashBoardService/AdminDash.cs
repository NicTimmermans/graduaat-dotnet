using Restaurant.ViewModels.DashBoard;

namespace Restaurant.Services.AdminDashBoardService;

public class AdminDash : IAdminDash
{
    public List<ButtonsAdminDash> GetAdminButtons()
    {
        return new List<ButtonsAdminDash>()
        {
            new ButtonsAdminDash { Name = "Account", Controller = "Account"},
            new ButtonsAdminDash { Name = "Mails", Controller = "Mail" },
            new ButtonsAdminDash { Name = "Logs", Controller = "DbLoggin" },
            new ButtonsAdminDash { Name = "Reservaties", Controller = "Reservatie",Action = "ReservatieBeheer"},
            new ButtonsAdminDash { Name = "Enquêtes", Controller = "Enquete", Action = "EnqueteBeheer" },
            new ButtonsAdminDash { Name = "Tafels", Controller = "OldTafel" },
            new ButtonsAdminDash { Name = "Parameters", Controller = "Parameter" },
            new ButtonsAdminDash { Name = "Producten", Controller = "Product" },
            new ButtonsAdminDash { Name = "Kok overzicht", Controller = "Bestelling", Action = "KokIndex"},
            new ButtonsAdminDash { Name = "Ober overzicht", Controller = "Bestelling", Action = "OberIndex" },
            new ButtonsAdminDash { Name = "Admin account", Controller = "Account", Action = "Dashboard" },
            new ButtonsAdminDash { Name = "Categorie beheren", Controller = "Categorie" },
            new ButtonsAdminDash { Name = "Afrekenen", Controller = "OldTafel", Action = "Afrekenen" },
        };
    }
    
    public List<ButtonsAdminDash> GetKokButtons()
    {
        return new List<ButtonsAdminDash>()
        {
            new ButtonsAdminDash { Name = "Service Tickets", Controller = "Kok" },
            new ButtonsAdminDash { Name = "Producten", Controller = "Product" },
        };
    }
    
    public List<ButtonsAdminDash> GetOberButtons()
    {
        return new List<ButtonsAdminDash>()
        {
            new ButtonsAdminDash { Name = "Service Tickets", Controller = "Ober" },
            new ButtonsAdminDash { Name = "Producten", Controller = "Product" },
            
        };
    }
}
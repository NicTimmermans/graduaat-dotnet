using Restaurant.ViewModels.DashBoard;

namespace Restaurant.Services.AdminDashBoardService;

public interface IAdminDash
{
    List<ButtonsAdminDash> GetAdminButtons();
    List<ButtonsAdminDash> GetOberButtons();
    List<ButtonsAdminDash> GetKokButtons();
}
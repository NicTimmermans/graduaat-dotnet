namespace Restaurant.ViewModels
{
    public class AccountRole
    {
        public CustomUser Account { get; set; }
        public IdentityRole Rol { get; set; }
    }
    public class AccountListViewModel
    {
        public List<AccountRole> AccountRoles { get; set; }
    }
}

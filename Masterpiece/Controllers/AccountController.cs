
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Update;
using Restaurant.Services.MailService;
using Restaurant.ViewModels.Account;

namespace Restaurant.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<CustomUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private SignInManager<CustomUser> _signInManager;
        private IMailSender _mailSender;

        public AccountController(UserManager<CustomUser> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork, IMapper mapper, SignInManager<CustomUser> signInManager, IMailSender mailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _signInManager = signInManager;
            _mapper = mapper;
            _mailSender = mailSender;
        }
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new AccountLoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AccountLoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Account niet gevonden.");
                return View(model);
            }

            if (!user.Actief)
            {
                ModelState.AddModelError("", "Dit account is gedeactiveerd.");
                return View(model);
            }

            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError(nameof(model.Email), "Account niet gevonden.");
                ViewData["AccountNotFound"] = true;
                return View(model);
            }

            //if (!user.Actief)    
            if (await _userManager.CheckPasswordAsync(user, model.Password) == false)
            {
                ModelState.AddModelError(string.Empty, "Dit account is gedeactiveerd.");
                return View(model);
            }

            var passwordOk = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordOk)
            {
                ModelState.AddModelError(nameof(model.Password), "Onjuist wachtwoord.");
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            if (await _userManager.IsInRoleAsync(user, "Eigenaar"))
            {
                return RedirectToAction("Index", "Admin");
            }

            return RedirectToAction("Dashboard", "Account");
        }


        [AllowAnonymous]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (!User.IsInRole("Eigenaar")) return RedirectToAction("Index", "Home");

            List<CustomUser> accounts = await _userManager.Users.ToListAsync();
            List<Land> landen = (await _unitOfWork.LandRepository.GetAllAsync()).ToList();
            List<IdentityRole> rollen = _roleManager.Roles.ToList();
            List<AccountIndexViewModel> viewModels = new List<AccountIndexViewModel>();

            foreach (CustomUser account in accounts)
            {
                account.Land = landen.FirstOrDefault(l => l.Id == account.LandId);
                AccountIndexViewModel vm = _mapper.Map<AccountIndexViewModel>(account);
                vm.Rol = rollen.FirstOrDefault(r =>
                {
                    IList<string> roleNames = _userManager.GetRolesAsync(account).Result;
                    return roleNames != null && roleNames.Count > 0 && r.Name == roleNames[0];
                });
                viewModels.Add(vm);
            }
            ViewBag.Roles = rollen;
            return View(viewModels.OrderBy(vm => !vm.Actief).ThenBy(vm => vm.Voornaam).ThenBy(vm => vm.Achternaam).ToList());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Create()
        {
            Expression<Func<Land, bool>> voorwaarden = l => l.Actief == true;
            AccountCreateViewModel viewModel = new AccountCreateViewModel()
            {
                Landen = new SelectList(await _unitOfWork.LandRepository.Find(voorwaarden), "Id", "Naam"),
                Rollen = new SelectList(_roleManager.Roles.ToList(), "Id", "Name")
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create(AccountCreateViewModel viewModel)
        {
            Expression<Func<Land, bool>> voorwaarden = l => l.Actief == true;

            if (!User.Identity.IsAuthenticated)
            {
                ModelState.Remove("RolId");
                viewModel.RolId = (await _roleManager.FindByNameAsync("Klant")).Id;
            }

            if (ModelState.IsValid)
            {
                CustomUser existingEmail = await _userManager.FindByEmailAsync(viewModel.Email);
                if (existingEmail != null)
                {
                    ModelState.AddModelError("Email", "Email already in use.");
                    viewModel.Landen = new SelectList(await _unitOfWork.LandRepository.Find(voorwaarden), "Id", "Naam");
                    viewModel.Rollen = new SelectList(_roleManager.Roles.ToList(), "Id", "Name");
                    return View(viewModel);
                }

                CustomUser account = _mapper.Map<CustomUser>(viewModel);

                IdentityResult result = await _userManager.CreateAsync(account, viewModel.Password);

                if (result.Succeeded)
                {
                    CustomUser user = await _userManager.FindByEmailAsync(viewModel.Email);
                    IdentityRole role = await _roleManager.FindByIdAsync(viewModel.RolId);

                    IdentityResult Resultaat = await _userManager.AddToRoleAsync(user, role.Name);

                    if (User.Identity != null && User.Identity.IsAuthenticated)
                    {
                        return RedirectToAction("Index");
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            viewModel.Landen = new SelectList(await _unitOfWork.LandRepository.Find(voorwaarden), "Id", "Naam");
            viewModel.Rollen = new SelectList(_roleManager.Roles.ToList(), "Id", "Name");
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            // Huidige ingelogde gebruiker ophalen
            var currentUserId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(currentUserId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Gebruiker ophalen inclusief land en reservaties (met tijdslots en tafels)
            CustomUser account = await _userManager.Users
                .Include(u => u.Land)
                .Include(u => u.Reservaties)
                    .ThenInclude(r => r.Tijdslot)
                .Include(u => u.Reservaties)
                    .ThenInclude(r => r.Tafellijsten)
                        .ThenInclude(tl => tl.Tafel)
                .FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (account == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // ViewModel voorbereiden
            var viewModel = _mapper.Map<AccountDashboardViewModel>(account);

            // Rol bepalen
            var roles = await _userManager.GetRolesAsync(account);
            viewModel.RolNaam = roles != null && roles.Count > 0 ? roles[0] : "Klant";

            // Reservaties filteren en mappen
            viewModel.Reservaties = new List<ReservatieDashboardViewModel>();
            var today = DateTime.Today;

            foreach (var r in account.Reservaties
                                    .Where(r => r.Datum.HasValue && r.Datum.Value.Date >= today))
            {
                var reservatieVm = _mapper.Map<ReservatieDashboardViewModel>(r);
                reservatieVm.ReservatieId = r.Id;

                // Alleen vandaag mag bestellen
                reservatieVm.MagBestellen = r.Datum?.Date == DateTime.Today;

                // Tafelnummers ophalen
                reservatieVm.TafelNummers = r.Tafellijsten
                    .Select(tl => tl.Tafel?.TafelNummer.ToString() ?? "-")
                    .ToList();

                viewModel.Reservaties.Add(reservatieVm);
            }

            return View(viewModel);
        }



        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (!User.IsInRole("Eigenaar") && currentUserId != id)
            {
                return RedirectToAction("Index", "Home");
            }

            CustomUser account = await _userManager.Users
                .Where(k => k.Id == id)
                .FirstOrDefaultAsync();

            Expression<Func<Land, bool>> voorwaarden = l => l.Actief == true;

            if (account != null)
            {
                AccountEditViewModel viewModel = _mapper.Map<AccountEditViewModel>(account);

                viewModel.Landen = new SelectList(
                    await _unitOfWork.LandRepository.Find(voorwaarden), "Id", "Naam");

                viewModel.IsSelf = (currentUserId == id);

                if (User.IsInRole("Eigenaar"))
                {
                    viewModel.Rollen = new SelectList(_roleManager.Roles.ToList(), "Id", "Name");

                    IList<string> roleNames = await _userManager.GetRolesAsync(account);
                    if (roleNames != null && roleNames.Count > 0)
                    {
                        IdentityRole role = await _roleManager.FindByNameAsync(roleNames[0]);
                        if (role != null)
                        {
                            viewModel.RolId = role.Id;
                        }
                    }
                }

                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AccountEditViewModel viewModel)
        {
            Expression<Func<Land, bool>> voorwaarden = l => l.Actief == true;
            var currentUserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                CustomUser account = await _userManager.FindByIdAsync(viewModel.Id);
                if (account == null)
                {
                    return NotFound("Account not found");
                }

                if (!string.Equals(account.Email, viewModel.Email, StringComparison.OrdinalIgnoreCase))
                {
                    CustomUser existingEmail = await _userManager.FindByEmailAsync(viewModel.Email);
                    if (existingEmail != null && existingEmail.Id != account.Id)
                    {
                        ModelState.AddModelError("Email", "Email already in use.");
                        viewModel.Landen = new SelectList(await _unitOfWork.LandRepository.Find(voorwaarden), "Id", "Naam");
                        viewModel.Rollen = new SelectList(_roleManager.Roles.ToList(), "Id", "Name");
                        viewModel.IsSelf = (currentUserId == viewModel.Id);
                        return View(viewModel);
                    }
                }

                IList<string> oldRoleNames = await _userManager.GetRolesAsync(account);
                IdentityRole? oldRole = (oldRoleNames != null && oldRoleNames.Count > 0)
                    ? await _roleManager.FindByNameAsync(oldRoleNames[0])
                    : null;

                if (oldRole != null && oldRole.Name == "Eigenaar")
                {
                    int eigenaarCount = (await _userManager.GetUsersInRoleAsync("Eigenaar")).Count;

                    if (eigenaarCount <= 1 &&
                        !string.IsNullOrEmpty(viewModel.RolId) &&
                        viewModel.RolId != oldRole.Id)
                    {
                        ModelState.AddModelError("", "Kan de rol van de laatste 'Eigenaar' niet veranderen.");

                        viewModel.Landen = new SelectList(await _unitOfWork.LandRepository.Find(voorwaarden), "Id", "Naam");
                        viewModel.Rollen = new SelectList(_roleManager.Roles.ToList(), "Id", "Name");
                        viewModel.IsSelf = (currentUserId == viewModel.Id);
                        return View(viewModel);
                    }
                }

                _mapper.Map(viewModel, account);

                IdentityResult result = await _userManager.UpdateAsync(account);

                if (result.Succeeded)
                {
                    if (User.IsInRole("Eigenaar") && !string.IsNullOrEmpty(viewModel.RolId))
                    {
                        IdentityRole? newRole = await _roleManager.FindByIdAsync(viewModel.RolId);

                        if (oldRole != null && newRole != null && oldRole.Id != newRole.Id)
                        {
                            await _userManager.RemoveFromRoleAsync(account, oldRole.Name);
                            await _userManager.AddToRoleAsync(account, newRole.Name);
                        }
                    }

                    if (currentUserId == viewModel.Id)
                    {
                        return RedirectToAction("Dashboard", "Account");
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            viewModel.Landen = new SelectList(await _unitOfWork.LandRepository.Find(voorwaarden), "Id", "Naam");
            viewModel.Rollen = new SelectList(_roleManager.Roles.ToList(), "Id", "Name");
            viewModel.IsSelf = (currentUserId == viewModel.Id);
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var currentUserId = _userManager.GetUserId(User);

            CustomUser account = await _userManager.FindByIdAsync(id);

            if (account == null)
                return RedirectToAction("Index");

            if (currentUserId != id && !User.IsInRole("Eigenaar"))
                return RedirectToAction("Index", "Home");

            IList<string> roleNames = await _userManager.GetRolesAsync(account);
            IdentityRole role = (roleNames != null && roleNames.Count > 0)
                ? await _roleManager.FindByNameAsync(roleNames[0])
                : null;

            if (role != null && role.Name == "Eigenaar")
            {
                int eigenaarCount = (await _userManager.GetUsersInRoleAsync("Eigenaar")).Count;
                if (eigenaarCount <= 1)
                    return RedirectToAction("Index");
            }

            account.Actief = false;
            account.Voornaam = "<>";
            account.Achternaam = "<>";
            account.Adres = "<>";
            account.Huisnummer = "<>";
            account.Postcode = "<>";
            account.Gemeente = "<>";

            IdentityResult result = await _userManager.UpdateAsync(account);

            if (result.Succeeded)
            {
                if (roleNames != null && roleNames.Count > 0 && role != null)
                    await _userManager.RemoveFromRoleAsync(account, role.Name);

                if (currentUserId == id)
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToAction("Index", "Home");
                }

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPasswordFormView()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ResetPasswordConfirmView(Guid token)
        {
            if (token == Guid.Empty)
                return View("~/Views/FeedbackViews/AccesDenied.cshtml");

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.ForgotPasswordResetToken == token);

            if (user == null)
                return View("~/Views/FeedbackViews/AccesDenied.cshtml");

            if (user.PassWordResetCodeHash == null || user.PassWordResetCodeExpiry < DateTime.Now)
                return View("~/Views/FeedbackViews/AccesDenied.cshtml");

            var vm = new ResetPasswordViewModel
            {
                Email = user.Email,
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> SendPasswordResetMail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Email is required");
                return View("ResetPasswordFormView");
            }

            var token = Guid.NewGuid();
            var link = Url.Action(
                "ResetPasswordConfirmView",
                "Account",
                new { token = token },
                protocol: Request.Scheme
            );

            await _mailSender.SendPasswordResetMail(14, email, link, token);
            return View("CheckMailView");
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
        {
            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Verkeerde login poging");
                return View("ResetPasswordConfirmView", vm);
            }
            if (vm.NewPassword != vm.ConfirmPassword)
            {
                ModelState.AddModelError("", "Wachtwoorden komen niet overeen");
                return View("ResetPasswordConfirmView", vm);
            }

            var hasher = new PasswordHasher<IdentityUser>();
            user.PasswordHash = hasher.HashPassword(user, vm.NewPassword);
            user.PassWordResetCodeHash = null;
            user.ForgotPasswordResetToken = Guid.Empty;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Login");
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidateCode(ResetPasswordViewModel vm)
        {
            var user = await _userManager.FindByEmailAsync(vm.Email);

            if (string.IsNullOrWhiteSpace(vm.Code))
            {
                ModelState.AddModelError("", "Code is required");
                return View("ResetPasswordConfirmView", vm);
            }

            var inputHash = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(vm.Code)
                )
            );

            if (inputHash == user.PassWordResetCodeHash && user.PassWordResetCodeExpiry > DateTime.Now)
            {
                vm.IsValidated = true;
            }
            else
            {
                ModelState.AddModelError("", "Code is ongeldig of verlopen");
                vm.IsValidated = false;
            }

            return View("ResetPasswordConfirmView", vm);
        }
    }
}

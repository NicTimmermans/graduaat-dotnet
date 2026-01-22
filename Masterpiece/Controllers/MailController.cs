using System.Collections.Immutable;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Restaurant.Services.MailService;

namespace Restaurant.Controllers
{
    [Authorize(Roles = "Eigenaar")]
    public class MailController : Controller
    {
        private readonly ILogger<MailController> _logger;
        private readonly IResend _resend;
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;
        private readonly IMailSender _mailSender;
                
        public MailController(ILogger<MailController> logger, IResend resend, IUnitOfWork context, IMapper mapper, IMailSender mailSender )
        {
            _logger = logger;
            _resend = resend;
            _context = context;
            _mapper = mapper;
            _mailSender = mailSender;
        }
        
        
        //Main Navigation
        [Authorize(Roles = "Eigenaar")]
        public async Task<IActionResult> Index()
        {
            var mails = await _context.MailRepository.GetAllAsync();
                        
            var viewModel = new MailVersturenViewModel();
            viewModel.AvailableMails = mails.ToList();

            return View(viewModel);
        }
        
        //Create navigation
        [Authorize(Roles = "Eigenaar")]
        public IActionResult MailsAanmaken()
        {
            return View();
        }
        
        //Put Navigation
        [Authorize(Roles = "Eigenaar")]
        public async Task<IActionResult> MailsAanpassen(int id)
        {
            var mail = await _context.MailRepository.GetByIdAsync(id);
            if (mail != null)
            {
                MailAanpassenViewModel vm = _mapper.Map<MailAanpassenViewModel>(mail);
                return View(vm);
            }else
                return RedirectToAction("Index");
        }
        
        //Create Action
        [Authorize(Roles = "Eigenaar")]
        [HttpPost]
        public async Task<IActionResult> MailsAanmaken(MailsAanmakenViewModel vm)
        {
            if (!ModelState.IsValid)
                return BadRequest("Er zit een fout in het model");
            
            Mail mail = _mapper.Map<Mail>(vm);
            await _context.MailRepository.AddAsync(mail);
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "Er is een probleem opgetreden bij het wegschrijven naar de database.");
                return View(vm);
            }
        } 
        
        //Put Action
        [Authorize(Roles = "Eigenaar")]
        [HttpPost]
        public async Task<IActionResult> MailsAanpassen(int id, MailAanpassenViewModel vm)
        {
            if (id != vm.Id)
            {
                ModelState.AddModelError(nameof(vm.Id), $"Id mismatch");
                return View(vm);
            }
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);            

            try
            {
                    Mail mail = _mapper.Map<Mail>(vm);
                    _context.MailRepository.Update(mail);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                    if (_context.MailRepository.GetByIdAsync(id) != null)
                    {
                        ModelState.AddModelError("", "Er is een probleem opgetreden bij het wegschrijven naar de database.");
                        return View(vm);
                    }
            }
            return View(vm); 
        }
        
        //Delete Action
        [Authorize(Roles = "Eigenaar")]
        [HttpPost]
        public async Task<IActionResult> MailsVerwijderen(int id)
        {
            var mail = await _context.MailRepository.GetByIdAsync(id);
            if (mail == null)
                return NotFound("Er is een probleem opgetreden de mail met dit id kan niet worden terug gevonden");
            _context.MailRepository.Delete(mail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<ActionResult> Cheeky(int mailId)
        {
            var reservatie = new DummyReservatie();
            var DummyReservatie = _mapper.Map<Reservatie>(reservatie);
            
            await _mailSender.SendMail(mailId, DummyReservatie);
            return RedirectToAction(nameof(Index));
        }
    }
}

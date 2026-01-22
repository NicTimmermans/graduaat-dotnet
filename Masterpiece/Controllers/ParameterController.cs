
using Restaurant.Models;

namespace Restaurant.Controllers
{
    [Authorize(Roles = "Eigenaar")]
    public class ParameterController : Controller
    {
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;

        public ParameterController(IUnitOfWork context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var parameters = await _context.ParameterRepository.GetAllAsync();

            var vm = new ParameterListViewModel
            {
                Parameters = _mapper.Map<List<ParameterViewModel>>(parameters),
                CreateParameter = new ParameterCreateViewModel(),
                EditParameter = new ParameterEditViewModel()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind(Prefix = "CreateParameter")] ParameterCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var vm = new ParameterListViewModel
                {
                    Parameters = (await _context.ParameterRepository.GetAllAsync()).Select(p => _mapper.Map<ParameterViewModel>(p)).ToList(),
                    IsError = true,
                    FeedbackMessage = "Ongeldige invoer — parameter is niet opgeslagen."
                };
                return View("Index", vm);
            }

            try
            {
                var entity = _mapper.Map<Parameter>(model);
                await _context.ParameterRepository.AddAsync(entity);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(WithSuccess), new { message = "Parameter toegevoegd." });
            }
            catch
            {
                return RedirectToAction(nameof(WithError), new { message = "Fout bij opslaan — wijziging niet opgeslagen." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind(Prefix = "EditParameter")] ParameterEditViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Waarde))
            {
                return RedirectToAction(nameof(WithError),
                    new { message = "Waarde mag niet leeg zijn." });
            }

            if (string.IsNullOrWhiteSpace(model.Naam))
            {
                return RedirectToAction(nameof(WithError),
                    new { message = "Naam mag niet leeg zijn." });
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(WithError),
                    new { message = "Waarde of Naam ongeldig — wijziging niet opgeslagen." });
            }

            try
            {
                var entity = _mapper.Map<Parameter>(model);
                _context.ParameterRepository.Update(entity);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(WithSuccess),
                    new { message = "Wijziging opgeslagen." });
            }
            catch
            {
                return RedirectToAction(nameof(WithError),
                    new { message = "Opslagfout — waarde blijft ongewijzigd." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var param = await _context.ParameterRepository.GetByIdAsync(id);
            if (param == null)
                return RedirectToAction(nameof(WithError), new { message = "Parameter niet gevonden." });

            _context.ParameterRepository.Delete(param);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(WithSuccess), new { message = "Parameter verwijderd." });
        }

        public async Task<IActionResult> WithSuccess(string message)
        {
            var vm = new ParameterListViewModel
            {
                Parameters = (await _context.ParameterRepository.GetAllAsync())
                    .Select(p => _mapper.Map<ParameterViewModel>(p))
                    .ToList(),
                FeedbackMessage = message,
                IsError = false
            };
            return View("Index", vm);
        }

        public async Task<IActionResult> WithError(string message)
        {
            var vm = new ParameterListViewModel
            {
                Parameters = (await _context.ParameterRepository.GetAllAsync())
                    .Select(p => _mapper.Map<ParameterViewModel>(p))
                    .ToList(),
                FeedbackMessage = message,
                IsError = true
            };
            return View("Index", vm);
        }
    }
}


using Microsoft.AspNetCore.Mvc;
using Restaurant.Models;

namespace Restaurant.Controllers
{
    [Authorize(Roles = "Kok, Eigenaar")]
    public class CategorieController : Controller
    {
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;

        public CategorieController(IUnitOfWork context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var categorien = await _context.CategorieRepository.GetCategorieAsync();
            var types = await _context.CategorieTypeRepository.GetAllAsync();

            var vm = new CategorieListViewModel
            {
                Categorien = _mapper.Map<List<CategorieViewModel>>(categorien),
                CreateCategorie = new CategorieCreateViewModel(),
                EditCategorie = new CategorieEditViewModel(),
                Types = types.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Naam
                })
                    .ToList()
            };

            vm.FeedbackMessage = TempData["Success"] as string
                 ?? TempData["Error"] as string;
            vm.IsError = TempData["Error"] != null;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind(Prefix = "CreateCategorie")] CategorieCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            if (await _context.CategorieRepository.ExistsByNameAsync(model.Naam))
            {
                ModelState.AddModelError("CreateCategorie.Naam",
                    "Deze categorienaam bestaat al.");
                return RedirectToAction("Index");
            }

            var entity = _mapper.Map<Categorie>(model);

            await _context.CategorieRepository.AddAsync(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind(Prefix = "EditCategorie")] CategorieEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            if (await _context.CategorieRepository
                .ExistsByNameAsync(model.Naam, model.Id))
            {
                ModelState.AddModelError("EditCategorie.Naam",
                    "Deze categorienaam bestaat al.");
                return RedirectToAction("Index");
            }

            var entity = await _context.CategorieRepository.GetByIdAsync(model.Id);
            if (entity == null)
                return RedirectToAction("Index");

            entity.Naam = model.Naam;
            entity.Actief = model.Actief;
            entity.TypeId = model.TypeId;

            _context.CategorieRepository.Update(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.CategorieRepository.GetCategorieMetProductenAsync(id);
            if (entity == null)
                return RedirectToAction("Index");

            if (entity.Producten.Any())
            {
                TempData["Error"] = "Categorie bevat gerechten. Herplaats deze eerst.";
                return RedirectToAction("Index");
            }

            _context.CategorieRepository.Delete(entity);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Categorie verwijderd.";
            return RedirectToAction("Index");
        }
    }
}

using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Models;
using Restaurant.ViewModels;

namespace Restaurant.Controllers
{
    [Authorize(Roles = "Kok,Ober,Eigenaar")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;

        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork context, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            if (!User.IsInRole("Kok") && !User.IsInRole("Ober") && !User.IsInRole("Eigenaar")) return RedirectToAction("Index", "Home");

            Expression<Func<CategorieType, bool>> filter = ct => true;
            if (User.IsInRole("Kok"))
            {
                filter = ct => ct.Naam != "Dranken";
            }
            else if (User.IsInRole("Ober"))
            {
                filter = ct => ct.Naam == "Dranken";
            }
            List<CategorieType> types = (await _context.CategorieTypeRepository.Find(filter)).ToList();
            List<Product> producten = (await _context.MenuRepository.GetProductsByTypesAsync(types)).OrderBy(p => p.Naam).ToList();

            Expression<Func<Categorie, bool>> categoriefilter = c => types.Contains(c.Type);
            List<Categorie> categorien = (await _context.CategorieRepository.Find(categoriefilter)).ToList();

            List<ProductListViewModel> model = _mapper.Map<List<ProductListViewModel>>(producten);

            Dictionary<ProductListViewModel, bool> modelDictionary = new Dictionary<ProductListViewModel, bool>();

            IEnumerable<Bestelling> bestellingen = await _context.BestellingRepository.GetAllAsync();
            foreach (var product in model)
            {
                modelDictionary.Add(product, bestellingen.Any(b => b.ProductId == product.Id));
            }
            ViewBag.Categorien = categorien;
            return View(modelDictionary);
        }


        [AllowAnonymous]
        public async Task<ActionResult> Create()
        {
            if (!User.IsInRole("Kok") && !User.IsInRole("Ober") && !User.IsInRole("Eigenaar")) return RedirectToAction("Index", "Home");


            Expression<Func<CategorieType, bool>> typeFilter = ct => true;
            if (User.IsInRole("Kok"))
            {
                typeFilter = ct => ct.Naam != "Dranken";
            }
            else if (User.IsInRole("Ober"))
            {
                typeFilter = ct => ct.Naam == "Dranken";
            }
            List<int> types = (await _context.CategorieTypeRepository.Find(typeFilter)).Select(t => t.Id).ToList();

            Expression<Func<Categorie, bool>> categoriefilter = c => types.Contains(c.TypeId);
            List<Categorie> categorien = (await _context.CategorieRepository.Find(categoriefilter)).ToList();

            ProductCreateViewModel viewModel = new ProductCreateViewModel()
            {
                CategorieList = new SelectList(categorien, "Id", "Naam")
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Product product = _mapper.Map<Product>(viewModel);
                await _context.MenuRepository.AddAsync(product);
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "Er is een probleem opgetreden bij het wegschrijven naar de database.");
                    return View(viewModel);
                }
            }
            return View(viewModel);
        }

        [AllowAnonymous]
        public async Task<ActionResult> Edit(int id)
        {
            if (!User.IsInRole("Kok") && !User.IsInRole("Ober") && !User.IsInRole("Eigenaar")) return RedirectToAction("Index", "Home");

            Product product = await _context.MenuRepository.GetProductByIdAsync(id);
            Expression<Func<Bestelling, bool>> bestellingFilter = b => b.ProductId == id; ;
            IList<Bestelling> bestellingen = await _context.BestellingRepository.Find(bestellingFilter);

            if (product == null) return RedirectToAction(nameof(Index));

            ProductEditViewModel viewModel = _mapper.Map<ProductEditViewModel>(product);

            Expression<Func<CategorieType, bool>> typeFilter = ct => true;
            if (User.IsInRole("Kok"))
            {
                typeFilter = ct => ct.Naam != "Dranken";
            }
            else if (User.IsInRole("Ober"))
            {
                typeFilter = ct => ct.Naam == "Dranken";
            }
            List<int> types = (await _context.CategorieTypeRepository.Find(typeFilter)).Select(t => t.Id).ToList();

            Expression<Func<Categorie, bool>> categoriefilter = c => types.Contains(c.TypeId);
            List<Categorie> categorien = (await _context.CategorieRepository.Find(categoriefilter)).ToList();

            viewModel.CategorieList = new SelectList(categorien, "Id", "Naam");
            viewModel.ExistsInBestelling = bestellingen.Any();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }


            Product product = await _context.MenuRepository.GetProductByIdAsync(viewModel.Id);

            if (product == null) return NotFound("Product niet gevonden");

            _mapper.Map(viewModel, product);

            if (viewModel.Afbeelding != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/producten");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }


                string fileName = $"{product.Id}_product" + Path.GetExtension(viewModel.Afbeelding?.FileName ?? string.Empty);
                string fileSavePath = Path.Combine(uploadsFolder, fileName);

                using (FileStream fileStream = new FileStream(fileSavePath, FileMode.Create))
                {
                    await viewModel.Afbeelding?.CopyToAsync(fileStream);
                }
                product.AfbeeldingNaam = fileName;
            }

            _context.MenuRepository.Update(product);
            try
            {
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "Er is een probleem opgetreden bij het wegschrijven naar de database.");
                return View(viewModel);
            }

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Product product = await _context.MenuRepository.GetProductByIdAsync(id);


            if (product != null)
            {
                Expression<Func<Bestelling, bool>> bestellingFilter = b => b.ProductId == id;
                IList<Bestelling> bestellingen = await _context.BestellingRepository.Find(bestellingFilter);

                if (bestellingen.Any())
                {
                    product.Actief = false;
                    _context.MenuRepository.Update(product);
                }
                else
                {
                    if (product.PrijsProducten.Any())
                    {
                        foreach (PrijsProduct prijs in product.PrijsProducten.ToList())
                        {
                            _context.PrijsRepository.Delete(prijs);
                        }
                    }
                    _context.MenuRepository.Delete(product);
                }
                _context.SaveChanges();
            }
            else
                ModelState.AddModelError("", "Product Not Found");
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActief(int id)
        {
            Product product = await _context.MenuRepository.GetProductByIdAsync(id);
            if (product != null)
            {
                product.Actief = !product.Actief;
                _context.MenuRepository.Update(product);
                _context.SaveChanges();
            }
            else
                ModelState.AddModelError("", "Product Not Found");
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleSuggestie(int id)
        {
            Product product = await _context.MenuRepository.GetProductByIdAsync(id);
            if (product != null)
            {
                product.IsSuggestie = !product.IsSuggestie;
                _context.MenuRepository.Update(product);
                _context.SaveChanges();
            }
            else
                ModelState.AddModelError("", "Product Not Found");
            return RedirectToAction("Index");
        }
    }
}

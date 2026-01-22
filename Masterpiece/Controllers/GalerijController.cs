using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Restaurant.Controllers
{
    [Authorize(Roles = "Eigenaar")]
    public class GalerijController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public GalerijController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "img", "galerij");

            var imageUrls = Directory.GetFiles(folderPath)
                .Select(file => "/img/galerij/" + Path.GetFileName(file))
                .ToList();

            return View(imageUrls);
        }
        [AllowAnonymous]
        public IActionResult Edit()
        {
            if (!User.IsInRole("Kok") && !User.IsInRole("Ober") && !User.IsInRole("Eigenaar")) return RedirectToAction("Index", "Home");

            string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "img", "galerij");

            var imageUrls = Directory.GetFiles(folderPath)
                .Select(file => "/img/galerij/" + Path.GetFileName(file))
                .ToList();
            GalerijEditViewModel model = new GalerijEditViewModel
            {
                Afbeeldingen = imageUrls
            };

            return View(model);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GalerijEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            if (viewModel.NieuweAfbeelding != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/galerij");

                var name = Guid.NewGuid().ToString();
                string fileName = $"{name}" + Path.GetExtension(viewModel.NieuweAfbeelding?.FileName ?? string.Empty);
                string fileSavePath = Path.Combine(uploadsFolder, fileName);

                using (FileStream fileStream = new FileStream(fileSavePath, FileMode.Create))
                {
                    await viewModel.NieuweAfbeelding?.CopyToAsync(fileStream);
                }
            }

            return RedirectToAction("Edit");
        }

        [ValidateAntiForgeryToken]
        public IActionResult Delete(string relativePath)
        {
            if (!string.IsNullOrEmpty(relativePath))
            {
                relativePath = relativePath.Substring(1);

                string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            return RedirectToAction("Edit");
        }
    }
}

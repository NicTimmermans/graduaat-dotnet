using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Restaurant.Controllers;
using Restaurant.Data.UnitOfWork;
using Restaurant.Models;
using Restaurant.ViewModels;

namespace Masterpiece_Test.Controllers
{
    internal class ProductControllerTest
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IWebHostEnvironment> _envMock;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _envMock = new Mock<IWebHostEnvironment>();
        }

        public ProductController SetUserWithRole(string role)
        {
            ProductController _controller = new ProductController(_unitOfWorkMock.Object, _mapperMock.Object, _envMock.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] {
                    new Claim(ClaimTypes.Name, "TestUser"),
                    new Claim(ClaimTypes.Role, role)
                },
                "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user
                }
            };
            return _controller;
        }

        [Test]
        public async Task Index_UserIsKok_ReturnsViewWithDictionary()
        {
            ProductController _controller = SetUserWithRole("Kok");

            // Mock Categorie Types
            var categories = new List<CategorieType>()
                {
                    new CategorieType { Id = 1, Naam = "Gerechten" },
                    new CategorieType { Id = 2, Naam = "Dranken" }   // Should be filtered out
                };

            _unitOfWorkMock.Setup(u => u.CategorieTypeRepository.Find(
                It.IsAny<Expression<Func<CategorieType, bool>>>(),
                It.IsAny<Expression<Func<CategorieType, object>>[]>()
            ))
            .ReturnsAsync((Expression<Func<CategorieType, bool>> filter,
                           Expression<Func<CategorieType, object>>[] inc) =>
                categories.Where(filter.Compile()).ToList());

            // Mock products
            var products = new List<Product>()
                {
                    new Product { Id = 10, Naam = "Broodje met Brie", Categorie = new Categorie{ Id = 1, Naam = "Brood", TypeId = 1 } }
                };

            _unitOfWorkMock.Setup(u => u.MenuRepository.GetProductsByTypesAsync(It.IsAny<List<CategorieType>>()))
                .ReturnsAsync(products);

            // Mock categories for ViewBag
            _unitOfWorkMock.Setup(u => u.CategorieRepository.Find(
                It.IsAny<Expression<Func<Categorie, bool>>>(),
                It.IsAny<Expression<Func<Categorie, object>>[]>()
            ))
            .ReturnsAsync(new List<Categorie>());

            // Mock existing orders (Bestellingen)
            _unitOfWorkMock.Setup(u => u.BestellingRepository.GetAllAsync())
                .ReturnsAsync(new List<Bestelling>()
                {
                    new Bestelling { ProductId = 10 }
                });

            // Mapper: Product → ProductListViewModel
            _mapperMock.Setup(m => m.Map<List<ProductListViewModel>>(products))
                .Returns(new List<ProductListViewModel>()
                {
                    new ProductListViewModel { Id = 10, Naam = "TestProduct" }
                });

            // ACT
            var result = await _controller.Index() as ViewResult;

            // ASSERT
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Dictionary<ProductListViewModel, bool>>(result.Model);

            var dict = (Dictionary<ProductListViewModel, bool>)result.Model;

            Assert.AreEqual(1, dict.Count);
            Assert.IsTrue(dict.First().Value); // Product is in a bestelling
        }


        [TestCase("klant")]
        [TestCase("Zaalverantwoordelijke")]
        public async Task Index_InvalidUser_RedirectToHome(string rol)
        {
            ProductController _controller = SetUserWithRole(rol);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);

            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirect.ActionName);
            Assert.AreEqual("Home", redirect.ControllerName);
        }
        [Test]
        public async Task CreatePost_Success_ReturnsToIndex()
        {
            ProductController _controller = SetUserWithRole("Eigenaar");

            var viewModel = new ProductCreateViewModel
            {
                Naam = "Test Product",
                Beschrijving = "Test Beschrijving",
                AllergenenInfo = "Noten",
                Actief = true,
                IsSuggestie = false,
                CategorieId = 1,
                Prijs = 12.5m
            };

            var product = new Product
            {
                Naam = viewModel.Naam,
                Beschrijving = viewModel.Beschrijving,
                AllergenenInfo = viewModel.AllergenenInfo,
                Actief = viewModel.Actief,
                IsSuggestie = viewModel.IsSuggestie,
                CategorieId = viewModel.CategorieId,
                PrijsProducten = new List<PrijsProduct>
                {
                    new PrijsProduct
                    {
                        Prijs = viewModel.Prijs,
                        DatumVanaf = DateTime.Now
                    }
                }
            };
            _mapperMock.Setup(m => m.Map<Product>(viewModel)).Returns(product);

            _unitOfWorkMock.Setup(u => u.MenuRepository.AddAsync(product))
                           .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _controller.Create(viewModel);

            _mapperMock.Verify(m => m.Map<Product>(viewModel), Times.Once);
            _unitOfWorkMock.Verify(u => u.MenuRepository.AddAsync(product), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirect.ActionName);
        }
        [Test]
        public async Task CreatePost_Fail_ReturnsToIndex()
        {
            ProductController _controller = SetUserWithRole("Eigenaar");

            var viewModel = new ProductCreateViewModel
            {
                Naam = "Test Product",
                Beschrijving = "Test Beschrijving",
                AllergenenInfo = "Noten",
                Actief = true,
                IsSuggestie = false,
                CategorieId = 1,
                Prijs = 12.5m
            };

            var product = new Product
            {
                Naam = viewModel.Naam,
                Beschrijving = viewModel.Beschrijving,
                AllergenenInfo = viewModel.AllergenenInfo,
                Actief = viewModel.Actief,
                IsSuggestie = viewModel.IsSuggestie,
                CategorieId = viewModel.CategorieId,
                PrijsProducten = new List<PrijsProduct>
                {
                    new PrijsProduct
                    {
                        Prijs = viewModel.Prijs,
                        DatumVanaf = DateTime.Now
                    }
                }
            };
            _mapperMock.Setup(m => m.Map<Product>(viewModel)).Returns(product);

            _unitOfWorkMock.Setup(u => u.MenuRepository.AddAsync(product))
                           .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ThrowsAsync(new DbUpdateConcurrencyException());

            var result = await _controller.Create(viewModel);

            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(viewModel, viewResult.Model);
            Assert.IsTrue(_controller.ModelState.ErrorCount > 0);
            Assert.That(_controller.ModelState.Values.SelectMany(v => v.Errors)
                .Any(e => e.ErrorMessage.Contains("Er is een probleem opgetreden bij het wegschrijven naar de database.")));
        }

        [Test]
        public async Task CreatePost_ModelStateInvalid_ReturnsViewWithSameModel()
        {
            ProductController _controller = SetUserWithRole("Kok");

            _controller.ModelState.AddModelError("Prijs", "Required");

            var viewModel = new ProductCreateViewModel();

            // Act
            var result = await _controller.Create(viewModel);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var view = (ViewResult)result;
            Assert.AreEqual(viewModel, view.Model);
        }
    }
}

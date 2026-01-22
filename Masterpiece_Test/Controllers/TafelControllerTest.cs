using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Restaurant.Controllers;
using Restaurant.Data.UnitOfWork;
using Restaurant.Models;
using Restaurant.ViewModels;

namespace Masterpiece_Test.Controllers
{
    internal class OldTafelControllerTest
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
        }

        private OldTafelController SetUserWithRole(string role)
        {
            var controller = new OldTafelController(_unitOfWorkMock.Object, _mapperMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] {
                    new Claim(ClaimTypes.Name, "TestUser"),
                    new Claim(ClaimTypes.Role, role)
                },
                "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user
                }
            };

            return controller;
        }

        [Test]
        public async Task Index_TabIsToewijzen_RedirectsToToewijzen()
        {
            var controller = SetUserWithRole("Eigenaar");

            var result = await controller.Index(filter: "active", tab: "toewijzen");

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Toewijzen", redirect.ActionName);
        }

        [Test]
        public async Task Index_FilterActive_ReturnsView_WithOnlyActiveTables()
        {
            var controller = SetUserWithRole("Eigenaar");

            var tafels = new List<Tafel>
            {
                new Tafel { Id = 1, TafelNummer = "T01", AantalPersonen = 4, MinAantalPersonen = 2, Actief = true },
                new Tafel { Id = 2, TafelNummer = "T02", AantalPersonen = 2, MinAantalPersonen = 1, Actief = false }
            };

            var reservaties = new List<Reservatie>(); // leeg is ok voor deze test

            _unitOfWorkMock.Setup(u => u.TafelRepository.GetAllAsync())
                .ReturnsAsync(tafels);

            _unitOfWorkMock.Setup(u => u.ReservatieRepository.GetAllActiveAsync())
                .ReturnsAsync(reservaties);

            var result = await controller.Index(filter: "active", tab: "beheren");

            Assert.IsInstanceOf<ViewResult>(result);
            var view = (ViewResult)result;

            Assert.IsInstanceOf<TafelListViewModel>(view.Model);
            var vm = (TafelListViewModel)view.Model;

            Assert.AreEqual("active", vm.ActiveFilter);
            Assert.AreEqual(1, vm.Tafels.Count);
            Assert.IsTrue(vm.Tafels.All(t => t.Actief));
        }

        [Test]
        public async Task Toewijzen_MapsReservatiesAndFiltersAlleenZonderTafel()
        {
            var controller = SetUserWithRole("Eigenaar");

            var targetDate = DateTime.Today;

            var resWithTable = new Reservatie
            {
                Id = 1,
                Datum = targetDate,
                TijdSlotId = 1,
                AantalPersonen = 2,
                Tafellijsten = new List<TafelLijst>
                {
                    new TafelLijst
                    {
                        TafelId = 1,
                        Tafel = new Tafel { Id = 1, TafelNummer = "T01" }
                    }
                }
            };

            var resNoTable = new Reservatie
            {
                Id = 2,
                Datum = targetDate,
                TijdSlotId = 1,
                AantalPersonen = 2,
                Tafellijsten = new List<TafelLijst>() // geen tafels
            };

            _unitOfWorkMock.Setup(u => u.ReservatieRepository.GetAllWithUserAndTijdslotAsync())
                .ReturnsAsync(new List<Reservatie> { resWithTable, resNoTable });

            // mapper output bepalen (zodat de filter op HeeftTafel werkt op VM-niveau)
            _mapperMock.Setup(m => m.Map<List<OldTafelToewijzenReservatieViewModel>>(It.IsAny<List<Reservatie>>()))
                .Returns(new List<OldTafelToewijzenReservatieViewModel>
                {
                    new OldTafelToewijzenReservatieViewModel { ReservatieId = 1, TafelNummers = new List<string>{ "T01" } },
                    new OldTafelToewijzenReservatieViewModel { ReservatieId = 2, TafelNummers = new List<string>() }
                });

            var result = await controller.Toewijzen(datum: targetDate, alleenZonderTafel: true);

            Assert.IsInstanceOf<ViewResult>(result);
            var view = (ViewResult)result;

            Assert.IsInstanceOf<List<OldTafelToewijzenReservatieViewModel>>(view.Model);
            var model = (List<OldTafelToewijzenReservatieViewModel>)view.Model;

            Assert.AreEqual(1, model.Count);
            Assert.AreEqual(2, model[0].ReservatieId);
            Assert.IsFalse(model[0].HeeftTafel);
        }

        [Test]
        public async Task DeleteConfirmed_SoftDelete_SetsActiefFalse_AndSaves()
        {
            var controller = SetUserWithRole("Eigenaar");

            var tafel = new Tafel { Id = 1, Actief = true };

            _unitOfWorkMock.Setup(u => u.TafelRepository.GetByIdAsync(1))
                .ReturnsAsync(tafel);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // filter active => soft delete pad
            var result = await controller.DeleteConfirmed(id: 1, currentFilter: "active");

            Assert.IsFalse(tafel.Actief);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirect.ActionName);
        }

        [Test]
        public async Task DeleteConfirmed_HardDelete_WhenNoLinks_DeletesEntity_AndSaves()
        {
            var controller = SetUserWithRole("Eigenaar");

            var tafel = new Tafel { Id = 1, Actief = false };

            _unitOfWorkMock.Setup(u => u.TafelRepository.GetByIdAsync(1))
                .ReturnsAsync(tafel);

            _unitOfWorkMock.Setup(u => u.TafelRepository.HasLinkedReservatiesAsync(1))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await controller.DeleteConfirmed(id: 1, currentFilter: "inactive");

            _unitOfWorkMock.Verify(u => u.TafelRepository.Delete(tafel), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirect.ActionName);
        }
    }
}

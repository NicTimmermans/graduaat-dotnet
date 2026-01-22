using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Restaurant.Controllers;
using Restaurant.Data.Repository;
using Restaurant.Data.Repository.Interfaces;
using Restaurant.Data.UnitOfWork;
using Restaurant.Models;
using Restaurant.Services;
using Restaurant.Services.LoggingService;
using Restaurant.Services.MailService;
using Restaurant.ViewModels;

namespace Masterpiece_Test.Controllers
{
    internal class BestellingControllerTest
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IMailSender> _mailSenderMock;
        private Mock<ICustomLogger> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _mailSenderMock = new Mock<IMailSender>();
            _loggerMock = new Mock<ICustomLogger>();
        }

        private BestellingController CreateController()
        {
            return new BestellingController(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _mailSenderMock.Object,
                _loggerMock.Object,
                hub: null
            );
        }

        // -------------------------------
        // TEST 1: Add → succesvolle toevoeging Nick
        // -------------------------------
        [Test]
        public async Task Add_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var bestellingRepoMock = new Mock<IBestellingRepository>();

            _unitOfWorkMock
                .Setup(u => u.BestellingRepository)
                .Returns(bestellingRepoMock.Object);

            _mapperMock
                .Setup(m => m.Map<Bestelling>(It.IsAny<BestellingCreateViewModel>()))
                .Returns(new Bestelling());

            var controller = CreateController();

            var vm = new BestellingCreateViewModel
            {
                ReservatieId = 1,
                ProductId = 2,
                Aantal = 3,
                ViewMode = 1
            };

            bestellingRepoMock
                .Setup(r => r.ProductExistsAsync(vm.ProductId))
                .ReturnsAsync(true);

            bestellingRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Bestelling>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await controller.Add(vm);

            // Assert
            bestellingRepoMock.Verify(
                r => r.AddAsync(It.IsAny<Bestelling>()),
                Times.Once
            );

            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);
            Assert.AreEqual("Menu", redirect.ControllerName);
            Assert.AreEqual(vm.ReservatieId, redirect.RouteValues["reservatieId"]);
            Assert.AreEqual(vm.ViewMode, redirect.RouteValues["viewMode"]);
        }


        // -------------------------------
        // TEST 2: Update → aantal aanpassen Nick
        // -------------------------------
        [Test]
        public async Task Update_ValidModel_RedirectsToIndex()
        {
            // Arrange: controller en viewmodel aanmaken
            var controller = CreateController();

            //mock bestelling
            var vm = new BestellingEditViewModel
            {
                Id = 1,
                Aantal = 5,
                ViewMode = 1
            };


            var bestelling = new Bestelling { Id = 1, ReservatieId = 10, Aantal = 3 };


            _unitOfWorkMock.Setup(u => u.BestellingRepository.UpdateAantalAsync(vm.Id, vm.Aantal))
                .Returns(Task.CompletedTask);


            _unitOfWorkMock.Setup(u => u.BestellingRepository.GetAsync(vm.Id))
                .ReturnsAsync(bestelling);


            var result = await controller.Update(vm);

            // Assert: controleer dat UpdateAantalAsync 1 keer is aangeroepen
            _unitOfWorkMock.Verify(u => u.BestellingRepository.UpdateAantalAsync(vm.Id, vm.Aantal), Times.Once);

            // Assert: controleer dat het resultaat een redirect is naar Index
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;

            // Controleer dat het redirect naar de juiste reservatieId gaat
            Assert.AreEqual("Index", redirect.ActionName);
            Assert.AreEqual(bestelling.ReservatieId, redirect.RouteValues["reservatieId"]);
        }
    }
}

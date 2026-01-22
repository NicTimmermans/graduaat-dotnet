using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Restaurant.Configuration;
using Restaurant.Models;
using Restaurant.ViewModels;

namespace Masterpiece_Test.Configuration
{
    internal class MapperProfileTest
    {
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = config.CreateMapper();
        }
        //Nic heeft deze geschreven
        [TestCase("test_image.png", "/img/producten/test_image.png")]
        [TestCase(null, "/img/producten/default_product.png")]
        public void ProductToMenuViewModel_Mapping(string? afbeeldingNaam, string afbeeldingUrl)
        {
            Product product = new Product
            {
                Id = 1,
                Naam = "Test Product",
                AfbeeldingNaam = afbeeldingNaam,
                IsSuggestie = true,
                Categorie = new Categorie
                {
                    Naam = "Hoofdgerechten",
                    Actief = true,
                    Type = new CategorieType
                    {
                        Naam = "Gerechten",
                        Actief = true
                    }
                },
                PrijsProducten = new List<PrijsProduct>
                {
                    new PrijsProduct { Id = 1, Prijs = 10.0m, DatumVanaf = new DateTime(2023, 1, 1) },
                    new PrijsProduct { Id = 2, Prijs = 12.0m, DatumVanaf = new DateTime(2023, 6, 1) }
                }
            };

            MenuViewModel viewModel = _mapper.Map<MenuViewModel>(product);

            Assert.That(viewModel.Naam, Is.EqualTo("Test Product"));
            Assert.That(viewModel.AfbeeldingUrl, Is.EqualTo(afbeeldingUrl));
            Assert.That(viewModel.IsSuggestie, Is.True);
            Assert.That(viewModel.Prijs, Is.EqualTo(12.0m));
            Assert.That(viewModel.CategorieNaam, Is.EqualTo("Hoofdgerechten"));
            Assert.That(viewModel.CategorieActief, Is.True);
            Assert.That(viewModel.CategorieTypeNaam, Is.EqualTo("Gerechten"));
            Assert.That(viewModel.CategorieTypeActief, Is.True);
        }
        [Test]
        public void ProductCreateViewModelToProduct_Mapping()
        {
            ProductCreateViewModel createViewModel = new ProductCreateViewModel
            {
                Naam = "New Product",
                Beschrijving = "A new product description",
                AllergenenInfo = "None",
                Actief = true,
                IsSuggestie = false,
                CategorieId = 2,
                Prijs = 15.5m
            };


            Product product = _mapper.Map<Product>(createViewModel);

            Assert.That(product.Naam, Is.EqualTo("New Product"));
            Assert.That(product.Beschrijving, Is.EqualTo("A new product description"));
            Assert.That(product.AllergenenInfo, Is.EqualTo("None"));
            Assert.That(product.Actief, Is.True);
            Assert.That(product.IsSuggestie, Is.False);
            Assert.That(product.CategorieId, Is.EqualTo(2));
            Assert.That(product.PrijsProducten.Count, Is.EqualTo(1));
            Assert.That(product.PrijsProducten.First().Prijs, Is.EqualTo(15.5m));
            Assert.That(product.PrijsProducten.First().DatumVanaf.Value.Date, Is.EqualTo(DateTime.Now.Date));
        }

        [TestCase("test_image.png", "/img/producten/test_image.png")]
        [TestCase(null, "/img/producten/default_product.png")]
        public void ProductToProductEditViewModel_Mapping(string? afbeeldingNaam, string afbeeldingUrl)
        {
            Product product = new Product
            {
                Id = 1,
                Naam = "Test Product",
                AfbeeldingNaam = afbeeldingNaam,
                IsSuggestie = true,
                CategorieId = 1,
                PrijsProducten = new List<PrijsProduct>
                {
                    new PrijsProduct { Id = 1, Prijs = 10.0m, DatumVanaf = new DateTime(2023, 1, 1) },
                    new PrijsProduct { Id = 2, Prijs = 12.0m, DatumVanaf = new DateTime(2023, 6, 1) }
                }
            };

            ProductEditViewModel viewModel = _mapper.Map<ProductEditViewModel>(product);

            Assert.That(viewModel.Naam, Is.EqualTo("Test Product"));
            Assert.That(viewModel.AfbeeldingUrl, Is.EqualTo(afbeeldingUrl));
            Assert.That(viewModel.IsSuggestie, Is.True);
            Assert.That(viewModel.Prijs, Is.EqualTo(12.0m));
            Assert.That(viewModel.CategorieId, Is.EqualTo(1));
        }
        [Test]
        public void ProductEditViewModelToProduct_Mapping()
        {
            ProductEditViewModel editViewModel = new ProductEditViewModel
            {
                Id = 1,
                Naam = "Updated Product",
                Beschrijving = "An updated product description",
                AllergenenInfo = "Gluten",
                Actief = false,
                IsSuggestie = true,
                CategorieId = 3,
                Prijs = 20.0m
            };

            Product product = _mapper.Map<Product>(editViewModel);

            Assert.That(product.Id, Is.EqualTo(1));
            Assert.That(product.Naam, Is.EqualTo("Updated Product"));
            Assert.That(product.Beschrijving, Is.EqualTo("An updated product description"));
            Assert.That(product.AllergenenInfo, Is.EqualTo("Gluten"));
            Assert.That(product.Actief, Is.False);
            Assert.That(product.IsSuggestie, Is.True);
            Assert.That(product.CategorieId, Is.EqualTo(3));
            Assert.That(product.PrijsProducten.Count, Is.EqualTo(1));
            Assert.That(product.PrijsProducten.First().Prijs, Is.EqualTo(20.0m));
            Assert.That(product.PrijsProducten.First().DatumVanaf.Value.Date, Is.EqualTo(DateTime.Now.Date));
        }

        // -------------------------------
        // TEST 1: testing op grouperen + data  Nick
        // -------------------------------

        [Test]
        public void BestellingEnumerable_To_BestellingOverzichtViewModel_Mapping()
        {
            // -------------------------------
            // Arrange: testdata opbouwen
            // -------------------------------

            var gerechtenType = new CategorieType { Naam = "Gerechten" };
            var drankenType = new CategorieType { Naam = "Dranken" };

            var reservatie = new Reservatie
            {
                Id = 10,
                Tafellijsten = new List<TafelLijst?>
        {
            new TafelLijst
            {
                Tafel = new Tafel { TafelNummer = "T05" }
            },
            new TafelLijst
            {
                Tafel = new Tafel { TafelNummer = "T07" }
            }
        }
            };

            var bestellingen = new List<Bestelling>
    {
        new Bestelling
        {
            Id = 1,
            ReservatieId = 10,
            Reservatie = reservatie,
            TijdstipBestelling = new DateTime(2024, 1, 1, 12, 10, 0),
            Product = new Product
            {
                Naam = "Steak",
                Categorie = new Categorie
                {
                    Type = gerechtenType
                }
            }
        },
        new Bestelling
        {
            Id = 2,
            ReservatieId = 10,
            Reservatie = reservatie,
            TijdstipBestelling = new DateTime(2024, 1, 1, 12, 05, 0),
            Product = new Product
            {
                Naam = "Cola",
                Categorie = new Categorie
                {
                    Type = drankenType
                }
            }
        }
    };

            // -------------------------------
            // Act: mapping uitvoeren
            // -------------------------------

            BestellingOverzichtViewModel vm =
                _mapper.Map<BestellingOverzichtViewModel>(bestellingen);

            // -------------------------------
            // Assert: resultaten controleren
            // -------------------------------

            
            Assert.That(vm.ReservatieId, Is.EqualTo(10));

            
            Assert.That(vm.TafelNummers.Count, Is.EqualTo(2));
            Assert.That(vm.TafelNummers, Does.Contain("T05"));
            Assert.That(vm.TafelNummers, Does.Contain("T07"));

            // Gerechten
            Assert.That(vm.Gerechten.Count(), Is.EqualTo(1));
            Assert.That(vm.Gerechten.First().ProductNaam, Is.EqualTo("Steak"));

            // Dranken
            Assert.That(vm.Dranken.Count(), Is.EqualTo(1));
            Assert.That(vm.Dranken.First().ProductNaam, Is.EqualTo("Cola"));
        }

        // -------------------------------
        // TEST 2: testing op correcte tafel(s) of lege string indien nog geen tafel toegewezen  Nick
        // -------------------------------

        [Test]
        public void Reservatie_To_ReservatieKiesViewModel_Mapping()
        {
            // -------------------------------
            // Arrange
            // -------------------------------

            var datum = new DateTime(2024, 5, 10);

            var reservatie = new Reservatie
            {
                Id = 15,
                Datum = datum,
                Tafellijsten = new List<TafelLijst?>
        {
            new TafelLijst
            {
                Tafel = new Tafel
                {
                    TafelNummer = "T01"
                }
            },
            new TafelLijst
            {
                Tafel = new Tafel
                {
                    TafelNummer = "T05"
                }
            },
            new TafelLijst
            {
                Tafel = null   // expliciete null-case
            }
        }
            };

            // -------------------------------
            // Act
            // -------------------------------

            ReservatieKiesViewModel vm =
                _mapper.Map<ReservatieKiesViewModel>(reservatie);

            // -------------------------------
            // Assert
            // -------------------------------

            // Datum wordt 1-op-1 gemapt
            Assert.That(vm.Datum, Is.EqualTo(datum));

            // Tafelnummers correct opgebouwd
            Assert.That(vm.TafelNummers.Count, Is.EqualTo(3));

            Assert.That(vm.TafelNummers[0], Is.EqualTo("T01"));
            Assert.That(vm.TafelNummers[1], Is.EqualTo("T05"));
            Assert.That(vm.TafelNummers[2], Is.EqualTo("")); // null tafel → lege string
        }


        //Thomas heeft deze geschreven
        [Test]
        public void BestellingToBestellingViewModel_Mapping()
        {
            Bestelling bestelling = new Bestelling
            {
                Id = 1,
                ReservatieId = 1,
                Product = new Product
                {
                    Id = 1,
                    Naam = "Test Product",
                    PrijsProducten = new List<PrijsProduct>
                    {
                        new PrijsProduct { Id = 1, Prijs = 10.0m, DatumVanaf = new DateTime(2023, 1, 1) },
                        new PrijsProduct { Id = 2, Prijs = 12.0m, DatumVanaf = new DateTime(2023, 6, 1) }
                    }
                },
                ProductId = 1,
                StatusId = 4,
                Aantal = 1,
                Opmerking = "Test bestelling geen rekening met houden."
            };

            BestellingViewModel viewModel = _mapper.Map<BestellingViewModel>(bestelling);

            Assert.That(viewModel.ProductNaam, Is.EqualTo("Test Product"));
            Assert.That(viewModel.HuidigePrijs, Is.EqualTo(12.0m));
            Assert.That(viewModel.ProductId, Is.EqualTo(1));
            Assert.That(viewModel.ReservatieId, Is.EqualTo(1));
            Assert.That(viewModel.Id, Is.EqualTo(1));
            Assert.That(viewModel.StatusId, Is.EqualTo(4));
            Assert.That(viewModel.Aantal, Is.EqualTo(1));
            Assert.That(viewModel.Opmerking, Is.EqualTo("Test bestelling geen rekening met houden."));
            Assert.That(viewModel.LijnTotaal, Is.EqualTo(12.0m));
        }
    }
}

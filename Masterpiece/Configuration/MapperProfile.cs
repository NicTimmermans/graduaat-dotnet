
using System.Collections.Generic;
using Restaurant.Models;
namespace Restaurant.Configuration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CustomUser, AccountIndexViewModel>();

            CreateMap<Mail, MailVersturenViewModel>();
            CreateMap<Mail, MailAanpassenViewModel>().ReverseMap();
            CreateMap<MailsAanmakenViewModel, Mail>().ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<Reservatie, DummyReservatie>().ReverseMap();

            CreateMap<Product, MenuViewModel>()
                .ForMember(dest => dest.AfbeeldingUrl, opt => opt.MapFrom(src => src.AfbeeldingNaam != null ? "/img/producten/" + src.AfbeeldingNaam : "/img/producten/default_product.png"))
                .ForMember(dest => dest.IsSuggestie, opt => opt.MapFrom(src => src.IsSuggestie))
                .ForMember(dest => dest.Prijs, opt => opt.MapFrom(
                    src => src.PrijsProducten
                        .OrderByDescending(p => p.DatumVanaf)
                        .ThenByDescending(p => p.Id)
                        .Select(p => p.Prijs)
                        .FirstOrDefault()))
                .ForMember(dest => dest.CategorieNaam, opt => opt.MapFrom(src => src.Categorie.Naam))
                .ForMember(dest => dest.CategorieActief, opt => opt.MapFrom(src => src.Categorie.Actief))
                .ForMember(dest => dest.CategorieTypeNaam, opt => opt.MapFrom(src => src.Categorie.Type.Naam))
                .ForMember(dest => dest.CategorieTypeActief, opt => opt.MapFrom(src => src.Categorie.Type.Actief));

            CreateMap<Bestelling, BestellingViewModel>()
                .ForMember(dest => dest.ProductNaam,
                    opt => opt.MapFrom(src => src.Product.Naam))
                .ForMember(dest => dest.HuidigePrijs,
                    opt => opt.MapFrom(src =>
                        src.Product.PrijsProducten
                            .OrderByDescending(p => p.DatumVanaf)
                            .ThenByDescending(p => p.Id)
                            .Select(p => (decimal?)p.Prijs)
                            .FirstOrDefault() ?? 0m
                    ))
                .ForMember(dest => dest.StatusNaam,
                    opt => opt.MapFrom(src => src.Status.Naam));

            CreateMap<AccountCreateViewModel, CustomUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Actief, opt => opt.MapFrom(src => true));

            CreateMap<CustomUser, AccountEditViewModel>().ReverseMap();

            CreateMap<ReservatieViewModel, Reservatie>()
                .ForMember(dest => dest.Datum, opt => opt.MapFrom(src => src.GekozenDatum))
                .ForMember(dest => dest.TijdSlotId, opt => opt.MapFrom(src => src.GekozenTijdslotId))
                .ForMember(dest => dest.KlantId, opt => opt.Ignore())
                .ForMember(dest => dest.Tafellijsten, opt => opt.Ignore())
                .ForMember(dest => dest.Betaald, opt => opt.Ignore())
                .ForMember(dest => dest.IsAanwezig, opt => opt.Ignore())
                .ForMember(dest => dest.WelkomstmailVerstuurdOp, opt => opt.Ignore())
                .ForMember(dest => dest.Bestellingen, opt => opt.Ignore());

            CreateMap<Reservatie, ReservatieViewModel>()
                .ForMember(dest => dest.GekozenDatum, opt => opt.MapFrom(src => src.Datum))
                .ForMember(dest => dest.GekozenTijdslotId, opt => opt.MapFrom(src => src.TijdSlotId))
                .ForMember(dest => dest.Tijdsloten, opt => opt.Ignore())
                .ForMember(dest => dest.BeschikbareDatums, opt => opt.Ignore())
                .ForMember(dest => dest.ReserveringVoltooid, opt => opt.Ignore())
                .ForMember(dest => dest.Foutmelding, opt => opt.Ignore());

            CreateMap<Reservatie, ReservatieBeheerViewModel>()
                .ForMember(dest => dest.KlantNaam, opt => opt.MapFrom(src => src.CustomUser.Voornaam + " " + src.CustomUser.Achternaam))
                .ForMember(dest => dest.TijdslotNaam, opt => opt.MapFrom(src => src.Tijdslot.Naam));

            CreateMap<BestellingCreateViewModel, Bestelling>();
            CreateMap<BestellingEditViewModel, Bestelling>();

            CreateMap<Parameter, ParameterViewModel>();
            CreateMap<ParameterCreateViewModel, Parameter>();
            CreateMap<ParameterEditViewModel, Parameter>();

            CreateMap<Product, ProductListViewModel>()
                .ForMember(dest => dest.Categorie, opt => opt.MapFrom(src => src.Categorie.Naam))
                .ForMember(dest => dest.Prijs, opt => opt.MapFrom(
                    src => src.PrijsProducten
                        .OrderByDescending(p => p.DatumVanaf)
                        .ThenByDescending(p => p.Id)
                        .Select(p => p.Prijs)
                        .FirstOrDefault()));

            CreateMap<ProductCreateViewModel, Product>()
            .AfterMap((src, dest) =>
             {
                 dest.PrijsProducten.Add(new PrijsProduct
                 {
                     DatumVanaf = DateTime.Now,
                     Prijs = src.Prijs
                 });
             });
            CreateMap<Product, ProductEditViewModel>()
                .ForMember(dest => dest.AfbeeldingUrl, opt => opt.MapFrom(src => src.AfbeeldingNaam != null ? "/img/producten/" + src.AfbeeldingNaam : "/img/producten/default_product.png"))
                .ForMember(dest => dest.Prijs, opt => opt.MapFrom(
                    src => src.PrijsProducten
                        .OrderByDescending(p => p.DatumVanaf)
                        .ThenByDescending(p => p.Id)
                        .Select(p => p.Prijs)
                        .FirstOrDefault()));

            CreateMap<ProductEditViewModel, Product>()
                .AfterMap((src, dest) =>
                {
                    dest.PrijsProducten.Add(new PrijsProduct
                    {
                        DatumVanaf = DateTime.Now,
                        Prijs = src.Prijs
                    });
                });

            CreateMap<CustomUser, AccountDashboardViewModel>()
                .ForMember(dest => dest.LandNaam,
                    opt => opt.MapFrom(src => src.Land.Naam))
                .ForMember(dest => dest.RolNaam,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Reservaties,
                    opt => opt.Ignore()); // vullen we in controller

            CreateMap<Reservatie, ReservatieDashboardViewModel>()
                .ForMember(dest => dest.TijdslotNaam,
                    opt => opt.MapFrom(src => src.Tijdslot.Naam))
                .ForMember(dest => dest.AantalPersonen, opt => opt.MapFrom(src => src.AantalPersonen));


            CreateMap<Reservatie, ReservatieKiesViewModel>()
            .ForMember(d => d.TafelNummers,
                opt => opt.MapFrom(s => s.Tafellijsten
                    .Select(tl => tl.Tafel != null ? tl.Tafel.TafelNummer ?? "" : "")
                    .ToList()))
            .ForMember(d => d.Datum,
                opt => opt.MapFrom(s => s.Datum));


            CreateMap<Tafel, TafelTileViewModel>()
                .ForMember(dest => dest.DisplayOrder, opt => opt.Ignore())
                .ForMember(d => d.DisplayLabel,
                    opt => opt.MapFrom(src => src.TafelNummer))
                .ForMember(d => d.SizeCssClass,
                    opt => opt.MapFrom(src =>
                        src.AantalPersonen <= 2 ? "size-s" :
                        src.AantalPersonen <= 4 ? "size-m" :
                        src.AantalPersonen <= 6 ? "size-l" :
                                                  "size-xl"
                    ));

       
            CreateMap<Reservatie, EnqueteBeheerViewModel>()
                 .ForMember(dest => dest.KlantNaam,
               opt => opt.MapFrom(src => src.CustomUser != null
                                          ? $"{src.CustomUser.Voornaam} {src.CustomUser.Achternaam}"
                                          : "Onbekende klant"));

            CreateMap<EnqueteBeheerViewModel, Reservatie>();
            CreateMap<Categorie, CategorieViewModel>();
            CreateMap<CategorieCreateViewModel, Categorie>();
            CreateMap<CategorieEditViewModel, Categorie>();


            ///test
            ///
            CreateMap<Bestelling, BestellingItemViewModel>()
                .ForMember(dest => dest.BestellingId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductNaam, opt => opt.MapFrom(src => src.Product.Naam))
                .ForMember(dest => dest.ProductActief, opt => opt.MapFrom(src => src.Product.Actief))
                .ForMember(dest => dest.StatusNaam, opt => opt.MapFrom(src => src.Status.Naam))
                .ForMember(dest => dest.LokaleStatus, opt => opt.MapFrom(src => src.Status.Naam))
                .ForMember(dest => dest.CategorieType, opt => opt.MapFrom(src => src.Product.Categorie.Type.Naam))
                .ForMember(dest => dest.AllergenenInfo, opt => opt.MapFrom(src => src.Product.AllergenenInfo));


            //ienumerable nodig om te groeperen op reservatieId in de controller
            CreateMap<IEnumerable<Bestelling>, BestellingOverzichtViewModel>()
                    .ForMember(dest => dest.ReservatieId, opt => opt.MapFrom(src => src.First().ReservatieId))
                    .ForMember(dest => dest.TafelNummers, opt => opt.MapFrom(src => src.First().Reservatie.Tafellijsten
                        .Select(t => t.Tafel.TafelNummer).ToList()))
                    .ForMember(dest => dest.Gerechten, opt => opt.MapFrom(src =>
                        src.Where(b => b.Product.Categorie.Type.Naam != "Dranken").OrderBy(b => b.TijdstipBestelling)))
                    .ForMember(dest => dest.Dranken, opt => opt.MapFrom(src =>
                        src.Where(b => b.Product.Categorie.Type.Naam == "Dranken").OrderBy(b => b.TijdstipBestelling)))
                    .ForMember(dest => dest.GerechtenAllemaalKlaar, opt => opt.Ignore())
                    .ForMember(dest => dest.DrankenAllemaalGeleverd, opt => opt.Ignore());



            //So old files work aka redunant once others are fixed
            CreateMap<Tafel, TafelCreateViewModel>().ReverseMap();
            CreateMap<Tafel, TafelEditViewModel>().ReverseMap();

            //
            // ► Mapping voor TafelToewijzenReservatieViewModel
            //
            CreateMap<Reservatie, OldTafelToewijzenReservatieViewModel>()
                .ForMember(d => d.ReservatieId,
                    opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.KlantNaam,
                    opt => opt.MapFrom(s =>
                        s.CustomUser != null
                            ? $"{s.CustomUser.Voornaam} {s.CustomUser.Achternaam}".Trim()
                            : "Onbekende klant"))
                .ForMember(d => d.KlantEmail,
                    opt => opt.MapFrom(s => s.CustomUser != null ? s.CustomUser.Email : string.Empty))
                .ForMember(d => d.Datum,
                    opt => opt.MapFrom(s => s.Datum))
                .ForMember(d => d.TijdslotNaam,
                    opt => opt.MapFrom(s => s.Tijdslot != null ? s.Tijdslot.Naam : string.Empty))
                .ForMember(d => d.AantalPersonen,
                    opt => opt.MapFrom(s => s.AantalPersonen))
                .ForMember(d => d.Opmerking,
                    opt => opt.MapFrom(s => s.Opmerking))
                 .ForMember(d => d.TafelNummers,
        opt => opt.MapFrom(s =>
            s.Tafellijsten != null
                ? s.Tafellijsten
                    .Select(tl =>
                        tl.Tafel != null && !string.IsNullOrWhiteSpace(tl.Tafel.TafelNummer)
                            ? tl.Tafel.TafelNummer!
                            : $"T{tl.TafelId}")        // fallback als Tafel niet is ingeladen
                    .ToList()
                : new System.Collections.Generic.List<string>()))
                .ForMember(d => d.IsAanwezig,
                    opt => opt.MapFrom(s => s.IsAanwezig))
                .ForMember(d => d.BeschikbareTafels,
                    opt => opt.Ignore())
                .ForMember(d => d.GeselecteerdeTafelIds,
                    opt => opt.Ignore())
                .ForMember(d => d.WachttijdMelding,
                    opt => opt.Ignore())
                .ForMember(d => d.IsActief,
                    opt => opt.Ignore());


            ///test
            ///
            CreateMap<Bestelling, BestellingItemViewModel>()
                .ForMember(dest => dest.BestellingId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductNaam, opt => opt.MapFrom(src => src.Product.Naam))
                .ForMember(dest => dest.ProductActief, opt => opt.MapFrom(src => src.Product.Actief))
                .ForMember(dest => dest.StatusNaam, opt => opt.MapFrom(src => src.Status.Naam))
                .ForMember(dest => dest.LokaleStatus, opt => opt.MapFrom(src => src.Status.Naam))
                .ForMember(dest => dest.CategorieType, opt => opt.MapFrom(src => src.Product.Categorie.Type.Naam))
                .ForMember(dest => dest.AllergenenInfo, opt => opt.MapFrom(src => src.Product.AllergenenInfo));


            //ienumerable nodig om te groeperen op reservatieId in de controller
            CreateMap<IEnumerable<Bestelling>, BestellingOverzichtViewModel>()
                    .ForMember(dest => dest.ReservatieId, opt => opt.MapFrom(src => src.First().ReservatieId))
                    .ForMember(dest => dest.TafelNummers, opt => opt.MapFrom(src => src.First().Reservatie.Tafellijsten
                        .Select(t => t.Tafel.TafelNummer).ToList()))
                    .ForMember(dest => dest.Gerechten, opt => opt.MapFrom(src =>
                        src.Where(b => b.Product.Categorie.Type.Naam != "Dranken").OrderBy(b => b.TijdstipBestelling)))
                    .ForMember(dest => dest.Dranken, opt => opt.MapFrom(src =>
                        src.Where(b => b.Product.Categorie.Type.Naam == "Dranken").OrderBy(b => b.TijdstipBestelling)))
                    .ForMember(dest => dest.GerechtenAllemaalKlaar, opt => opt.Ignore())
                    .ForMember(dest => dest.DrankenAllemaalGeleverd, opt => opt.Ignore());


            //mapper voor reservatieItemViewModel om afrekenen bij OldTafelController te opimalizeren
            CreateMap<Reservatie, ReservatieItemViewModel>()
                .ForMember(dest => dest.Voornaam,
                    opt => opt.MapFrom(src => src.CustomUser != null ? src.CustomUser.Voornaam : ""))
                .ForMember(dest => dest.Naam,
                    opt => opt.MapFrom(src => src.CustomUser != null ? src.CustomUser.Achternaam : ""))
                .ForMember(dest => dest.IsBetaald,
                    opt => opt.MapFrom(src => src.Betaald))
                .ForMember(dest => dest.Totaal,
                     opt => opt.MapFrom(src =>
                            src.Bestellingen
                            .Where(b => b.StatusId ==3)
                            .Sum(b => (b.Product != null &&
                                    b.Product.PrijsProducten.Any())
                                    ? b.Product.PrijsProducten
                                    .OrderByDescending(p => p.DatumVanaf)
                                    .First().Prijs * b.Aantal
                                    : 0)
                      ));



        }





    }
}

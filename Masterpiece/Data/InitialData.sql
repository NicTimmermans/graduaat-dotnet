
-- Restaurant Database Seed Data Script
-- !!Dit bestand voegt geen records toe aan de tabellen AspNetUsers, reservaties en bestellingen

-- AspNetRoles
INSERT INTO AspNetRoles (Id, Name) VALUES 
(1, 'Eigenaar'),
(2, 'Zaalverantwoordelijke'),
(3, 'Ober'),
(4, 'Kok'),
(5, 'Klant');

SET IDENTITY_INSERT [dbo].[Land] ON
-- Land
INSERT INTO Land (Id, Naam, Actief) VALUES 
(1, 'België', 1),
(2, 'Nederland', 1),
(3, 'Frankrijk', 1),
(4, 'Duitsland', 1),
(5, 'Verenigd Koninkrijk', 0);
SET IDENTITY_INSERT [dbo].[Land] OFF

-- Status
SET IDENTITY_INSERT [dbo].[Status] ON
INSERT INTO Status (Id, Naam, Actief) VALUES 
(1, 'In Behandeling', 1),
(2, 'Klaar', 1),
(3, 'Geserveerd', 1),
(4, 'Geannuleerd', 1);
SET IDENTITY_INSERT [dbo].[Status] OFF

-- TijdSlot
SET IDENTITY_INSERT [dbo].[TijdSlot] ON
INSERT INTO TijdSlot (Id, Naam, Actief) VALUES 
(1, 'lunch van 11u30 tot 12u30', 1),
(2, 'lunch van 12u30 tot 13u30', 1),
(3, 'diner van 17u tot 19u', 1),
(4, 'diner van 19u tot 21u', 1);
SET IDENTITY_INSERT [dbo].[TijdSlot] OFF

-- Tafel
SET IDENTITY_INSERT [dbo].[Tafel] ON
INSERT INTO Tafel (Id, TafelNummer, AantalPersonen, MinAantalPersonen, Actief, QrBarcode) VALUES 
(1, 'T01', 2, 1, 1, 'QR001'),
(2, 'T02', 2, 1, 1, 'QR002'),
(3, 'T03', 4, 2, 1, 'QR003'),
(4, 'T04', 4, 2, 1, 'QR004'),
(5, 'T05', 6, 4, 1, 'QR005'),
(6, 'T06', 6, 4, 1, 'QR006'),
(7, 'T07', 8, 6, 1, 'QR007'),
(8, 'T08', 2, 1, 0, 'QR008'); -- Niet actief
SET IDENTITY_INSERT [dbo].[Tafel] OFF

-- Type
SET IDENTITY_INSERT [dbo].[CategorieType] ON
INSERT INTO CategorieType (Id, Naam, Actief) VALUES 
(1, 'Dranken', 1),
(2, 'Eten', 1),
(3, 'Desserts', 1),
(4, 'Specials', 1);
SET IDENTITY_INSERT [dbo].[CategorieType] OFF

-- Categorie
SET IDENTITY_INSERT [dbo].[Categorie] ON
INSERT INTO Categorie (Id, Naam, Actief, TypeId) VALUES 
-- Dranken
(1, 'Warme Dranken', 1, 1),
(2, 'Koude Dranken', 1, 1),
(3, 'Alcoholische Dranken', 1, 1),
(4, 'Wijnen', 1, 1),
-- Eten
(5, 'Voorgerechten', 1, 2),
(6, 'Hoofdgerechten', 1, 2),
(7, 'Salades', 1, 2),
(8, 'Pizza''s', 1, 2),
-- Desserts
(9, 'Warme Desserts', 1, 3),
(10, 'Koude Desserts', 1, 3),
-- Specials
(11, 'Chef''s Special', 1, 4),
(12, 'Seizoensgerechten', 1, 4);
SET IDENTITY_INSERT [dbo].[Categorie] OFF

-- Product
SET IDENTITY_INSERT [dbo].[Product] ON
INSERT INTO Product (Id, Naam, Beschrijving, AllergenenInfo, CategorieId, Actief, IsSuggestie) VALUES 
-- Warme Dranken
(1, 'Espresso', 'Sterke Italiaanse koffie', 'Cafeïne', 1, 1, 0),
(2, 'Cappuccino', 'Espresso met melkschuim', 'Cafeïne, Lactose', 1, 1, 0),
(3, 'Thee Earl Grey', 'Klassieke Engelse thee', 'Cafeïne', 1, 1, 0),
-- Koude Dranken
(4, 'Cola', 'Frisdrank', 'Cafeïne', 2, 1, 0),
(5, 'Spa Rood', 'Bruisend water', 'Geen', 2, 1, 0),
(6, 'Verse Jus d''Orange', 'Vers geperst sinaasappelsap', 'Geen', 2, 1, 1),
-- Alcoholische Dranken
(7, 'Jupiler', 'Belgisch bier 25cl', 'Gluten, Alcohol', 3, 1, 0),
(8, 'Gin Tonic', 'Premium gin met tonic', 'Alcohol', 3, 1, 0),
-- Wijnen
(9, 'Chardonnay', 'Witte wijn glas', 'Alcohol, Sulfiten', 4, 1, 1),
(10, 'Merlot', 'Rode wijn glas', 'Alcohol, Sulfiten', 4, 1, 0),
-- Voorgerechten
(11, 'Carpaccio', 'Runderfilet met rucola en parmezaan', 'Lactose', 5, 1, 1),
(12, 'Garnaalkroketjes', '6 stuks met cocktailsaus', 'Gluten, Schaaldieren, Eieren', 5, 1, 0),
(13, 'Soep van de dag', 'Dagverse soep', 'Variabel', 5, 1, 0),
-- Hoofdgerechten
(14, 'Steak met frietjes', 'Ribeye 250g met huisgemaakte friet', 'Geen', 6, 1, 1),
(15, 'Zalmfilet', 'Gegrilde zalm met groenten', 'Vis', 6, 1, 0),
(16, 'Vegetarische Lasagne', 'Met seizoensgroenten', 'Gluten, Lactose, Eieren', 6, 1, 0),
-- Salades
(17, 'Caesar Salade', 'Romeinse sla, croutons, parmezaan', 'Gluten, Lactose, Eieren, Ansjovis', 7, 1, 0),
(18, 'Geitenkaas Salade', 'Gemengde sla met warme geitenkaas', 'Lactose, Noten', 7, 1, 1),
-- Pizza''s
(19, 'Margherita', 'Tomatensaus, mozzarella, basilicum', 'Gluten, Lactose', 8, 1, 0),
(20, 'Quattro Stagioni', 'Ham, champignons, artisjok, olijven', 'Gluten, Lactose', 8, 1, 0),
-- Desserts
(21, 'Tiramisu', 'Klassiek Italiaans dessert', 'Gluten, Lactose, Eieren, Alcohol', 10, 1, 1),
(22, 'Chocolademousse', 'Huisgemaakte mousse', 'Lactose, Eieren', 10, 1, 0),
(23, 'Dame Blanche', 'Vanille-ijs met warme chocoladesaus', 'Lactose', 10, 1, 0),
-- Chef''s Special
(24, 'Lobster Thermidor', 'Gegrilde kreeft met champagnesaus', 'Schaaldieren, Lactose, Alcohol', 11, 1, 1),
(25, 'Wagyu Beef', 'Premium rund 200g', 'Geen', 11, 1, 1);
SET IDENTITY_INSERT [dbo].[Product] OFF

-- PrijsProduct
SET IDENTITY_INSERT [dbo].[PrijsProduct] ON
INSERT INTO PrijsProduct (Id, DatumVanaf, Prijs, ProductId) VALUES 
-- Warme Dranken
(1, '2024-01-01', 2.50, 1),
(2, '2024-01-01', 3.20, 2),
(3, '2024-01-01', 2.80, 3),
-- Koude Dranken
(4, '2024-01-01', 2.90, 4),
(5, '2024-01-01', 2.40, 5),
(6, '2024-01-01', 4.20, 6),
-- Alcoholische Dranken
(7, '2024-01-01', 3.80, 7),
(8, '2024-01-01', 8.50, 8),
-- Wijnen
(9, '2024-01-01', 6.20, 9),
(10, '2024-01-01', 5.80, 10),
-- Voorgerechten
(11, '2024-01-01', 14.50, 11),
(12, '2024-01-01', 12.80, 12),
(13, '2024-01-01', 8.90, 13),
-- Hoofdgerechten
(14, '2024-01-01', 28.50, 14),
(15, '2024-01-01', 24.90, 15),
(16, '2024-01-01', 18.50, 16),
-- Salades
(17, '2024-01-01', 15.20, 17),
(18, '2024-01-01', 16.80, 18),
-- Pizza''s
(19, '2024-01-01', 14.20, 19),
(20, '2024-01-01', 17.50, 20),
-- Desserts
(21, '2024-01-01', 7.80, 21),
(22, '2024-01-01', 6.50, 22),
(23, '2024-01-01', 5.90, 23),
-- Chef''s Special
(24, '2024-01-01', 42.00, 24),
(25, '2024-01-01', 65.00, 25);
SET IDENTITY_INSERT [dbo].[PrijsProduct] OFF

-- Sluitingsdag
SET IDENTITY_INSERT [dbo].[Sluitingsdag] ON
INSERT INTO Sluitingsdag (Id, Datum, Naam) VALUES 
(1, '2025-12-25', 'Kerstmis'), 
(2, '2026-01-01', 'Nieuwjaar'), 
(3, '2026-12-25', 'Kerstmis'), 
(4, '2026-07-21', 'Nationale feestdag België'), 
(5, '2026-05-01', 'Dag van de arbeid'),
(6, '2026-08-15', 'O.L.V. Hemelvaart');
SET IDENTITY_INSERT [dbo].[Sluitingsdag] OFF

-- Mail templates
SET IDENTITY_INSERT [dbo].[Mail] ON
INSERT INTO Mail (Id, Naam, Onderwerp, Body) VALUES 
(1, 'WelkomstMail', 'Welkom bij Restaurant [NAAM]', 'Beste [VOORNAAM] [ACHTERNAAM],\n\nHartelijk dank voor uw reservatie op [DATUM] om [TIJD] voor [AANTAL] personen.\n\nWij kijken ernaar uit u te mogen verwelkomen!\n\nMet vriendelijke groeten,\nHet team van Restaurant [NAAM]'),
(2, 'BevestigingMail', 'Bevestiging reservatie', 'Beste [VOORNAAM],\n\nUw reservatie is bevestigd:\nDatum: [DATUM]\nTijd: [TIJD]\nAantal personen: [AANTAL]\nTafel: [TAFEL]\n\nTot binnenkort!'),
(3, 'AnnulatieKlant', 'Annulatie reservatie', 'Beste [VOORNAAM],\n\nUw reservatie van [DATUM] om [TIJD] is geannuleerd zoals gevraagd.\n\nWe hopen u binnenkort opnieuw te mogen verwelkomen.\n\nMet vriendelijke groeten'),
(4, 'EvaluatieMail', 'Hoe was uw bezoek?', 'Beste [VOORNAAM],\n\nWe hopen dat u heeft genoten van uw bezoek op [DATUM].\n\nUw feedback is zeer waardevol voor ons. Zou u enkele minuten kunnen nemen om uw ervaring te evalueren?\n\nHartelijk dank!'),
(5, 'VerjaardagWens', 'Gelukkige verjaardag!', 'Beste [VOORNAAM],\n\nVan harte gefeliciteerd met uw verjaardag!\n\nWij willen u graag uitnodigen voor een speciale verjaardagsdiner. Neem contact met ons op voor een reservatie.\n\nMet feestelijke groeten');
SET IDENTITY_INSERT [dbo].[Mail] OFF

-- Parameter (configuratie instellingen)
SET IDENTITY_INSERT [dbo].[Parameter] ON
INSERT INTO Parameter (Id, Naam, Waarde) VALUES 
(1, 'RestaurantNaam', 'Chez Antoine'),
(2, 'MaxReservatiesDagelijks', '50'),
(3, 'OpeningstijdWeekdag', '17:00'),
(4, 'SluitingstijdWeekdag', '23:00'),
(5, 'OpeningstijdWeekend', '12:00'),
(6, 'SluitingstijdWeekend', '24:00'),
(7, 'MaxPersonenPerReservatie', '10'),
(8, 'MinPersonenPerReservatie', '1'),
(9, 'AnnulatieTermijn', '24'),
(10, 'EmailVerzendingActief', '1'),
(11, 'BTWPercentage', '21'),
(12, 'ServiceKostPercentage', '0'),
(13, 'MaxDagenVooruitReserveren', '90'),
(14, 'WelkomstmailVerzendDagen', '1'),
(15, 'EvaluatiemailVerzendDagen', '3'),
(16, 'RestaurantTelefoon', '+32 9 123 45 67'),
(17, 'RestaurantEmail', 'info@chezantoine.be'),
(18, 'RestaurantAdres', 'Korenmarkt 15, 9000 Gent'),
(19, 'KeukenSluitingstijd', '22:00'),
(20, 'ReservatieBevestigingVereist', '0');
SET IDENTITY_INSERT [dbo].[Parameter] OFF
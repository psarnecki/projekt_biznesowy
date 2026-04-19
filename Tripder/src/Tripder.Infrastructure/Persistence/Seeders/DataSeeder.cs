using Microsoft.EntityFrameworkCore;
using Tripder.Domain.AttractionDefinition.Entities;
using Tripder.Domain.AttractionDefinition.Enums;
using Tripder.Domain.AttractionDefinition.ValueObjects;

namespace Tripder.Infrastructure.Persistence.Seeders;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.ExecuteSqlRawAsync(@"
            DO $$ DECLARE
                r RECORD;
            BEGIN
                FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = current_schema() AND tablename != '__EFMigrationsHistory') LOOP
                    EXECUTE 'TRUNCATE TABLE ' || quote_ident(r.tablename) || ' RESTART IDENTITY CASCADE;';
                END LOOP;
            END $$;
        ");
        
        if (await db.Attractions.AnyAsync())
            return;
        
        var catMuseum    = new Category(Guid.Parse("10000000-0000-0000-0000-000000000001"), "Muzeum");
        var catNature    = new Category(Guid.Parse("10000000-0000-0000-0000-000000000002"), "Przyroda");
        var catHistory   = new Category(Guid.Parse("10000000-0000-0000-0000-000000000003"), "Historia");
        var catArt       = new Category(Guid.Parse("10000000-0000-0000-0000-000000000004"), "Sztuka");
        var catLeisure   = new Category(Guid.Parse("10000000-0000-0000-0000-000000000005"), "Rozrywka");
        var catReligion  = new Category(Guid.Parse("10000000-0000-0000-0000-000000000006"), "Sakralne");
        var catSport     = new Category(Guid.Parse("10000000-0000-0000-0000-000000000007"), "Sport");

        db.Categories.AddRange(catMuseum, catNature, catHistory, catArt, catLeisure, catReligion, catSport);
        
        var tagFamily     = new Tag(Guid.Parse("20000000-0000-0000-0000-000000000001"), "rodzinny");
        var tagOutdoor    = new Tag(Guid.Parse("20000000-0000-0000-0000-000000000002"), "na świeżym powietrzu");
        var tagIndoor     = new Tag(Guid.Parse("20000000-0000-0000-0000-000000000003"), "wnętrza");
        var tagFree       = new Tag(Guid.Parse("20000000-0000-0000-0000-000000000004"), "bezpłatny");
        var tagGuided     = new Tag(Guid.Parse("20000000-0000-0000-0000-000000000005"), "z przewodnikiem");
        var tagKids       = new Tag(Guid.Parse("20000000-0000-0000-0000-000000000006"), "dla dzieci");
        var tagWwII       = new Tag(Guid.Parse("20000000-0000-0000-0000-000000000007"), "II Wojna Światowa");
        var tagMedieval   = new Tag(Guid.Parse("20000000-0000-0000-0000-000000000008"), "średniowiecze");
        var tagViewpoint  = new Tag(Guid.Parse("20000000-0000-0000-0000-000000000009"), "punkt widokowy");
        var tagUnderground = new Tag(Guid.Parse("20000000-0000-0000-0000-000000000010"), "podziemia");
        var tagNature     = new Tag(Guid.Parse("20000000-0000-0000-0000-000000000011"), "przyroda");
        var tagHistory    = new Tag(Guid.Parse("20000000-0000-0000-0000-000000000012"), "historia");
        var tagReligion   = new Tag(Guid.Parse("20000000-0000-0000-0000-000000000013"), "sakralne");
        var tagArt        = new Tag(Guid.Parse("20000000-0000-0000-0000-000000000014"), "sztuka");
        var tagSport = new Tag(Guid.Parse("20000000-0000-0000-0000-000000000015"),"sport");

        db.Tags.AddRange(tagFamily, tagOutdoor, tagIndoor, tagFree, tagGuided,
                         tagKids, tagWwII, tagMedieval, tagViewpoint, tagUnderground,
                         tagNature, tagHistory, tagReligion, tagArt, tagSport);
        
        var ruleWeekdaysOnly = new RuleDefinition(
            Guid.Parse("30000000-0000-0000-0000-000000000001"),
            RuleType.Weekly, RuleEffect.Allow, 10,
            new TimeOnly(9, 0), new TimeOnly(17, 0));
        ruleWeekdaysOnly.AddDay(new DayOfWeekEntry(Guid.NewGuid(), "Monday"));
        ruleWeekdaysOnly.AddDay(new DayOfWeekEntry(Guid.NewGuid(), "Tuesday"));
        ruleWeekdaysOnly.AddDay(new DayOfWeekEntry(Guid.NewGuid(), "Wednesday"));
        ruleWeekdaysOnly.AddDay(new DayOfWeekEntry(Guid.NewGuid(), "Thursday"));
        ruleWeekdaysOnly.AddDay(new DayOfWeekEntry(Guid.NewGuid(), "Friday"));

        var ruleWeekend = new RuleDefinition(
            Guid.Parse("30000000-0000-0000-0000-000000000002"),
            RuleType.Weekly, RuleEffect.Allow, 10,
            new TimeOnly(10, 0), new TimeOnly(18, 0));
        ruleWeekend.AddDay(new DayOfWeekEntry(Guid.NewGuid(), "Saturday"));
        ruleWeekend.AddDay(new DayOfWeekEntry(Guid.NewGuid(), "Sunday"));

        var ruleSummer = new RuleDefinition(
            Guid.Parse("30000000-0000-0000-0000-000000000003"),
            RuleType.Seasonal, RuleEffect.Allow, 5,
            null, null,
            new DateOnly(2025, 6, 1), new DateOnly(2025, 8, 31));

        db.RuleDefinitions.AddRange(ruleWeekdaysOnly, ruleWeekend, ruleSummer);
        
        static Attraction MakeAttraction(string name, Guid categoryId, double lat, double lon,
            string locationName, int? capacity = null) =>
            new(Guid.NewGuid(), name, categoryId,
                new Location(lat, lon, locationName), capacity);

        static Scenario MakeScenario(Guid attractionId, string name, string description, int minutes) =>
            new(Guid.NewGuid(), attractionId, name, description, minutes);

        var attractions = new List<Attraction>();

        // 1
        var wawel = MakeAttraction("Zamek Królewski na Wawelu", catHistory.Id, 50.0542, 19.9354, "Wawel, Kraków", 500);
        wawel.AddTag(tagMedieval); wawel.AddTag(tagGuided); wawel.AddTag(tagIndoor);
        var wawelS1 = MakeScenario(wawel.Id, "Komnaty Królewskie", "Zwiedzanie reprezentacyjnych komnat królewskich z kolekcją arrasów.", 90);
        wawelS1.Publish(); wawel.Publish(); attractions.Add(wawel);

        // 2
        var sukiennice = MakeAttraction("Sukiennice", catHistory.Id, 50.0617, 19.9373, "Rynek Główny, Kraków", 300);
        sukiennice.AddTag(tagMedieval); sukiennice.AddTag(tagIndoor);
        var sukS1 = MakeScenario(sukiennice.Id, "Galeria Sukiennic", "Malarstwo polskie XIX wieku w oddziale Muzeum Narodowego.", 60);
        sukS1.Publish(); sukiennice.Publish(); attractions.Add(sukiennice);

        // 3
        var collegium = MakeAttraction("Collegium Maius", catHistory.Id, 50.0621, 19.9318, "ul. Jagiellońska 15, Kraków", 50);
        collegium.AddTag(tagGuided); collegium.AddTag(tagIndoor); collegium.AddTag(tagMedieval);
        var collS1 = MakeScenario(collegium.Id, "Muzeum UJ", "Najstarsza część Uniwersytetu Jagiellońskiego — aula, skarbiec, dziedziniec.", 75);
        collS1.Publish(); collegium.Publish(); attractions.Add(collegium);

        // 4
        var schindler = MakeAttraction("Fabryka Schindlera", catMuseum.Id, 50.0484, 19.9668, "ul. Lipowa 4, Kraków", 200);
        schindler.AddTag(tagWwII); schindler.AddTag(tagIndoor); schindler.AddTag(tagGuided);
        var schS1 = MakeScenario(schindler.Id, "Kraków — czas okupacji", "Multimedialna ekspozycja o Krakowie podczas II Wojny Światowej.", 120);
        schS1.Publish(); schindler.Publish(); attractions.Add(schindler);

        // 5
        var wieliczka = MakeAttraction("Kopalnia Soli Wieliczka", catHistory.Id, 49.9841, 20.0550, "ul. Daniłowicza 10, Wieliczka", 150);
        wieliczka.AddTag(tagUnderground); wieliczka.AddTag(tagGuided); wieliczka.AddTag(tagFamily);
        var wielS1 = MakeScenario(wieliczka.Id, "Trasa Turystyczna", "2 km tras podziemnych, kaplica św. Kingi, jezioro solne.", 150);
        wielS1.Publish(); wieliczka.Publish(); attractions.Add(wieliczka);

        // 6
        var auschwitz = MakeAttraction("Muzeum Auschwitz-Birkenau", catMuseum.Id, 50.0274, 19.2044, "ul. Więźniów Oświęcimia 20, Oświęcim", 400);
        auschwitz.AddTag(tagWwII); auschwitz.AddTag(tagGuided);
        var ausS1 = MakeScenario(auschwitz.Id, "Zwiedzanie z przewodnikiem", "Trasa przez bloki wystawowe Auschwitz I i teren Birkenau.", 210);
        ausS1.Publish(); auschwitz.Publish(); attractions.Add(auschwitz);

        // 7
        var florian = MakeAttraction("Brama Floriańska i Barbakan", catHistory.Id, 50.0647, 19.9390, "ul. Floriańska, Kraków");
        florian.AddTag(tagMedieval); florian.AddTag(tagOutdoor); florian.AddTag(tagViewpoint);
        var florS1 = MakeScenario(florian.Id, "Spacer po murach", "Gotyckie mury obronne, baszty i Barbakan — perła gotyku.", 45);
        florS1.Publish(); florian.Publish(); attractions.Add(florian);

        // 8
        var nowaHuta = MakeAttraction("Nowa Huta — Szlak Socrealizmu", catHistory.Id, 50.0693, 20.0370, "Plac Centralny, Nowa Huta, Kraków");
        nowaHuta.AddTag(tagGuided); nowaHuta.AddTag(tagOutdoor);
        var nHS1 = MakeScenario(nowaHuta.Id, "Spacer z przewodnikiem", "Historia budowy socjalistycznego miasta, architektura i codzienność PRL.", 120);
        nHS1.Publish(); nowaHuta.Publish(); attractions.Add(nowaHuta);

        // 9
        var kazimierz = MakeAttraction("Dzielnica Kazimierz", catHistory.Id, 50.0510, 19.9454, "Kazimierz, Kraków");
        kazimierz.AddTag(tagOutdoor); kazimierz.AddTag(tagFamily);
        var kazS1 = MakeScenario(kazimierz.Id, "Szlak Żydowski", "Synagogi, cmentarz Remuh, klimatyczne podwórka i galerie.", 90);
        kazS1.Publish(); kazimierz.Publish(); attractions.Add(kazimierz);

        // 10
        var smoczyJama = MakeAttraction("Smocza Jama", catLeisure.Id, 50.0537, 19.9343, "Wzgórze Wawelskie, Kraków", 30);
        smoczyJama.AddTag(tagFamily); smoczyJama.AddTag(tagUnderground); smoczyJama.AddTag(tagKids);
        var smoS1 = MakeScenario(smoczyJama.Id, "Jaskinia Smoka Wawelskiego", "Legendarny legowisko smoka — 270 m podziemnych korytarzy.", 20);
        smoS1.Publish(); smoczyJama.Publish(); attractions.Add(smoczyJama);

        // 11
        var mnk = MakeAttraction("Muzeum Narodowe w Krakowie — Gmach Główny", catMuseum.Id, 50.0598, 19.9189, "al. 3 Maja 1, Kraków", 250);
        mnk.AddTag(tagIndoor); mnk.AddTag(tagArt);
        var mnkS1 = MakeScenario(mnk.Id, "Sztuka polska XX w.", "Galeria stała — malarstwo, rzeźba i rzemiosło artystyczne.", 90);
        mnkS1.Publish(); mnk.Publish(); attractions.Add(mnk);

        // 12
        var czartoryskich = MakeAttraction("Muzeum Czartoryskich", catMuseum.Id, 50.0632, 19.9352, "ul. św. Jana 19, Kraków", 120);
        czartoryskich.AddTag(tagIndoor); czartoryskich.AddTag(tagGuided); czartoryskich.AddTag(tagMedieval);
        var czartS1 = MakeScenario(czartoryskich.Id, "Dama z gronostajem", "Arcydzieło Leonarda da Vinci i zbiory europejskie Czartoryskich.", 75);
        czartS1.Publish(); czartoryskich.Publish(); attractions.Add(czartoryskich);

        // 13
        var kosciolMariacki = MakeAttraction("Kościół Mariacki", catReligion.Id, 50.0617, 19.9393, "Plac Mariacki 5, Kraków", 200);
        kosciolMariacki.AddTag(tagMedieval); kosciolMariacki.AddTag(tagIndoor); kosciolMariacki.AddTag(tagGuided);
        var marS1 = MakeScenario(kosciolMariacki.Id, "Ołtarz Wita Stwosza", "Największy gotycki ołtarz w Polsce — hejnał z wieży co godzinę.", 40);
        marS1.Publish(); kosciolMariacki.Publish(); attractions.Add(kosciolMariacki);

        // 14
        var lagiewniki = MakeAttraction("Sanktuarium Bożego Miłosierdzia", catReligion.Id, 50.0225, 19.9456, "ul. Siostry Faustyny 3, Kraków-Łagiewniki");
        lagiewniki.AddTag(tagReligion); lagiewniki.AddTag(tagIndoor);
        var lagS1 = MakeScenario(lagiewniki.Id, "Kaplica i Bazylika", "Miejsce kultu, kaplica z obrazem Jezusa Miłosiernego i nowa bazylika.", 60);
        lagS1.Publish(); lagiewniki.Publish(); attractions.Add(lagiewniki);

        // 15
        var kopiec = MakeAttraction("Kopiec Kościuszki", catHistory.Id, 50.0547, 19.8958, "al. Waszyngtona, Kraków");
        kopiec.AddTag(tagOutdoor); kopiec.AddTag(tagViewpoint);
        var kopS1 = MakeScenario(kopiec.Id, "Wejście na Kopiec", "Panorama Krakowa z wysokości 326 m n.p.m., fort z XIX w.", 45);
        kopS1.Publish(); kopiec.Publish(); attractions.Add(kopiec);

        // 16
        var lotnicze = MakeAttraction("Muzeum Lotnictwa Polskiego", catMuseum.Id, 50.0796, 19.9762, "al. Jana Pawła II 39, Kraków", 300);
        lotnicze.AddTag(tagFamily); lotnicze.AddTag(tagKids); lotnicze.AddTag(tagOutdoor);
        var lotS1 = MakeScenario(lotnicze.Id, "Ekspozycja plenerowa i hangary", "Ponad 200 historycznych samolotów i śmigłowców z całego świata.", 120);
        lotS1.Publish(); lotnicze.Publish(); attractions.Add(lotnicze);

        // 17
        var zodiak = MakeAttraction("Ogród Zoologiczny w Krakowie", catLeisure.Id, 50.0630, 19.8880, "ul. Kasy Oszczędności Miasta Krakowa 14", 1000);
        zodiak.AddTag(tagFamily); zodiak.AddTag(tagKids); zodiak.AddTag(tagOutdoor);
        var zooS1 = MakeScenario(zodiak.Id, "Zwiedzanie Zoo", "Ponad 1400 zwierząt z 280 gatunków na 16 ha zieleni.", 180);
        zooS1.Publish(); zodiak.Publish(); attractions.Add(zodiak);

        // 18
        var planty = MakeAttraction("Planty Krakowskie", catNature.Id, 50.0620, 19.9350, "Planty, Kraków");
        planty.AddTag(tagOutdoor); planty.AddTag(tagFree); planty.AddTag(tagFamily);
        var planS1 = MakeScenario(planty.Id, "Spacer dookoła Starego Miasta", "4 km parku miejskiego wokół historycznego centrum.", 60);
        planS1.Publish(); planty.Publish(); attractions.Add(planty);

        // 19
        var blonia = MakeAttraction("Błonia Krakowskie", catNature.Id, 50.0594, 19.9143, "Błonia, Kraków");
        blonia.AddTag(tagOutdoor); blonia.AddTag(tagFree); blonia.AddTag(tagSport);
        var bloS1 = MakeScenario(blonia.Id, "Rekreacja na Błoniach", "Największy zielony teren rekreacyjny w centrum miasta — 48 ha.", 60);
        bloS1.Publish(); blonia.Publish(); attractions.Add(blonia);

        // 20
        var lasFolkowy = MakeAttraction("Las Wolski", catNature.Id, 50.0638, 19.8742, "Las Wolski, Kraków");
        lasFolkowy.AddTag(tagOutdoor); lasFolkowy.AddTag(tagFamily); lasFolkowy.AddTag(tagFree);
        var lasS1 = MakeScenario(lasFolkowy.Id, "Szlaki piesze", "20 km oznakowanych tras w najpiękniejszym lesie Krakowa.", 120);
        lasS1.Publish(); lasFolkowy.Publish(); attractions.Add(lasFolkowy);

        // 21
        var tyniec = MakeAttraction("Opactwo Benedyktynów w Tyńcu", catReligion.Id, 50.0149, 19.8344, "ul. Benedyktyńska 37, Tyniec");
        tyniec.AddTag(tagMedieval); tyniec.AddTag(tagOutdoor); tyniec.AddTag(tagGuided);
        var tynS1 = MakeScenario(tyniec.Id, "Zwiedzanie opactwa", "Romańsko-gotyckie opactwo na skale nad Wisłą — najstarsze w Polsce.", 60);
        tynS1.Publish(); tyniec.Publish(); attractions.Add(tyniec);

        // 22
        var ojcow = MakeAttraction("Ojcowski Park Narodowy", catNature.Id, 50.2037, 19.8301, "Ojców");
        ojcow.AddTag(tagOutdoor); ojcow.AddTag(tagFamily); ojcow.AddTag(tagNature);
        var ojcS1 = MakeScenario(ojcow.Id, "Dolina Prądnika", "Skały, jaskinie, zamek Ojców i Pieskowa Skała w jednym szlaku.", 180);
        ojcS1.Publish(); ojcow.Publish(); attractions.Add(ojcow);

        // 23
        var pieskowa = MakeAttraction("Zamek Pieskowa Skała", catHistory.Id, 50.2204, 19.8127, "Pieskowa Skała 1, Sułoszowa", 100);
        pieskowa.AddTag(tagMedieval); pieskowa.AddTag(tagGuided); pieskowa.AddTag(tagFamily);
        var pieS1 = MakeScenario(pieskowa.Id, "Muzeum zamkowe", "Renesansowy zamek z dziedzińcem arkadowym i zbiorem europejskiej sztuki.", 90);
        pieS1.Publish(); pieskowa.Publish(); attractions.Add(pieskowa);

        // 24
        var jaskiniaCiemna = MakeAttraction("Jaskinia Ciemna w Ojcowie", catNature.Id, 50.2051, 19.8262, "Ojców");
        jaskiniaCiemna.AddTag(tagUnderground); jaskiniaCiemna.AddTag(tagFamily); jaskiniaCiemna.AddTag(tagGuided);
        var jcS1 = MakeScenario(jaskiniaCiemna.Id, "Trasa przez jaskinię", "230 m trasy — stalaktyty, ślady neandertalczyka i zimowisko nietoperzy.", 45);
        jcS1.Publish(); jaskiniaCiemna.Publish(); attractions.Add(jaskiniaCiemna);

        // 25
        var kalwaria = MakeAttraction("Kalwaria Zebrzydowska", catReligion.Id, 49.8769, 19.6852, "ul. Bernardyńska 46, Kalwaria Zebrzydowska");
        kalwaria.AddTag(tagReligion); kalwaria.AddTag(tagOutdoor); kalwaria.AddTag(tagGuided);
        var kalS1 = MakeScenario(kalwaria.Id, "Droga Krzyżowa", "UNESCO — 42 kaplice i kościoły w malowniczym krajobrazie Beskidów.", 120);
        kalS1.Publish(); kalwaria.Publish(); attractions.Add(kalwaria);

        // 26
        var wadowice = MakeAttraction("Dom Rodzinny Jana Pawła II — Wadowice", catMuseum.Id, 49.8835, 19.4966, "ul. Kościelna 7, Wadowice", 80);
        wadowice.AddTag(tagIndoor); wadowice.AddTag(tagGuided); wadowice.AddTag(tagFamily);
        var wadS1 = MakeScenario(wadowice.Id, "Muzeum Domu Rodzinnego", "Miejsce urodzenia Karola Wojtyły — ekspozycja multimedialna na 3 piętrach.", 75);
        wadS1.Publish(); wadowice.Publish(); attractions.Add(wadowice);

        // 27
        var zamekLipiowiec = MakeAttraction("Zamek Lipowiec", catHistory.Id, 50.0630, 19.5261, "Babice");
        zamekLipiowiec.AddTag(tagMedieval); zamekLipiowiec.AddTag(tagOutdoor); zamekLipiowiec.AddTag(tagViewpoint);
        var lipS1 = MakeScenario(zamekLipiowiec.Id, "Zwiedzanie ruin", "Gotycki zamek biskupów krakowskich z panoramą Jury Krakowsko-Częstochowskiej.", 60);
        lipS1.Publish(); zamekLipiowiec.Publish(); attractions.Add(zamekLipiowiec);

        // 28
        var muzHutaLenina = MakeAttraction("Centrum Zwiedzania ArcelorMittal Nowa Huta", catHistory.Id, 50.0747, 20.0447, "al. Solidarności 33, Kraków", 40);
        muzHutaLenina.AddTag(tagGuided); muzHutaLenina.AddTag(tagIndoor); muzHutaLenina.AddTag(tagWwII);
        var hutS1 = MakeScenario(muzHutaLenina.Id, "Zwiedzanie huty", "Jedyna w Polsce okazja do zobaczenia działającej wielkiej huty od środka.", 120);
        hutS1.Publish(); muzHutaLenina.Publish(); attractions.Add(muzHutaLenina);

        // 29
        var szlakOrlich = MakeAttraction("Szlak Orlich Gniazd — odcinek Krakowski", catNature.Id, 50.1957, 19.8186, "Dolina Prądnika");
        szlakOrlich.AddTag(tagOutdoor); szlakOrlich.AddTag(tagSport); szlakOrlich.AddTag(tagViewpoint);
        var szlS1 = MakeScenario(szlakOrlich.Id, "Trekking Dolina Prądnika–Olsztyn", "Odcinek 25 km przez ruiny zamków, skałki i jaskinie.", 360);
        szlS1.Publish(); szlakOrlich.Publish(); attractions.Add(szlakOrlich);

        // 30
        var fortKosciuszko = MakeAttraction("Fort Kościuszko (Fort 2)", catHistory.Id, 50.0554, 19.8965, "al. Waszyngtona, Kraków");
        fortKosciuszko.AddTag(tagHistory); fortKosciuszko.AddTag(tagOutdoor); fortKosciuszko.AddTag(tagGuided);
        var fortS1 = MakeScenario(fortKosciuszko.Id, "Twierdza Kraków — Fort 2", "XIX-wieczna fortyfikacja pierścienia wewnętrznego z ekspozycją militarną.", 50);
        fortS1.Publish(); fortKosciuszko.Publish(); attractions.Add(fortKosciuszko);

        // 31
        var rynekGlowny = MakeAttraction("Rynek Główny w Krakowie", catHistory.Id, 50.0617, 19.9373, "Rynek Główny, Kraków");
        rynekGlowny.AddTag(tagOutdoor); rynekGlowny.AddTag(tagFree); rynekGlowny.AddTag(tagMedieval);
        var rynS1 = MakeScenario(rynekGlowny.Id, "Spacer po Rynku", "Największy średniowieczny rynek w Europie — 4 ha historycznego serca Krakowa.", 45);
        rynS1.Publish(); rynekGlowny.Publish(); attractions.Add(rynekGlowny);

        // 32
        var krakowianka = MakeAttraction("Krakowskie Podziemia Rynku", catMuseum.Id, 50.0616, 19.9366, "Rynek Główny 1, Kraków", 80);
        krakowianka.AddTag(tagUnderground); krakowianka.AddTag(tagIndoor); krakowianka.AddTag(tagGuided);
        var podS1 = MakeScenario(krakowianka.Id, "Podziemna trasa multimedialna", "Wykopane przez archeologów kupieckie uliczki sprzed 800 lat.", 60);
        podS1.Publish(); krakowianka.Publish(); attractions.Add(krakowianka);

        // 33
        var wisla = MakeAttraction("Spływ Wisłą — Kraków", catLeisure.Id, 50.0471, 19.9475, "Bulwar Czerwieński, Kraków");
        wisla.AddTag(tagOutdoor); wisla.AddTag(tagFamily); wisla.AddTag(tagSport);
        var wisS1 = MakeScenario(wisla.Id, "Kajak lub ponton Wisłą", "Panorama Wawelu z poziomu wody — trasa od Tyniec do Bielan.", 150);
        wisS1.Publish(); wisla.Publish(); attractions.Add(wisla);

        // 34
        var estrada = MakeAttraction("Teatr Słowackiego", catArt.Id, 50.0646, 19.9422, "pl. Świętego Ducha 1, Kraków", 600);
        estrada.AddTag(tagIndoor); estrada.AddTag(tagArt);
        var teatS1 = MakeScenario(estrada.Id, "Zwiedzanie gmachu", "Neo-barokowy gmach z 1893 r. — kulisy, widownia i historia teatru.", 60);
        teatS1.Publish(); estrada.Publish(); attractions.Add(estrada);

        // 35
        var mocak = MakeAttraction("MOCAK — Muzeum Sztuki Współczesnej", catMuseum.Id, 50.0493, 19.9697, "ul. Lipowa 4, Kraków", 180);
        mocak.AddTag(tagIndoor); mocak.AddTag(tagArt);
        var mocakS1 = MakeScenario(mocak.Id, "Kolekcja stała", "Polska i europejska sztuka po 1989 r. — malarstwo, instalacje, wideo.", 90);
        mocakS1.Publish(); mocak.Publish(); attractions.Add(mocak);

        // 36
        var kopiecWandyDot = MakeAttraction("Kopiec Wandy", catHistory.Id, 50.0806, 20.0554, "Nowa Huta, Kraków");
        kopiecWandyDot.AddTag(tagOutdoor); kopiecWandyDot.AddTag(tagFree); kopiecWandyDot.AddTag(tagViewpoint);
        var kwS1 = MakeScenario(kopiecWandyDot.Id, "Wejście na Kopiec Wandy", "Legendarny grobowiec księżniczki — panorama Nowej Huty i Tatr.", 30);
        kwS1.Publish(); kopiecWandyDot.Publish(); attractions.Add(kopiecWandyDot);

        // 37
        var bochnia = MakeAttraction("Kopalnia Soli Bochnia", catHistory.Id, 49.9694, 20.4302, "ul. Campi 15, Bochnia", 120);
        bochnia.AddTag(tagUnderground); bochnia.AddTag(tagGuided); bochnia.AddTag(tagFamily);
        var bochS1 = MakeScenario(bochnia.Id, "Trasa Górnicza", "Najstarsza kopalnia soli w Polsce (1248 r.) — kaplice, jezioro, zjazd windą.", 130);
        bochS1.Publish(); bochnia.Publish(); attractions.Add(bochnia);

        // 38
        var zamoekKrzyzanowice = MakeAttraction("Zamek w Korzkwi", catHistory.Id, 50.1736, 19.8793, "Korzkiew");
        zamoekKrzyzanowice.AddTag(tagMedieval); zamoekKrzyzanowice.AddTag(tagOutdoor);
        var korzS1 = MakeScenario(zamoekKrzyzanowice.Id, "Zwiedzanie zamku", "Gotycki zamek rycerski odrestaurowany jako hotel — dostępny do zwiedzania.", 45);
        korzS1.Publish(); zamoekKrzyzanowice.Publish(); attractions.Add(zamoekKrzyzanowice);

        // 39
        var wielkaRawka = MakeAttraction("Sanktuarium w Mogile — Klasztor Cystersów", catReligion.Id, 50.0762, 20.0486, "ul. Klasztorna 11, Kraków-Mogiła");
        wielkaRawka.AddTag(tagMedieval); wielkaRawka.AddTag(tagReligion); wielkaRawka.AddTag(tagGuided);
        var mogS1 = MakeScenario(wielkaRawka.Id, "Zwiedzanie klasztoru", "Romańsko-gotycki kościół z 1225 r. i krużganki pełne fresków.", 50);
        mogS1.Publish(); wielkaRawka.Publish(); attractions.Add(wielkaRawka);

        // 40
        var rezerwatPazurkow = MakeAttraction("Rezerwat Skały Przegorzalskie", catNature.Id, 50.0811, 19.8882, "Przegorzały, Kraków");
        rezerwatPazurkow.AddTag(tagOutdoor); rezerwatPazurkow.AddTag(tagFree); rezerwatPazurkow.AddTag(tagViewpoint);
        var rezS1 = MakeScenario(rezerwatPazurkow.Id, "Spacer wśród skałek", "Jurajskie ostańce nad Wisłą — punkt widokowy na Las Wolski i Wawel.", 40);
        rezS1.Publish(); rezerwatPazurkow.Publish(); attractions.Add(rezerwatPazurkow);

        // 41
        var wierzynekMuz = MakeAttraction("Muzeum Inżynierii Miejskiej", catMuseum.Id, 50.0526, 19.9471, "ul. Wawrzyńca 15, Kraków", 150);
        wierzynekMuz.AddTag(tagFamily); wierzynekMuz.AddTag(tagKids); wierzynekMuz.AddTag(tagIndoor);
        var mizS1 = MakeScenario(wierzynekMuz.Id, "Tramwaje, maszyny i technika", "Interaktywna ekspozycja historii transportu i techniki miejskiej Krakowa.", 90);
        mizS1.Publish(); wierzynekMuz.Publish(); attractions.Add(wierzynekMuz);

        // 42
        var etnoMuz = MakeAttraction("Muzeum Etnograficzne w Krakowie", catMuseum.Id, 50.0518, 19.9427, "pl. Wolnica 1, Kraków", 100);
        etnoMuz.AddTag(tagIndoor); etnoMuz.AddTag(tagFamily);
        var etnoS1 = MakeScenario(etnoMuz.Id, "Kultura ludowa Małopolski", "Stroje, obrzędy i rzemiosło regionu — od szopek krakowskich po wycinanki.", 75);
        etnoS1.Publish(); etnoMuz.Publish(); attractions.Add(etnoMuz);

        // 43
        var bulwaryWislane = MakeAttraction("Bulwary Wiślane", catLeisure.Id, 50.0510, 19.9430, "Bulwar Czerwieński, Kraków");
        bulwaryWislane.AddTag(tagOutdoor); bulwaryWislane.AddTag(tagFree); bulwaryWislane.AddTag(tagFamily);
        var bulS1 = MakeScenario(bulwaryWislane.Id, "Spacer nadwiślański", "5 km promenady z widokiem na Wawel — rowery, rolki, gastronomia.", 60);
        bulS1.Publish(); bulwaryWislane.Publish(); attractions.Add(bulwaryWislane);

        // 44
        var collegiumNovum = MakeAttraction("Wieża Ratuszowa", catHistory.Id, 50.0614, 19.9366, "Rynek Główny 1, Kraków", 30);
        collegiumNovum.AddTag(tagViewpoint); collegiumNovum.AddTag(tagMedieval);
        var ratS1 = MakeScenario(collegiumNovum.Id, "Wejście na wieżę", "Gotycka wieża z XIV w. — panorama Rynku i Krakowa z 70 m.", 30);
        ratS1.Publish(); collegiumNovum.Publish(); attractions.Add(collegiumNovum);

        // 45
        var cystersiOjcow = MakeAttraction("Zamek Ojców — ruiny", catHistory.Id, 50.2032, 19.8311, "Ojców");
        cystersiOjcow.AddTag(tagMedieval); cystersiOjcow.AddTag(tagOutdoor); cystersiOjcow.AddTag(tagFree);
        var ojcZS1 = MakeScenario(cystersiOjcow.Id, "Spacer po ruinach", "XIV-wieczny zamek Kazimierza Wielkiego nad Prądnikiem.", 30);
        ojcZS1.Publish(); cystersiOjcow.Publish(); attractions.Add(cystersiOjcow);

        // 46
        var lazienki = MakeAttraction("Park Decjusza — Villa Decius", catHistory.Id, 50.0706, 19.8798, "ul. 28 Lipca 1943 nr 17a, Kraków", 60);
        lazienki.AddTag(tagOutdoor); lazienki.AddTag(tagHistory); lazienki.AddTag(tagArt);
        var decS1 = MakeScenario(lazienki.Id, "Villa Decius i park", "Renesansowa rezydencja z XVI w. — wystawy, ogród i festiwale.", 60);
        decS1.Publish(); lazienki.Publish(); attractions.Add(lazienki);

        // 47
        var kosciolPaulini = MakeAttraction("Kościół na Skałce — Paulini", catReligion.Id, 50.0503, 19.9416, "ul. Skałeczna 15, Kraków");
        kosciolPaulini.AddTag(tagMedieval); kosciolPaulini.AddTag(tagReligion); kosciolPaulini.AddTag(tagIndoor);
        var paulS1 = MakeScenario(kosciolPaulini.Id, "Krypta Zasłużonych", "Kościół św. Michała i Stanisława — krypta z sarkofagami wybitnych Polaków.", 40);
        paulS1.Publish(); kosciolPaulini.Publish(); attractions.Add(kosciolPaulini);

        // 48
        var ogrodBotaniczny = MakeAttraction("Ogród Botaniczny UJ", catNature.Id, 50.0639, 19.9492, "ul. Kopernika 27, Kraków");
        ogrodBotaniczny.AddTag(tagOutdoor); ogrodBotaniczny.AddTag(tagFamily); ogrodBotaniczny.AddTag(tagKids);
        var botS1 = MakeScenario(ogrodBotaniczny.Id, "Spacer po ogrodzie", "Najstarszy ogród botaniczny w Polsce (1783) — szklarnie, zielnik, arboretum.", 75);
        botS1.Publish(); ogrodBotaniczny.Publish(); attractions.Add(ogrodBotaniczny);

        // 49
        var ikea = MakeAttraction("Centrum Wspinaczkowe Krakow Climbing", catSport.Id, 50.0741, 19.9212, "ul. Półkole 1, Kraków", 80);
        ikea.AddTag(tagSport); ikea.AddTag(tagFamily); ikea.AddTag(tagIndoor);
        var wspS1 = MakeScenario(ikea.Id, "Wspinaczka — trasy dla początkujących", "Ścianki boulderingowe i linowe, wynajem sprzętu, instruktor.", 120);
        wspS1.Publish(); ikea.Publish(); attractions.Add(ikea);

        // 50
        var cmentarzRakowicki = MakeAttraction("Cmentarz Rakowicki", catHistory.Id, 50.0720, 19.9530, "ul. Rakowicka 26, Kraków");
        cmentarzRakowicki.AddTag(tagOutdoor); cmentarzRakowicki.AddTag(tagFree); cmentarzRakowicki.AddTag(tagHistory);
        var cemS1 = MakeScenario(cmentarzRakowicki.Id, "Spacer historyczny", "Najstarszy cmentarz Krakowa (1803) — groby Wyspiańskiego, Fałata, Malczewskiego.", 60);
        cemS1.Publish(); cmentarzRakowicki.Publish(); attractions.Add(cmentarzRakowicki);
        
        var allScenarios = new[]
        {
            wawelS1, sukS1, collS1, schS1, wielS1, ausS1, florS1, nHS1, kazS1, smoS1,
            mnkS1, czartS1, marS1, lagS1, kopS1, lotS1, zooS1, planS1, bloS1, lasS1,
            tynS1, ojcS1, pieS1, jcS1, kalS1, wadS1, lipS1, hutS1, szlS1, fortS1,
            rynS1, podS1, wisS1, teatS1, mocakS1, kwS1, bochS1, korzS1, mogS1, rezS1,
            mizS1, etnoS1, bulS1, ratS1, ojcZS1, decS1, paulS1, botS1, wspS1, cemS1
        };

        db.Attractions.AddRange(attractions);
        db.Scenarios.AddRange(allScenarios);

        await db.SaveChangesAsync();
    }
}
using Microsoft.EntityFrameworkCore;
using Tripder.Domain.AttractionDefinition.Entities;
using Tripder.Domain.AttractionDefinition.ValueObjects;

namespace Tripder.Infrastructure.Persistence.Seeders;

public static class OjcowskiParkSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Attractions.AnyAsync(a => a.Name == "Jaskinia Łokietka"))
            return;

        var catNature   = await db.Categories.FirstAsync(c => c.Name == "Przyroda");
        var catHistory  = await db.Categories.FirstAsync(c => c.Name == "Historia");
        var catReligion = await db.Categories.FirstAsync(c => c.Name == "Sakralne");

        var tagFamily      = await db.Tags.FirstAsync(t => t.Name == "rodzinny");
        var tagOutdoor     = await db.Tags.FirstAsync(t => t.Name == "na świeżym powietrzu");
        var tagIndoor      = await db.Tags.FirstAsync(t => t.Name == "wnętrza");
        var tagFree        = await db.Tags.FirstAsync(t => t.Name == "bezpłatny");
        var tagGuided      = await db.Tags.FirstAsync(t => t.Name == "z przewodnikiem");
        var tagMedieval    = await db.Tags.FirstAsync(t => t.Name == "średniowiecze");
        var tagViewpoint   = await db.Tags.FirstAsync(t => t.Name == "punkt widokowy");
        var tagUnderground = await db.Tags.FirstAsync(t => t.Name == "podziemia");
        var tagNature      = await db.Tags.FirstAsync(t => t.Name == "przyroda");
        var tagHistory     = await db.Tags.FirstAsync(t => t.Name == "historia");
        var tagReligion    = await db.Tags.FirstAsync(t => t.Name == "sakralne");
        var tagSport       = await db.Tags.FirstAsync(t => t.Name == "sport");

        static Attraction MakeAttraction(string name, Guid categoryId, double lat, double lon,
            string locationName, int? capacity = null) =>
            new(Guid.NewGuid(), name, categoryId,
                new Location(lat, lon, locationName), capacity);

        static Scenario MakeScenario(Guid attractionId, string name, string description, int minutes) =>
            new(Guid.NewGuid(), attractionId, name, description, minutes);

        var attractions = new List<Attraction>();

        // 1
        var zamekOjcow = MakeAttraction("Zamek Kazimierzowski w Ojcowie", catHistory.Id, 50.2037, 19.8280, "Ojców 1, Ojcowski Park Narodowy", 80);
        zamekOjcow.AddTag(tagMedieval); zamekOjcow.AddTag(tagOutdoor); zamekOjcow.AddTag(tagViewpoint); zamekOjcow.AddTag(tagHistory);
        var zamekS1 = MakeScenario(zamekOjcow.Id, "Ruiny zamku i taras widokowy", "XIV-wieczna warownia Kazimierza Wielkiego — neogotycka wieża bramna, ekspozycja i panorama Doliny Prądnika. Bilet 22 zł (gotówka).", 40);
        zamekS1.Publish(); zamekOjcow.Publish(); attractions.Add(zamekOjcow);

        // 2
        var jaskiniaLokietka = MakeAttraction("Jaskinia Łokietka", catNature.Id, 50.2082, 19.8195, "Ojcowski Park Narodowy", 25);
        jaskiniaLokietka.AddTag(tagUnderground); jaskiniaLokietka.AddTag(tagGuided); jaskiniaLokietka.AddTag(tagHistory); jaskiniaLokietka.AddTag(tagNature);
        var lokS1 = MakeScenario(jaskiniaLokietka.Id, "Trasa z przewodnikiem", "Legenda o królu Łokietku, krata w kształcie pajęczyny i stalaktyty. Temperatura 7-8°C — kurtka obowiązkowa nawet latem. Parking w Czajowicach (300 m).", 50);
        lokS1.Publish(); jaskiniaLokietka.Publish(); attractions.Add(jaskiniaLokietka);

        // 3
        var bramaKrakowska = MakeAttraction("Brama Krakowska i Jonaszówka", catNature.Id, 50.2045, 19.8295, "Ojcowski Park Narodowy");
        bramaKrakowska.AddTag(tagOutdoor); bramaKrakowska.AddTag(tagViewpoint); bramaKrakowska.AddTag(tagFamily); bramaKrakowska.AddTag(tagFree);
        var bramaS1 = MakeScenario(bramaKrakowska.Id, "Brama Krakowska i Źródło Miłości", "15-metrowa skalna brama, wywierzysko krasowe w kształcie serca i punkt widokowy Jonaszówka. Teren płaski — przyjazny dla wózków. W pobliżu Pstrąg Ojcowski.", 35);
        bramaS1.Publish(); bramaKrakowska.Publish(); attractions.Add(bramaKrakowska);

        // 4
        var szlakCzerwony = MakeAttraction("Szlak Orlich Gniazd przez OPN (Czerwony)", catNature.Id, 50.2037, 19.8301, "Dolina Prądnika, OPN");
        szlakCzerwony.AddTag(tagOutdoor); szlakCzerwony.AddTag(tagFamily); szlakCzerwony.AddTag(tagNature);
        var szlakCS1 = MakeScenario(szlakCzerwony.Id, "Główna oś Parku", "13,6 km w większości asfaltem dnem doliny — jedyny szlak OPN polecany dla wózków inwalidzkich i dziecięcych (odcinek Zamek Ojców–Brama Krakowska).", 200);
        szlakCS1.Publish(); szlakCzerwony.Publish(); attractions.Add(szlakCzerwony);

        // 5
        var szlakNiebieski = MakeAttraction("Szlak Warowni Jurajskich — Wąwóz Ciasne Skałki (Niebieski)", catNature.Id, 50.2065, 19.8235, "Wąwóz Ciasne Skałki, OPN");
        szlakNiebieski.AddTag(tagOutdoor); szlakNiebieski.AddTag(tagSport); szlakNiebieski.AddTag(tagNature);
        var szlakNS1 = MakeScenario(szlakNiebieski.Id, "Wąwóz Ciasne Skałki", "Łączy Jaskinię Łokietka z Bramą Krakowską. Duże nachylenie — wymagane buty z profilowanym bieżnikiem.", 90);
        szlakNS1.Publish(); szlakNiebieski.Publish(); attractions.Add(szlakNiebieski);

        // 6
        var szlakZielony = MakeAttraction("Szlak Park Zamkowy – Jaskinia Ciemna (Zielony)", catNature.Id, 50.2055, 19.8265, "Góra Koronna, OPN");
        szlakZielony.AddTag(tagOutdoor); szlakZielony.AddTag(tagSport); szlakZielony.AddTag(tagViewpoint);
        var szlakZS1 = MakeScenario(szlakZielony.Id, "Trasa widokowa Góry Koronnej", "Szlak górski przez szczyty z ekspozycją. Odradzany osobom z lękiem wysokości i rodzinom z wózkami dziecięcymi.", 75);
        szlakZS1.Publish(); szlakZielony.Publish(); attractions.Add(szlakZielony);

        // 7
        var szlakZolty = MakeAttraction("Szlak Doliny Sąspowskiej (Żółty)", catNature.Id, 50.1985, 19.8405, "Dolina Sąspowska, OPN");
        szlakZolty.AddTag(tagOutdoor); szlakZolty.AddTag(tagNature); szlakZolty.AddTag(tagFree);
        var szlakZoS1 = MakeScenario(szlakZolty.Id, "Dolina Sąspowska", "Najspokojniejszy szlak OPN z dala od tłumów. Teren podmokły — wymagane wodoodporne buty.", 120);
        szlakZoS1.Publish(); szlakZolty.Publish(); attractions.Add(szlakZolty);

        // 8
        var szlakCzarny = MakeAttraction("Szlak Łącznik Widokowy — Skała Jonaszówka (Czarny)", catNature.Id, 50.2044, 19.8292, "Skała Jonaszówka, OPN");
        szlakCzarny.AddTag(tagOutdoor); szlakCzarny.AddTag(tagViewpoint); szlakCzarny.AddTag(tagNature);
        var szlakCzS1 = MakeScenario(szlakCzarny.Id, "Punkt widokowy Skała Jonaszówka", "Szlak łącznikowy prowadzący na najlepszy punkt fotograficzny w Ojcowie z panoramą całej doliny.", 45);
        szlakCzS1.Publish(); szlakCzarny.Publish(); attractions.Add(szlakCzarny);

        // 9
        var kaplicaNaWodzie = MakeAttraction("Kaplica 'Na Wodzie'", catReligion.Id, 50.2033, 19.8308, "Ojcowski Park Narodowy");
        kaplicaNaWodzie.AddTag(tagReligion); kaplicaNaWodzie.AddTag(tagOutdoor); kaplicaNaWodzie.AddTag(tagFree);
        var kaplicaS1 = MakeScenario(kaplicaNaWodzie.Id, "Unikalna kaplica na palach", "Drewniana kaplica pw. Wniebowzięcia NMP stojąca na palach nad Prądnikiem. Widoczna bezpośrednio ze szlaku czerwonego.", 15);
        kaplicaS1.Publish(); kaplicaNaWodzie.Publish(); attractions.Add(kaplicaNaWodzie);

        // 10
        var grodzisko = MakeAttraction("Grodzisko OPN — Kompleks Salomei", catHistory.Id, 50.2010, 19.8355, "Grodzisko, Ojcowski Park Narodowy");
        grodzisko.AddTag(tagOutdoor); grodzisko.AddTag(tagHistory); grodzisko.AddTag(tagFree);
        var grodzS1 = MakeScenario(grodzisko.Id, "Kompleks Salomei", "Dawny teren klasztorny z rzeźbą słonia i kaplicą bł. Salomei — idealne miejsce na wyciszenie z dala od głównego ruchu turystycznego.", 30);
        grodzS1.Publish(); grodzisko.Publish(); attractions.Add(grodzisko);

        // 11
        var mlynBoronia = MakeAttraction("Młyn Boronia", catHistory.Id, 50.2022, 19.8318, "Ojcowski Park Narodowy", 20);
        mlynBoronia.AddTag(tagIndoor); mlynBoronia.AddTag(tagGuided); mlynBoronia.AddTag(tagHistory);
        var mlynS1 = MakeScenario(mlynBoronia.Id, "Zabytkowy młyn wodny", "Najlepiej zachowany zabytek techniki przemysłowej w OPN. Wnętrza dostępne wyłącznie po wcześniejszym telefonicznym umówieniu.", 40);
        mlynS1.Publish(); mlynBoronia.Publish(); attractions.Add(mlynBoronia);

        var allScenarios = new[]
        {
            zamekS1, lokS1, bramaS1, szlakCS1, szlakNS1, szlakZS1, szlakZoS1, szlakCzS1,
            kaplicaS1, grodzS1, mlynS1
        };

        db.Attractions.AddRange(attractions);
        db.Scenarios.AddRange(allScenarios);

        await db.SaveChangesAsync();
    }
}

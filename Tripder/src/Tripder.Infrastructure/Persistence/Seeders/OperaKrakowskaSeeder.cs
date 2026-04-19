using Microsoft.EntityFrameworkCore;
using Tripder.Domain.AttractionDefinition.Entities;
using Tripder.Domain.AttractionDefinition.Enums;
using Tripder.Domain.AttractionDefinition.ValueObjects;

namespace Tripder.Infrastructure.Persistence.Seeders;

public static class OperaKrakowskaSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        var catArt = await db.Categories.FindAsync(Guid.Parse("10000000-0000-0000-0000-000000000004"));
        var catLeisure = await db.Categories.FindAsync(Guid.Parse("10000000-0000-0000-0000-000000000005"));

        var tagIndoor = await db.Tags.FindAsync(Guid.Parse("20000000-0000-0000-0000-000000000003"));
        var tagGuided = await db.Tags.FindAsync(Guid.Parse("20000000-0000-0000-0000-000000000005"));
        var tagFamily = await db.Tags.FindAsync(Guid.Parse("20000000-0000-0000-0000-000000000001"));
        var tagArt = await db.Tags.FindAsync(Guid.Parse("20000000-0000-0000-0000-000000000014"));
        var tagKids = await db.Tags.FindAsync(Guid.Parse("20000000-0000-0000-0000-000000000006"));

        var ruleEveningWeekdays = new RuleDefinition(
            Guid.Parse("30000000-0000-0000-0000-000000000010"),
            RuleType.Weekly, RuleEffect.Allow, 8,
            new TimeOnly(17, 30), new TimeOnly(23, 0));
        ruleEveningWeekdays.AddDay(new DayOfWeekEntry(Guid.NewGuid(), "Tuesday"));
        ruleEveningWeekdays.AddDay(new DayOfWeekEntry(Guid.NewGuid(), "Wednesday"));
        ruleEveningWeekdays.AddDay(new DayOfWeekEntry(Guid.NewGuid(), "Thursday"));
        ruleEveningWeekdays.AddDay(new DayOfWeekEntry(Guid.NewGuid(), "Friday"));

        var ruleEveningWeekend = new RuleDefinition(
            Guid.Parse("30000000-0000-0000-0000-000000000011"),
            RuleType.Weekly, RuleEffect.Allow, 8,
            new TimeOnly(15, 0), new TimeOnly(23, 0));
        ruleEveningWeekend.AddDay(new DayOfWeekEntry(Guid.NewGuid(), "Saturday"));
        ruleEveningWeekend.AddDay(new DayOfWeekEntry(Guid.NewGuid(), "Sunday"));

        var ruleDaytimeWeekdays = new RuleDefinition(
            Guid.Parse("30000000-0000-0000-0000-000000000012"),
            RuleType.Weekly, RuleEffect.Allow, 6,
            new TimeOnly(10, 0), new TimeOnly(14, 0));
        ruleDaytimeWeekdays.AddDay(new DayOfWeekEntry(Guid.NewGuid(), "Monday"));
        ruleDaytimeWeekdays.AddDay(new DayOfWeekEntry(Guid.NewGuid(), "Tuesday"));
        ruleDaytimeWeekdays.AddDay(new DayOfWeekEntry(Guid.NewGuid(), "Wednesday"));
        ruleDaytimeWeekdays.AddDay(new DayOfWeekEntry(Guid.NewGuid(), "Thursday"));
        ruleDaytimeWeekdays.AddDay(new DayOfWeekEntry(Guid.NewGuid(), "Friday"));

        var ruleSummerOutdoor = new RuleDefinition(
            Guid.Parse("30000000-0000-0000-0000-000000000013"),
            RuleType.Seasonal, RuleEffect.Allow, 5,
            null, null,
            new DateOnly(2025, 7, 1), new DateOnly(2025, 8, 31));

        db.RuleDefinitions.AddRange(ruleEveningWeekdays, ruleEveningWeekend, ruleDaytimeWeekdays, ruleSummerOutdoor);

        static Attraction MakeAttraction(string name, Guid categoryId, double lat, double lon,
            string locationName, int? capacity = null) =>
            new(Guid.NewGuid(), name, categoryId,
                new Location(lat, lon, locationName), capacity);

        static Scenario MakeScenario(Guid attractionId, string name, string description, int minutes) =>
            new(Guid.NewGuid(), attractionId, name, description, minutes);

        var attractions = new List<Attraction>();

        var spektaklOpera = MakeAttraction("Opera Krakowska — Spektakl Operowy", catArt!.Id, 50.0647, 19.9502, "ul. Lubicz 48, Kraków", 700);
        spektaklOpera.AddTag(tagIndoor!); spektaklOpera.AddTag(tagArt!);
        var operaS1 = MakeScenario(spektaklOpera.Id, "Wieczór Operowy — scena główna", "Wieczorny spektakl operowy na głównej scenie Opery Krakowskiej. Repertuar obejmuje klasykę włoską i polską — m.in. Traviatę, Toskę i Straszny Dwór.", 150);
        operaS1.Publish(); spektaklOpera.Publish(); attractions.Add(spektaklOpera);

        var spektaklBalet = MakeAttraction("Opera Krakowska — Spektakl Baletowy", catArt!.Id, 50.0647, 19.9502, "ul. Lubicz 48, Kraków", 700);
        spektaklBalet.AddTag(tagIndoor!); spektaklBalet.AddTag(tagArt!);
        var baletS1 = MakeScenario(spektaklBalet.Id, "Wieczór Baletowy", "Spektakl w wykonaniu Baletu Opery Krakowskiej. Program obejmuje klasykę i choreografię współczesną — Dziadek do Orzechów, Jezioro Łabędzie.", 135);
        baletS1.Publish(); spektaklBalet.Publish(); attractions.Add(spektaklBalet);

        var spektaklOperetka = MakeAttraction("Opera Krakowska — Operetka", catArt!.Id, 50.0647, 19.9502, "ul. Lubicz 48, Kraków", 700);
        spektaklOperetka.AddTag(tagIndoor!); spektaklOperetka.AddTag(tagArt!); spektaklOperetka.AddTag(tagFamily!); spektaklOperetka.AddTag(tagKids!);
        var operetkaS1 = MakeScenario(spektaklOperetka.Id, "Wieczór Operetkowy", "Lekki repertuar operetkowy idealny dla rodzin i pierwszych widzów. Wesołe piosenki, kostiumy i taniec — świetne wprowadzenie do świata opery.", 120);
        operetkaS1.Publish(); spektaklOperetka.Publish(); attractions.Add(spektaklOperetka);

        var backstour = MakeAttraction("Opera Krakowska — Zwiedzanie za Kulisami", catLeisure!.Id, 50.0647, 19.9502, "ul. Lubicz 48, Kraków", 15);
        backstour.AddTag(tagGuided!); backstour.AddTag(tagIndoor!); backstour.AddTag(tagFamily!);
        var backS1 = MakeScenario(backstour.Id, "Wycieczka za kulisy", "Około 90-minutowe zwiedzanie z przewodnikiem — pracownia kostiumów, maszyneria sceniczna, sala prób i główna scena. Grupy do 15 osób.", 90);
        backS1.Publish(); backstour.Publish(); attractions.Add(backstour);

        var warsztaty = MakeAttraction("Opera Krakowska — Warsztaty Wokalne", catLeisure!.Id, 50.0647, 19.9502, "ul. Lubicz 48, Kraków", 10);
        warsztaty.AddTag(tagGuided!); warsztaty.AddTag(tagIndoor!);
        var warsztatyS1 = MakeScenario(warsztaty.Id, "Warsztaty z solistą Opery", "Godzinne warsztaty prowadzone przez solistę Opery Krakowskiej. Otwarte dla dorosłych bez doświadczenia wokalnego. Maks. 10 uczestników.", 60);
        warsztatyS1.Publish(); warsztaty.Publish(); attractions.Add(warsztaty);

        var foyer = MakeAttraction("Opera Krakowska — Bar w Foyer", catLeisure!.Id, 50.0647, 19.9502, "ul. Lubicz 48, Kraków", 120);
        foyer.AddTag(tagIndoor!);
        var foyerS1 = MakeScenario(foyer.Id, "Przerwa z szampanem w foyer", "Elegancki bar w foyer Opery czynny podczas przerwy spektaklu. Szampan, wino, napoje i przekąski w otoczeniu zabytkowych wnętrz.", 30);
        foyerS1.Publish(); foyer.Publish(); attractions.Add(foyer);

        var kolacja = MakeAttraction("Opera Krakowska — Kolacja Przedspektaklowa", catLeisure!.Id, 50.0647, 19.9502, "ul. Lubicz 48, Kraków", 30);
        kolacja.AddTag(tagIndoor!);
        var kolacjaS1 = MakeScenario(kolacja.Id, "Kolacja przed spektaklem", "Trzydaniowa kolacja w restauracji partnerskiej Opery (17:00–19:00). Dostępna wyłącznie w dniach spektakli. Rezerwacja wymagana minimum 2 dni wcześniej.", 120);
        kolacjaS1.Publish(); kolacja.Publish(); attractions.Add(kolacja);

        var operaDziedziniec = MakeAttraction("Opera Krakowska — Opera na Dziedzińcu", catArt!.Id, 50.0647, 19.9502, "ul. Lubicz 48, Kraków (dziedziniec)", 400);
        operaDziedziniec.AddTag(tagArt!); operaDziedziniec.AddTag(tagFamily!);
        var dziedzS1 = MakeScenario(operaDziedziniec.Id, "Letni spektakl plenerowy", "Wyjątkowe przedstawienia operowe i baletowe na letniej scenie plenerowej w dziedzińcu Opery. Sezon lipiec–sierpień, bilety limitowane.", 150);
        dziedzS1.Publish(); operaDziedziniec.Publish(); attractions.Add(operaDziedziniec);

        var allScenarios = new[] { operaS1, baletS1, operetkaS1, backS1, warsztatyS1, foyerS1, kolacjaS1, dziedzS1 };

        db.Attractions.AddRange(attractions);
        db.Scenarios.AddRange(allScenarios);

        await db.SaveChangesAsync();
    }
}
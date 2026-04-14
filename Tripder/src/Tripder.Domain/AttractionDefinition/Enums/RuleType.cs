namespace Tripder.Domain.AttractionDefinition.Enums;

// Serce naszego systemu dostępności (data-driven). 
// Zamiast pisać hardkodowane if'y w kodzie, trzymamy typy reguł w bazie.
// Dzięki temu możemy łatwo rozbudowywać silnik o nowe przypadki w przyszłości.
public enum RuleType
{
    Weekly,         // Działa co tydzień (np. otwarte od pon do pt)
    Seasonal,       // Działa w konkretnym sezonie (np. tylko wakacje)
    DateException   // Konkretny wyjątek (np. zamknięte 1 maja)
}
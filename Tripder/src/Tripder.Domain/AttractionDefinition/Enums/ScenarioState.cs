namespace Tripder.Domain.AttractionDefinition.Enums;

// Scenariusze mają swój własny, niezależny cykl życia.
// Dlaczego? Bo Kopalnia Wieliczka (Attraction) może być w stanie Catalog, 
// ale jeden z jej wariantów np. "Trasa Świąteczna" (Scenario) może być już Archived. 
// Taki podział daje nam pełną elastyczność w zarządzaniu ofertą.
public enum ScenarioState
{
    Draft,
    Catalog,
    Internal,
    Archived
}
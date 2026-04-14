namespace Tripder.Domain.AttractionDefinition.Enums;

// Definiuje globalny cykl życia dla całej atrakcji. 
// Używamy silnie typowanego enuma zamiast luźnych stringów (np. "Draft", "Catalog"), 
// żeby uniknąć literówek w bazie i ułatwić sterowanie maszynami stanu (np. nie można opublikować czegoś, co jest Archived).
public enum AttractionState
{
    Draft,
    Catalog,
    Internal,
    Archived
}
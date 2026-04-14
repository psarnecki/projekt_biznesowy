namespace Tripder.Api.Models;

public sealed class TagNameBody
{
    // super prosty DTO na tag — jak ktoś nie wie po co: tylko żeby JSON miał { "name": "..." } i tyle
    public string Name { get; init; } = string.Empty;
}

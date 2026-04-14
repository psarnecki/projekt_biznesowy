namespace Tripder.Domain.AttractionDefinition.Enums;

// Określa rezultat działania reguły. 
// Prosty binarny podział sprawia, że łatwiej rozwiązywać konflikty na poziomie logiki domenowej
// (np. reguła Deny z wyższym priorytetem nadpisuje regułę Allow z niższym).
public enum RuleEffect
{
    Allow,
    Deny
}
using System.Text.RegularExpressions;
using StockAnalyzer.Domain.Exceptions;

namespace StockAnalyzer.Domain.ValueObjects;

public sealed partial record Ticker
{
    public string Valor { get; }

    public Ticker(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DomainException("Ticker não pode ser vazio.");

        var normalizado = valor.Trim().ToUpperInvariant();

        if (!FormatoB3().IsMatch(normalizado))
            throw new DomainException($"Ticker '{valor}' fora do formato da B3 (ex.: PETR4, TAEE11).");

        Valor = normalizado;
    }

    public override string ToString() => Valor;

    [GeneratedRegex(@"^[A-Z]{4}\d{1,2}$")]
    private static partial Regex FormatoB3();
}
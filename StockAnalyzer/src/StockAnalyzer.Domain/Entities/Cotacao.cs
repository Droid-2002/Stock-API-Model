using StockAnalyzer.Domain.Exceptions;

namespace StockAnalyzer.Domain.Entities;

public sealed class Cotacao
{
    public int Id { get; private set; }
    public DateOnly Data { get; private set; }
    public decimal Abertura { get; private set; }
    public decimal Maxima { get; private set; }
    public decimal Minima { get; private set; }
    public decimal Fechamento { get; private set; }
    public long Volume { get; private set; }

    private Cotacao() { } // EF Core

    internal Cotacao(DateOnly data, decimal abertura, decimal maxima,
                     decimal minima, decimal fechamento, long volume)
    {
        Validar(data, abertura, maxima, minima, fechamento, volume);

        Data = data;
        Abertura = abertura;
        Maxima = maxima;
        Minima = minima;
        Fechamento = fechamento;
        Volume = volume;
    }

    internal void Atualizar(decimal abertura, decimal maxima,
                            decimal minima, decimal fechamento, long volume)
    {
        Validar(Data, abertura, maxima, minima, fechamento, volume);

        Abertura = abertura;
        Maxima = maxima;
        Minima = minima;
        Fechamento = fechamento;
        Volume = volume;
    }

    private static void Validar(DateOnly data, decimal abertura, decimal maxima,
                                decimal minima, decimal fechamento, long volume)
    {
        if (data == default(DateOnly))
            throw new DomainException("A data da cotação é obrigatória.");
        if (maxima <= 0)
            throw new DomainException("O valor máximo deve ser maior que zero.");
        if (minima <= 0)
            throw new DomainException("O valor mínimo deve ser maior que zero.");
        if (minima > maxima)
            throw new DomainException("O valor mínimo não pode ser maior que o valor máximo.");
        if (abertura <= 0)
            throw new DomainException("O valor de abertura deve ser maior que zero.");
        if (fechamento <= 0)
            throw new DomainException("O valor de fechamento deve ser maior que zero.");
        if (abertura > maxima || abertura < minima)
            throw new DomainException("O valor de abertura não pode ser maior que o valor máximo, e nem menor que o valor mínimo.");
        if (fechamento > maxima || fechamento < minima)
            throw new DomainException("O valor de fechamento não pode ser maior que o valor máximo, e nem menor que o valor mínimo.");
        if (volume < 0)
            throw new DomainException("O volume não pode ser negativo.");
    }
}
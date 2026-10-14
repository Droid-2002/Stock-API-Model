
using StockAnalyzer.Domain.Entities;
using StockAnalyzer.Domain.Exceptions;
namespace StockAnalyzer.Tests.Domain;

public class CotacaoTests
{
    private static readonly DateOnly Data = new(2024, 6, 1);

    [Theory]
    [InlineData(0,   39, 37, 38)]   // abertura zero
    [InlineData(38, -39, 37, 38)]   // máxima negativa
    // mais casos: mínima e fechamento
    public void Construtor_ComPrecoZeroOuNegativo_LancaDomainException(
        decimal abertura, decimal maxima, decimal minima, decimal fechamento)
{
    Assert.Throws<DomainException>(() =>
        new Cotacao(Data, abertura, maxima, minima, fechamento, volume: 1000));
}
    [Fact]
    public void Construtor_ComCotacaoValida_CriaCotacao()
    {
        var cotacao = new Cotacao(Data, abertura: 38m, maxima: 39m,
                                  minima: 37m, fechamento: 38.5m, volume: 1000);

        Assert.Equal(Data, cotacao.Data);
        Assert.Equal(38m, cotacao.Abertura);
        Assert.Equal(39m, cotacao.Maxima);
        Assert.Equal(37m, cotacao.Minima);
        Assert.Equal(38.5m, cotacao.Fechamento);
        Assert.Equal(1000, cotacao.Volume);
    }
}
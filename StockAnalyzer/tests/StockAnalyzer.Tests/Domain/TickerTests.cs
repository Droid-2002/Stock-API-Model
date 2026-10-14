
using StockAnalyzer.Domain.Exceptions;
using StockAnalyzer.Domain.ValueObjects;

namespace StockAnalyzer.Tests.Domain;

public class TickerTests
{
    [Theory]
    [InlineData("petr4", "PETR4")]
    [InlineData("  vale3 ", "VALE3")]
    [InlineData("TAEE11", "TAEE11")]
    public void Construtor_ComTickerValido_NormalizaValor(string entrada, string esperado)
    {
        var ticker = new Ticker(entrada);

        Assert.Equal(esperado, ticker.Valor);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("PETR")]     // sem número
    [InlineData("PETR123")]  // número demais
    [InlineData("PE4")]      // letras de menos
    [InlineData("PETR-4")]   // caractere inválido
    public void Construtor_ComTickerInvalido_LancaDomainException(string entrada)
    {
        Assert.Throws<DomainException>(() => new Ticker(entrada));
    }

    [Fact]
    public void Tickers_ComMesmoValor_SaoIguais()
    {
        Assert.Equal(new Ticker("petr4"), new Ticker("PETR4"));
    }
}
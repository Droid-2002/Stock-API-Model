using StockAnalyzer.Domain.Entities;
using StockAnalyzer.Domain.Exceptions;

namespace StockAnalyzer.Tests.Domain;

public class CotacaoTests
{
    private static readonly DateOnly Data = new(2024, 6, 1);

    private static Cotacao CriarValida() =>
        new(Data, abertura: 38m, maxima: 39m, minima: 37m, fechamento: 38.5m, volume: 1000);

    // ---------- Caminho feliz ----------

    [Fact]
    public void Construtor_ComValoresValidos_AtribuiPropriedades()
    {
        var cotacao = CriarValida();

        Assert.Equal(Data, cotacao.Data);
        Assert.Equal(38m, cotacao.Abertura);
        Assert.Equal(39m, cotacao.Maxima);
        Assert.Equal(37m, cotacao.Minima);
        Assert.Equal(38.5m, cotacao.Fechamento);
        Assert.Equal(1000, cotacao.Volume);
    }

    [Fact]
    public void Construtor_ComTodosOsPrecosIguaisEVolumeZero_Aceita()
    {
        var cotacao = new Cotacao(Data, abertura: 38m, maxima: 38m, minima: 38m, fechamento: 38m, volume: 0);

        Assert.Equal(0, cotacao.Volume);
    }

    // ---------- Invariantes ----------

    [Fact]
    public void Construtor_ComDataDefault_LancaDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Cotacao(default, abertura: 38m, maxima: 39m, minima: 37m, fechamento: 38m, volume: 1000));
    }

    [Theory]
    [InlineData(0, 39, 37, 38)]    // abertura zero
    [InlineData(-1, 39, 37, 38)]   // abertura negativa
    [InlineData(38, 0, 37, 38)]    // máxima zero
    [InlineData(38, -39, 37, 38)]  // máxima negativa
    [InlineData(38, 39, 0, 38)]    // mínima zero
    [InlineData(38, 39, -37, 38)]  // mínima negativa
    [InlineData(38, 39, 37, 0)]    // fechamento zero
    [InlineData(38, 39, 37, -38)]  // fechamento negativo
    public void Construtor_ComPrecoZeroOuNegativo_LancaDomainException(
        decimal abertura, decimal maxima, decimal minima, decimal fechamento)
    {
        Assert.Throws<DomainException>(() =>
            new Cotacao(Data, abertura, maxima, minima, fechamento, volume: 1000));
    }

    [Fact]
    public void Construtor_ComMinimaMaiorQueMaxima_LancaDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Cotacao(Data, abertura: 39m, maxima: 38m, minima: 40m, fechamento: 39m, volume: 1000));
    }

    [Theory]
    [InlineData(41)]  // acima da máxima
    [InlineData(35)]  // abaixo da mínima
    public void Construtor_ComAberturaForaDoIntervalo_LancaDomainException(decimal abertura)
    {
        Assert.Throws<DomainException>(() =>
            new Cotacao(Data, abertura, maxima: 40m, minima: 36m, fechamento: 38m, volume: 1000));
    }

    [Theory]
    [InlineData(41)]  // acima da máxima
    [InlineData(35)]  // abaixo da mínima
    public void Construtor_ComFechamentoForaDoIntervalo_LancaDomainException(decimal fechamento)
    {
        Assert.Throws<DomainException>(() =>
            new Cotacao(Data, abertura: 38m, maxima: 40m, minima: 36m, fechamento, volume: 1000));
    }

    [Fact]
    public void Construtor_ComVolumeNegativo_LancaDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Cotacao(Data, abertura: 38m, maxima: 39m, minima: 37m, fechamento: 38m, volume: -1));
    }

    // ---------- Atualizar ----------

    [Fact]
    public void Atualizar_ComValoresValidos_AlteraPrecosMasMantemData()
    {
        var cotacao = CriarValida();

        cotacao.Atualizar(abertura: 40m, maxima: 42m, minima: 39m, fechamento: 41m, volume: 2000);

        Assert.Equal(Data, cotacao.Data);
        Assert.Equal(40m, cotacao.Abertura);
        Assert.Equal(42m, cotacao.Maxima);
        Assert.Equal(39m, cotacao.Minima);
        Assert.Equal(41m, cotacao.Fechamento);
        Assert.Equal(2000, cotacao.Volume);
    }

    [Fact]
    public void Atualizar_ComValoresInvalidos_NaoAlteraEstado()
    {
        var cotacao = CriarValida();

        // Abertura e máxima válidas, mínima inválida: se a atribuição fosse feita
        // campo a campo, abertura e máxima já teriam sido alteradas quando o erro surgisse.
        Assert.Throws<DomainException>(() =>
            cotacao.Atualizar(abertura: 50m, maxima: 55m, minima: 60m, fechamento: 52m, volume: 5000));

        Assert.Equal(38m, cotacao.Abertura);
        Assert.Equal(39m, cotacao.Maxima);
        Assert.Equal(37m, cotacao.Minima);
        Assert.Equal(38.5m, cotacao.Fechamento);
        Assert.Equal(1000, cotacao.Volume);
    }
}

public class AcaoTests
{

    private static readonly DateOnly Hoje = new DateOnly(2026, 10, 14);
    
    private static Acao CriarAcao() => new(new Ticker("PETR4"), "Petrobras");
    
    [Fact]
    public void RegistrarCotacao_DeveRegistrarNovaCotacao()
    {
        // Arrange
        var ticker = new Ticker("AAPL");
        var acao = new Acao(ticker, "Apple Inc.");
        var data = new DateOnly(2024, 6, 1);
        decimal abertura = 150.00m;
        decimal maxima = 155.00m;
        decimal minima = 149.00m;
        decimal fechamento = 154.00m;
        long volume = 1000000;

        // Act
        var cotacaoRegistrada = acao.RegistrarCotacao(data, abertura, maxima, minima, fechamento, volume);

        // Assert
        Assert.NotNull(cotacaoRegistrada);
        Assert.Equal(data, cotacaoRegistrada.Data);
        Assert.Equal(abertura, cotacaoRegistrada.Abertura);
        Assert.Equal(maxima, cotacaoRegistrada.Maxima);
        Assert.Equal(minima, cotacaoRegistrada.Minima);
        Assert.Equal(fechamento, cotacaoRegistrada.Fechamento);
        Assert.Equal(volume, cotacaoRegistrada.Volume);
    }
}
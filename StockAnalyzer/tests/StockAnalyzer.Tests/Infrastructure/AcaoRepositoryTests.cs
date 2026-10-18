using StockAnalyzer.Domain.Entities;
using StockAnalyzer.Domain.ValueObjects;
using StockAnalyzer.Infrastructure.Repositories;

namespace StockAnalyzer.Tests.Infrastructure;

public sealed class AcaoRepositoryTests : IDisposable
{
    private static readonly DateOnly Hoje = new(2026, 10, 14);
    private readonly SqliteEmMemoria _banco = new();

    public void Dispose() => _banco.Dispose();

    [Fact]
    public async Task ObterPorTicker_AposSalvar_RetornaAcaoComCotacoes()
    {
        // Arrange: contexto 1
        await using (var contexto = _banco.CriarContexto())
        {
            var repositorio = new AcaoRepository(contexto);
            var acao = new Acao(new Ticker("PETR4"), "Petrobras");
            acao.RegistrarCotacao(Hoje, abertura: 38m, maxima: 39m,
                                  minima: 37m, fechamento: 38.5m, volume: 1000);
            repositorio.Adicionar(acao);
            await repositorio.SalvarAsync();
        }

        // Act + Assert: contexto 2
        await using (var contexto = _banco.CriarContexto())
        {
            var repositorio = new AcaoRepository(contexto);

            var acao = await repositorio.ObterPorTickerAsync(new Ticker("petr4"));

            Assert.NotNull(acao);
            Assert.Equal("Petrobras", acao.Nome);
            var cotacao = Assert.Single(acao.Cotacoes);
            Assert.Equal(38.5m, cotacao.Fechamento);
        }
    }
}

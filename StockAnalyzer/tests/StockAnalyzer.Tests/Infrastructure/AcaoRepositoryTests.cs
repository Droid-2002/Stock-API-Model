using StockAnalyzer.Domain.Entities;
using StockAnalyzer.Domain.ValueObjects;
using StockAnalyzer.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

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
   
   
    [Fact]
    public async Task ObterPorTicker_Inexistente_RetornaNull()
        {
            // Arrange
            await using var contexto = _banco.CriarContexto();
            var repositorio = new AcaoRepository(contexto);

            // Act
            var acao = await repositorio.ObterPorTickerAsync(new Ticker("PETR4"));

            // Assert
            Assert.Null(acao);
        }
    [Fact]
    public async Task Salvar_ComTickerDuplicado_LancaDbUpdateException()
        {

            await using var contexto = _banco.CriarContexto();
            var repositorio = new AcaoRepository(contexto);

            // Arrange
            var acao1 = new Acao(new Ticker("PETR4"), "Petrobras");
            repositorio.Adicionar(acao1);
            await repositorio.SalvarAsync();

            var acao2 = new Acao(new Ticker("PETR4"), "Petrobras");
            repositorio.Adicionar(acao2);

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => repositorio.SalvarAsync());
        }

    [Fact]
    public async Task RegistrarCotacao_EmAcaoCarregada_NaMesmaData_AtualizaSemDuplicar()
    {
        // Contexto 1: primeira importação
        await using (var contexto = _banco.CriarContexto())
        {
            var repositorio = new AcaoRepository(contexto);
            var acao = new Acao(new Ticker("PETR4"), "Petrobras");
            acao.RegistrarCotacao(Hoje, abertura: 38m, maxima: 39m,
                                  minima: 37m, fechamento: 38.5m, volume: 1000);
            repositorio.Adicionar(acao);
            await repositorio.SalvarAsync();        }

        // Contexto 2: segunda importação, mesmo dia
        await using (var contexto = _banco.CriarContexto())
        {
            var repositorio = new AcaoRepository(contexto);
            var acao = await repositorio.ObterPorTickerAsync(new Ticker("PETR4"));
            Assert.NotNull(acao);

            acao.RegistrarCotacao(Hoje, abertura: 38m, maxima: 39.5m,
            minima: 37m, fechamento: 39.5m, volume: 1000);            // TODO: salvar (sem Adicionar, lembre por quê)
            await repositorio.SalvarAsync();

        }


        // Contexto 3: conferência
        await using (var contexto = _banco.CriarContexto())
        {
            // TODO: Obter a ação do contexto
            await using var contexto1 = _banco.CriarContexto();
            var repositorio = new AcaoRepository(contexto1);
            var acao = await repositorio.ObterPorTickerAsync(new Ticker("PETR4"));
            Assert.NotNull(acao);

            var cotacao = Assert.Single(acao.Cotacoes);
            
            Assert.Single(acao.Cotacoes);// TODO: Assert.Single nas cotações
            Assert.Equal(39.5m, cotacao.Fechamento);
            // TODO: Assert.Equal(39m, cotacao.Fechamento)
        }
    }
}

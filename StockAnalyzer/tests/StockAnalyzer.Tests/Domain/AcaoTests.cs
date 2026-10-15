using StockAnalyzer.Domain.Entities;
using StockAnalyzer.Domain.Exceptions;
using StockAnalyzer.Domain.ValueObjects;

namespace StockAnalyzer.Tests.Domain;

public class AcaoTests
{
    private static readonly DateOnly Hoje = new(2026, 10, 14);

    private static Acao CriarAcao() => new(new Ticker("PETR4"), "Petrobras");

    private static Cotacao Registrar(Acao acao, DateOnly data, decimal fechamento) =>
        acao.RegistrarCotacao(data, abertura: fechamento, maxima: fechamento,
                              minima: fechamento, fechamento: fechamento, volume: 1000);

    // ---------- Construtor e nome ----------

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Construtor_ComNomeVazio_UsaTickerComoNome(string? nome)
    {
        var acao = new Acao(new Ticker("PETR4"), nome);

        Assert.Equal("PETR4", acao.Nome);
    }

    [Fact]
    public void Construtor_ComNomeComEspacos_AplicaTrim()
    {
        var acao = new Acao(new Ticker("PETR4"), "  Petrobras  ");

        Assert.Equal("Petrobras", acao.Nome);
    }

    [Fact]
    public void Construtor_ComNomeDe100Caracteres_Aceita()
    {
        var nome = new string('a', 100);

        var acao = new Acao(new Ticker("PETR4"), nome);

        Assert.Equal(nome, acao.Nome);
    }

    [Fact]
    public void Construtor_ComNomeMuitoLongo_LancaDomainException()
    {
        var nome = new string('a', 101);

        Assert.Throws<DomainException>(() => new Acao(new Ticker("PETR4"), nome));
    }

    [Fact]
    public void Construtor_ComTickerNulo_LancaArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Acao(null!, "Petrobras"));
    }

    [Fact]
    public void AlterarNome_ComNomeVazio_UsaTickerComoNome()
    {
        var acao = CriarAcao();

        acao.AlterarNome("   ");

        Assert.Equal("PETR4", acao.Nome);
    }

    // ---------- RegistrarCotacao ----------

    [Fact]
    public void RegistrarCotacao_NaMesmaData_AtualizaEmVezDeDuplicar()
    {
        var acao = CriarAcao();
        Registrar(acao, Hoje, 38m);

        Registrar(acao, Hoje, 39m);

        var unica = Assert.Single(acao.Cotacoes);
        Assert.Equal(39m, unica.Fechamento);
    }

    [Fact]
    public void RegistrarCotacao_EmDataAntigaJaExistente_AtualizaEmVezDeDuplicar()
    {
        var acao = CriarAcao();
        Registrar(acao, Hoje.AddDays(-2), 38m);
        Registrar(acao, Hoje, 40m);

        Registrar(acao, Hoje.AddDays(-2), 39m);

        Assert.Equal(2, acao.Cotacoes.Count);
        var antiga = Assert.Single(acao.Cotacoes, c => c.Data == Hoje.AddDays(-2));
        Assert.Equal(39m, antiga.Fechamento);
    }

    [Fact]
    public void RegistrarCotacao_NaMesmaDataComValoresInvalidos_MantemCotacaoOriginal()
    {
        var acao = CriarAcao();
        Registrar(acao, Hoje, 38m);

        Assert.Throws<DomainException>(() => Registrar(acao, Hoje, -1m));

        var unica = Assert.Single(acao.Cotacoes);
        Assert.Equal(38m, unica.Fechamento);
    }

    [Fact]
    public void RegistrarCotacao_EmDatasDiferentes_AdicionaAmbas()
    {
        var acao = CriarAcao();

        Registrar(acao, Hoje.AddDays(-1), 38m);
        Registrar(acao, Hoje, 39m);

        Assert.Equal(2, acao.Cotacoes.Count);
    }

    // ---------- UltimaCotacao ----------

    [Fact]
    public void UltimaCotacao_SemCotacoes_RetornaNull()
    {
        var acao = CriarAcao();

        Assert.Null(acao.UltimaCotacao);
    }

    [Fact]
    public void UltimaCotacao_ComVariasCotacoes_RetornaAMaisRecente()
    {
        var acao = CriarAcao();
        Registrar(acao, Hoje, 40m);              // dia 14
        Registrar(acao, Hoje.AddDays(-2), 38m);  // dia 12
        Registrar(acao, Hoje.AddDays(-1), 39m);  // dia 13

        var ultima = acao.UltimaCotacao;

        Assert.NotNull(ultima);
        Assert.Equal(Hoje, ultima.Data);
    }

    // ---------- ObterHistorico ----------

    [Fact]
    public void ObterHistorico_RetornaOrdenadoPorData()
    {
        var acao = CriarAcao();
        Registrar(acao, Hoje, 40m);
        Registrar(acao, Hoje.AddDays(-2), 38m);
        Registrar(acao, Hoje.AddDays(-1), 39m);

        var historico = acao.ObterHistorico();

        var esperado = new[] { Hoje.AddDays(-2), Hoje.AddDays(-1), Hoje };
        Assert.Equal(esperado, historico.Select(c => c.Data));
    }

    // ---------- Encapsulamento ----------

    [Fact]
    public void Cotacoes_NaoPodeSerModificadaDeFora()
    {
        var acao = CriarAcao();
        Registrar(acao, Hoje, 38m);

        Assert.IsNotType<List<Cotacao>>(acao.Cotacoes);

        var colecao = Assert.IsAssignableFrom<ICollection<Cotacao>>(acao.Cotacoes);
        var intrusa = new Cotacao(Hoje.AddDays(-1), 38m, 38m, 38m, 38m, 1000);

        Assert.Throws<NotSupportedException>(() => colecao.Add(intrusa));
        Assert.Single(acao.Cotacoes);
    }
}
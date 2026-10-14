using StockAnalyzer.Domain.Exceptions;
using StockAnalyzer.Domain.ValueObjects;
using StockAnalyzer.Domain.Entities;

namespace StockAnalyzer.Domain.Entities;

public sealed class Acao
{
    private const int TamanhoMaximoNome = 100;
    private readonly List<Cotacao> _cotacoes = new();

    public int Id { get; private set; }
    public Ticker Ticker { get; private set; } = null!;
    public string Nome { get; private set; } = string.Empty;
    public IReadOnlyCollection<Cotacao> Cotacoes => _cotacoes.AsReadOnly();

    public Cotacao? UltimaCotacao => _cotacoes
        .OrderByDescending(c => c.Data)
        .FirstOrDefault();

    private Acao() { } // EF Core

    public Acao(Ticker ticker, string? nome)
    {
        Validar(ticker, nome);
        Ticker = ticker;
        Nome = NormalizarNome(nome, ticker);
    }

    public void AlterarNome(string? nome)
    {
        Nome = NormalizarNome(nome, Ticker);
    }

    public Cotacao RegistrarCotacao(DateOnly data, decimal abertura, decimal maxima,
                                    decimal minima, decimal fechamento, long volume)
    {
        var cotacaoExistente = _cotacoes.FirstOrDefault(c => c.Data == data);

        if (cotacaoExistente is not null)
        {
            cotacaoExistente.Atualizar(abertura, maxima, minima, fechamento, volume);
            return cotacaoExistente;
        }

        var novaCotacao = new Cotacao(data, abertura, maxima, minima, fechamento, volume);
        _cotacoes.Add(novaCotacao);
        return novaCotacao;
    }

    public IReadOnlyList<Cotacao> ObterHistorico()
    {
        return _cotacoes
            .OrderBy(c => c.Data)
            .ToList();
    }

    private static void Validar(Ticker? ticker, string? nome)
    {
        if (ticker is null)
            throw new DomainException("Ticker da ação não pode ser nulo.");

        if (string.IsNullOrWhiteSpace(nome) is false && nome.Trim().Length > TamanhoMaximoNome)
            throw new DomainException($"Nome da ação deve ter no máximo {TamanhoMaximoNome} caracteres.");
    }

    private static string NormalizarNome(string? nome, Ticker ticker)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return ticker.ToString();

        var textoNormalizado = nome.Trim();
        if (textoNormalizado.Length > TamanhoMaximoNome)
            throw new DomainException($"Nome da ação deve ter no máximo {TamanhoMaximoNome} caracteres.");

        return textoNormalizado;
    }
}
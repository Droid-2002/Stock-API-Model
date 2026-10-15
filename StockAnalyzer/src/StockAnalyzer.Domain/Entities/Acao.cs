using StockAnalyzer.Domain.Exceptions;
using StockAnalyzer.Domain.ValueObjects;

namespace StockAnalyzer.Domain.Entities;

public sealed class Acao
{
    private const int TamanhoMaximoNome = 100;
    private readonly List<Cotacao> _cotacoes = new();

    public int Id { get; private set; }
    public Ticker Ticker { get; private set; } = null!;
    public string Nome { get; private set; } = string.Empty;
    public IReadOnlyCollection<Cotacao> Cotacoes => _cotacoes.AsReadOnly();

    public Cotacao? UltimaCotacao => _cotacoes.MaxBy(c => c.Data);

    private Acao() { } // EF Core

    public Acao(Ticker ticker, string? nome)
    {
        Ticker = ticker ?? throw new ArgumentNullException(nameof(ticker));
        Nome = NormalizarNome(nome, ticker);
    }

    public void AlterarNome(string? nome)
    {
        Nome = NormalizarNome(nome, Ticker);
    }

    public Cotacao RegistrarCotacao(DateOnly data, decimal abertura, decimal maxima,
                                    decimal minima, decimal fechamento, long volume)
    {
        var existente = _cotacoes.FirstOrDefault(c => c.Data == data);

        if (existente is not null)
        {
            existente.Atualizar(abertura, maxima, minima, fechamento, volume);
            return existente;
        }

        var cotacao = new Cotacao(data, abertura, maxima, minima, fechamento, volume);
        _cotacoes.Add(cotacao);
        return cotacao;
    }

    public IReadOnlyList<Cotacao> ObterHistorico()
    {
        return _cotacoes.OrderBy(c => c.Data).ToList();
    }

    private static string NormalizarNome(string? nome, Ticker ticker)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return ticker.Valor;

        var nomeNormalizado = nome.Trim();

        if (nomeNormalizado.Length > TamanhoMaximoNome)
        {
            throw new DomainException(
                $"O nome da ação não pode ter mais que " +
                $"{TamanhoMaximoNome} caracteres.");
        }

        return nomeNormalizado;
    }
}
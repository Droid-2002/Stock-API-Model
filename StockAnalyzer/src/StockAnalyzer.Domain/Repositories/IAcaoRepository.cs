using StockAnalyzer.Domain.Entities;
using StockAnalyzer.Domain.ValueObjects;

namespace StockAnalyzer.Domain.Repositories;

public interface IAcaoRepository
{
    Task<Acao?> ObterPorTickerAsync(Ticker ticker, CancellationToken ct = default);
    Task<IReadOnlyList<Acao>> ListarAsync(CancellationToken ct = default);
    void Adicionar(Acao acao);
    Task SalvarAsync(CancellationToken ct = default);
}

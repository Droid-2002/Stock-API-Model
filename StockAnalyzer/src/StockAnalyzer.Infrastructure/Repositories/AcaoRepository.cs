using Microsoft.EntityFrameworkCore;
using StockAnalyzer.Domain.Entities;
using StockAnalyzer.Domain.Repositories; 
using StockAnalyzer.Domain.ValueObjects; 
using StockAnalyzer.Infrastructure.Persistence;

namespace StockAnalyzer.Infrastructure.Repositories;

internal sealed class AcaoRepository : IAcaoRepository
{
    private readonly AppDbContext _context;

    public AcaoRepository(AppDbContext context) => _context = context;

    public Task<Acao?> ObterPorTickerAsync(Ticker ticker, CancellationToken ct = default)
        => _context.Acoes
            .Include(a => a.Cotacoes)
            .FirstOrDefaultAsync(a => a.Ticker == ticker, ct);


    public void Adicionar(Acao acao) => _context.Acoes.Add(acao);

    public async Task<IReadOnlyList<Acao>> ListarAsync(CancellationToken ct = default)
    => await _context.Acoes
        .Include(a => a.Cotacoes)
        .OrderBy(a => a.Ticker)
        .ToListAsync(ct);

    public Task SalvarAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);
}
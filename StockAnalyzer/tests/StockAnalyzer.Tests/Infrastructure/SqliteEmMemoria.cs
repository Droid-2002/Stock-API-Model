using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StockAnalyzer.Infrastructure.Persistence;

namespace StockAnalyzer.Tests.Infrastructure;

public sealed class SqliteEmMemoria : IDisposable
{
    private readonly SqliteConnection _conexao;
    private readonly DbContextOptions<AppDbContext> _opcoes;

    public SqliteEmMemoria()
    {
        _conexao = new SqliteConnection("DataSource=:memory:");   // (1)
        _conexao.Open();

        _opcoes = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_conexao)                                   // (2)
            .Options;

        using var contexto = CriarContexto();
        contexto.Database.EnsureCreated();                         // (3)
    }

    public AppDbContext CriarContexto() => new(_opcoes);

    public void Dispose() => _conexao.Dispose();                   // (4)
}

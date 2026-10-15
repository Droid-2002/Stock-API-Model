using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockAnalyzer.Domain.Entities;

namespace StockAnalyzer.Infrastructure.Persistence.Configurations;

internal sealed class CotacaoConfiguration : IEntityTypeConfiguration<Cotacao>
{
    public void Configure(EntityTypeBuilder<Cotacao> builder)
    {
        builder.ToTable("Cotacoes");
        builder.HasKey(c => c.Id);

        builder.HasIndex("AcaoId", nameof(Cotacao.Data)).IsUnique();

        builder.Property(c => c.Abertura).HasPrecision(18, 4);
        builder.Property(c => c.Máxima).HasPrecision(18, 4);
        builder.Property(c => c.Mínima).HasPrecision(18, 4);
        builder.Property(c => c.Fechamento).HasPrecision(18, 4);
    }
}
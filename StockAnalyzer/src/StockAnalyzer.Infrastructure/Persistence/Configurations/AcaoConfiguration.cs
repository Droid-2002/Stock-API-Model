using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockAnalyzer.Domain.Entities;
using StockAnalyzer.Domain.ValueObjects;

namespace StockAnalyzer.Infrastructure.Persistence.Configurations;

internal sealed class AcaoConfiguration : IEntityTypeConfiguration<Acao>
{
    public void Configure(EntityTypeBuilder<Acao> builder)
    {
        builder.ToTable("Acoes");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Ticker)
            .HasConversion(t => t.Valor, v => new Ticker(v))
            .HasMaxLength(6)
            .IsRequired();
        
        builder.HasIndex(a => a.Ticker).IsUnique();

        builder.Property(a => a.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasMany(a => a.Cotacoes)
            .WithOne()
            .HasForeignKey("AcaoId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(a => a.Cotacoes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(a => a.UltimaCotacao);
    }
}

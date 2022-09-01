using Microsoft.EntityFrameworkCore;
using MyJetWallet.Sdk.Postgres;
using Service.Trading.Domain.Models;

namespace Service.Trading.Postgres;

public class MyContext: MyDbContext
{
    public const string Schema = "cb_trading";
    public const string TradeRequestTableName = "trade_request";
    public const string ExternalTradeTableName = "external_trade";


    public MyContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<TradeRequest>().ToTable(TradeRequestTableName);
        modelBuilder.Entity<TradeRequest>().HasKey(e => e.RequestId);
        modelBuilder.Entity<TradeRequest>().Property(e => e.ClientId).HasMaxLength(1024);
        modelBuilder.Entity<TradeRequest>().Property(e => e.FeeAsset).HasMaxLength(1024);
        modelBuilder.Entity<TradeRequest>().Property(e => e.SellAssetSymbol).HasMaxLength(128);
        modelBuilder.Entity<TradeRequest>().Property(e => e.BuyAssetSymbol).HasMaxLength(128);
        modelBuilder.Entity<TradeRequest>().Property(e => e.ErrorMessage).HasMaxLength(1024);
        modelBuilder.Entity<TradeRequest>().Property(e => e.RequestId).HasMaxLength(256);
        modelBuilder.Entity<TradeRequest>().HasIndex(e => e.ClientId);
        modelBuilder.Entity<TradeRequest>().HasIndex(e => e.Timestamp);
            
        modelBuilder.Entity<ExternalTrade>().ToTable(ExternalTradeTableName);
        modelBuilder.Entity<ExternalTrade>().HasKey(e => e.TradeId);
        modelBuilder.Entity<ExternalTrade>().Property(e => e.Market).HasMaxLength(128);
        modelBuilder.Entity<ExternalTrade>().Property(e => e.Source).HasMaxLength(128);
        modelBuilder.Entity<ExternalTrade>().Property(e => e.RequestId).HasMaxLength(256);
        modelBuilder.Entity<ExternalTrade>().Property(e => e.TradeId).HasMaxLength(256);
        modelBuilder.Entity<ExternalTrade>().Property(e => e.ExchangeTradeId).HasMaxLength(256);
        modelBuilder.Entity<ExternalTrade>().HasIndex(e => e.RequestId);
        modelBuilder.Entity<ExternalTrade>().HasIndex(e => e.Timestamp);
        modelBuilder.Entity<ExternalTrade>().HasIndex(e => e.Status);
        
        base.OnModelCreating(modelBuilder);
    }
}
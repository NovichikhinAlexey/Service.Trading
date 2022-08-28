using System.Collections.Generic;
using System.Linq;
using Service.Trading.Domain.Models;

namespace Service.Trading.Domain;

public class AssetMapperService : IAssetMapperService
{
    public readonly Dictionary<string, Dictionary<string, AssetMap>> Map = new();

    public List<AssetMap> GetAllBySource(string source)
    {
        lock (Map)
        {
            if (!Map.TryGetValue(source, out var data))
                return new List<AssetMap>();

            return data.Values.ToList();
        }
    }

    public AssetMapperService Add(AssetMap map)
    {
        lock (Map)
        {
            if (!Map.TryGetValue(map.Source, out var source))
            {
                source = new Dictionary<string, AssetMap>();
                Map[map.Source] = source;
            }
            source[map.Market] = map;
        }

        return this;
    }

    public void Start()
    {
        this
            .Add(new AssetMap()
                {
                    Source = ExternalMarketConst.HuobiSource,
                    Market = "btcusdt",
                    ExternalBaseAsset = "btc",
                    ExternalQuoteAsset = "usdt",
                    BaseAsset = "BTC",
                    QuoteAsset = "USDT",
                    PriceAccuracy = 2,
                    AmountAccuracy = 6,
                    VolumeAccuracy = 8,
                    MinAmount = 0.0001m,
                    MaxAmount = 100,
                    MinVolume = 1,
                    MaxVolume = 1000000m
                })
            .Add(new AssetMap()
            {
                Source = ExternalMarketConst.HuobiSource,
                Market = "ethusdt",
                ExternalBaseAsset = "eth",
                ExternalQuoteAsset = "usdt",
                BaseAsset = "ETH",
                QuoteAsset = "USDT",
                PriceAccuracy = 2,
                AmountAccuracy = 4,
                VolumeAccuracy = 8,
                MinAmount = 0.001m,
                MaxAmount = 1000,
                MinVolume = 1,
                MaxVolume = 1000000.0m
            })
            .Add(new AssetMap()
            {
                Source = ExternalMarketConst.HuobiSource,
                Market = "xlmusdt",
                ExternalBaseAsset = "xlm",
                ExternalQuoteAsset = "usdt",
                BaseAsset = "XLM",
                QuoteAsset = "USDT",
                PriceAccuracy = 6,
                AmountAccuracy = 4,
                VolumeAccuracy = 8,
                MinAmount = 0.1m,
                MaxAmount = 1000000,
                MinVolume = 1,
                MaxVolume = 50000m
            })
            .Add(new AssetMap()
            {
                Source = ExternalMarketConst.HuobiSource,
                Market = "xrpusdt",
                ExternalBaseAsset = "xrp",
                ExternalQuoteAsset = "usdt",
                BaseAsset = "XRP",
                QuoteAsset = "USDT",
                PriceAccuracy = 5,
                AmountAccuracy = 2,
                VolumeAccuracy = 8,
                MinAmount = 1m,
                MaxAmount = 500000,
                MinVolume = 1,
                MaxVolume = 100000
            });
    }
}
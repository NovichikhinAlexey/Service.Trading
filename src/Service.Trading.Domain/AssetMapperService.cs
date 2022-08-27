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
}
using System.Collections.Generic;
using Service.Trading.Domain.Models;

namespace Service.Trading.Domain.Configurations;

public interface IAssetMapperService
{ 
    List<AssetMap> GetAllBySource(string source);
    AssetMapperService Add(AssetMap map);
}
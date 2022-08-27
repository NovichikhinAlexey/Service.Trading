using System;
using System.Threading.Tasks;
using CubiTradingService;
using Grpc.Core;

namespace Service.Trading.Services
{
    public class CubiTradingGrpc: CubiTrading.CubiTradingBase
    {
        public override async Task<ExecuteTradeResponse> ExecuteTrade(ExecuteTradeRequest request, ServerCallContext context)
        {
            var resp = new ExecuteTradeResponse()
            {
                SellAssetSymbol = request.SellAssetSymbol,
                BuyAssetSymbol = request.BuyAssetSymbol,
                SellAmount = request.SellAmount,
                BuyAmount = request.BuyAmount,
                FeeAsset = request.SellAssetSymbol,
                FeeAmount = 0,
                RequestId = request.RequestId,
                ErrorCode = 0,
                ErrorMessage = "OK"
            };

            return resp;
        }
    }
}
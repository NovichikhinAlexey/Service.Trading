using System;
using System.Threading.Tasks;
using CubiTradingService;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.Trading.Domain;
using Service.Trading.Domain.Models;

namespace Service.Trading.Services
{
    public class CubiTradingGrpc: CubiTrading.CubiTradingBase
    {
        private readonly ITradeService _tradeService;
        private readonly ILogger<CubiTradingGrpc> _logger;

        public CubiTradingGrpc(ITradeService tradeService, ILogger<CubiTradingGrpc> logger)
        {
            _tradeService = tradeService;
            _logger = logger;
        }

        public override async Task<ExecuteTradeResponse> ExecuteTrade(ExecuteTradeRequest request, ServerCallContext context)
        {
            _logger.LogInformation("[Trade] Request: {jsonString}", JsonConvert.SerializeObject(request));
            try
            {
                var trade = new TradeRequest()
                {
                    RequestId = request.RequestId,
                    ClientId = request.ClientId,

                    SellAssetSymbol = request.SellAssetSymbol,
                    BuyAssetSymbol = request.BuyAssetSymbol,
                    SellAmount = (decimal) request.SellAmount,
                    BuyAmount = (decimal) request.BuyAmount,

                    FeeAsset = request.SellAssetSymbol,
                    FeeAmount = 0m,

                    Timestamp = DateTime.UtcNow,

                    ErrorCode = (int) ErrorCodeTrade.Ok,
                    ErrorMessage = ErrorCodeTrade.Ok.ToString()
                };

                trade = await _tradeService.ExecuteTrade(trade);


                var resp = new ExecuteTradeResponse()
                {
                    SellAssetSymbol = trade.SellAssetSymbol,
                    BuyAssetSymbol = trade.BuyAssetSymbol,
                    SellAmount = (double) trade.SellAmount,
                    BuyAmount = (double) trade.BuyAmount,
                    FeeAsset = trade.SellAssetSymbol,
                    FeeAmount = (double) trade.FeeAmount,
                    RequestId = trade.RequestId,
                    ErrorCode = trade.ErrorCode,
                    ErrorMessage = trade.ErrorMessage
                };

                _logger.LogInformation("[Trade] Response: {jsonString}", JsonConvert.SerializeObject(resp));

                return resp;
            }
            catch (PriceNotFoundException ex)
            {
                _logger.LogError(ex, "Cannot execute trade {requestId}, client: {clientId}", 
                    request.RequestId, request.ClientId);
                
                var resp = new ExecuteTradeResponse()
                {
                    RequestId = request.RequestId,
                    ErrorCode = (int)ErrorCodeTrade.NotEnoughLiquidity,
                    ErrorMessage = ErrorCodeTrade.NotEnoughLiquidity.ToString()
                };

                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot execute trade {requestId}, client: {clientId}", 
                    request.RequestId, request.ClientId);
                
                var resp = new ExecuteTradeResponse()
                {
                    RequestId = request.RequestId,
                    ErrorCode = (int)ErrorCodeTrade.InternalError,
                    ErrorMessage = ErrorCodeTrade.InternalError.ToString()
                };

                return resp;
            }
        }
    }
}
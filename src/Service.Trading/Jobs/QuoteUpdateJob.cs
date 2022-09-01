using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service.Tools;
using MyNoSqlServer.Abstractions;
using Service.Trading.Domain.Configurations;
using Service.Trading.Domain.Models;
using Service.Trading.Domain.Trade;

namespace Service.Trading.Jobs
{
    public class QuoteUpdateJob
    {
        private readonly ITradePriceCalculator _tradePriceCalculator;
        private readonly ILogger<QuoteUpdateJob> _logger;
        private readonly IMyNoSqlServerDataWriter<QuotePrice> _writer;
        private readonly ITradeConfiguration _configuration;
        private MyTaskTimer _timer;
        private DateTime _lastCleanTime = DateTime.MinValue;

        public QuoteUpdateJob(
            ITradePriceCalculator tradePriceCalculator, 
            ILogger<QuoteUpdateJob> logger,
            IMyNoSqlServerDataWriter<QuotePrice> writer,
            ITradeConfiguration configuration)
        {
            _tradePriceCalculator = tradePriceCalculator;
            _logger = logger;
            _writer = writer;
            _configuration = configuration;
        }

        private async Task DoTime()
        {
            var quotes = _tradePriceCalculator.CalculateQuotes();

            if ((DateTime.UtcNow - _lastCleanTime).TotalSeconds > 120)
            {
                await _writer.CleanAndBulkInsertAsync(quotes);
                _lastCleanTime = DateTime.UtcNow;
                _logger.LogInformation($"Quote Prices recalculated, count = {quotes.Count}. Clean storage.");
            }
            else
            {
                await _writer.BulkInsertOrReplaceAsync(quotes);
                _logger.LogInformation($"Quote Prices recalculated, count = {quotes.Count}");
            }
            
            var config = _configuration.GetTradeConfig();
            _timer.ChangeInterval(TimeSpan.FromSeconds(config.QuotePriceRecalculateIntervalInSec));
        }

        public void Start()
        {
            _timer = MyTaskTimer.Create<QuoteUpdateJob>(TimeSpan.FromSeconds(15), _logger, DoTime);
            _timer.Start();
        }
    }
}
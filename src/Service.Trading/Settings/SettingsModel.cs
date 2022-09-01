using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.Trading.Settings
{
    public class SettingsModel
    {
        [YamlProperty("Trading.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("Trading.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("Trading.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }
        
        [YamlProperty("Trading.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }
        
        [YamlProperty("Trading.HuobiApiKey")]
        public string HuobiApiKey { get; set; }
        
        [YamlProperty("Trading.HuobiApiSecret")]
        public string HuobiApiSecret { get; set; }
        
        [YamlProperty("Trading.MinMarkupPercentage")]
        public string MinMarkupPercentage { get; set; }
        
        [YamlProperty("Trading.MaxMarkupPercentage")]
        public string MaxMarkupPercentage { get; set; }
        
        [YamlProperty("Trading.QuotePriceRecalculateIntervalInSec")]
        public int QuotePriceRecalculateIntervalInSec { get; set; }
        
        [YamlProperty("Trading.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }
        
        
        
        
    }
}

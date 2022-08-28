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
        
        [YamlProperty("Trading.HuobiApiKey")]
        public string HuobiApiKey { get; set; }
        
        [YamlProperty("Trading.HuobiApiSecret")]
        public string HuobiApiSecret { get; set; }
        
        [YamlProperty("Trading.MinMarkupPercentage")]
        public decimal MinMarkupPercentage { get; set; }
        
        [YamlProperty("Trading.MaxMarkupPercentage")]
        public decimal MaxMarkupPercentage { get; set; }
        
    }
}

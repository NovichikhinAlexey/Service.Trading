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
    }
}

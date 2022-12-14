

using Huobi.Net.Clients;
using Huobi.Net.Enums;
using MySettingsReader;
using Newtonsoft.Json;
using Service.Trading.Settings;

//https://github.com/JKorf/Huobi.Net/blob/master/Examples/Huobi.Net.ConsoleClient/Program.cs

var settings = SettingsReader.GetSettings<SettingsModel>(".cubi");

using (var client = new HuobiClient())
{
    // Public method
    var marketDetails = await client.SpotApi.ExchangeData.GetSymbolDetails24HAsync("ethusdt");
    if (marketDetails.Success) // Check the success flag for error handling
        Console.WriteLine($"Got market stats, last price: {marketDetails.Data.ClosePrice}");
    else
        Console.WriteLine($"Failed to get stats, error: {marketDetails.Error}");

    // Private method
    client.SetApiCredentials(new CryptoExchange.Net.Authentication.ApiCredentials(settings.HuobiApiKey, settings.HuobiApiSecret));
    var accounts = await client.SpotApi.Account.GetAccountsAsync();
    if (accounts.Success) // Check the success flag for error handling
    {
        Console.WriteLine($"Got account list, account id #1: {accounts.Data.First().Id}");
    }
    else
        Console.WriteLine($"Failed to get account list, error: {accounts.Error}");

    foreach (var acc in accounts.Data)
    {
        var balance = await client.SpotApi.Account.GetBalancesAsync(acc.Id);
        Console.WriteLine($"Account {acc.Id}, Type: {acc.Type}");
        foreach (var bal in balance.Data.Where(e => e.Balance > 0))
        {
            Console.WriteLine(JsonConvert.SerializeObject(bal, Formatting.Indented));
        }
        Console.WriteLine("=====================\n");
    }

    {
        var res = await client.SpotApi.ExchangeData.GetSymbolsAsync();
        var symbols = new List<string>()
        {
            "btcusdt", "ethusdt", "xlmusdt", "xrpusdt"
        };
        
        Console.WriteLine(JsonConvert.SerializeObject(res.Data.Where(e => symbols.Contains(e.Name)), Formatting.Indented));
        return;
    }
    
}



Console.WriteLine("");
Console.WriteLine("Press enter to continue to the socket client..");
Console.ReadLine();




// Socket client
var socketClient = new HuobiSocketClient();

//await socketClient.SpotStreams.SubscribeToTickerUpdatesAsync()

await socketClient.SpotStreams.SubscribeToKlineUpdatesAsync("ethusdt", KlineInterval.FiveMinutes, data =>
{
    Console.WriteLine("Received kline update. Last price: " + data.Data.ClosePrice);
});

Console.ReadLine();
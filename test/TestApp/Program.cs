using System;
using System.Threading.Tasks;
using ProtoBuf.Grpc.Client;
using Service.Trading.Client;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();


            var factory = new TradingClientFactory("http://localhost:80");
            var client = factory.GetCubiTradingClient();

            // var resp = await  client.SayHelloAsync(new HelloRequest(){Name = "Alex"});
            // Console.WriteLine(resp?.Message);

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}

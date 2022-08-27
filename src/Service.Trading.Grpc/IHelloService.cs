using System.ServiceModel;
using System.Threading.Tasks;
using Service.Trading.Grpc.Models;

namespace Service.Trading.Grpc
{
    [ServiceContract]
    public interface IHelloService
    {
        [OperationContract]
        Task<HelloMessage> SayHelloAsync(HelloRequest request);
    }
}
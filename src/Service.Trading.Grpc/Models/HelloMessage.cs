using System.Runtime.Serialization;
using Service.Trading.Domain.Models;

namespace Service.Trading.Grpc.Models
{
    [DataContract]
    public class HelloMessage : IHelloMessage
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
    }
}
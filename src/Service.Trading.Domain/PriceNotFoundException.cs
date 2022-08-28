using System;
using System.Runtime.Serialization;

namespace Service.Trading.Domain;

public class PriceNotFoundException : Exception
{
    public PriceNotFoundException()
    {
    }

    protected PriceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public PriceNotFoundException(string message) : base(message)
    {
    }

    public PriceNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
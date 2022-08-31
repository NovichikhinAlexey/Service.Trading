using System;
using System.Runtime.Serialization;

namespace Service.Trading.Domain.Exceptions;

public class NotEnoughLiquidityException : Exception
{
    public NotEnoughLiquidityException()
    {
    }

    protected NotEnoughLiquidityException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public NotEnoughLiquidityException(string message) : base(message)
    {
    }

    public NotEnoughLiquidityException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
using System;
using System.Runtime.Serialization;

[Serializable]
internal class BaseStationBLException : Exception
{
    public BaseStationBLException()
    {
    }

    public BaseStationBLException(string message) : base(message)
    {
    }

    public BaseStationBLException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected BaseStationBLException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
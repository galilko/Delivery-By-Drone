using System;
using System.Runtime.Serialization;

[Serializable]
internal class BlUpdateEntityException : Exception
{
    public BlUpdateEntityException()
    {
    }

    public BlUpdateEntityException(string message) : base(message)
    {
    }

    public BlUpdateEntityException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected BlUpdateEntityException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
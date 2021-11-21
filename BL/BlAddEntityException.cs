using System;
using System.Runtime.Serialization;

[Serializable]
internal class BlAddEntityException : Exception
{
    public BlAddEntityException()
    {
    }

    public BlAddEntityException(string message) : base(message)
    {
    }

    public BlAddEntityException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected BlAddEntityException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
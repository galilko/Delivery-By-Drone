using System;
using System.Runtime.Serialization;

[Serializable]
internal class BlFindItemException : Exception
{
    public BlFindItemException()
    {
    }

    public BlFindItemException(string message) : base(message)
    {
    }

    public BlFindItemException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected BlFindItemException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
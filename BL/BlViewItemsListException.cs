using System;
using System.Runtime.Serialization;

[Serializable]
internal class BlViewItemsListException : Exception
{
    public BlViewItemsListException()
    {
    }

    public BlViewItemsListException(string message) : base(message)
    {
    }

    public BlViewItemsListException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected BlViewItemsListException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
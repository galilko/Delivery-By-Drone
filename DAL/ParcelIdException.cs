using System;
using System.Runtime.Serialization;

namespace DalObject
{
    [Serializable]
    internal class ParcelIdException : Exception
    {
        public ParcelIdException()
        {
        }

        public ParcelIdException(string message) : base(message)
        {
        }

        public ParcelIdException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ParcelIdException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
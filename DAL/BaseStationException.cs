using System;
using System.Runtime.Serialization;

namespace DalObject
{
    [Serializable]
    internal class BaseStationException : Exception
    {
        public BaseStationException()
        {
        }

        public BaseStationException(string message) : base(message)
        {
        }

        public BaseStationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BaseStationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
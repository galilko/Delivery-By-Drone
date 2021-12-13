using System;
using System.Runtime.Serialization;

namespace DO
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

    [Serializable]
    internal class CustomerException : Exception
    {
        public CustomerException()
        {
        }

        public CustomerException(string message) : base(message)
        {
        }

        public CustomerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CustomerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    internal class ParcelException : Exception
    {
        public ParcelException()
        {
        }

        public ParcelException(string message) : base(message)
        {
        }

        public ParcelException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ParcelException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    internal class DroneException : Exception
    {
        public DroneException()
        {
        }

        public DroneException(string message) : base($"Drone Exception: " + message)
        {
        }

        public DroneException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DroneException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
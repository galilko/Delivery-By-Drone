using System;
using System.Runtime.Serialization;

namespace DO
{
    [Serializable]
    public class BaseStationException : Exception
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
    public class CustomerException : Exception
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
    public class ParcelException : Exception
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
    public class DroneException : Exception
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

    [Serializable]
    public class XMLFileLoadCreateException : Exception
    {
        private string filePath;
        private string v;
        private Exception ex;

        public XMLFileLoadCreateException()
        {
        }

        public XMLFileLoadCreateException(string message) : base(message)
        {
        }

        public XMLFileLoadCreateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public XMLFileLoadCreateException(string filePath, string v, Exception ex)
        {
            this.filePath = filePath;
            this.v = v;
            this.ex = ex;
        }

        protected XMLFileLoadCreateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
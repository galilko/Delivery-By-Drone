using System;
using System.Runtime.Serialization;

namespace DalObject
{
    [Serializable]
    internal class DroneIdException : Exception
    {
        public DroneIdException()
        {
        }

        public DroneIdException(string message) : base($"Drone Exception" + message)
        {
        }

        public DroneIdException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DroneIdException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
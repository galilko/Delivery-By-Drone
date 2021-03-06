using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace BO
{
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

    [Serializable]
    internal class BlDeleteEntityException : Exception
    {
        public BlDeleteEntityException()
        {
        }

        public BlDeleteEntityException(string message) : base(message)
        {
        }

        public BlDeleteEntityException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BlDeleteEntityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    internal class BlDeleteItemException : Exception
    {
        public BlDeleteItemException()
        {
        }

        public BlDeleteItemException(string message) : base(message)
        {
        }

        public BlDeleteItemException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BlDeleteItemException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }


    [Serializable]
    internal class BadStatusException : Exception
    {
        public BadStatusException()
        {
        }

        public BadStatusException(string message) : base(message)
        {
        }

        public BadStatusException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BadStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

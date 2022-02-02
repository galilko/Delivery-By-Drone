using System;
using System.Runtime.Serialization;

namespace DO
{
    [Serializable]
    public class ExistIdException : Exception
    {
        private int id;

        public ExistIdException()
        {
        }

        public ExistIdException(string message) : base(message)
        {
        }

        public ExistIdException(string message, int id) : base(message)
        {
            this.id = id;
        }

        public ExistIdException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ExistIdException(SerializationInfo info, StreamingContext context) : base(info, context)
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
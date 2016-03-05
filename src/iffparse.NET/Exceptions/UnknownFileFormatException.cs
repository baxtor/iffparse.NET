using System;

namespace net.iffparse.parser
{
    public class UnknownFileFormatException : Exception
    {
        public UnknownFileFormatException() : base()
        {
        }

        public UnknownFileFormatException(string message) : base(message)
        {
            
        }
    }
}


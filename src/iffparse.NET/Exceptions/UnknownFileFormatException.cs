using System;

namespace IffParse.Exceptions
{
	/// <summary>
	/// Unknown file format exception.
	/// </summary>
    public class UnknownFileFormatException : Exception
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:IffParse.Exceptions.UnknownFileFormatException"/> class.
		/// </summary>
        public UnknownFileFormatException() : base()
        {
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:IffParse.Exceptions.UnknownFileFormatException"/> class.
		/// </summary>
		/// <param name="message">Message.</param>
        public UnknownFileFormatException(string message) : base(message)
        {
            
        }
    }
}


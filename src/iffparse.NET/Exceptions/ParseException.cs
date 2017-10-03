using System;

namespace IffParse.Exceptions
{
	/// <summary>
	/// Parse exception.
	/// </summary>
	public class ParseException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ParseException"/> class.
		/// </summary>
		public ParseException () : base()
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ParseException"/> class.
		/// </summary>
		/// <param name="message">Message.</param>
		public ParseException (string message) : base(message)
		{
		}
	}
}


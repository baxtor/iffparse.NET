using System;

namespace net.iffparse.parser
{
	/// <summary>
	/// Parse exception.
	/// </summary>
	public class ParseException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="iffparse.NET.ParseException"/> class.
		/// </summary>
		public ParseException () : base()
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="iffparse.NET.ParseException"/> class.
		/// </summary>
		/// <param name="message">Message.</param>
		public ParseException (string message) : base(message)
		{
		}
	}
}


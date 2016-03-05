using System;

namespace Net.Iffparse
{
	public enum ParserStatus  
	{
		/// <summary>
		/// Operation not supported at this time.
		/// </summary>
		NotSupported = -15,
		/// <summary>
		/// Bad IFFParser parameter.
		/// </summary>
		BadPointer = -14,
		/// <summary>
		/// Unknown tag supplied.
		/// </summary>
		BadTag = -13,
		/// <summary>
		/// No open type specified.
		/// </summary>
		NoOpenType = -12,
		/// <summary>
		/// Hit the end of file too soon.
		/// </summary>
		PrematureEndOfFile = -11,
		/// <summary>
		/// Not an IFF file.
		/// </summary>
		NotIFF = -10,
		/// <summary>
		/// IFF syntax error.
		/// </summary>
		SyntaxError = -9,
		/// <summary>
		/// Data is corrupt.
		/// </summary>
		Mangled = -8,
		/// <summary>
		/// Bad mode parameter.
		/// </summary>
		BadMode = -7,
		/// <summary>
		/// Bad storage location given.
		/// </summary>
		TooBig = -6,
		/// <summary>
		/// Bad storage location given.
		/// </summary>
		BadLocation = -5,
		/// <summary>
		/// No memory.
		/// </summary>
		OutOfMemory = -4,
		/// <summary>
		/// No valid scope for property.
		/// </summary>
		NoScope = -3,
		/// <summary>
		/// About to leave context.
		/// </summary>
		EndOfContext = -2,
		/// <summary>
		/// Reached logical end of file.
		/// </summary>
		EndOfFile = -1,
		/// <summary>
		/// Ok.
		/// </summary>
		Ok = 0
	}
}


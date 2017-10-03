using System;
using System.Runtime.InteropServices;

namespace IffParse.Chunks
{
	/// <summary>
	/// Small chunk.
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct SmallChunk
	{
		/// <summary>
		/// The chunk identifier.
		/// </summary>
		[FieldOffset(0)]
		public UInt32 Id;
		/// <summary>
		/// The chunk size.
		/// </summary>
		[FieldOffset(4)]
		public UInt32 Size;
	}
}
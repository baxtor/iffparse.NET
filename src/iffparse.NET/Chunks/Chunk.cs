using System;
using System.Runtime.InteropServices;

namespace IffParse.Chunks
{
	/// <summary>
	/// Chunk structure
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct Chunk
	{
		/// <summary>
		/// The chunk identifier.
		/// </summary>
		[FieldOffset(0)]
		public UInt32 Id;
		/// <summary>
		/// The chunk filler.
		/// </summary>
		[FieldOffset(4)]
		public UInt32 Filler;
		/// <summary>
		/// The chunk size.
		/// </summary>
		[FieldOffset(8)]
		public UInt64 Size;
	}
}
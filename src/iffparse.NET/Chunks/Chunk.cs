using System;
using System.Runtime.InteropServices;

namespace Net.Iffparse
{
	[StructLayout(LayoutKind.Explicit)]
	public struct Chunk
	{
		[FieldOffset(0)]
		public UInt32 Id;
		[FieldOffset(4)]
		public UInt32 Filler;
		[FieldOffset(8)]
		public UInt64 Size;
	}
}
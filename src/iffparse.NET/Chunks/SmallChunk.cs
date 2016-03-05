using System;
using System.Runtime.InteropServices;

namespace Net.Iffparse
{
	[StructLayout(LayoutKind.Explicit)]
	public struct SmallChunk
	{
		[FieldOffset(0)]
		public UInt32 Id;
		[FieldOffset(4)]
		public UInt32 Size;
	}
}
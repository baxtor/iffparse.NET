using System;

namespace Net.Iffparse
{
	public enum ChunkSize : uint
	{
		Alignment = 4U,
		Unknown32Bit = 0xffffffffU,
		Unknown64Bit = 0xfffffffeU,
		Known64Bit = 0xfffffffdU,
		Reserved = 0xfffffff0U
	}
}


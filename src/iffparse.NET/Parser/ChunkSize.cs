namespace IffParse.Parser
{
	/// <summary>
	/// Chunk size.
	/// </summary>
    public enum ChunkSize : uint
	{
		/// <summary>
		/// Alignment.
		/// </summary>
		Alignment = 4U,
		/// <summary>
		/// Unknown 32 bit.
		/// </summary>
		Unknown32Bit = 0xffffffffU,
		/// <summary>
		/// Unknown 64 bit.
		/// </summary>
		Unknown64Bit = 0xfffffffeU,
		/// <summary>
		/// Known 64 bit.
		/// </summary>
		Known64Bit = 0xfffffffdU,
		/// <summary>
		/// Reserved.
		/// </summary>
		Reserved = 0xfffffff0U
	}
}


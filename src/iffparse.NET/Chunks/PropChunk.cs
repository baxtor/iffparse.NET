using System;
using System.Runtime.InteropServices;

namespace IffParse.Chunks
{
	/// <summary>
	/// Property chunk.
	/// </summary>
    [StructLayout(LayoutKind.Explicit)]
    struct PropChunk
    {
		/// <summary>
		/// The size of the data.
		/// </summary>
        [FieldOffset(0)]
        public UInt64 DataSize;
        /// <summary>
        /// The data.
        /// </summary>
		[FieldOffset(8)]
        public byte[] Data;
    }
}

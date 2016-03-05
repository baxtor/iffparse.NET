using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Net.Iffparse
{
    [StructLayout(LayoutKind.Explicit)]
    struct PropChunk
    {
        [FieldOffset(0)]
        public UInt64 DataSize;
        [FieldOffset(8)]
        public byte[] Data;
    }
}

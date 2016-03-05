using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Net.Iffparse
{
    internal struct ChunkHandler
    {
        public IFFCallBack ChunkHandlerCallBack;
        public object UserData;
    }
}

using System;
using System.IO;

namespace Net.Iffparse
{
	internal delegate Stream IFFOpenCallback(string openKey, bool writeMode);
	internal delegate Int32 IFFCloseCallback(Stream stream);
	internal delegate Int32 IFFReadCallback(Stream stream, byte[] buffer,int offset, int count);
	internal delegate Int32 IFFWriteCallback(Stream stream, byte[] buffer,int offset, int count);
	internal delegate Int64 IFFSeekCallback(Stream stream, long position);

	internal struct IFFIOCallbacks
	{
		public IFFOpenCallback Open;
		public IFFCloseCallback Close;
		public IFFReadCallback Read;
		public IFFWriteCallback Write;
		public IFFSeekCallback Seek;
	}
}
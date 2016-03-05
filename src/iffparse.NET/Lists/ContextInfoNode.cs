using System;

namespace Net.Iffparse
{
	public class ContextInfoNode : MinNode
	{
		private bool disposed = false;

		public ContextInfoNode(UInt32 id, UInt32 type, UInt32 identifier, UInt64 dataSize, IFFCallBack callback)
		{
			this.Id = id;
			this.Type = type;
			this.Identifier = identifier;
			this.PurgeCallBack = callback;

			if (dataSize > 0) {
				this.Data = new byte[dataSize];
				this.DataSize = dataSize;
			}
			else {
				this.Data = null;
			}
		}

		public UInt64 DataSize {
			get;
			internal set;
		}

		public byte[] Data {
			get;
			internal set;
		}

		public UInt32 Id {
			get;
			internal set;
		}

		public UInt32 Type {
			get;
			internal set;
		}

		public UInt32 Identifier {
			get;
			internal set;
		}

		internal IFFCallBack PurgeCallBack {
			get;
			set;
		}

        internal ChunkHandler ChunkHandler
        {
            get;
            set;
        }

		protected override void Dispose(bool disposing)
		{
			if (!disposed) {
				if (disposing) {
					Data = null;
					PurgeCallBack = null;
				}
				disposed = true;
			}
			base.Dispose(disposing);
		}

		~ContextInfoNode()
		{
			Dispose(false);
		}
	}
}


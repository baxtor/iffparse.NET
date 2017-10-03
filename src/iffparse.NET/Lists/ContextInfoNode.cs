using System;
using IffParse.Parser;

namespace IffParse.Lists
{
	/// <summary>
	/// Context info node.
	/// </summary>
	public class ContextInfoNode : MinNode
	{
		private bool disposed = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:IffParse.Lists.ContextInfoNode"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="type">Type.</param>
		/// <param name="identifier">Identifier.</param>
		/// <param name="dataSize">Data size.</param>
		/// <param name="callback">Callback.</param>
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

		/// <summary>
		/// Gets or sets the size of the data.
		/// </summary>
		/// <value>The size of the data.</value>
		public UInt64 DataSize {
			get;
			internal set;
		}

		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		/// <value>The data.</value>
		public byte[] Data {
			get;
			internal set;
		}

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public UInt32 Id {
			get;
			internal set;
		}

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		public UInt32 Type {
			get;
			internal set;
		}

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
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

		/// <summary>
		/// Dispose the specified <see cref="ContextInfoNode"/>.
		/// </summary>
		/// <returns>The dispose.</returns>
		/// <param name="disposing">If set to <c>true</c> disposing.</param>
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

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="T:IffParse.Lists.ContextInfoNode"/> is reclaimed by garbage collection.
		/// </summary>
		~ContextInfoNode()
		{
			Dispose(false);
		}
	}
}


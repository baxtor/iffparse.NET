using System;
using IffParse.Util;

namespace IffParse.Lists
{
	/// <summary>
	/// Context node.
	/// </summary>
	public class ContextNode : MinNode
	{
		private List<ContextInfoNode> contextInfoNodes;
		private bool disposed = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:IffParse.Lists.ContextNode"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="type">Type.</param>
		public ContextNode(UInt32 id, UInt32 type)
		{
			Id = id;
			Type = type;
			contextInfoNodes = new List<ContextInfoNode>();
		}

		/// <summary>
		/// Gets or sets the chunk identifier.
		/// </summary>
		/// <value>The chunk identifier.</value>
		public UInt32 Id {
			get;
			internal set;
		}

		internal string IdString {
			get {
				return IdUtility.IdToString(Id);
			}
		}

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		public UInt32 Type {
			get;
			internal set;
		}

		internal string TypeString {
			get {
				return IdUtility.IdToString(Type);
			}
		}

		/// <summary>
		/// Gets or sets the chunk size.
		/// </summary>
		/// <value>The size.</value>
		public UInt64 Size {
			get;
			internal set;
		}

		/// <summary>
		/// Gets or sets the offset.
		/// </summary>
		/// <value>The offset.</value>
		public UInt64 Offset {
			get;
			internal set;
		}

		/// <summary>
		/// Gets or sets the size of the current.
		/// </summary>
		/// <value>The size of the current.</value>
		public UInt64 CurrentSize {
			get;
			internal set;
		}

		/// <summary>
		/// Gets the context info nodes.
		/// </summary>
		/// <value>The context info nodes.</value>
		public List<ContextInfoNode> ContextInfoNodes {
			get {
				return contextInfoNodes;
			}
		}

		/// <summary>
		/// Dispose the specified <see cref="ContextNode"/>.
		/// </summary>
		/// <param name="disposing">If set to <c>true</c> disposing.</param>
		protected override void Dispose(bool disposing)
		{
			if (!disposed) {
				if (disposing) {
					while (!contextInfoNodes.IsEmpty) {
						var node = contextInfoNodes.RemoveHead();
						node.Dispose();
						node = null;
					}
				}
				disposed = true;
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="T:IffParse.Lists.ContextNode"/> is reclaimed by garbage collection.
		/// </summary>
		~ContextNode()
		{
			Dispose(false);
		}
	}
}
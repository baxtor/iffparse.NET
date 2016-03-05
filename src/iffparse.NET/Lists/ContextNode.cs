using System;

namespace Net.Iffparse
{
	public class ContextNode : MinNode
	{
		private List<ContextInfoNode> contextInfoNodes;
		private bool disposed = false;

		public ContextNode(UInt32 id, UInt32 type)
		{
			Id = id;
			Type = type;
			contextInfoNodes = new List<ContextInfoNode>();
		}

		public UInt32 Id {
			get;
			internal set;
		}

		internal string IdString {
			get {
				return IFFUtility.IdToString(Id);
			}
		}

		public UInt32 Type {
			get;
			internal set;
		}

		internal string TypeString {
			get {
				return IFFUtility.IdToString(Type);
			}
		}

		public UInt64 Size {
			get;
			internal set;
		}

		public UInt64 Offset {
			get;
			internal set;
		}

		public UInt64 CurrentSize {
			get;
			internal set;
		}

		public List<ContextInfoNode> ContextInfoNodes {
			get {
				return contextInfoNodes;
			}
		}

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

		~ContextNode()
		{
			Dispose(false);
		}
	}
}
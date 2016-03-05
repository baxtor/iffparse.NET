using System;

namespace Net.Iffparse
{
	public class Node : MinNode
	{
		private bool disposed = false;

		/// <summary>
		/// Gets or sets the node type.
		/// </summary>
		/// <remarks>Node type.</remarks>
		/// <value>The node type.</value>
		public Byte Type {
			get;
			set;
		}

		public SByte Pri {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the node name.
		/// </summary>
		/// <value>The node name.</value>
		public string Name {
			get;
			set;
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposed) {
				if (disposing) {
					Name = null;
				}
				disposed = true;
			}

			base.Dispose(disposing);
		}

		~Node() 
		{
			Dispose(false);
		}
	}
}


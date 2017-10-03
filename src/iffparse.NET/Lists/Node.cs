using System;

namespace IffParse.Lists
{
	/// <summary>
	/// Node.
	/// </summary>
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

		/// <summary>
		/// Gets or sets the pri.
		/// </summary>
		/// <value>The pri.</value>
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

		/// <summary>
		/// Dispose the specified <see cref="Node"/>.
		/// </summary>
		/// <returns>The dispose.</returns>
		/// <param name="disposing">If set to <c>true</c> disposing.</param>
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

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the <see cref="T:IffParse.Lists.Node"/> is
		/// reclaimed by garbage collection.
		/// </summary>
		~Node() 
		{
			Dispose(false);
		}
	}
}


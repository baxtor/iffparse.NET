using System;

namespace IffParse.Lists
{
	/// <summary>
	/// Minimum node.
	/// </summary>
	public class MinNode : IDisposable
	{
		private bool disposed = false;

		/// <summary>
		/// Gets or sets the predecessor node.
		/// </summary>
		/// <remarks>The previous node.</remarks>
		/// <value>The predecessor.</value>
		internal MinNode Predecessor {
			get;
		    set;
		}
		/// <summary>
		/// Gets or sets the successor node.
		/// </summary>
		/// <remarks>The next node.</remarks>
		/// <value>The successor.</value>
		internal MinNode Successor {
			get;
			set;
		}

		internal static T Remove<T>(T node) where T : MinNode
		{
			var previousNode = node.Predecessor;
			previousNode.Successor = node.Successor;
			previousNode.Successor.Predecessor = node.Predecessor;
			return node;
		}

		#region IDisposable implementation

		/// <summary>
		/// Releases all resource used by the <see cref="T:Net.Iffparse.MinNode"/> object.
		/// </summary>
		/// <remarks>Call <see cref="M:net.iffparse.MinNode.Dispose"/> when you are finished using the <see cref="T:Net.Iffparse.MinNode"/>. The
		/// <see cref="M:net.iffparse.MinNode.Dispose"/> method leaves the <see cref="T:Net.Iffparse.MinNode"/> in an unusable state. After calling
		/// <see cref="M:net.iffparse.MinNode.Dispose"/>, you must release all references to the <see cref="T:Net.Iffparse.MinNode"/> so the garbage
		/// collector can reclaim the memory that the <see cref="T:Net.Iffparse.MinNode"/> was occupying.</remarks>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Dispose the specified <see cref="MinNode"/>.
		/// </summary>
		/// <returns>The dispose.</returns>
		/// <param name="disposing">If set to <c>true</c> disposing.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposed) {
				if (disposing) {
//					if (Predecessor != null) {
//						Predecessor.Dispose();
//					}
//					if (Successor != null) {
//						Successor.Dispose();
//					}
				}
				disposed = true;
			}
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the <see cref="T:IffParse.Lists.MinNode"/>
		/// is reclaimed by garbage collection.
		/// </summary>
		~MinNode() 
		{
			Dispose(false);
		}

		#endregion
	}
}

using System;

namespace Net.Iffparse
{
	public class MinNode : IDisposable
	{
		private bool disposed = false;

		/// <summary>
		/// Gets or sets the predecessor.
		/// </summary>
		/// <remarks>The previous node.</remarks>
		/// <value>The predecessor.</value>
		internal MinNode Predecessor {
			get;
		    set;
		}
		/// <summary>
		/// Gets or sets the successor.
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

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

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

		~MinNode() 
		{
			Dispose(false);
		}

		#endregion
	}
}

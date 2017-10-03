using System;
using System.Collections;
using System.Collections.Generic;

namespace IffParse.Lists
{
	/// <summary>
	/// Generic linked list.
	/// </summary>
	public class List<T> 
		: IEnumerable<T>, ICollection
		where T : MinNode, IDisposable
	{
		private int count;
		private T head;
		private T tail;
		private object _syncRoot;

		/// <summary>
		/// Gets the list head node.
		/// </summary>
		/// <value>The list head node.</value>
		public T Head {
			get {
				return head;
			}
		}

		/// <summary>
		/// Gets the list tail node.
		/// </summary>
		/// <value>The tail.</value>
		public T Tail {
			get {
				return tail;
			}
		}

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		public Byte Type {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the pad.
		/// </summary>
		/// <value>The pad.</value>
		public Byte Pad {
			get;
			set;
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:IffParse.Lists.List`1"/> is empty.
		/// </summary>
		/// <value><c>true</c> if is empty; otherwise, <c>false</c>.</value>
		public Boolean IsEmpty {
			get {
				return count == 0;
			}
		}

		private void AddFirst(T node)
		{
			head = node;
			tail = head;
			head.Successor = tail;
			head.Predecessor = tail;
		}

		/// <summary>
		/// Adds a node to the head.
		/// </summary>
		/// <param name="node">Node.</param>
		public void AddHead(T node)
		{
			if (head == null)
				AddFirst(node);
			else
			{
				head.Predecessor = node;
				node.Predecessor = tail;
				node.Successor = head;
				tail.Successor = node;
				head = node;
			}
			++count;
		}

		/// <summary>
		/// Adds a node to the tail.
		/// </summary>
		/// <param name="node">Node.</param>
		public void AddTail(T node)
		{
			if (head == null)
				AddFirst(node);
			else
			{
				tail.Successor = node;
				node.Successor = head;
				node.Predecessor = tail;
				tail = node;
				head.Predecessor = tail;
			}
			++count;
		}

		/// <summary>
		/// Removes the head node.
		/// </summary>
		/// <returns>The removed head node.</returns>
		public T RemoveHead()
		{
			return Remove(head);
		}

		/// <summary>
		/// Remove the specified node.
		/// </summary>
		/// <returns>The removed node.</returns>
		/// <param name="node">Node.</param>
		public T Remove(T node)
		{
			var previousNode = node.Predecessor;
			previousNode.Successor = node.Successor;
			previousNode.Successor.Predecessor = node.Predecessor;

			if (head == node) {
				head = (T)node.Successor;
			} else if (tail == node) {
				tail = (T)tail.Predecessor;
			}
			--count;
			return node;
		}

		/// <summary>
		/// Removes the tail node.
		/// </summary>
		/// <returns>The tail node.</returns>
		public T RemoveTail()
		{
			return Remove(tail);
		}

		/// <summary>
		/// Copies the list content to array.
		/// </summary>
		/// <param name="array">Destination array.</param>
		/// <param name="index">Beginning array index.</param>
		public void CopyTo(Array array, int index)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			if (index < 0 || index > array.Length)
				throw new ArgumentOutOfRangeException("index");

			var node = head;
			do
			{
				array.SetValue(node.Successor,index);
				array.SetValue(node.Predecessor,index);
				index++;
				node = (T)node.Successor;
			} while (node != head);
		}

		/// <summary>
		/// Gets the list node count.
		/// </summary>
		/// <value>The list node count.</value>
		public int Count {
			get {
				return count;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:IffParse.Lists.List`1"/> is synchronized.
		/// </summary>
		/// <value><c>true</c> if is synchronized; otherwise, <c>false</c>.</value>
		public bool IsSynchronized {
			get {
				return false;
			}
		}

		/// <summary>
		/// Gets the sync root.
		/// </summary>
		/// <value>The sync root.</value>
		public object SyncRoot {
			get {
				if( _syncRoot == null) {
					System.Threading.Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);    
				}
				return _syncRoot;
			}
		}

		#region IEnumerable implementation

		/// <summary>
		/// System.s the collections. IE numerable. get enumerator.
		/// </summary>
		/// <returns>The collections. IE numerable. get enumerator.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region IEnumerable<T> implementation

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<T> GetEnumerator (){
            var node = head;
            while (node != null)
            {
                yield return node;
                node = (T)(node.Successor != head ? node.Successor : null);
            }
            //for (var node = this.head; node.Successor != null; node = (T)node.Successor) {
            //    yield return node;
            //}
		}

		#endregion
	}
}


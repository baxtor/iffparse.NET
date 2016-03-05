using System;
using System.Collections;
using System.Collections.Generic;

namespace Net.Iffparse
{
	public class List<T> 
		: IEnumerable<T>, ICollection
		where T : MinNode, IDisposable
	{
		private int count;
		private T head;
		private T tail;
		private object _syncRoot;

		public T Head {
			get {
				return head;
			}
		}

		public T Tail {
			get {
				return tail;
			}
		}

		public Byte Type {
			get;
			set;
		}

		public Byte Pad {
			get;
			set;
		}

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

		public T RemoveHead()
		{
			return Remove(head);
		}

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

		public T RemoveTail()
		{
			return Remove(tail);
		}

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

		public int Count {
			get {
				return count;
			}
		}

		public bool IsSynchronized {
			get {
				return false;
			}
		}

		public object SyncRoot {
			get {
				if( _syncRoot == null) {
					System.Threading.Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);    
				}
				return _syncRoot;
			}
		}

		#region IEnumerable implementation

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region IEnumerable<T> implementation

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


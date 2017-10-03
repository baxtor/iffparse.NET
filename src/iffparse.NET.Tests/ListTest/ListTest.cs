using NUnit.Framework;
using System;
using IffParse.Lists;

namespace ListTest
{
	[TestFixture()]
	public class ListTest
	{
		[Test()]
		public void CreateList()
		{
			var list = new List<Node>();
			var node1 = new Node {
				Name = "Node 1"
			};
			var node2 = new Node {
				Name = "Node 2"
			};
			var node3 = new Node {
				Name = "Node 3"
			};
			list.AddHead(node1);
			list.AddTail(node2);
			list.AddTail(node3);

			Assert.AreEqual(list.Head, node1);
			Assert.AreEqual(list.Head.Successor, node2);
			Assert.AreEqual(list.Tail, node3);
		}
	}
}


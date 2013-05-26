﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace JeuDuMoulin
{
	public enum Occupation
	{
		None,
		Player1,
		Player2,
	}

	public class Node
	{
		public int Id { get; private set; }
		public HashSet<Node> Neighbors { get; private set; }
		public Occupation Occupation { get; set; }
		/// <summary>
		/// Graphical representation
		/// </summary>
		public Point RelativeLocation { get; private set; }

		public Node(int id, int x, int y)
		{
			this.Id = id;
			this.RelativeLocation = new Point(x, y);
			this.Neighbors = new HashSet<Node>();
		}

		public void LinkTo(Node b)
		{
			this.Neighbors.Add(b);
			b.Neighbors.Add(this);
		}

		public Point GetAbsoluteLocation(Point origin, int coef)
		{
			return new Point(origin.X + this.RelativeLocation.X * coef, origin.Y + this.RelativeLocation.Y * coef);
		}

		public IEnumerable<Tuple<Point, Point>> GetAbsoluteEdges(Point origin, int coef)
		{
			var thisLocation = this.GetAbsoluteLocation(origin, coef);
			foreach (var item in this.Neighbors)
			{
				yield return new Tuple<Point, Point>(thisLocation, item.GetAbsoluteLocation(origin, coef));
			}
		}

	}

	public class Graph
	{
		public Node SelectedNode { get; set; }
		public HashSet<Node> Nodes { get; private set; }
		public Graph()
		{
			this.Nodes = new HashSet<Node>();
			//small rectangle
			var node1 = new Node(1, -1, 1);
			var node2 = new Node(2, 0, 1);
			var node3 = new Node(3, 1, 1);
			var node4 = new Node(4, 1, 0);
			var node5 = new Node(5, 1, -1);
			var node6 = new Node(6, 0, -1);
			var node7 = new Node(7, -1, -1);
			var node8 = new Node(8, -1, 0);
			//medium
			var node10 = new Node(10, -2, 2);
			var node20 = new Node(20, 0, 2);
			var node30 = new Node(30, 2, 2);
			var node40 = new Node(40, 2, 0);
			var node50 = new Node(50, 2, -2);
			var node60 = new Node(60, 0, -2);
			var node70 = new Node(70, -2, -2);
			var node80 = new Node(80, -2, 0);
			//large
			var node100 = new Node(100, -3, 3);
			var node200 = new Node(200, 0, 3);
			var node300 = new Node(300, 3, 3);
			var node400 = new Node(400, 3, 0);
			var node500 = new Node(500, 3, -3);
			var node600 = new Node(600, 0, -3);
			var node700 = new Node(700, -3, -3);
			var node800 = new Node(800, -3, 0);

			node100.LinkTo(node200);
			node200.LinkTo(node300);
			node300.LinkTo(node400);
			node400.LinkTo(node500);
			node500.LinkTo(node600);
			node600.LinkTo(node700);
			node700.LinkTo(node800);
			node800.LinkTo(node100);

			node10.LinkTo(node20);
			node20.LinkTo(node30);
			node30.LinkTo(node40);
			node40.LinkTo(node50);
			node50.LinkTo(node60);
			node60.LinkTo(node70);
			node70.LinkTo(node80);
			node80.LinkTo(node10);

			node1.LinkTo(node2);
			node2.LinkTo(node3);
			node3.LinkTo(node4);
			node4.LinkTo(node5);
			node5.LinkTo(node6);
			node6.LinkTo(node7);
			node7.LinkTo(node8);
			node8.LinkTo(node1);

			node200.LinkTo(node20);
			node20.LinkTo(node2);

			node400.LinkTo(node40);
			node40.LinkTo(node4);

			node600.LinkTo(node60);
			node60.LinkTo(node6);

			node800.LinkTo(node80);
			node80.LinkTo(node8);

			Nodes.Add(node1);
			Nodes.Add(node2);
			Nodes.Add(node3);
			Nodes.Add(node4);
			Nodes.Add(node5);
			Nodes.Add(node6);
			Nodes.Add(node7);
			Nodes.Add(node8);
			Nodes.Add(node10);
			Nodes.Add(node20);
			Nodes.Add(node30);
			Nodes.Add(node40);
			Nodes.Add(node50);
			Nodes.Add(node60);
			Nodes.Add(node70);
			Nodes.Add(node80);
			Nodes.Add(node100);
			Nodes.Add(node200);
			Nodes.Add(node300);
			Nodes.Add(node400);
			Nodes.Add(node500);
			Nodes.Add(node600);
			Nodes.Add(node700);
			Nodes.Add(node800);
		}

		public IEnumerable<Tuple<Point, Point>> GetAllEdges(Point origin, int coef)
		{
			var result = new HashSet<Tuple<Point, Point>>();
			//TODO: change algorithm to avoid returning a segment more than once
			foreach (var node in this.Nodes)
			{
				foreach (var segment in node.GetAbsoluteEdges(origin, coef))
				{
					result.Add(segment);
				}
			}
			return result;
		}

		/// <param name="toTest">mouse location</param>
		/// <returns>a node, or null</returns>
		public Node MapPointToNode(Point toTest, Point origin, int coef)
		{
			//absX = originX + pointX * coef
			//(absX - originX)/coef = pointX
			Point guess = new Point((int)Math.Round(((toTest.X - origin.X) / (double)coef)), (int)Math.Round((toTest.Y - origin.Y) / (double)coef));
			var correspondingNode = Nodes.Where(x => x.RelativeLocation == guess).FirstOrDefault();
			return correspondingNode;
		}

	}
}

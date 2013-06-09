using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;

namespace JeuDuMoulin
{

	public class Node
	{
		public int Id { get; private set; }
		public HashSet<Node> Neighbors { get; private set; }
		public IPlayer Owner { get; set; }
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

		public override string ToString()
		{
			return string.Format("{0}({1})", Id, Owner == null ? "empty" : Owner.ToString());
		}

	}

	public static class Graph
	{

		/// <summary>
		/// test if a player can move any of their pawns. otherwise, the other player have won.
		/// </summary>
		public static bool CanPlayerMove(IEnumerable<Node> nodes, IPlayer player)
		{
			foreach (var node in nodes.Where(x => x.Owner == player))
			{
				if (node.Neighbors.Any(x => x.Owner == null))
				{
					return true;
				}
			}
			return false;
		}

		/// <param name="oldPlace">if we are checking a moving pawn</param>
		public static bool IsCreatingAMill(Node newPlace, IPlayer checkForPlayer, Node oldPlace = null)
		{

			int resultX = CheckMill(newPlace, true, checkForPlayer, newPlace, oldPlace);
			if (resultX == 3)
			{
#if DEBUG
				//Logging.Log("Mill on X for {0}", checkForPlayer);
#endif
				return true;
			}
			int resultY = CheckMill(newPlace, false, checkForPlayer, newPlace, oldPlace);
			if (resultY == 3)
			{
#if DEBUG
				//Logging.Log("Mill on Y for {0}", checkForPlayer);
#endif
				return true;
			}
			return false;
		}
		/// <summary>
		/// retourne 3 si un moulin va être créé.
		/// </summary>
		/// <remarks>
		/// cherche récursivement les noeuds voisins qui ont une valeur commune (soit x soit y suivant checkAbscisse)
		/// </remarks>
		private static int CheckMill(Node currentNode, bool checkAbscisse, IPlayer player, Node newPlace, Node oldPlace = null, Node previousNode = null)
		{
			if (currentNode.Owner != player && currentNode != newPlace)
			{
				return 0;
			}
			int compteur = 0;
			int currentValue = 0;
			int previousValue = 0;
			switch (checkAbscisse)
			{
				case true:
					currentValue = currentNode.RelativeLocation.X;
					previousValue = previousNode == null ? 42 : previousNode.RelativeLocation.X;
					break;
				case false:
					currentValue = currentNode.RelativeLocation.Y;
					previousValue = previousNode == null ? 42 : previousNode.RelativeLocation.Y;
					break;
			}
			if (previousNode == null)
			{
				//first loop
				compteur++;
			}
			else
			{
				if (currentValue == previousValue)
				{
					compteur++;
				}
				else
				{
					return compteur;
				}
			}
			foreach (var node in currentNode.Neighbors.Where(x => x != previousNode && x != oldPlace && x.Owner == player))
			{
				compteur += CheckMill(node, checkAbscisse, player, newPlace, oldPlace, currentNode);
			}
			return compteur;
		}

		public static HashSet<Node> CreateGraph()
		{
			var result = new HashSet<Node>();
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

			result.Add(node1);
			result.Add(node2);
			result.Add(node3);
			result.Add(node4);
			result.Add(node5);
			result.Add(node6);
			result.Add(node7);
			result.Add(node8);
			result.Add(node10);
			result.Add(node20);
			result.Add(node30);
			result.Add(node40);
			result.Add(node50);
			result.Add(node60);
			result.Add(node70);
			result.Add(node80);
			result.Add(node100);
			result.Add(node200);
			result.Add(node300);
			result.Add(node400);
			result.Add(node500);
			result.Add(node600);
			result.Add(node700);
			result.Add(node800);

			return result;
		}

	}
}

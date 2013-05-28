using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace JeuDuMoulin
{
	public partial class Plateau : UserControl, IPlayer
	{

		public Game Game { get; private set; }
		public Game.PlayerControl Control { get; private set; }
		private Guid currentToken;

		private CalledFunction currentAction = CalledFunction.None;

		#region Graphic representation
		Point Origin;
		const int SpacingCoef = 65;
		Node SelectedNode;
		const int PieRadius = 10;
		Color background = Color.White;
		Pen defaultPen = new Pen(Color.Black, 2);
		Pen selectionPen = new Pen(Color.Yellow, 2);
		Brush player1Brush = new SolidBrush(Color.Orange);
		Brush player2Brush = new SolidBrush(Color.Cyan);
		#endregion

		public event EventHandler GraphicRefresh;

		private void OnGraphicRefresh()
		{
			var handler = GraphicRefresh;
			if (handler != null)
			{
				handler(this, EventArgs.Empty);
			}
		}

		public void Initialize(Game game)
		{
			this.Game = game;
			this.Control = new Game.PlayerControl(this);
			this.Invalidate();

			//graphic debug
			//Random r = new Random(42);
			//foreach (var node in Game.Board.Nodes)
			//{
			//    node.Occupation = (Occupation)r.Next(3);
			//}
			//SelectedNode = Game.Board.Nodes.ElementAt(3);
		}

		public Plateau()
		{
			InitializeComponent();
		}

		private void Plateau_Paint(object sender, PaintEventArgs e)
		{
			if (this.Game == null)
			{
				//not yet initialized
				e.Graphics.Clear(Color.Purple);
			}
			else
			{
				DrawPlateau(e.Graphics);
			}
		}

		private void DrawPlateau(Graphics g)
		{
			g.Clear(background);
			g.DrawString(currentAction.ToString(), new Font(FontFamily.GenericSansSerif, 12), new SolidBrush(Color.Blue), 10, 10);
			foreach (var segment in GetAllEdges(Game.Board, Origin, SpacingCoef))
			{
				g.DrawLine(defaultPen, segment.Item1, segment.Item2);
			}
			foreach (var node in Game.Board)
			{
				Point nodeCenter = node.GetAbsoluteLocation(Origin, SpacingCoef);
#if DEBUG
				//g.DrawString(string.Format("{0}: {1},{2}", node.Id, node.RelativeLocation.X, node.RelativeLocation.Y),
				//    new Font(FontFamily.GenericSansSerif, 10), new SolidBrush(Color.Red), nodeCenter/* new Point(nodeCenter.X + PieRadius, nodeCenter.Y + PieRadius)*/);
				g.DrawString(node.Id.ToString(), new Font(FontFamily.GenericSansSerif, 10), new SolidBrush(Color.Red), new Point(nodeCenter.X + PieRadius / 2, nodeCenter.Y + PieRadius / 2));
#endif
				if (node.Owner == null)
				{
					continue;
				}
				if (node.Owner == Game.Player1)
				{
					g.FillEllipse(player1Brush, nodeCenter.X - PieRadius, nodeCenter.Y - PieRadius, PieRadius * 2, PieRadius * 2);
					g.DrawEllipse(defaultPen, nodeCenter.X - PieRadius, nodeCenter.Y - PieRadius, PieRadius * 2, PieRadius * 2);
				}
				else if (node.Owner == Game.Player2)
				{
					g.FillEllipse(player2Brush, nodeCenter.X - PieRadius, nodeCenter.Y - PieRadius, PieRadius * 2, PieRadius * 2);
					g.DrawEllipse(defaultPen, nodeCenter.X - PieRadius, nodeCenter.Y - PieRadius, PieRadius * 2, PieRadius * 2);
				}
				if (SelectedNode == node)
				{
					g.DrawEllipse(selectionPen, nodeCenter.X - PieRadius, nodeCenter.Y - PieRadius, PieRadius * 2, PieRadius * 2);
				}
			}

			//origin
			//g.FillPie(new SolidBrush(Color.Gray), Origin.X - PieRadius, Origin.Y - PieRadius, PieRadius * 2, PieRadius * 2, 0, 360);
		}

		private IEnumerable<Tuple<Point, Point>> GetAllEdges(IEnumerable<Node> nodes, Point origin, int coef)
		{
			var result = new HashSet<Tuple<Point, Point>>();
			//TODO: change algorithm to avoid returning a segment more than once
			foreach (var node in nodes)
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
		private Node MapPointToNode(IEnumerable<Node> nodes, Point toTest, Point origin, int coef)
		{
			//absX = originX + pointX * coef
			//(absX - originX)/coef = pointX
			Point guess = new Point((int)Math.Round(((toTest.X - origin.X) / (double)coef)), (int)Math.Round((toTest.Y - origin.Y) / (double)coef));
			var correspondingNode = nodes.Where(x => x.RelativeLocation == guess).FirstOrDefault();
			return correspondingNode;
		}

		private void Plateau_MouseClick(object sender, MouseEventArgs e)
		{
			if (!Game.TurnHandler.IsMyTurn(currentToken))
			{
				return;
			}
			var clickedNode = MapPointToNode(Game.Board, e.Location, Origin, SpacingCoef);
			if (clickedNode != null)
			{
#if DEBUG
				Console.WriteLine("clicked on " + clickedNode.Id.ToString());
#endif
				if (e.Button == System.Windows.Forms.MouseButtons.Left)
				{
					//normal selection
					if (clickedNode.Owner != null)
					{
						if (currentAction == CalledFunction.RemoveOpponentPawn)
						{
							if (clickedNode.Owner != null && clickedNode.Owner != this)
							{
								Control.RemoveOpponentPawn(currentToken, clickedNode);
								currentAction = CalledFunction.None;
								this.Invalidate();
								OnGraphicRefresh();
							}
						}
						if (currentAction == CalledFunction.MovePawnConstrained || currentAction == CalledFunction.MovePawnFreely)
						{
							if (SelectedNode != clickedNode)
							{
								if (clickedNode.Owner == this)
								{
									SelectedNode = clickedNode;
									this.Invalidate();
								}
							}
						}
					}
					else //empty node
					{
						if (currentAction == CalledFunction.PlacePawn)
						{
							Control.PlacePawn(currentToken, clickedNode);
							currentAction = CalledFunction.None;
							this.Invalidate();
							OnGraphicRefresh();
						}
					}
				}
				else if (e.Button == System.Windows.Forms.MouseButtons.Right)
				{
					if (currentAction == CalledFunction.MovePawnConstrained || currentAction == CalledFunction.MovePawnFreely)
					{
						//move the pawn
						if (SelectedNode != null &&
							clickedNode.Owner == null &&
							SelectedNode != clickedNode)
						{
							if (currentAction == CalledFunction.MovePawnConstrained)
							{
								if (SelectedNode.Neighbors.Contains(clickedNode))
								{
									Control.MovePawnConstrained(currentToken, SelectedNode, clickedNode);
									currentAction = CalledFunction.None;
									SelectedNode = null;
									this.Invalidate();
									OnGraphicRefresh();
								}
							}
							else if (currentAction == CalledFunction.MovePawnFreely)
							{
								Control.MovePawnFreely(currentToken, SelectedNode, clickedNode);
								currentAction = CalledFunction.None;
								SelectedNode = null;
								this.Invalidate();
								OnGraphicRefresh();
							}
						}
					}
				}
			}
			else
			{
#if DEBUG
				Console.WriteLine("clicked on nothing");
#endif
			}
		}

		private void Plateau_Load(object sender, EventArgs e)
		{
			Origin = new Point(this.Width / 2, this.Height / 2);
		}

		public void PlacePawn(Guid token)
		{
			currentAction = CalledFunction.PlacePawn;
			this.currentToken = token;
			this.Invalidate();
		}

		public void RemoveOpponentPawn(Guid token)
		{
			currentAction = CalledFunction.RemoveOpponentPawn;
			this.currentToken = token;
			this.Invalidate();
		}

		public void MovePawnConstrained(Guid token)
		{
			currentAction = CalledFunction.MovePawnConstrained;
			this.currentToken = token;
			this.Invalidate();
		}

		public void MovePawnFreely(Guid token)
		{
			currentAction = CalledFunction.MovePawnFreely;
			this.currentToken = token;
			this.Invalidate();
		}

		public override string ToString()
		{
			return Game.Player1 == this ? "Player1" : "Player2";
		}

		private enum CalledFunction
		{
			None,
			PlacePawn,
			RemoveOpponentPawn,
			MovePawnConstrained,
			MovePawnFreely,
		}

	}

}

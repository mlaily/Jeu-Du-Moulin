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
		public int PawnCount { get; set; }
		private Guid currentToken;

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
			this.Invalidate();

			//graphic debug
			Random r = new Random(42);
			foreach (var node in Game.Board.Nodes)
			{
				node.Occupation = (Occupation)r.Next(3);
			}
			SelectedNode = Game.Board.Nodes.ElementAt(3);
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
			foreach (var segment in Game.Board.GetAllEdges(Origin, SpacingCoef))
			{
				g.DrawLine(defaultPen, segment.Item1, segment.Item2);
			}
			foreach (var node in Game.Board.Nodes)
			{
				Point nodeCenter = node.GetAbsoluteLocation(Origin, SpacingCoef);
				switch (node.Occupation)
				{
					case Occupation.Player1:
						g.FillEllipse(player1Brush, nodeCenter.X - PieRadius, nodeCenter.Y - PieRadius, PieRadius * 2, PieRadius * 2);
						g.DrawEllipse(defaultPen, nodeCenter.X - PieRadius, nodeCenter.Y - PieRadius, PieRadius * 2, PieRadius * 2);
						break;
					case Occupation.Player2:
						g.FillEllipse(player2Brush, nodeCenter.X - PieRadius, nodeCenter.Y - PieRadius, PieRadius * 2, PieRadius * 2);
						g.DrawEllipse(defaultPen, nodeCenter.X - PieRadius, nodeCenter.Y - PieRadius, PieRadius * 2, PieRadius * 2);
						break;
					case Occupation.None:
					default:
						break;
				}
				if (node.Occupation != Occupation.None && SelectedNode == node)
				{
					g.DrawEllipse(selectionPen, nodeCenter.X - PieRadius, nodeCenter.Y - PieRadius, PieRadius * 2, PieRadius * 2);
				}
#if DEBUG
				g.DrawString(node.Id.ToString(), new Font(FontFamily.GenericSansSerif, 10), new SolidBrush(Color.Red), nodeCenter);
#endif
			}

			//origin
			//g.FillPie(new SolidBrush(Color.Gray), Origin.X - PieRadius, Origin.Y - PieRadius, PieRadius * 2, PieRadius * 2, 0, 360);
		}

		private void Plateau_MouseClick(object sender, MouseEventArgs e)
		{
			var clickedNode = Game.Board.MapPointToNode(e.Location, Origin, SpacingCoef);
			if (clickedNode != null)
			{
#if DEBUG
				Console.WriteLine("clicked on " + clickedNode.Id.ToString());
#endif
				if (e.Button == System.Windows.Forms.MouseButtons.Left)
				{
					//normal selection
					if (clickedNode.Occupation != Occupation.None)
					{
						if (SelectedNode != clickedNode)
						{
							//todo: check if the current pawn belongs to the appropriate player...
							SelectedNode = clickedNode;
							this.Invalidate();
							OnGraphicRefresh();
						}
					}
					else
					{
						//TODO change to appropriate return value
						Game.TurnHandler.PlacePawn.Placement = clickedNode;
						this.Game.TurnHandler.EndTurn(currentToken);
					}
				}
				else if (e.Button == System.Windows.Forms.MouseButtons.Right)
				{
					//move the pawn
					if (SelectedNode != null &&
						clickedNode.Occupation == Occupation.None &&
						SelectedNode != clickedNode)
					{
						if (Game.Board.MovePawn(SelectedNode, clickedNode, SelectedNode.Occupation))
						{
							SelectedNode = clickedNode;
							this.Invalidate();
							OnGraphicRefresh();
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
			//start the turn for this player
			this.currentToken = token;
		}

	}

}

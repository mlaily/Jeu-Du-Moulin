using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JeuDuMoulin
{
	public partial class Plateau : UserControl
	{

		Point Origin;
		const int SpacingCoef = 50;
		Graph graph = new Graph();
		Node SelectedNode;

		const int PieRadius = 10;
		Color background = Color.White;
		Pen defaultPen = new Pen(Color.Black, 2);
		Pen selectionPen = new Pen(Color.Yellow, 2);
		Brush player1Brush = new SolidBrush(Color.Orange);
		Brush player2Brush = new SolidBrush(Color.Cyan);

		public Plateau()
		{
			InitializeComponent();
			Origin = new Point(this.Width / 2, this.Height / 2);
			Random r = new Random(42);
			foreach (var node in graph.Nodes)
			{
				node.Occupation = (Occupation)r.Next(3);
			}
			SelectedNode = graph.Nodes.ElementAt(3);
		}

		private void Plateau_Paint(object sender, PaintEventArgs e)
		{
			DrawPlateau(e.Graphics);
		}

		private void DrawPlateau(Graphics g)
		{
			g.Clear(background);
			foreach (var segment in graph.GetAllEdges(Origin, SpacingCoef))
			{
				g.DrawLine(defaultPen, segment.Item1, segment.Item2);
			}
			foreach (var node in graph.Nodes)
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
			var clickedNode = graph.MapPointToNode(e.Location, Origin, SpacingCoef);
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
						}
					}
				}
				else if (e.Button == System.Windows.Forms.MouseButtons.Right)
				{
					//move the pawn
					if (SelectedNode != null &&
						clickedNode.Occupation == Occupation.None &&
						SelectedNode != clickedNode)
					{
						if (graph.MovePawn(SelectedNode, clickedNode, SelectedNode.Occupation))
						{
							SelectedNode = clickedNode;
							this.Invalidate();
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
	}

}

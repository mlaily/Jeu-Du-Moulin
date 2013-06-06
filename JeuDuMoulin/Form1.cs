using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JeuDuMoulin
{
	public partial class Form1 : Form
	{

		Game game;
		IPlayer player1;
		IPlayer player2;

		public Form1()
		{
			InitializeComponent();
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			//12 42
			var plateau1 = new Human() { Location = new Point(12, 42), Size = new Size(500, 500) };
			this.Controls.Add(plateau1);
			//700 42
			var plateau2 = new Plateau() { Location = new Point(700, 42), Size = new Size(500, 500) };
			this.Controls.Add(plateau2);
			//plateau1.GraphicRefresh += (o, e) => plateau2.Invalidate();
			//plateau2.GraphicRefresh += (o, e) => plateau1.Invalidate();
			player1 = plateau1; //new RandomAI("AI1");
			player2 = new RandomAI("AI2");
			game = new Game(player1, player2);
			game.ArtificialWait = TimeSpan.FromMilliseconds(200);
			plateau2.Game = game;
			plateau2.Player = player2;
			plateau2.Invalidate();
			game.TurnEnded += (o, e2) => plateau1.Invalidate();
			game.TurnEnded += (o, e2) => plateau2.Invalidate();
			game.Start();
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			System.Diagnostics.Process.GetCurrentProcess().Kill();
		}

	}
}

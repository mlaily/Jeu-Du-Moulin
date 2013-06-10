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



		public Form1()
		{
			InitializeComponent();
			Logging.TextBoxLog = this.textBox1;
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			for (int i = this.Controls.Count - 1; i > 0; i--)
			{
				if (this.Controls[i] is Plateau)
				{
					((Plateau)this.Controls[i]).Hide();
					this.Controls.RemoveAt(i);
				}
			}
			StartAIVsAI();
			//StartHumanVsAI();
		}

		private void StartHumanVsAI()
		{
			Game game;
			IPlayer player1;
			IPlayer player2;
			//12 42
			var plateau1 = new Human() { Location = new Point(12, 42), Size = new Size(500, 500) };
			this.Controls.Add(plateau1);
			//700 42
			var plateau2 = new Plateau() { Location = new Point(700, 42), Size = new Size(500, 500) };
			this.Controls.Add(plateau2);
			player1 = plateau1; //new RandomAI("AI1");
			player2 = new MinMaxAI("AI", 3);
			game = new Game(player1, player2);
			//game.ArtificialWait = TimeSpan.FromMilliseconds(200);
			//plateau1.Game = game;
			//plateau1.Player = player1;
			//plateau1.Invalidate();
			plateau2.Game = game;
			plateau2.Player = player2;
			plateau2.Invalidate();
			game.TurnEnded += (o, e2) => plateau1.Invalidate();
			game.TurnEnded += (o, e2) => plateau2.Invalidate();
			game.Start();
		}

		private void StartAIVsAI()
		{
			Game game;
			IPlayer player1;
			IPlayer player2;
			//12 42
			var plateau1 = new Plateau() { Location = new Point(12, 42), Size = new Size(500, 500) };
			this.Controls.Add(plateau1);
			//700 42
			var plateau2 = new Plateau() { Location = new Point(700, 42), Size = new Size(500, 500) };
			this.Controls.Add(plateau2);
			player1 = new MinMaxAI("AI1", 3);
			player2 = new MinMaxAI("AI2", 3);
			game = new Game(player1, player2);
			game.ArtificialWait = TimeSpan.FromMilliseconds(500);
			plateau1.Game = game;
			plateau1.Player = player1;
			plateau1.Invalidate();
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

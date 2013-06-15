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

		private Game currentGame = null;

		private int MaxDepth
		{
			get
			{
				int casted;
				if (int.TryParse(txtMaxDepth.Text, out casted))
				{
					return casted;
				}
				else
				{
					return 3;
				}
			}
		}

		private int Delay
		{
			get
			{
				int casted;
				if (int.TryParse(txtDelay.Text, out casted))
				{
					return casted;
				}
				else
				{
					return 500;
				}
			}
		}

		public Form1()
		{
			InitializeComponent();
			Logging.TextBoxLog = this.textBox1;
		}

		private void StartHumanVsHuman()
		{
			Game game;
			IPlayer player1;
			IPlayer player2;
			//12 42
			var plateau1 = new Human() { Location = new Point(12, 42), Size = new Size(500, 500) };
			this.Controls.Add(plateau1);
			//700 42
			var plateau2 = new Human() { Location = new Point(700, 42), Size = new Size(500, 500) };
			this.Controls.Add(plateau2);
			player1 = plateau1;
			player2 = plateau2;
			game = new Game(player1, player2);
			game.TurnEnded += (o, e2) => plateau1.Invalidate();
			game.TurnEnded += (o, e2) => plateau2.Invalidate();
			currentGame = game;
			game.Start();
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
			player1 = plateau1;
			player2 = new MiniMaxAI("AI", this.MaxDepth);
			game = new Game(player1, player2);
			//game.ArtificialWait = TimeSpan.FromMilliseconds(200);
			plateau2.Game = game;
			plateau2.Player = player2;
			plateau2.Invalidate();
			game.TurnEnded += (o, e2) => plateau1.Invalidate();
			game.TurnEnded += (o, e2) => plateau2.Invalidate();
			currentGame = game;
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
			player1 = new MiniMaxAI("AI1", this.MaxDepth);
			player2 = new MiniMaxAI("AI2", this.MaxDepth);
			game = new Game(player1, player2);
			game.ArtificialWait = TimeSpan.FromMilliseconds(this.Delay);
			plateau1.Game = game;
			plateau1.Player = player1;
			plateau1.Invalidate();
			plateau2.Game = game;
			plateau2.Player = player2;
			plateau2.Invalidate();
			game.TurnEnded += (o, e2) => plateau1.Invalidate();
			game.TurnEnded += (o, e2) => plateau2.Invalidate();
			currentGame = game;
			game.Start();
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			System.Diagnostics.Process.GetCurrentProcess().Kill();
		}

		private void CleanGames()
		{
			if (currentGame != null)
			{
				currentGame.CancelGame = true;
			}
			for (int i = this.Controls.Count - 1; i > 0; i--)
			{
				if (this.Controls[i] is Plateau)
				{
					((Plateau)this.Controls[i]).Hide();
					this.Controls.RemoveAt(i);
				}
			}
		}

		private void humanhumanbtn_Click(object sender, EventArgs e)
		{
			CleanGames();
			StartHumanVsHuman();
		}

		private void humanaibtn_Click(object sender, EventArgs e)
		{
			CleanGames();
			StartHumanVsAI();
		}

		private void aiaibtn_Click(object sender, EventArgs e)
		{
			CleanGames();
			StartAIVsAI();
		}

	}
}

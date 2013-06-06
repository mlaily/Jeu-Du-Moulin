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
			//plateau1.GraphicRefresh += (o, e) => plateau2.Invalidate();
			//plateau2.GraphicRefresh += (o, e) => plateau1.Invalidate();
			player1 = plateau1;
			player2 = new RandomAI();
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			game = new Game(player1, player2);
			game.Start();
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			System.Diagnostics.Process.GetCurrentProcess().Kill();
		}

	}
}

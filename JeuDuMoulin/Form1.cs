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

		public Form1()
		{
			InitializeComponent();
			plateau1.GraphicRefresh += (o,e) => plateau2.Invalidate();
			plateau2.GraphicRefresh += (o, e) => plateau1.Invalidate();
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			game = new Game(this.plateau1, this.plateau2);
			game.Start();
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			LockAndReturn < IReturnValue>.ReleaseAll();
			if (game != null)
			{
				game.Abort();
			}
		}

	}
}

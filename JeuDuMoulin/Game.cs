using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace JeuDuMoulin
{
	public class Game
	{
		private bool cancelAll = false;

		public Graph Board { get; private set; }
		public int Turn { get; private set; }
		public Phase Phase { get; private set; }
		public IPlayer Player1 { get; private set; }
		public IPlayer Player2 { get; private set; }

		public Game(IPlayer player1, IPlayer player2)
		{
			Board = new Graph();
			Player1 = player1;
			Player2 = player2;
			Player1.Initialize(this);
			Player2.Initialize(this);
			Phase = JeuDuMoulin.Phase.First;
			Turn = 1;
		}

		/// <summary>
		/// à utiliser pour libérer les boucles bloquantes quand on quitte le programme
		/// </summary>
		public void Abort()
		{
			cancelAll = true;
		}

		public void Start()
		{
			System.Threading.Thread t = new Thread(GameLoop);
			t.Name = "Game Loop";
			t.Start();
		}

		private void GameLoop()
		{
			while (!cancelAll && Player1.PawnCount < 9 && Player2.PawnCount < 9)
			{
				using (var l = new Lock("Waiting for Player 1...", "Player 1 ended their turn!"))
				{
					Player1.Play(l);
				}
				using (var l = new Lock("Waiting for Player 2...", "Player 2 ended their turn!"))
				{
					Player2.Play(l);
				}
			}
			Phase = JeuDuMoulin.Phase.Second;
		}

	}

	public enum Phase
	{
		/// <summary>
		/// placement
		/// </summary>
		First,
		/// <summary>
		/// déplacement
		/// </summary>
		Second,
	}

	public class GameRuleBrokenException : Exception
	{
		public GameRuleBrokenException(string message, Exception inner = null) : base(message, inner) { }
	}
}

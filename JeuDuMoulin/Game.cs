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
			Phase = JeuDuMoulin.Phase.First;
			Turn = 1;
			while (!cancelAll/* && Player1.PawnCount < 9 && Player2.PawnCount < 9*/)
			{
				var l = new LockAndReturn<PlacePawnReturn>("Waiting for Player 1...", "Player 1 ended their turn!");
				Player1.PlacePawn(l);
				l.WaitFor();
				//using (var l = new LockAndReturn<IReturnValue>("Waiting for Player 2...", "Player 2 ended their turn!"))
				//{
				//    Player2.PlacePawn(l);
				//}
			}
			Phase = JeuDuMoulin.Phase.Second;
		}

	}

	public class PlacePawnReturn : IReturnValue
	{

	}

	public class GameToken
	{
		private ManualResetEvent m = new ManualResetEvent(false);
		public PlacePawn PlacePawn { get; private set; }
		public GameToken()
		{
			this.PlacePawn = new PlacePawn();
		}

		public void SetResponse()
		{
			m.Set();
		}

		public void WaitForResponse()
		{
			m.WaitOne();
		}

	}
	public class PlacePawn
	{
		public Node Placement { get; set; }
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

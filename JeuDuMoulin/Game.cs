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

		public HashSet<Node> Board { get; private set; }

		public int Turn { get; private set; }
		public Phase Phase { get; private set; }
		public IPlayer Player1 { get; private set; }
		public IPlayer Player2 { get; private set; }
		public TurnHandler TurnHandler { get; private set; }

		public Game(IPlayer player1, IPlayer player2)
		{
			TurnHandler = new TurnHandler();
			Board = Graph.CreateGraph();
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

		//public bool MovePawn(Node a, Node b, Occupation player = Occupation.Player1)
		//{
		//    if (a.Neighbors.Contains(b))
		//    {
		//        a.Occupation = Occupation.None;
		//        b.Occupation = player;
		//        return true;
		//    }
		//    else
		//    {
		//        return false;
		//    }
		//}

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
			while (!cancelAll /*&& Player1.PawnCount < 9 && Player2.PawnCount < 9*/)
			{
				Player1.PlacePawn(TurnHandler.NewTurn());
				TurnHandler.WaitForPlayer();
				//access data
				Console.WriteLine("Player1 played {0}", TurnHandler.PlacePawn.Placement.Id);

				Player2.PlacePawn(TurnHandler.NewTurn());
				TurnHandler.WaitForPlayer();
				//access data
				Console.WriteLine("Player2 played {0}", TurnHandler.PlacePawn.Placement.Id);
			}
			Phase = JeuDuMoulin.Phase.Second;
		}

	}

	public class TurnHandler
	{
		private ManualResetEvent m = new ManualResetEvent(false);
		private Guid currentToken;

		public PlacePawn PlacePawn { get; private set; }

		public TurnHandler()
		{
			this.PlacePawn = new PlacePawn();
		}

		public Guid NewTurn()
		{
			return currentToken = Guid.NewGuid();
		}

		public bool IsMyTurn(Guid token)
		{
			return currentToken == token;
		}

		public void EndTurn(Guid token)
		{
			if (token != currentToken)
			{
				throw new GameRuleBrokenException("Not your turn! (token does not match)");
			}
			else
			{
				m.Set();
			}
		}

		public void WaitForPlayer()
		{
			m.Reset();
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

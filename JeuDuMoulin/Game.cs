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
			while (!cancelAll && (Player1.Control.PawnsToPlace > 0 || Player2.Control.PawnsToPlace > 0))
			{
				Player1.PlacePawn(TurnHandler.NewTurn());
				TurnHandler.WaitForPlayer();

				Player2.PlacePawn(TurnHandler.NewTurn());
				TurnHandler.WaitForPlayer();
			}
			Phase = JeuDuMoulin.Phase.Second;
		}

		public class PlayerControl
		{
			//convenient reference
			private Game game;

			public int PawnsToPlace { get; private set; }
			public int PawnCount { get; private set; }
			public IPlayer Player { get; private set; }

			public PlayerControl(IPlayer player)
			{
				Player = player;
				game = player.Game;
				PawnsToPlace = 9;
				PawnCount = 0;
			}

			/// <summary>
			/// place a pawn and end the turn.
			/// </summary>
			/// <param name="token"></param>
			/// <param name="node"></param>
			/// <param name="opponentPawnToRemove">si le joueur a fait un moulin, il doit demander ici le pion adverse qu'il veut enlever.</param>
			public void PlacePawn(Guid token, Node node, Node opponentPawnToRemove = null)
			{
				if (!game.TurnHandler.IsMyTurn(token))
				{
					throw new NotYourTurnException();
				}

				if (node.Owner != null)
				{
					throw new GameRuleBrokenException("There already is a pawn there!");
				}

				bool isCreatingAMill = Graph.IsCreatingAMill(game.Board, node, Player);

				//toutes les conditions sont remplies

				node.Owner = Player;
				PawnsToPlace--;
				PawnCount++;

				if (isCreatingAMill)
				{
					if (opponentPawnToRemove == null || opponentPawnToRemove.Owner == null || opponentPawnToRemove.Owner == Player)
					{
						throw new ArgumentException("You formed a mill, you must remove a pawn from your opponent.", "opponentPawnToRemove");
					}
					//ok
					opponentPawnToRemove.Owner = null;
				}

#if DEBUG
				Console.WriteLine("{0} placed a pawn on {1}", this.Player, node.Id);
				if (isCreatingAMill) Console.WriteLine("And removed opponent pawn in {0} after creating a mill.", opponentPawnToRemove.Id);
#endif
				game.TurnHandler.EndTurn(token);
			}
		}

	}

	public class TurnHandler
	{
		private ManualResetEvent m = new ManualResetEvent(false);
		private Guid currentToken;

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
				throw new NotYourTurnException();
			}
			else
			{
				m.Set();
				currentToken = Guid.Empty;
			}
		}

		public void WaitForPlayer()
		{
			m.Reset();
			m.WaitOne();
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


	public class NotYourTurnException : Exception
	{
		public NotYourTurnException(Exception inner = null) : base("Not your turn! (token does not match)", inner) { }
	}
}

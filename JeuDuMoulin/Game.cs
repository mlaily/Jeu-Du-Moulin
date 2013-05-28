using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;

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
		public List<Step> History { get; private set; }

		public IPlayer Winner { get; private set; }
		public IPlayer Loser { get; private set; }

		public Game(IPlayer player1, IPlayer player2)
		{
			TurnHandler = new TurnHandler();
			Board = Graph.CreateGraph();
			History = new List<Step>();
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
			while (!cancelAll && (Player1.Control.PawnsToPlace > 0 || Player2.Control.PawnsToPlace > 0))
			{
				//player1
				Player1.PlacePawn(TurnHandler.NewTurn());
				if (TurnHandler.WaitForPlayer())
				{
					Player1.RemoveOpponentPawn(TurnHandler.NewTurn());
					TurnHandler.WaitForPlayer();
				}
				//player2
				Player2.PlacePawn(TurnHandler.NewTurn());
				if (TurnHandler.WaitForPlayer())
				{
					Player2.RemoveOpponentPawn(TurnHandler.NewTurn());
					TurnHandler.WaitForPlayer();
				}
			}
			Phase = JeuDuMoulin.Phase.Second;
			IPlayer currentPlayer = Player1;
			int countSinceLastTakenPawn = 0;
			//activated when both players have only 3 pawns left
			int finalCountDown = 10;
			while (true)
			{
				if (finalCountDown <= 0)
				{
					//tie
					break;
				}
				if (countSinceLastTakenPawn >= 50)
				{
					//tie
					break;
				}
				if (Player1.Control.PawnCount < 3 || !Graph.CanPlayerMove(Board, Player1))
				{
					Winner = Player2;
					Loser = Player1;
					break;
				}
				if (Player2.Control.PawnCount < 3 || !Graph.CanPlayerMove(Board, Player2))
				{
					Winner = Player1;
					Loser = Player2;
					break;
				}
				//TODO TIE GAME IF:
				//La position des pions est répétée trois fois sur le plateau.

				countSinceLastTakenPawn++;
				DoPhase2PlayerTurn(currentPlayer, ref countSinceLastTakenPawn);
				//switch player
				currentPlayer = currentPlayer == Player1 ? Player2 : Player1;

				if (Player1.Control.PawnCount == 3 && Player2.Control.PawnCount == 3)
				{
					finalCountDown--;
				}
			}
			//end of the game
			SaveHistory("history.log");
		}

		public void SaveHistory(string path)
		{
			using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write))
			using (var sw = new System.IO.StreamWriter(fs))
			{
				foreach (var item in History)
				{
					sw.WriteLine(item.ToString());
				}
			}
		}

		public void LoadHistory(string path)
		{
			History.Clear();
			using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
			using (var sr = new System.IO.StreamReader(fs))
			{
				string line;
				while ((line = sr.ReadLine()) != null)
				{
					History.Add(Step.Parse(line, Player1, Player2, Board));
				}
			}
		}

		private void DoPhase2PlayerTurn(IPlayer player, ref int countSinceLastTakenPawn)
		{
			if (player.Control.PawnCount == 3)
			{
				player.MovePawnFreely(TurnHandler.NewTurn());
				if (TurnHandler.WaitForPlayer())
				{
					player.RemoveOpponentPawn(TurnHandler.NewTurn());
					TurnHandler.WaitForPlayer();
					countSinceLastTakenPawn = 0;
				}
			}
			else
			{
				player.MovePawnConstrained(TurnHandler.NewTurn());
				if (TurnHandler.WaitForPlayer())
				{
					player.RemoveOpponentPawn(TurnHandler.NewTurn());
					TurnHandler.WaitForPlayer();
					countSinceLastTakenPawn = 0;
				}
			}
		}

		public class PlayerControl
		{
			//convenient reference
			private Game game;

			public int PawnsToPlace { get; private set; }
			public int PawnCount { get; private set; }
			public IPlayer Player { get; private set; }
			private IPlayer _Opponent;
			//lazy loading because the player are not necessarily initialized when the ctor is called
			public IPlayer Opponent
			{
				get
				{
					if (_Opponent == null)
					{
						_Opponent = game.Player1 == Player ? game.Player2 : game.Player1;
					}
					return _Opponent;
				}
			}

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
			public void PlacePawn(Guid token, Node node)
			{
				if (!game.TurnHandler.IsMyTurn(token))
				{
					throw new NotYourTurnException();
				}

				if (game.Phase != Phase.First)
				{
					throw new GameRuleBrokenException("You can only place pawns in the first phase!");
				}

				if (node.Owner != null)
				{
					throw new GameRuleBrokenException("There already is a pawn there!");
				}

				bool isCreatingAMill = Graph.IsCreatingAMill(node, Player);

				//toutes les conditions sont remplies

				node.Owner = Player;
				PawnsToPlace--;
				PawnCount++;
#if DEBUG
				Console.WriteLine("{0} placed a pawn on {1}", this.Player, node.Id);
				//if (isCreatingAMill) Console.WriteLine("And removed opponent pawn in {0} after creating a mill.", opponentPawnToRemove.Id);
#endif
				game.History.Add(new Step() { Action = StepAction.PlacePawn, Player = this.Player, NodeA = node });
				game.TurnHandler.EndTurn(token, isCreatingAMill);
			}

			public void RemoveOpponentPawn(Guid token, Node node)
			{
				if (!game.TurnHandler.IsMyTurn(token))
				{
					throw new NotYourTurnException();
				}

				if (node.Owner == null || node.Owner == Player)
				{
					throw new GameRuleBrokenException("You must select an opponent pawn to remove.");
				}

				node.Owner = null;
				Opponent.Control.PawnCount--;

				game.History.Add(new Step() { Action = StepAction.RemoveOpponentPawn, Player = this.Player, NodeA = node });
				game.TurnHandler.EndTurn(token, false);
			}

			public void MovePawnConstrained(Guid token, Node origin, Node destination)
			{
				if (!game.TurnHandler.IsMyTurn(token))
				{
					throw new NotYourTurnException();
				}

				if (origin.Owner == null || origin.Owner == Opponent)
				{
					throw new GameRuleBrokenException("You can only move one of your own pawns!");
				}

				if (destination.Owner != null)
				{
					throw new GameRuleBrokenException("You can only move your pawn to an empty node!");
				}

				if (!origin.Neighbors.Contains(destination))
				{
					throw new GameRuleBrokenException("You can only move your pawn to an adjacent node!");
				}

				bool isCreatingAMill = Graph.IsCreatingAMill(destination, Player, origin);

				//ok
				origin.Owner = null;
				destination.Owner = Player;

				game.History.Add(new Step() { Action = StepAction.MovePawnConstrained, Player = this.Player, NodeA = origin, NodeB = destination });
				game.TurnHandler.EndTurn(token, isCreatingAMill);
			}

			public void MovePawnFreely(Guid token, Node origin, Node destination)
			{
				if (!game.TurnHandler.IsMyTurn(token))
				{
					throw new NotYourTurnException();
				}

				if (origin.Owner == null || origin.Owner == Opponent)
				{
					throw new GameRuleBrokenException("You can only move one of your own pawns!");
				}

				if (destination.Owner != null)
				{
					throw new GameRuleBrokenException("You can only move your pawn to an empty node!");
				}

				bool isCreatingAMill = Graph.IsCreatingAMill(destination, Player, origin);

				//ok
				origin.Owner = null;
				destination.Owner = Player;

				game.History.Add(new Step() { Action = StepAction.MovePawnFreely, Player = this.Player, NodeA = origin, NodeB = destination });
				game.TurnHandler.EndTurn(token, isCreatingAMill);
			}
		}

	}

	public class TurnHandler
	{
		private ManualResetEvent m = new ManualResetEvent(false);
		private Guid currentToken;
		private bool returnValue;

		public Guid NewTurn()
		{
			returnValue = false;
			return currentToken = Guid.NewGuid();
		}

		public bool IsMyTurn(Guid token)
		{
			return currentToken == token;
		}

		public void EndTurn(Guid token, bool haveCreatedAMill)
		{
			if (token != currentToken)
			{
				throw new NotYourTurnException();
			}
			else
			{
				returnValue = haveCreatedAMill;
				currentToken = Guid.Empty;
				m.Set();
			}
		}

		/// <summary>
		/// retourne true si le joueur a créé un moulin et peut donc supprimer un pion adverse
		/// </summary>
		public bool WaitForPlayer()
		{
			m.Reset();
			m.WaitOne();
			return returnValue;
		}

	}

	public class Step
	{
		public StepAction Action { get; set; }
		public IPlayer Player { get; set; }
		public Node NodeA { get; set; }
		public Node NodeB { get; set; }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0}: {1}", Player, Action);
			if (Action == StepAction.MovePawnConstrained || Action == StepAction.MovePawnFreely)
			{
				sb.AppendFormat("({0}, {1})", NodeA.Id, NodeB.Id);
			}
			else
			{
				sb.AppendFormat("({0})", NodeA.Id);
			}
			return sb.ToString();
		}

		public static Step Parse(string line, IPlayer player1, IPlayer player2, IEnumerable<Node> nodes)
		{
			Step newStep = new Step();
			var m = Regex.Match(line, @"(?<player>[^:]*): (?<action>[^\(]*)\((?<nodeA>[0-9]+)(, )?(?<nodeB>[0-9]*)\)");
			if (m.Success)
			{
				string playerName = m.Groups["player"].Value;
				newStep.Player = player1.ToString() == playerName ? player1 : player2.ToString() == playerName ? player2 : null;
				newStep.Action = (StepAction)Enum.Parse(typeof(StepAction), m.Groups["action"].Value);
				int idA = int.Parse(m.Groups["nodeA"].Value);
				newStep.NodeA = nodes.First(x => x.Id == idA);
				if (m.Groups["nodeB"].Length > 0)
				{
					int idB = int.Parse(m.Groups["nodeB"].Value);
					newStep.NodeB = nodes.First(x => x.Id == idB);
				}
			}
			return newStep;
		}
	}

	public enum StepAction
	{
		None,
		PlacePawn,
		RemoveOpponentPawn,
		MovePawnConstrained,
		MovePawnFreely,
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JeuDuMoulin
{
	class MinMaxAI : IPlayer
	{
		public Game Game { get; private set; }

		public Game.PlayerControl Control { get; private set; }

		public StepAction CurrentAction { get; set; }

		public string Name { get; private set; }

		private GameCopy GameCopy;

		Node OpponentToRemoveReminder = null;

		public MinMaxAI(string name)
		{
			this.Name = name;
		}

		public void Initialize(Game game)
		{
			this.Game = game;
			this.Control = new Game.PlayerControl(this);
			GameCopy = new GameCopy(this, this.Control.Opponent);
		}

		public void PlacePawn(Guid token)
		{
			GameCopy.SynchronizeBoardState(Game);
			var move = GameCopy.MiniMax(this, Control.Opponent);
			if (move.Removal != null)
			{
				OpponentToRemoveReminder = Game.Board.First(x => x.Id == move.Removal.Id);
			}
			Control.PlacePawn(token, Game.Board.First(x => x.Id == move.Destination.Id));
			CurrentAction = StepAction.None;
		}

		public void RemoveOpponentPawn(Guid token)
		{
			if (OpponentToRemoveReminder == null)
			{
				throw new Exception("Inconsistent state!");
			}
			Control.RemoveOpponentPawn(token, OpponentToRemoveReminder);
			OpponentToRemoveReminder = null;
			CurrentAction = StepAction.None;
		}

		public void MovePawnConstrained(Guid token)
		{
			GameCopy.SynchronizeBoardState(Game);
			var move = GameCopy.MiniMax(this, Control.Opponent);
			if (move.Removal != null)
			{
				OpponentToRemoveReminder = Game.Board.First(x => x.Id == move.Removal.Id);
			}
			Control.MovePawnConstrained(token, Game.Board.First(x => x.Id == move.Origin.Id), Game.Board.First(x => x.Id == move.Destination.Id));
			CurrentAction = StepAction.None;
		}

		public void MovePawnFreely(Guid token)
		{
			GameCopy.SynchronizeBoardState(Game);
			var move = GameCopy.MiniMax(this, Control.Opponent);
			if (move.Removal != null)
			{
				OpponentToRemoveReminder = Game.Board.First(x => x.Id == move.Removal.Id);
			}
			Control.MovePawnFreely(token, Game.Board.First(x => x.Id == move.Origin.Id), Game.Board.First(x => x.Id == move.Destination.Id));
			CurrentAction = StepAction.None;
		}

		public override string ToString()
		{
			return this.Name;
		}

	}

	class Move
	{
		public IPlayer Player { get; set; }
		public IPlayer Opponent { get; set; }
		public Node Origin { get; set; }
		public Node Destination { get; set; }
		public Node Removal { get; set; }

		public int Valuation { get; set; }

		/// <summary>
		/// set only if the move was applied
		/// </summary>
		public bool SwitchedPhase { get; set; }

		public Move(IPlayer player, IPlayer opponent)
		{
			this.Player = player;
			this.Opponent = opponent;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0}|", Valuation);
			sb.Append(Player);
			sb.Append(": ");
			if (Origin != null) sb.AppendFormat("From {0} ", Origin);
			if (Destination != null) sb.AppendFormat("To {0} ", Destination);
			if (Removal != null) sb.AppendFormat("+Remove {0}", Removal);
			return sb.ToString();
		}
	}

	class GameCopy
	{
		public Dictionary<int, Node> Board = new Dictionary<int, Node>();
		public IPlayer Player { get; set; }
		public IPlayer Opponent { get; set; }

		public Phase phase;
		public int pawnsToPlace;
		public int opponentPawnsToPlace;

		public GameCopy(IPlayer player, IPlayer opponent)
		{
			this.Player = player;
			this.Opponent = opponent;
			var newBoard = Graph.CreateGraph();
			foreach (var item in newBoard)
			{
				Board.Add(item.Id, item);
			}
		}

		public void SynchronizeBoardState(Game game)
		{
			foreach (var item in game.Board)
			{
				Board[item.Id].Owner = item.Owner;
			}
			phase = game.Phase;
			pawnsToPlace = Player.Control.PawnsToPlace;
			opponentPawnsToPlace = Opponent.Control.PawnsToPlace;
		}

		private void ApplyMove(Move move)
		{
			if (move.Origin != null)
			{
				Board[move.Origin.Id].Owner = null;
			}
			if (move.Destination != null)
			{
				Board[move.Destination.Id].Owner = move.Player;
				if (move.Origin == null)	//it's a pawn placement, not a move
				{
					if (move.Player == Player)
					{
						pawnsToPlace--;
					}
					else
					{
						opponentPawnsToPlace--;
					}
					if (phase == Phase.First && pawnsToPlace <= 0 && opponentPawnsToPlace <= 0)
					{
						phase = Phase.Second;
						move.SwitchedPhase = true;
					}
				}
			}
			if (move.Removal != null)
			{
				Board[move.Removal.Id].Owner = null;
			}
		}

		private void CancelMove(Move move)
		{
			if (move.Origin != null)
			{
				Board[move.Origin.Id].Owner = move.Player;
			}
			if (move.Destination != null)
			{
				Board[move.Destination.Id].Owner = null;
				if (move.Origin == null)	//it's a pawn placement, not a move
				{
					if (move.Player == Player)
					{
						pawnsToPlace++;
					}
					else
					{
						opponentPawnsToPlace++;
					}
					if (move.SwitchedPhase)
					{
						phase = Phase.First;
					}
				}
			}
			if (move.Removal != null)
			{
				Board[move.Removal.Id].Owner = move.Opponent;
			}
		}

		private List<Move> FindPossibleMoves(IPlayer playing, IPlayer opponent)
		{
			List<Move> availableMoves = new List<Move>();
			var opponentNodes = Board.Values.Where(x => x.Owner == opponent).ToList();

			if (phase == Phase.First)
			{
				//possibilités: une par position libre, plus toutes les suppressions de l'adversaire possibles si la position fait un moulin
				var availableNodes = Board.Values.Where(x => x.Owner == null);
				foreach (var node in availableNodes)
				{
					if (Graph.IsCreatingAMill(node, playing))
					{
						foreach (var opponentNode in opponentNodes)
						{
							availableMoves.Add(new Move(playing, opponent) { Destination = node, Removal = opponentNode });
						}
					}
					else
					{
						availableMoves.Add(new Move(playing, opponent) { Destination = node });
					}
				}
				return availableMoves;
			}
			else //phase2 -> move
			{
				//check if the current player has only 3 pawns (=> free move)
				if ((playing == Player ? pawnsToPlace == 3 : opponentPawnsToPlace == 3))
				{
					//possibilités: tous les emplacements libres pour chaque pion, plus tous les pions adverses pour chaque moulin
					//var availableNodes = Game.Board.Where(x => x.Owner == Control.Player).ToList();
					//var node = availableNodes.ElementAt(r.Next(availableNodes.Count));
					//var availableNeighbors = Game.Board.Where(x => x.Owner == null).ToList();
					var availableNodes = Board.Values.Where(x => x.Owner == playing);
					foreach (var node in availableNodes)
					{
						foreach (var emptyNode in Board.Values.Where(x => x.Owner == null))
						{
							if (Graph.IsCreatingAMill(emptyNode, playing, node))
							{
								foreach (var opponentNode in opponentNodes)
								{
									availableMoves.Add(new Move(playing, opponent) { Origin = node, Destination = emptyNode, Removal = opponentNode });
								}
							}
							else
							{
								availableMoves.Add(new Move(playing, opponent) { Origin = node, Destination = emptyNode });
							}
						}
					}
					return availableMoves;
				}
				else
				{
					//constrained move
					//possibilités: chaque emplacement adjacent à un pion + tous les pions adverse pour chaque moulin
					var availableNodes = Board.Values.Where(x => x.Owner == playing && x.Neighbors.Any(y => y.Owner == null));
					foreach (var node in availableNodes)
					{
						foreach (var neighbor in node.Neighbors.Where(x => x.Owner == null))
						{
							if (Graph.IsCreatingAMill(neighbor, playing, node))
							{
								foreach (var opponentNode in opponentNodes)
								{
									availableMoves.Add(new Move(playing, opponent) { Origin = node, Destination = neighbor, Removal = opponentNode });
								}
							}
							else
							{
								availableMoves.Add(new Move(playing, opponent) { Origin = node, Destination = neighbor });
							}
						}
					}
					return availableMoves;
				}
			}

		}

		private int EvaluateBoardState()
		{
			//TODO infinity if game won
			int result = 0;
			//each owned node adds 1
			result += (int)Math.Round(1.0 * Board.Values.Count(x => x.Owner == this));
			//each node owned by the opponent removes 1
			result -= (int)Math.Round(1.0 * Board.Values.Count(x => x.Owner != null && x.Owner != this));
			//each possible move add 1
			result += (int)Math.Round(0.25 * FindPossibleMoves(Player, Opponent).Count);
			//+1 for each mill -1 for each opponent mill
			int count1;
			int count2;
			CountPlayersMills(Player, Opponent, out count1, out count2);
			result += (int)Math.Round(2.0 * count1);
			result -= (int)Math.Round(2.0 * count2);
			return result;
		}

		private void CountPlayersMills(IPlayer player1, IPlayer player2, out int count1, out int count2)
		{
			//TODO optimiser pour éviter de bruteforce 3 fois de suites
			count1 = 0;
			count2 = 0;
			foreach (var node in Board.Values.Where(x => x.Owner == player1))
			{
				if (Graph.IsCreatingAMill(node, player1))
				{
					count1++;
				}
			}
			foreach (var node in Board.Values.Where(x => x.Owner == player2))
			{
				if (Graph.IsCreatingAMill(node, player2))
				{
					count2++;
				}
			}
			//la fonction n'est pas optimale et on bruteforce toutes les cases,
			//donc on compte 3 fois les moulins
			count1 = (int)Math.Round(count1 / 3.0);
			count2 = (int)Math.Round(count2 / 3.0);
		}

		public Move MiniMax(IPlayer playing, IPlayer opponent, Move move = null, int depth = 0)
		{
			const int MAX_DEPTH = 3;
			if (move != null) ApplyMove(move);
			if (depth == MAX_DEPTH)
			{
				int valuation = EvaluateBoardState();
				move.Valuation = valuation;
				CancelMove(move);
				return move;
			}
			else
			{
				if (depth % 2 == 0)
				{
					//playing => max
					var children = FindPossibleMoves(playing, opponent);
					Move max = null;
					foreach (var item in children)
					{
						var valuation = MiniMax(opponent, playing, item, depth + 1);
						if (max == null || valuation.Valuation > max.Valuation)
						{
							max = item;
						}
					}
					if (move != null) CancelMove(move);
					if (move != null) move.Valuation = max.Valuation;
					return max;
				}
				else
				{
					//opponent => min
					var children = FindPossibleMoves(playing, opponent);
					Move min = null;
					foreach (var item in children)
					{
						var valuation = MiniMax(opponent, playing, item, depth + 1);
						if (min == null || valuation.Valuation < min.Valuation)
						{
							min = item;
						}
					}
					if (move != null) CancelMove(move);
					if (move != null) move.Valuation = min.Valuation;
					return min;
				}
			}
		}

	}
}

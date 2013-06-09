﻿using System;
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

		Dictionary<int, Node> BoardCopy = new Dictionary<int, Node>();

		public MinMaxAI(string name)
		{
			this.Name = name;
		}

		public void Initialize(Game game)
		{
			this.Game = game;
			this.Control = new Game.PlayerControl(this);
			var newBoard = Graph.CreateGraph();
			foreach (var item in newBoard)
			{
				BoardCopy.Add(item.Id, item);
			}
		}

		private void CloneBoardState()
		{
			foreach (var item in Game.Board)
			{
				BoardCopy[item.Id].Owner = item.Owner;
			}
		}

		private void ApplyMoveToBoard(Move move)
		{
			if (move.Origin != null)
			{
				BoardCopy[move.Origin.Id].Owner = null;
			}
			if (move.Destination != null)
			{
				BoardCopy[move.Destination.Id].Owner = move.CurrentPlayer;
			}
			if (move.Removal != null)
			{
				BoardCopy[move.Removal.Id].Owner = null;
			}
		}

		private void CancelMoveFromBoard(Move move)
		{
			if (move.Origin != null)
			{
				BoardCopy[move.Origin.Id].Owner = move.CurrentPlayer;
			}
			if (move.Destination != null)
			{
				BoardCopy[move.Destination.Id].Owner = null;
			}
			if (move.Removal != null)
			{
				BoardCopy[move.Removal.Id].Owner = move.Opponent;
			}
		}

		private List<Move> FindAllMovesForCurrentBoard(IPlayer playing, IPlayer opponent, int depth)
		{
			List<Move> availableMoves = new List<Move>();
			var opponentNodes = BoardCopy.Values.Where(x => x.Owner == opponent).ToList();

			//possibilités: une par position libre, plus toutes les suppressions de l'adversaire possibles si la position fait un moulin
			var availableNodes = BoardCopy.Values.Where(x => x.Owner == null);
			foreach (var node in availableNodes)
			{
				if (Graph.IsCreatingAMill(node, playing))
				{
					foreach (var opponentNode in opponentNodes)
					{
						availableMoves.Add(new Move(playing, opponent) { Destination = node, Removal = opponentNode, Depth = depth });
					}
				}
				else
				{
					availableMoves.Add(new Move(playing, opponent) { Destination = node, Depth = depth });
				}
			}
			return availableMoves;
		}

		private Move MiniMax(TreeNode gameTree, IPlayer playing, IPlayer opponent, Move move = null, int depth = 0)
		{
			const int MAX_DEPTH = 3;
			if (move != null) ApplyMoveToBoard(move);
			if (depth == MAX_DEPTH)
			{
				int valuation = EvaluateBoardState(playing);
				move.Valuation = valuation;
				CancelMoveFromBoard(move);
				return move;
			}
			else
			{
				if (depth % 2 == 0)
				{
					//playing => max
					var children = FindAllMovesForCurrentBoard(playing, opponent, depth);
					Move max = null;
					foreach (var item in children)
					{
						var child = new TreeNode(playing);
						gameTree.Branches.Add(child);
						var valuation = MiniMax(child, opponent, playing, item, depth + 1);
						if (max == null || valuation.Valuation > max.Valuation)
						{
							max = item;
						}
					}
					if (move != null) CancelMoveFromBoard(move);
					gameTree.Move = max;
					if (move != null) move.Valuation = max.Valuation;
					return max;
				}
				else
				{
					//opponent => min
					var children = FindAllMovesForCurrentBoard(playing, opponent, depth);
					Move min = null;
					foreach (var item in children)
					{
						var child = new TreeNode(opponent);
						gameTree.Branches.Add(child);
						var valuation = MiniMax(child, opponent, playing, item, depth + 1);
						if (min == null || valuation.Valuation < min.Valuation)
						{
							min = item;
						}
					}
					if (move != null) CancelMoveFromBoard(move);
					gameTree.Move = min;
					if (move != null) move.Valuation = min.Valuation;
					return min;
				}
			}
		}

		private int EvaluateBoardState(IPlayer playing)
		{
			int result = 0;
			//each owned node adds 1
			result += (int)Math.Round(1.0 * BoardCopy.Values.Count(x => x.Owner == playing));
			//each node owned by the opponent removes 1
			result -= (int)Math.Round(1.0 * BoardCopy.Values.Count(x => x.Owner != null && x.Owner != playing));
			//each possible move add 1
			result += (int)Math.Round(0.25 * FindAllMovesForCurrentBoard(playing, playing.Control.Opponent,42).Count);
			//+1 for each mill -1 for each opponent mill
			int count1;
			int count2;
			CountPlayersMills(playing, playing.Control.Opponent, out count1, out count2);
			result += (int)Math.Round(2.0 * count1);
			result -= (int)Math.Round(2.0 * count2);
			return result;
		}

		private void CountPlayersMills(IPlayer player1, IPlayer player2, out int count1, out int count2)
		{
			//TODO optimiser pour éviter de bruteforce 3 fois de suites
			count1 = 0;
			count2 = 0;
			foreach (var node in BoardCopy.Values.Where(x => x.Owner == player1))
			{
				if (Graph.IsCreatingAMill(node, player1))
				{
					count1++;
				}
			}
			foreach (var node in BoardCopy.Values.Where(x => x.Owner == player2))
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

		public void PlacePawn(Guid token)
		{
			CloneBoardState();
			var gameTree = new TreeNode(this);
			var move = MiniMax(gameTree, this, Control.Opponent);
			//var node = availableNodes.ElementAt(r.Next(availableNodes.Count));
			Control.PlacePawn(token, Game.Board.First(x => x.Id == move.Destination.Id));
		}

		public void RemoveOpponentPawn(Guid token)
		{
			//var availableNodes = Game.Board.Where(x => x.Owner == Control.Opponent).ToList();
			//var node = availableNodes.ElementAt(r.Next(availableNodes.Count));
			//Control.RemoveOpponentPawn(token, node);
		}

		public void MovePawnConstrained(Guid token)
		{
			//var availableNodes = Game.Board.Where(x => x.Owner == Control.Player && x.Neighbors.Any(y => y.Owner == null)).ToList();
			//var node = availableNodes.ElementAt(r.Next(availableNodes.Count));
			//var availableNeighbors = node.Neighbors.Where(x => x.Owner == null).ToList();
			//Control.MovePawnConstrained(token, node, availableNeighbors.ElementAt(r.Next(availableNeighbors.Count)));
		}

		public void MovePawnFreely(Guid token)
		{
			//var availableNodes = Game.Board.Where(x => x.Owner == Control.Player).ToList();
			//var node = availableNodes.ElementAt(r.Next(availableNodes.Count));
			//var availableNeighbors = Game.Board.Where(x => x.Owner == null).ToList();
			//Control.MovePawnFreely(token, node, availableNeighbors.ElementAt(r.Next(availableNeighbors.Count)));
		}

		public override string ToString()
		{
			return this.Name;
		}

		class Move
		{
			public IPlayer CurrentPlayer { get; set; }
			public IPlayer Opponent { get; set; }
			public Node Origin { get; set; }
			public Node Destination { get; set; }
			public Node Removal { get; set; }

			public int Depth { get; set; }
			public int Valuation { get; set; }

			public Move(IPlayer current, IPlayer opponent)
			{
				this.CurrentPlayer = current;
				this.Opponent = opponent;
			}

			public override string ToString()
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("{0}|", Valuation);
				sb.Append(CurrentPlayer);
				sb.Append(": ");
				if (Origin != null) sb.AppendFormat("From {0} ", Origin);
				if (Destination != null) sb.AppendFormat("To {0} ", Destination);
				if (Removal != null) sb.AppendFormat("+Remove {0}", Removal);
				return sb.ToString();
			}
		}

		class TreeNode
		{
			private static int count = 0;
			public int Id { get; set; }
			public List<TreeNode> Branches { get; set; }
			public Move Move { get; set; }
			public IPlayer Player { get; set; }
			public TreeNode(IPlayer player)
			{
				this.Player = player;
				this.Id = count;
				count++;
				this.Branches = new List<TreeNode>();
			}

			public string ToGraphviz()
			{
				StringBuilder sb = new StringBuilder();
				if (Move == null)
				{
					//terminal
					sb.AppendFormat("n{0} [label=\"terminal\"];\n", Id);
				}
				else
				{
					sb.AppendFormat("n{0} [label=\"{1}\"];\n", Id, Move);
				}
				foreach (var item in Branches)
				{
					sb.AppendFormat("n{0} -- n{1} ;\n", Id, item.Id);
					sb.Append(item.ToGraphviz());
				}
				return sb.ToString();
			}

			public override string ToString()
			{
				if (Move != null)
				{
					return Move.Valuation.ToString();
				}
				else
				{
					return "no move";
				}
			}
		}
	}
}

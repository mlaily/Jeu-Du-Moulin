using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;

namespace JeuDuMoulin
{
	public class TurnHandler
	{
		private ManualResetEvent m = new ManualResetEvent(false);
		private Guid currentToken;
		private bool returnValue;

		public Guid NewTurn()
		{
			returnValue = false;
			m.Reset();
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

	public class TurnEndedEventArgs : EventArgs
	{
		public IPlayer Player { get; set; }
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

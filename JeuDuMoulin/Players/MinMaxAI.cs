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

		public MinMaxAI(string name)
		{
			this.Name = name;
		}

		public void Initialize(Game game)
		{
			this.Game = game;
			this.Control = new Game.PlayerControl(this);
		}

		public void PlacePawn(Guid token)
		{
			//var availableNodes = Game.Board.Where(x => x.Owner == null).ToList();
			//var node = availableNodes.ElementAt(r.Next(availableNodes.Count));
			//Control.PlacePawn(token, node);
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
	}
}

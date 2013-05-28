using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace JeuDuMoulin
{
	public interface IPlayer
	{
		Game Game { get; }
		Game.PlayerControl Control { get; }
		void Initialize(Game game);

		void PlacePawn(Guid token);
		void RemoveOpponentPawn(Guid token);
		/// <summary>
		/// the player can move their pawns only on an adjacent node
		/// </summary>
		/// <param name="token"></param>
		void MovePawnConstrained(Guid token);
		/// <summary>
		/// if a player only has 3 pawns left, they can move them anywhere
		/// </summary>
		/// <param name="token"></param>
		void MovePawnFreely(Guid token);

		string ToString();

	}

}

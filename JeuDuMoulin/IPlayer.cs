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
		void Initialize(Game game);

		void PlacePawn(Guid token);

	}

}

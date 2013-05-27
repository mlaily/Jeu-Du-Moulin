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
		int PawnCount { get; set; }
		void Initialize(Game game);

		void Play(Lock l);

	}

}

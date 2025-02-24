using ArtworkGames.Initialization;
using System;
using System.Collections.Generic;

namespace ArtworkGames.Signals
{
	public class RegisterInitializableSignal
	{
		public IInitializable Initializable;
		public List<Type> Dependencies;

		public RegisterInitializableSignal(IInitializable initializable, List<Type> dependencies)
		{
			Initializable = initializable;
			Dependencies = dependencies;
		}
	}
}
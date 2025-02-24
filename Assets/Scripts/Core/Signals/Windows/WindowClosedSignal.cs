using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArtworkGames.Signals
{
	public class WindowClosedSignal
	{
		public string WindowName { get; private set; }

		public WindowClosedSignal(string windowName)
		{
			WindowName = windowName;
		}
	}
}
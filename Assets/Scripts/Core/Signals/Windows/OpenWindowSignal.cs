using ArtworkGames.Windows;
using System;

namespace ArtworkGames.Signals
{
	public class OpenWindowSignal
	{
		public string WindowName;
		public BaseWindowParams Params;

		public bool Immediately;
		public Type BehaviourType;

		public OpenWindowSignal(string windowName)
		{
			WindowName = windowName;
		}

		public OpenWindowSignal(string windowName, BaseWindowParams @params)
		{
			WindowName = windowName;
			Params = @params;
		}
	}
}
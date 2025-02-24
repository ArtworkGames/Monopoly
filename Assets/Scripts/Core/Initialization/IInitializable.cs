using ArtworkGames.Signals;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using UnityEngine;

namespace ArtworkGames.Initialization
{
	public interface IInitializable
	{
		bool IsInitialised { get; }

		float InitializationWeight { get; }
		float InitializationProgress { get; }

		void Initialize();
		UniTask InitializeAsync();
		void MakeInitialised();
	}
}

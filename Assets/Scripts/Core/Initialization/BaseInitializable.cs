using ArtworkGames.Signals;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArtworkGames.Initialization
{
	public abstract class BaseInitializable : IInitializable
	{
		private IPublisher<RegisterInitializableSignal> _registerPublisher;

		protected bool _isInitialised = false;
		public bool IsInitialised => _isInitialised;

		public virtual float InitializationWeight => 1.0f;
		public virtual float InitializationProgress { get; protected set; }

		protected virtual List<Type> Dependencies => new List<Type> { };

		public BaseInitializable(
			IPublisher<RegisterInitializableSignal> registerPublisher)
		{
			_registerPublisher = registerPublisher;
		}

		public virtual void Initialize()
		{
			_registerPublisher.Publish(new RegisterInitializableSignal(this, Dependencies));
		}

		public abstract UniTask InitializeAsync();

		public void MakeInitialised()
		{
			_isInitialised = true;
		}
	}
}

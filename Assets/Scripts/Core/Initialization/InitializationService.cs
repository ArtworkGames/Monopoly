using ArtworkGames.Signals;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArtworkGames.Initialization
{
	public class InitializationService : IDisposable
	{
		private enum InitializationState
		{
			Wait,
			Initializing,
			Initialized,
			Error
		}

		private class InitializableItem
		{
			public Type Type;
			public IInitializable Initializable;
			public List<Type> Dependencies;
			public InitializationState State;
			public float InitializationWeight;
		}

		private IDisposable _subscriptions;
		private IPublisher<InitializationCompletedSignal> _completedPublisher;

		private List<InitializableItem> _initializableItems;

		public InitializationService(
			ISubscriber<RegisterInitializableSignal> registerSubscriber,
			ISubscriber<StartInitializationSignal> startSubscriber,
			IPublisher<InitializationCompletedSignal> completedPublisher)
		{
			DisposableBagBuilder bag = DisposableBag.CreateBuilder();
			registerSubscriber.Subscribe(OnRegister).AddTo(bag);
			startSubscriber.Subscribe(OnStart).AddTo(bag);
			_subscriptions = bag.Build();

			_completedPublisher = completedPublisher;

			_initializableItems = new List<InitializableItem>();
		}

		public void Dispose()
		{
			_subscriptions.Dispose();
		}

		private void OnRegister(RegisterInitializableSignal signal)
		{
			InitializationState state = InitializationState.Wait;
			foreach (Type type in signal.Dependencies)
			{
				if (!typeof(IInitializable).IsAssignableFrom(type))
				{
					Debug.LogError($"{signal.Initializable.GetType()} has wrong dependency from '{type}'. Must be inherited from '{typeof(IInitializable)}'");
					state = InitializationState.Error;
				}
			}

			InitializableItem bootableItem = new InitializableItem()
			{
				Type = signal.Initializable.GetType(),
				Initializable = signal.Initializable,
				Dependencies = signal.Dependencies,
				State = state,
				InitializationWeight = signal.Initializable.InitializationWeight
			};
			_initializableItems.Add(bootableItem);
		}

		private void OnStart(StartInitializationSignal signal)
		{
			InitializeAllAsync();
		}

		private async void InitializeAllAsync()
		{
			//UniTask[] tasks = _initializableItems.Select(x => InitializeAsync(x)).ToArray();
			//await UniTask.WhenAll(tasks);

			IEnumerable<InitializableItem> next = _initializableItems.Where(item => item.State == InitializationState.Wait && IsAllInitialized(item.Dependencies));
			do
			{
				if (next.Any())
				{
					UniTask[] tasks = next.Select(x => InitializeAsync(x)).ToArray();
					await UniTask.WhenAll(tasks);
				}
				next = _initializableItems.Where(item => item.State == InitializationState.Wait && IsAllInitialized(item.Dependencies));
			}
			while (next.Any());

			_completedPublisher.Publish(new InitializationCompletedSignal());
		}

		private async UniTask InitializeAsync(InitializableItem item)
		{
			if (item.State == InitializationState.Wait)
			{
				Debug.Log(Time.realtimeSinceStartup + " Initialize " + item.Type);

				item.State = InitializationState.Initializing;
				try
				{
					await item.Initializable.InitializeAsync();
					item.Initializable.MakeInitialised();
					item.State = InitializationState.Initialized;
					item.Initializable = null;

					Debug.Log(Time.realtimeSinceStartup + " " + item.Type + " initialized");
				}
				catch (Exception exception)
				{
					item.State = InitializationState.Error;
					Debug.LogException(exception);
				}
			}
		}

		public float GetInitializationProgress()
		{
			float fullWeight = 0.0f;
			float completed = 0.0f;
			for (int i = 0; i < _initializableItems.Count; i++)
			{
				fullWeight += _initializableItems[i].InitializationWeight;
				switch (_initializableItems[i].State)
				{
					case InitializationState.Wait:
						break;
					case InitializationState.Initializing:
						completed += _initializableItems[i].Initializable.InitializationProgress * _initializableItems[i].InitializationWeight;
						break;
					case InitializationState.Initialized:
						completed += _initializableItems[i].InitializationWeight;
						break;
					case InitializationState.Error:
						completed += _initializableItems[i].InitializationWeight;
						break;
				}
			}
			return (completed / fullWeight);
		}

		private bool IsAllInitialized(List<Type> dependencies)
		{
			if (dependencies == null || dependencies.Count == 0)
				return true;

			return dependencies.All(dependecy => _initializableItems.Any(
				item => item.State == InitializationState.Initialized && dependecy.IsAssignableFrom(item.Type))
			);
		}
	}
}

using ArtworkGames.Signals;
using MessagePipe;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace ArtworkGames.Initialization
{
	public class GameLoadingManager : MonoBehaviour, IDisposable
	{
		private IObjectResolver _diContainer;
		private InitializationService _initializationService;
		private IDisposable _subscriptions;
		private IPublisher<StartInitializationSignal> _startInitializationPublisher;
		private IPublisher<LoadSceneSignal> _loadScenePublisher;

		[SerializeField] private string loadSceneName;
		[Space]
		[SerializeField] private Slider progressSlider;

		[Inject]
		public void Construct(
			IObjectResolver diContainer,
			InitializationService initializationService,
			ISubscriber<InitializationCompletedSignal> initializationCompletedSubscriber,
			IPublisher<StartInitializationSignal> startInitializationPublisher,
			IPublisher<LoadSceneSignal> loadScenePublisher)
		{
			_diContainer = diContainer;
			_initializationService = initializationService;

			DisposableBagBuilder bag = DisposableBag.CreateBuilder();
			initializationCompletedSubscriber.Subscribe(OnInitializationCompleted).AddTo(bag);
			_subscriptions = bag.Build();

			_startInitializationPublisher = startInitializationPublisher;
			_loadScenePublisher = loadScenePublisher;
		}

		public void Dispose()
		{
			_subscriptions.Dispose();
		}

		private IEnumerator Start()
		{
			yield return new WaitForEndOfFrame();

			List<IInitializable> initializables = new List<IInitializable>(_diContainer.Resolve<IEnumerable<IInitializable>>());
			for (int i = 0; i < initializables.Count; i++)
			{
				initializables[i].Initialize();
			}

			progressSlider.value = 0.0f;
			_startInitializationPublisher.Publish(new StartInitializationSignal());
		}

		private void Update()
		{
			float progress = _initializationService.GetInitializationProgress();
			if (!float.IsNaN(progress))
			{
				progress = Mathf.Clamp01(progress);
				progressSlider.value = progress;
			}
		}

		private void OnInitializationCompleted(InitializationCompletedSignal signal)
		{
			_loadScenePublisher.Publish(new LoadSceneSignal(loadSceneName));
		}
	}
}

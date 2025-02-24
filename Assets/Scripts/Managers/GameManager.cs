using ArtworkGames.DiceValley.Windows.StockWindow;
using ArtworkGames.Signals;
using MessagePipe;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using ArtworkGames.DiceValley.Signals;
using System;

namespace ArtworkGames.DiceValley.Managers
{
	public class GameManager : MonoBehaviour, IDisposable
	{
		public static GameManager instance;

		public GameCamera gameCamera;
		public Camera uiCamera;
		public City city;
		public CardsList cardsList;

		private IDisposable _subscriptions;
		private IPublisher<GameStartedSignal> _gameStartedPublisher;
		private IPublisher<OpenWindowSignal> _openWindowPublisher;
		private IPublisher<RewardSignal> _rewardPublisher;

		[Inject]
		public void Construct(
			ISubscriber<SceneShownSignal> sceneShownSubscriber,
			IPublisher<GameStartedSignal> gameStartedPublisher,
			IPublisher<OpenWindowSignal> openWindowPublisher,
			IPublisher<RewardSignal> rewardPublisher)
		{
			DisposableBagBuilder bag = DisposableBag.CreateBuilder();
			sceneShownSubscriber.Subscribe(OnSceneShown).AddTo(bag);
			_subscriptions = bag.Build();

			_gameStartedPublisher = gameStartedPublisher;
			_openWindowPublisher = openWindowPublisher;
			_rewardPublisher = rewardPublisher;
		}

		public void Dispose()
		{
			_subscriptions.Dispose();
		}

		private void Awake()
		{
			instance = this;
		}

		private void Start()
		{
			Profile.Init();
			//city.Init(Profile.ci);
		}

		private void OnDestroy()
		{
			instance = null;
		}

		private void OnSceneShown(SceneShownSignal signal)
		{
			_gameStartedPublisher.Publish(new GameStartedSignal());
		}

		public void OnStockButtonClick()
		{
			_openWindowPublisher.Publish(new OpenWindowSignal(StockWindow.PrefabName));
		}

		public void OnChestButtonClick()
		{
			_rewardPublisher.Publish(new RewardSignal(new List<KeyValuePair<string, int>>()
			{
				new KeyValuePair<string, int>("xp", 50),
				new KeyValuePair<string, int>("energy", 10),
				new KeyValuePair<string, int>("coins", 100),
				new KeyValuePair<string, int>("bucks", 1),
			})
			{
				Type = RewardType.Chest
			});
		}
	}
}

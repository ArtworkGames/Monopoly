using ArtworkGames.DiceValley.Data.Private;
using ArtworkGames.DiceValley.Data.Public;
using ArtworkGames.DiceValley.Signals;
using ArtworkGames.DiceValley.Windows.LevelUpWindow;
using ArtworkGames.Initialization;
using ArtworkGames.Signals;
using ArtworkGames.Windows;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Collections.Generic;

namespace ArtworkGames.DiceValley.Managers
{
	public class LevelManager : BaseInitializable, IDisposable
	{
		private PublicDataProvider _publicDataProvider;
		private PrivateDataProvider _privateDataProvider;
		private WindowManager _windowManager;
		private StockManager _stockManager;
		private IDisposable _subscriptions;
		private IPublisher<OpenWindowSignal> _openWindowPublisher;
		private IPublisher<LevelUpSignal> _levelUpPublisher;

		private LevelsPublicModel _levelsPublicModel;
		private PlayerPrivateModel _playerPrivateModel;

		protected override List<Type> Dependencies => new List<Type> { typeof(PublicDataProvider), typeof(PrivateDataProvider) };

		public int Level => _playerPrivateModel.Level;

		private LevelPublicSchema _levelPublicSchema;
		public LevelPublicSchema LevelPublicSchema => _levelPublicSchema;

		private bool isGameStarted;

		public LevelManager(
			IPublisher<RegisterInitializableSignal> registerPublisher,
			PublicDataProvider publicDataProvider,
			PrivateDataProvider privateDataProvider,
			WindowManager windowManager,
			StockManager stockManager,
			ISubscriber<GameStartedSignal> gameStartedSubscriber,
			ISubscriber<StockItemAddedSignal> itemAddedSubscriber,
			ISubscriber<StockItemChangedSignal> itemChangedSubscriber,
			ISubscriber<StockItemRemovedSignal> itemRemovedSubscriber,
			ISubscriber<WindowClosedSignal> windowClosedSubscriber,
			IPublisher<OpenWindowSignal> openWindowPublisher,
			IPublisher<LevelUpSignal> levelUpPublisher) : base(registerPublisher)
		{
			_publicDataProvider = publicDataProvider;
			_privateDataProvider = privateDataProvider;
			_windowManager = windowManager;
			_stockManager = stockManager;

			DisposableBagBuilder bag = DisposableBag.CreateBuilder();
			gameStartedSubscriber.Subscribe(OnGameStarted).AddTo(bag);
			itemAddedSubscriber.Subscribe(OnItemAdded).AddTo(bag);
			itemChangedSubscriber.Subscribe(OnItemChanged).AddTo(bag);
			itemRemovedSubscriber.Subscribe(OnItemRemoved).AddTo(bag);
			windowClosedSubscriber.Subscribe(OnWindowClosed).AddTo(bag);
			_subscriptions = bag.Build();

			_openWindowPublisher = openWindowPublisher;
			_levelUpPublisher = levelUpPublisher;
		}

		public override async UniTask InitializeAsync()
		{
			_levelsPublicModel = _publicDataProvider.Get<LevelsPublicModel>();
			_playerPrivateModel = _privateDataProvider.Get<PlayerPrivateModel>();

			_levelPublicSchema = _levelsPublicModel.GetLevel(_playerPrivateModel.Level);
		}

		public void Dispose()
		{
			_subscriptions.Dispose();
		}

		private void OnGameStarted(GameStartedSignal signal)
		{
			if (isGameStarted) return;
			isGameStarted = true;

			CheckLevelUp();
		}

		private void OnItemAdded(StockItemAddedSignal signal)
		{
			if (signal.ItemId.Equals(SystemItemName.Xp))
			{
				CheckLevelUp();
			}
		}

		private void OnItemChanged(StockItemChangedSignal signal)
		{
			if (signal.ItemId.Equals(SystemItemName.Xp))
			{
				CheckLevelUp();
			}
		}

		private void OnItemRemoved(StockItemRemovedSignal signal)
		{
			if (signal.ItemId.Equals(SystemItemName.Xp))
			{
				CheckLevelUp();
			}
		}

		private void OnWindowClosed(WindowClosedSignal signal)
		{
			CheckLevelUp();
		}

		private void CheckLevelUp()
		{
			if (!isGameStarted) return;

			if (_stockManager.HasItemCount(SystemItemName.Xp, _levelPublicSchema.xp) && !_windowManager.HasOpenWindow())
			{
				_openWindowPublisher.Publish(new OpenWindowSignal(LevelUpWindow.PrefabName)
				{
					Params = new LevelUpWindowParams()
					{
						Level = _playerPrivateModel.Level + 1,
						LevelPublicSchema = _levelPublicSchema,
						OnConfirm = ConfirmLevelUp
					}
				});
			}
		}

		private void ConfirmLevelUp()
		{
			_stockManager.TakeItem(SystemItemName.Xp, _levelPublicSchema.xp);

			_playerPrivateModel.Level++;
			_privateDataProvider.SaveModel<PlayerPrivateModel>();
			_levelPublicSchema = _levelsPublicModel.GetLevel(_playerPrivateModel.Level); 
			
			_levelUpPublisher.Publish(new LevelUpSignal(_playerPrivateModel.Level));
		}
	}
}

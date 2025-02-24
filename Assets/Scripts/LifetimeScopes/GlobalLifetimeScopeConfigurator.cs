using ArtworkGames.DiceValley.Data.Private;
using ArtworkGames.DiceValley.Data.Public;
using ArtworkGames.DiceValley.Managers;
using ArtworkGames.DiceValley.Signals;
using ArtworkGames.Initialization;
using ArtworkGames.LifetimeScopes;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace ArtworkGames.DiceValley.LifetimeScopes
{
	public class GlobalLifetimeScopeConfigurator : LifetimeScopeConfigurator
	{
		[SerializeField] private RewardManager _rewardManager;

		public override void Configure(IContainerBuilder builder)
		{
			builder.Register<IInitializable, PublicDataProvider>(Lifetime.Singleton).AsSelf();
			builder.Register<IInitializable, PrivateDataProvider>(Lifetime.Singleton).AsSelf();

			builder.Register<IInitializable, BankManager>(Lifetime.Singleton).AsSelf();
			builder.Register<IInitializable, StockManager>(Lifetime.Singleton).AsSelf();
			builder.Register<IInitializable, LevelManager>(Lifetime.Singleton).AsSelf();

			builder.RegisterInstance(_rewardManager);
		}

		public override void ConfigureMessagePipe(IContainerBuilder builder, MessagePipeOptions options)
		{
			// game
			builder.RegisterMessageBroker<GameStartedSignal>(options);

			// stock
			builder.RegisterMessageBroker<StockItemAddedSignal>(options);
			builder.RegisterMessageBroker<StockItemRemovedSignal>(options);
			builder.RegisterMessageBroker<StockItemChangedSignal>(options);

			// level
			builder.RegisterMessageBroker<LevelUpSignal>(options);

			// reward
			builder.RegisterMessageBroker<RewardSignal>(options);
		}
	}
}
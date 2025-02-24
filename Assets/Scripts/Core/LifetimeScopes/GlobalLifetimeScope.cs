using ArtworkGames.Initialization;
using ArtworkGames.Localization;
using ArtworkGames.Scenes;
using ArtworkGames.Signals;
using ArtworkGames.Windows;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ArtworkGames.LifetimeScopes
{
	public class GlobalLifetimeScope : LifetimeScope
	{
		[Space]
		[SerializeField] private SceneLoadingManager _sceneLoadingManager;
		[SerializeField] private WindowManager _windowManager;
		[Space]
		[SerializeField] private LifetimeScopeConfigurator _extendedConfigurator;

		protected override void Awake()
		{
			base.Awake();
			DontDestroyOnLoad(gameObject);
		}

		protected override void Configure(IContainerBuilder builder)
		{
			RegisterMessagePipe(builder);

			builder.Register<InitializationService>(Lifetime.Singleton);

			builder.Register<ArtworkGames.Initialization.IInitializable, AppSettingsManager>(Lifetime.Singleton);
			builder.Register<ArtworkGames.Initialization.IInitializable, LocalizationManager>(Lifetime.Singleton).AsSelf();

			builder.RegisterInstance(_sceneLoadingManager);
			builder.RegisterInstance(_windowManager);

			if (_extendedConfigurator != null)
			{
				_extendedConfigurator.Configure(builder);
			}
		}

		private void RegisterMessagePipe(IContainerBuilder builder)
		{
			MessagePipeOptions options = builder.RegisterMessagePipe();
			builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));

			// initialization
			builder.RegisterMessageBroker<RegisterInitializableSignal>(options);
			builder.RegisterMessageBroker<StartInitializationSignal>(options);
			builder.RegisterMessageBroker<InitializationCompletedSignal>(options);

			// localization
			builder.RegisterMessageBroker<ChangeLocaleSignal>(options);
			builder.RegisterMessageBroker<LocaleChangedSignal>(options);

			// scenes
			builder.RegisterMessageBroker<LoadSceneSignal>(options);
			builder.RegisterMessageBroker<SceneShownSignal>(options);

			// windows
			builder.RegisterMessageBroker<OpenWindowSignal>(options);
			builder.RegisterMessageBroker<CloseWindowSignal>(options);
			builder.RegisterMessageBroker<CloseAllWindowsSignal>(options);
			builder.RegisterMessageBroker<WindowClosedSignal>(options);
			builder.RegisterMessageBroker<WindowOpenedSignal>(options);

			if (_extendedConfigurator != null)
			{
				_extendedConfigurator.ConfigureMessagePipe(builder, options);
			}
		}
	}
}

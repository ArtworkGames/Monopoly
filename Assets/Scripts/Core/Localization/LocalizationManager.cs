using ArtworkGames.Initialization;
using ArtworkGames.Signals;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ArtworkGames.Localization
{
	public class LocalizationManager : BaseInitializable, IDisposable
	{
		private IDisposable _subscriptions;
		private IPublisher<LocaleChangedSignal> _localeChangedPublisher;

		private string _localeName;
		public string LocaleName => _localeName;

		private LocalePublicModel _model;

		public LocalizationManager(
			IPublisher<RegisterInitializableSignal> registerPublisher,
			ISubscriber<ChangeLocaleSignal> changeLocaleSubscriber,
			IPublisher<LocaleChangedSignal> localeChangedPublisher) : base(registerPublisher)
		{
			DisposableBagBuilder bag = DisposableBag.CreateBuilder();
			changeLocaleSubscriber.Subscribe(OnChangeLocale).AddTo(bag);
			_subscriptions = bag.Build();

			_localeChangedPublisher = localeChangedPublisher;
		}

		public override async UniTask InitializeAsync()
		{
			await LoadLocaleAsync("en");
		}

		public void Dispose()
		{
			_subscriptions.Dispose();
		}

		private async UniTask LoadLocaleAsync(string localeName)
		{
			try
			{
				LocalePublicModel model = new LocalePublicModel();
				TextAsset textAsset = await Addressables.LoadAssetAsync<TextAsset>(GetDataPath(localeName));
				Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(textAsset.text);

				model.Init(data);
				_model = model;

				_localeName = localeName;
				_localeChangedPublisher.Publish(new LocaleChangedSignal(localeName));
			}
			catch (Exception e)
			{
				Debug.LogError("Error reading asset \"" + GetDataPath(localeName) + "\": " + e.Message);
			}
		}

		private string GetDataPath(string localeName)
		{
			return $"Data/Locales/{localeName}.json";
		}

		private void OnChangeLocale(ChangeLocaleSignal signal)
		{
			LoadLocaleAsync(signal.LocaleName);
		}

		public string GetString(string key, params string[] p)
		{
			if (_model == null) return "";

			string str = _model.GetString(key);

			if ((p != null) && (p.Length > 0))
			{
				str = string.Format(str, p);
			}

			return str;
		}
	}
}

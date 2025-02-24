using ArtworkGames.Localization;
using ArtworkGames.Signals;
using MessagePipe;
using System;
using TMPro;
using UnityEngine;
using VContainer;

namespace ArtworkGames.UI
{
	[RequireComponent(typeof(TMP_Text))]
	public class TMPTextLocalizer : MonoBehaviour, IDisposable
	{
		[SerializeField] private string _localeKey;

		public string LocaleKey => _localeKey;

		private TMP_Text _tmpText;
		public TMP_Text TMPText
		{
			get
			{
				if (_tmpText == null)
					_tmpText = GetComponent<TMP_Text>();
				return _tmpText;
			}
		}

		private LocalizationManager _localizationManager;
		private IDisposable _subscriptions;

		private string[] _params;
		private bool _isAlreadyLocalized;

		[Inject]
		public void Construct(
			LocalizationManager localizationManager,
			ISubscriber<LocaleChangedSignal> localeChangedSubscriber)
		{
			_localizationManager = localizationManager;

			DisposableBagBuilder bag = DisposableBag.CreateBuilder();
			localeChangedSubscriber.Subscribe(OnLocaleChanged).AddTo(bag);
			_subscriptions = bag.Build();
		}

		public void Dispose()
		{
			_subscriptions.Dispose();
		}

		private void Start()
		{
			if (_localizationManager != null && !_isAlreadyLocalized)
			{
				UpdateText();
			}
		}

		private void OnLocaleChanged(LocaleChangedSignal signal)
		{
			UpdateText();
		}

		public void Localize(string key, params string[] p)
		{
			_localeKey = key;
			_params = p;
			SetParams();
		}

		public void SetParams(params string[] p)
		{
			_params = p;
			UpdateText();
		}

		private void UpdateText()
		{
			if ((_localizationManager == null) || string.IsNullOrEmpty(_localeKey))
			{
				TMPText.SetText("");
				return;
			}

			string str = _localizationManager.GetString(_localeKey, _params);
			TMPText.SetText(str);

			_isAlreadyLocalized = true;
		}
	}
}

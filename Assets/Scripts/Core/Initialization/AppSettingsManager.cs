using ArtworkGames.Signals;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MessagePipe;
using System.Globalization;
using UnityEngine;

namespace ArtworkGames.Initialization
{
	public class AppSettingsManager : BaseInitializable
	{
		public AppSettingsManager(
			IPublisher<RegisterInitializableSignal> registerPublisher) : base(registerPublisher)
		{
		}

		public override async UniTask InitializeAsync()
		{
			GL.Clear(false, true, Color.black);

			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

			Application.targetFrameRate = 30;// 30;
			Application.runInBackground = true;
			Input.multiTouchEnabled = true;
			QualitySettings.vSyncCount = 0;
			QualitySettings.SetQualityLevel(QualitySettings.names.Length - 1);

			DOTween.Init();

			await UniTask.Yield(PlayerLoopTiming.Update);
		}
	}
}

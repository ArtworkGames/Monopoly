using ArtworkGames.Signals;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace ArtworkGames.Scenes
{
	public class SceneLoadingManager : MonoBehaviour, IDisposable
	{
		[SerializeField] private ScenesFade _fade;

		private IDisposable _subscriptions;
		private IPublisher<SceneShownSignal> _sceneShownPublisher;

		private string currentSceneName;

		[Inject]
		public void Construct(
			ISubscriber<LoadSceneSignal> loadSubscriber,
			IPublisher<SceneShownSignal> sceneShownPublisher)
		{
			DisposableBagBuilder bag = DisposableBag.CreateBuilder();
			loadSubscriber.Subscribe(OnLoadScene).AddTo(bag);
			_subscriptions = bag.Build();

			_sceneShownPublisher = sceneShownPublisher;
		}

		public void Dispose()
		{
			_subscriptions.Dispose();
		}

		private void Start()
		{
			_fade.HideAsync();
		}

		private void OnLoadScene(LoadSceneSignal signal)
		{
			currentSceneName = signal.SceneName;

			LoadSceneAsync();
		}

		private async void LoadSceneAsync()
		{
			await _fade.ShowAsync();

			AsyncOperation operation = SceneManager.LoadSceneAsync("Empty");
			await UniTask.WaitUntil(() => operation.isDone);

			operation = Resources.UnloadUnusedAssets();
			await UniTask.WaitUntil(() => operation.isDone);

			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

			await UniTask.WaitForEndOfFrame(this);

			operation = SceneManager.LoadSceneAsync(currentSceneName, LoadSceneMode.Single);
			await UniTask.WaitUntil(() => operation.isDone);

			await _fade.HideAsync();

			_sceneShownPublisher.Publish(new SceneShownSignal(currentSceneName));
		}
	}
}

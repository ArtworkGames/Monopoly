using ArtworkGames.Signals;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;

namespace ArtworkGames.Windows
{
	public class WindowManager : MonoBehaviour, IDisposable
	{
		[SerializeField] private GameObject _uiRoot;
		[SerializeField] private Transform _windowsParent;

		private IObjectResolver _diContainer;
		private IDisposable _subscriptions;
		private IPublisher<WindowOpenedSignal> _openedPublisher;
		private IPublisher<WindowClosedSignal> _closedPublisher;

		private WindowsQueue _queue;

		private bool _openingLock;

		[Inject]
		public void Construct(
			IObjectResolver diContainer,
			ISubscriber<OpenWindowSignal> openSubscriber,
			ISubscriber<CloseWindowSignal> closeSubscriber,
			ISubscriber<CloseAllWindowsSignal> closeAllSubscriber,
			IPublisher<WindowOpenedSignal> openedPublisher,
			IPublisher<WindowClosedSignal> closedPublisher)
		{
			_diContainer = diContainer;

			DisposableBagBuilder bag = DisposableBag.CreateBuilder();
			openSubscriber.Subscribe(OnOpenWindow).AddTo(bag);
			closeSubscriber.Subscribe(OnCloseWindow).AddTo(bag);
			closeAllSubscriber.Subscribe(OnCloseAllWindows).AddTo(bag);
			_subscriptions = bag.Build();

			_openedPublisher = openedPublisher;
			_closedPublisher = closedPublisher;

			_queue = new WindowsQueue();
		}

		public void Dispose()
		{
			_subscriptions.Dispose();
		}

		protected void Awake()
		{
			DontDestroyOnLoad(_uiRoot);
		}

		public bool HasOpenWindow()
		{
			return _queue.OpenedWindows.Count > 0;
		}

		public bool HasWindowInQueue()
		{
			return _queue.Queue.Count > 0;
		}

		public T GetOpenedWindow<T>() where T : BaseWindow
		{
			if (HasOpenWindow() == false)
			{
				return default;
			}

			BaseWindowBehaviour result = _queue.OpenedWindows.FirstOrDefault(x => x.Window is T);
			if (result == default)
			{
				return default;
			}

			return (T)result.Window;
		}

		private void OnOpenWindow(OpenWindowSignal signal)
		{
			if ((signal.BehaviourType != null) && !signal.BehaviourType.IsSubclassOf(typeof(BaseWindowBehaviour)))
			{
				Debug.LogError($"BehaviourType {signal.BehaviourType} must be subclass of {typeof(BaseWindowBehaviour)}");
				return;
			}

			if (_queue.Queue.Any(x => x.WindowName == signal.WindowName))
			{
				return;
			}

			BaseWindowBehaviour behaviour = CreateBehaviour(signal.BehaviourType, signal.WindowName, signal.Params, signal.Immediately);
			OpenWindowAsync(behaviour);
		}

		private BaseWindowBehaviour CreateBehaviour(Type behavioursType, string windowName, BaseWindowParams @params, bool immediately)
		{
			BaseWindowBehaviour behaviour;
			if (behavioursType == null)
			{
				behaviour = new BaseWindowBehaviour();
			}
			else
			{
				behaviour = (BaseWindowBehaviour)Activator.CreateInstance(behavioursType);
			}

			behaviour.Queue = _queue;
			behaviour.WindowName = windowName;
			behaviour.Params = @params;
			behaviour.OpenImmediately = immediately;
			return behaviour;
		}

		private async void OpenWindowAsync(BaseWindowBehaviour behaviour)
		{
			_queue.Queue.Add(behaviour);

			behaviour.Window = _queue.GetWindowInstance(behaviour.WindowName);
			if (behaviour.Window == null)
			{
				await InstantiateWindowAsync(behaviour);
			}

			TryOpenNextAsync().Forget();
		}

		private async UniTask InstantiateWindowAsync(BaseWindowBehaviour behaviour)
		{
			string prefabName = GetPrefabPath(behaviour.WindowName);
			GameObject prefab = await Addressables.LoadAssetAsync<GameObject>(prefabName);

			GameObject instance = _diContainer.Instantiate(prefab, _windowsParent);
			instance.name = behaviour.WindowName;
			instance.SetActive(false);

			BaseWindow window = instance.GetComponent<BaseWindow>();
			behaviour.Window = window;

			_queue.AddWindowInstance(behaviour.WindowName, window);
		}

		private string GetPrefabPath(string name)
		{
			return $"Windows/{name}.prefab";
		}

		private async UniTask TryOpenNextAsync()
		{
			if (_queue.Queue.Count == 0 || _openingLock)
			{
				return;
			}

			_openingLock = true;

			await UniTask.Yield();

			bool openedAny = false;

			List<BaseWindowBehaviour> queueClone = new List<BaseWindowBehaviour>();
			queueClone.AddRange(_queue.Queue);

			foreach (BaseWindowBehaviour behaviour in queueClone)
			{
				if (behaviour.Window == null)
				{
					continue;
				}

				if (_queue.OpenedWindows.Count > 0 && !behaviour.CanOpenOverOtherWindows)
				{
					continue;
				}

				if (behaviour.CancelOpening)
				{
					_queue.Queue.Remove(behaviour);
					continue;
				}

				await behaviour.OpenAsync();

				_queue.OpenedWindows.Add(behaviour);
				_queue.Queue.Remove(behaviour);

				_openedPublisher.Publish(new WindowOpenedSignal(behaviour.WindowName));

				if (behaviour.CancelOpening)
				{
					behaviour.Window.CloseWindow();
				}

				openedAny = true;
				break;
			}

			_openingLock = false;

			if (openedAny)
			{
				TryOpenNextAsync().Forget();
			}
		}

		private void OnCloseWindow(CloseWindowSignal signal)
		{
			if (_queue.Queue.Any(x => x.WindowName == signal.WindowName))
			{
				BaseWindowBehaviour windowBehaviour = _queue.Queue.Find(x => x.WindowName == signal.WindowName);
				windowBehaviour.CancelOpening = true;
				return;
			}

			BaseWindowBehaviour behaviour = _queue.GetOpenedWindow(signal.WindowName);
			BaseWindow window = behaviour?.Window;
			if (window == null || behaviour.State != WindowState.Opened)
			{
				return;
			}

			behaviour.CloseImmediately = signal.Immediately;
			CloseWindowAsync(behaviour);
		}

		private async void CloseWindowAsync(BaseWindowBehaviour behaviour)
		{
			await behaviour.CloseAsync();
			_queue.OpenedWindows.Remove(behaviour);

			_closedPublisher.Publish(new WindowClosedSignal(behaviour.WindowName));

			//if (window.NeedReleaseAfterClose)
			//{
			//	ReleaseWindowInstance(obj.Name);
			//}

			await TryOpenNextAsync();
		}

		private void OnCloseAllWindows(CloseAllWindowsSignal signal)
		{
			List<BaseWindowBehaviour> openedWindowsClone = new List<BaseWindowBehaviour>();
			openedWindowsClone.AddRange(_queue.OpenedWindows);

			foreach (BaseWindowBehaviour behaviour in openedWindowsClone)
			{
				behaviour.Window.CloseWindow(signal.Immediately);
			}
		}
	}
}

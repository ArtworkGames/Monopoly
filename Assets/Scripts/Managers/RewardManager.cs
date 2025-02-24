using ArtworkGames.DiceValley.Data.Private;
using ArtworkGames.DiceValley.Data.Public;
using ArtworkGames.DiceValley.Signals;
using ArtworkGames.DiceValley.UI.Rewards;
using ArtworkGames.DiceValley.Windows.RewardChestWindow;
using ArtworkGames.DiceValley.Windows.RewardItemWindow;
using ArtworkGames.Signals;
using ArtworkGames.Windows;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

namespace ArtworkGames.DiceValley.Managers
{
	public class RewardManager : MonoBehaviour, IDisposable
	{
		private PrivateDataProvider _privateDataProvider;
		private StockManager _stockManager;
		private IPublisher<OpenWindowSignal> _openWindowPublisher;
		private IDisposable _subscriptions;

		private List<RewardTarget> _targets = new List<RewardTarget>();

		private bool isGameStarted;

		[Inject]
		public void Construct(
			PrivateDataProvider privateDataProvider,
			StockManager stockManager,
			ISubscriber<GameStartedSignal> gameStartedSubscriber,
			ISubscriber<WindowClosedSignal> windowClosedSubscriber,
			ISubscriber<RewardSignal> rewardSubscriber,
			IPublisher<OpenWindowSignal> openWindowPublisher)
		{
			_privateDataProvider = privateDataProvider;
			_stockManager = stockManager;

			DisposableBagBuilder bag = DisposableBag.CreateBuilder();
			gameStartedSubscriber.Subscribe(OnGameStarted).AddTo(bag);
			windowClosedSubscriber.Subscribe(OnWindowClosed).AddTo(bag);
			rewardSubscriber.Subscribe(OnReward).AddTo(bag);
			_subscriptions = bag.Build();

			_openWindowPublisher = openWindowPublisher;
		}

		public void Dispose()
		{
			_subscriptions.Dispose();
		}

		public void RegisterTarget(RewardTarget rewardTarget)
		{
			if (!_targets.Contains(rewardTarget))
			{
				_targets.Add(rewardTarget);
			}
		}

		public void UnregisterTarget(RewardTarget rewardTarget)
		{
			_targets.Remove(rewardTarget);
		}

		private RewardTarget GetTarget(string itemId, string itemType)
		{
			if (_targets.Count == 0)
			{
				Debug.LogError($"No registered reward targets.");
				return default;
			}

			RewardTarget target = _targets.FirstOrDefault(t => t.ItemIds.Contains(itemId));
			if (target == default)
			{
				target = _targets.FirstOrDefault(t => t.ItemTypes.Contains(itemType));
				if (target == default)
				{
					target = _targets.FirstOrDefault(t => t.IsUniversal);
				}
			}
			if (target == default)
			{
				Debug.LogError($"No registered reward target found for \"{itemId}\" item of \"{itemType}\" type.");
			}

			return target;
		}

		private void OnGameStarted(GameStartedSignal signal)
		{
			if (isGameStarted) return;
			isGameStarted = true;

			CheckPendingRewardSignals();
		}

		private void OnWindowClosed(WindowClosedSignal signal)
		{
			CheckPendingRewardSignals();
		}

		private void CheckPendingRewardSignals()
		{
			if (!isGameStarted) return;

			RewardsPrivateModel rewardPrivateModel = _privateDataProvider.Get<RewardsPrivateModel>();
			int signalsCount = rewardPrivateModel.GetRewardsCount();

			if (signalsCount > 0)
			{
				RewardSignal signal = rewardPrivateModel.GetReward(0);
				OnReward(signal);
			}
		}

		private void OnReward(RewardSignal signal)
		{
			if (signal.Type == RewardType.FlyingRewards)
			{
				ShowFlyingRewards(signal);
			}
			else if (signal.Type == RewardType.ItemWindow)
			{
				ShowItemWindow(signal);
			}
			else if (signal.Type == RewardType.Chest)
			{
				ShowChestWindow(signal);
			}
		}

		private async void ShowFlyingRewards(RewardSignal signal)
		{
			if (!signal.OnlyConfirmPendingItems)
			{
				_stockManager.AddPendingItems(signal.Items);
			}

			for (int i = 0; i < signal.Items.Count; i++)
			{
				(ItemPublicSchema, int) itemData = _stockManager.GetItem(signal.Items[i].Key);

				ShowFlyingReward(signal.Items[i].Key, itemData.Item1.type.ToString(), signal.Items[i].Value,
					signal.Positions[i], signal.ByPiece);
			}

			await UniTask.WaitForSeconds(0.8f);

			_stockManager.ConfirmPendingItems(signal.Items);
		}

		private async void ShowFlyingReward(string itemId, string itemType, int count, Vector3 pos, bool byPiece)
		{
			RewardTarget target = GetTarget(itemId, itemType);
			if (target != default)
			{
				GameObject prefab = target.GetFlyingReward(itemId, itemType);
				Sprite sprite = await Addressables.LoadAssetAsync<Sprite>($"Icons/Items/{itemId}.png");

				if (byPiece)
				{
					int piecesCount = Mathf.Min(count, 5);
					for (int i = 0; i < piecesCount; i++)
					{
						Vector3 piecePos = pos + new Vector3(UnityEngine.Random.Range(-50.0f, 50.0f), UnityEngine.Random.Range(-50.0f, 50.0f), 0.0f);
						ShowFlyingRewardPiece(prefab, sprite, piecePos, target.transform.position);
						
						await UniTask.WaitForSeconds(0.1f);
					}
				}
				else
				{
					ShowFlyingRewardPiece(prefab, sprite, pos, target.transform.position);
				}
			}
		}

		private async void ShowFlyingRewardPiece(GameObject prefab, Sprite sprite, Vector3 pos, Vector3 destPos)
		{
			GameObject prefabInstance = Instantiate(prefab, transform, false);
			prefabInstance.transform.position = pos;

			FlyingReward flyingReward = prefabInstance.GetComponent<FlyingReward>();
			await flyingReward.Fly(sprite, destPos);
		}

		private void ShowItemWindow(RewardSignal signal)
		{
			AddRewardSignal(signal);

			_openWindowPublisher.Publish(new OpenWindowSignal(RewardItemWindow.PrefabName)
			{
				Params = new RewardItemWindowParams()
				{
					Items = signal.Items,
					OnRewardTaken = () =>
					{
						RemoveRewardSignal(signal);
					}
				},
				BehaviourType = typeof(PopUpWindowBehaviour)
			});
		}

		private void ShowChestWindow(RewardSignal signal)
		{
			AddRewardSignal(signal);

			_openWindowPublisher.Publish(new OpenWindowSignal(RewardChestWindow.PrefabName)
			{
				Params = new RewardChestWindowParams()
				{
					Items = signal.Items,
					OnRewardTaken = () =>
					{
						RemoveRewardSignal(signal);
					}
				},
				BehaviourType = typeof(PopUpWindowBehaviour)
			});
		}

		private void AddRewardSignal(RewardSignal signal)
		{
			RewardsPrivateModel rewardPrivateModel = _privateDataProvider.Get<RewardsPrivateModel>();
			rewardPrivateModel.AddReward(signal);
			_privateDataProvider.SaveModel<RewardsPrivateModel>();
		}

		private void RemoveRewardSignal(RewardSignal signal)
		{
			RewardsPrivateModel rewardPrivateModel = _privateDataProvider.Get<RewardsPrivateModel>();
			rewardPrivateModel.RemoveReward(signal);
			_privateDataProvider.SaveModel<RewardsPrivateModel>();
		}
	}
}

using ArtworkGames.DiceValley.Signals;
using ArtworkGames.DiceValley.UI.Components;
using ArtworkGames.Windows;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VContainer;

namespace ArtworkGames.DiceValley.Windows.RewardItemWindow
{
	public class RewardItemWindowParams : BaseWindowParams
	{
		public List<KeyValuePair<string, int>> Items;
		public Action OnRewardTaken;
	}

	public class RewardItemWindow : BaseWindow<RewardItemWindowParams>
	{
		public static string PrefabName = "RewardItemWindow";

		[Space]
		[SerializeField] private IconImage _image;
		[SerializeField] private TMP_Text _amount;
		[SerializeField] private Transform _rewardPoint;

		private IPublisher<RewardSignal> _rewardPublisher;

		[Inject]
		public void Construct(
			IPublisher<RewardSignal> rewardPublisher)
		{
			_rewardPublisher = rewardPublisher;
		}

		protected override void BeforeOpen()
		{
			_amount.text = Params.Items[0].Value.ToString();

			LoadImage();
		}

		private async void LoadImage()
		{
			_image.gameObject.SetActive(false);
			await _image.LoadIconAsync(Params.Items[0].Key);
			_image.gameObject.SetActive(true);
		}

		public async void OnTakeButtonClick()
		{
			_rewardPublisher.Publish(new RewardSignal(Params.Items)
			{
				Type = RewardType.FlyingRewards,
				Positions = new Vector3[1] { _rewardPoint.position },
				ByPiece = true,
				//OnlyConfirmPendingItems = true
			});
			Params.OnRewardTaken?.Invoke();

			await UniTask.WaitForSeconds(0.5f);

			CloseWindow();
		}
	}
}

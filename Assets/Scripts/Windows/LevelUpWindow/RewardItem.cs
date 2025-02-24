using ArtworkGames.DiceValley.Data.Public;
using ArtworkGames.DiceValley.UI.Components;
using TMPro;
using UnityEngine;

namespace ArtworkGames.DiceValley.Windows.LevelUpWindow
{
	public class RewardItem : MonoBehaviour
	{
		[SerializeField] private Transform _content;
		[SerializeField] private IconImage _image;
		[SerializeField] private TMP_Text _count;
		[SerializeField] private Transform _rewardPoint;

		public Transform Content => _content;
		public Transform RewardPoint => _rewardPoint;

		[HideInInspector] public ItemPublicSchema PublicSchema;
		[HideInInspector] public int Count;

		private async void Start()
		{
			if (PublicSchema == null) return;

			_count.text = Count.ToString();

			_image.gameObject.SetActive(false);
			await _image.LoadIconAsync(PublicSchema.id);
			_image.gameObject.SetActive(true);
		}
	}
}

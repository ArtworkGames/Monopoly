using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ArtworkGames.DiceValley.UI.Rewards
{
	public class FlyingReward : MonoBehaviour
	{
		[SerializeField] private Image _image;

		public async UniTask Fly(Sprite sprite, Vector3 destPos)
		{
			_image.sprite = sprite;

			bool flyComplete = false;
			transform.DOMove(destPos, 0.8f)
				.SetEase(Ease.InOutQuad)
				.OnComplete(() =>
				{
					transform.DOScale(0.0f, 0.2f)
						.SetEase(Ease.InBack)
						.OnComplete(() =>
						{
							flyComplete = true;
						});
				});

			await UniTask.WaitUntil(() => flyComplete);
			Destroy(gameObject);
		}
	}
}

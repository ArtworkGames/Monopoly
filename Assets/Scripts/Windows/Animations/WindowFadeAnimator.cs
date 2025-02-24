using ArtworkGames.Windows;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace ArtworkGames.DiceValley.Windows.Animations
{
	public class WindowFadeAnimator : BaseWindowAnimator
	{
		protected CanvasGroup canvasGroup;

		protected float showDuration = 0.2f;
		protected float hideDuration = 0.2f;

		private bool shown;
		private Tween alphaTween;

		protected virtual void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup>();
			if (canvasGroup == null)
			{
				canvasGroup = gameObject.AddComponent<CanvasGroup>();
			}
		}

		public override void Reset()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			alphaTween?.Kill();
			shown = false;
			canvasGroup.alpha = 0.0f;
		}

		protected override async UniTask ShowOpenAnimationAsync(bool immediately = false)
		{
			alphaTween?.Kill();
			shown = false;

			if (immediately)
			{
				canvasGroup.alpha = 1.0f;
				shown = true;
			}
			else
			{
				canvasGroup.alpha = 0.0f;

				alphaTween = canvasGroup.DOFade(1.0f, showDuration)
					.SetUpdate(true)
					.SetEase(Ease.OutCubic)
					.OnComplete(() =>
					{
						shown = true;
					});
			}
			await UniTask.WaitWhile(() => gameObject.activeSelf && !shown);
		}

		protected override async UniTask ShowCloseAnimationAsync(bool immediately = false)
		{
			alphaTween?.Kill();
			shown = true;

			if (immediately)
			{
				canvasGroup.alpha = 0.0f;
				shown = false;
			}
			else
			{
				alphaTween = canvasGroup.DOFade(0.0f, hideDuration)
					.SetUpdate(true)
					.SetEase(Ease.OutCubic)
					.OnComplete(() =>
					{
						shown = false;
					});
			}
			await UniTask.WaitWhile(() => gameObject.activeSelf && shown);
		}
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace ArtworkGames.Scenes
{
	public class ScenesFade : MonoBehaviour
	{
		[SerializeField] private CanvasGroup _canvasGroup;

		private float showDuration = 0.3f;
		private float hideDuration = 0.3f;

		private bool shown;
		private Tweener alphaTween;

		private void Awake()
		{
			_canvasGroup.alpha = 1.0f;
		}

		public async UniTask ShowAsync(bool immediately = false)
		{
			alphaTween?.Kill();
			shown = false;

			_canvasGroup.interactable = true;
			_canvasGroup.blocksRaycasts = true;

			if (immediately)
			{
				_canvasGroup.alpha = 1.0f;
				shown = true;
			}
			else
			{
				_canvasGroup.alpha = 0.0f;

				alphaTween = _canvasGroup.DOFade(1.0f, showDuration)
					.SetUpdate(true)
					.SetEase(Ease.OutCubic)
					.OnComplete(() =>
					{
						shown = true;
					});
			}
			await UniTask.WaitWhile(() => !shown);
		}

		public async UniTask HideAsync(bool immediately = false)
		{
			alphaTween?.Kill();
			shown = true;

			if (immediately)
			{
				_canvasGroup.alpha = 0.0f;
				_canvasGroup.interactable = false;
				_canvasGroup.blocksRaycasts = false;
				shown = false;
			}
			else
			{
				alphaTween = _canvasGroup.DOFade(0.0f, hideDuration)
					.SetUpdate(true)
					.SetEase(Ease.OutCubic)
					.OnComplete(() =>
					{
						_canvasGroup.interactable = false;
						_canvasGroup.blocksRaycasts = false;
						shown = false;
					});
			}
			await UniTask.WaitWhile(() => shown);
		}
	}
}

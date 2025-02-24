using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlaceSelection : MonoBehaviour
{
	[SerializeField] private SpriteRenderer spriteRenderer;

	private Tween selectTween;

	public void SetColor(Color c)
	{
		c.a = 0.0f;
		spriteRenderer.color = c;
	}

	public void Select()
	{
		selectTween?.Kill();
		selectTween = spriteRenderer.DOFade(0.5f, 0.3f)
			.SetEase(Ease.OutCubic);
	}

	public void Unselect()
	{
		selectTween?.Kill();
		selectTween = spriteRenderer.DOFade(0.0f, 0.3f)
			.SetEase(Ease.OutCubic);
	}
}

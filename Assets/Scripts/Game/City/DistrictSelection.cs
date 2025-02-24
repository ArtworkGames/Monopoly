using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DistrictSelection : MonoBehaviour
{
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] new private ParticleSystem particleSystem;

	private Tween colorTween;
	private Tween alphaTween;
	private Color color;

	private bool isSelected;

	public void SetColor(Color c)
	{
		color = c;
		c.a = 0.5f;
		spriteRenderer.color = c;

		var main = particleSystem.main;
		main.startColor = color;
	}

	public void Select()
	{
		if (isSelected) return;
		isSelected = true;

		colorTween?.Kill();
		color.a = 1.0f;
		colorTween = spriteRenderer.DOColor(color, 0.3f)
			.SetEase(Ease.OutCubic);
	}

	public void Unselect()
	{
		if (!isSelected) return;
		isSelected = false;

		color.a = 0.5f;
		colorTween?.Kill();
		colorTween = spriteRenderer.DOColor(color, 0.3f)
			.SetEase(Ease.OutCubic);
	}

	public void Flash()
	{
		particleSystem.Play();

		//colorTween?.Kill();
		//color.a = 1.0f;
		//colorTween = spriteRenderer.DOColor(color, 0.5f)
		//	.SetEase(Ease.OutCubic)
		//	.OnComplete(() =>
		//	{
		//		spriteRenderer.color = Color.white;
		//		colorTween = spriteRenderer.DOColor(color, 0.5f)
		//			.SetDelay(0.3f)
		//			.SetEase(Ease.OutCubic);
		//	});
	}
}

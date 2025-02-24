using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnergy : MonoBehaviour
{
	[HideInInspector] public Vector3 destPos;

	private void Start()
	{
		transform.DOLocalMove(destPos, 0.8f)
			.SetEase(Ease.InOutQuad)
			.OnComplete(() =>
			{
				transform.DOScale(0.0f, 0.2f)
					.SetEase(Ease.InBack)
					.OnComplete(() =>
					{
						Destroy(gameObject);
					});
			});
	}
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyingProduct : MonoBehaviour
{
	[SerializeField] private Image image;

	[HideInInspector] public ProductData productProfile;
	[HideInInspector] public Vector3 destPos;

	private void Start()
	{
		if (productProfile == null) return;

		image.sprite = Resources.Load<Sprite>("Products/" + productProfile.type.ToString().ToLower() + "_icon");

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

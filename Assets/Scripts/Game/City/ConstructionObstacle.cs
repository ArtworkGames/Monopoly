using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionObstacle : ClickableObject
{
	[SerializeField] private Animator animator;
	[SerializeField] new private Collider collider;

	[Space]
	public Transform popupPoint;

	private void Awake()
	{
		collider.enabled = false;
	}

	public void Show()
	{
		animator.SetBool("Show", true);
	}

	public void Hide()
	{
		isDown = false;
		collider.enabled = false;
		animator.SetBool("Show", false);
	}

	public void OnShowComplete()
	{
		collider.enabled = true;
	}

	public void OnHideComplete()
	{
		gameObject.SetActive(false);
	}
}

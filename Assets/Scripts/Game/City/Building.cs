using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Building : ClickableObject
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
		collider.enabled = true;
	}

	public void OnShowComplete()
	{
	}
}

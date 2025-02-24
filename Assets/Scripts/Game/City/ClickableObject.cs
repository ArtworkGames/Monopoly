using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableObject : MonoBehaviour
{
	public Action<ClickableObject> OnClick;

	protected bool isDown;
	protected Vector2 downPos;

	virtual protected void OnMouseDown()
	{
		Touch[] touches = Input.touches;
#if UNITY_EDITOR
		touches = GenerateTouches();
#endif

		bool canSelect = false;
		if ((touches.Length == 1) && (touches[0].phase == TouchPhase.Began) &&
			//!EventSystem.current.IsPointerOverGameObject(touches[0].fingerId))
			!MyUtils.IsPointerOverUIElement(touches[0].position))
		{
			canSelect = true;
		}
		if (canSelect)
		{
			isDown = true;
			downPos = touches[0].position;
		}
	}

	virtual protected void OnMouseDrag()
	{
		if (!isDown) return;

		Touch[] touches = Input.touches;
#if UNITY_EDITOR
		touches = GenerateTouches();
#endif

		if (touches.Length == 1)
		{
			Vector2 dragPos = touches[0].position;
			float distance = Vector2.Distance(downPos, dragPos) / Screen.width;
			if (distance > 0.02f)
			{
				isDown = false;
			}
		}
		else
		{
			isDown = false;
		}
	}

	virtual protected void OnMouseUp()
	{
		if (!isDown) return;
		OnClick?.Invoke(this);
	}

	virtual protected Touch[] GenerateTouches()
	{
		Touch[] touches = new Touch[0];
		if (Input.GetMouseButtonDown(0))
		{
			touches = new Touch[1];

			Touch touch = new Touch();
			touch.fingerId = 0;
			touch.phase = TouchPhase.Began;
			touch.position = Input.mousePosition;
			touches[0] = touch;
		}
		else if (Input.GetMouseButton(0))
		{
			touches = new Touch[1];

			Touch touch = new Touch();
			touch.fingerId = 0;
			touch.phase = TouchPhase.Moved;
			touch.position = Input.mousePosition;
			touches[0] = touch;
		}
		return touches;
	}
}

using ArtworkGames.Scenes;
using ArtworkGames.Windows;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using VContainer;

public class GameCamera : MonoBehaviour
{
	private const float minPositionX = -200.5f;
	private const float maxPositionX = 500.5f;
	private const float minDragPositionX = -201.0f;
	private const float maxDragPositionX = 501.0f;

	private const float minPositionZ = -500.0f;//-2.5f;
	private const float maxPositionZ = 500.0f;//2.5f;
	private const float minDragPositionZ = -500.5f;//-3.0f;
	private const float maxDragPositionZ = 500.5f;//3.0f;

	private const float minPitchAngle = 30.0f;
	private const float maxPitchAngle = 80.0f;
	private const float minDragPitchAngle = 25.0f;
	private const float maxDragPitchAngle = 85.0f;

	private const float minZoomDistance = -12.0f;
	private const float maxZoomDistance = -8.0f;
	private const float minDragZoomDistance = -13.0f;
	private const float maxDragZoomDistance = -7.0f;

	public Action OnStartDrag;

	[SerializeField] private Transform pivot;
	new public Camera camera;

	private Vector2 lastResolution = Vector2.zero;

	private Vector3 position;
	private Vector3 destPosition;
	private float yawAngle;
	private float destYawAngle;
	private float pitchAngle;
	private float destPitchAngle;
	private float zoomDistance;
	private float destZoomDistance;

	private bool isDrag;
	private int[] fingerIds;
	private int[] oldFingerIds;
	private Vector3[] screenTouchPoses;
	private Vector3[] oldScreenTouchPoses;

	private bool isMove;
	private Vector3 oldPosition;
	private float oldYawAngle;
	private float oldPitchAngle;
	private float oldZoomDistance;
	private Tween moveTween;
	private float moveFactor;

	[Inject]
	public virtual void Construct()
	{
	}

	private void Start()
	{
		position = transform.position;
		destPosition = position;

		yawAngle = transform.localEulerAngles.y;
		destYawAngle = yawAngle;

		pitchAngle = pivot.localEulerAngles.x;
		destPitchAngle = pitchAngle;

		zoomDistance = camera.transform.localPosition.z;
		destZoomDistance = zoomDistance;

		fingerIds = new int[2] { 0, 0 };
		oldFingerIds = new int[2] { 0, 0 };
		screenTouchPoses = new Vector3[2] { Vector3.zero, Vector3.zero };
		oldScreenTouchPoses = new Vector3[2] { Vector3.zero, Vector3.zero };
	}

	private void FixedUpdate()
	{
		if ((lastResolution.x != Screen.width) || (lastResolution.y != Screen.height))
		{
			lastResolution = new Vector2(Screen.width, Screen.height);
			Adjust();
		}
	}

	public void Adjust()
	{
		float ratio = (float)Screen.width / (float)Screen.height;
		float ratio9x16 = 9.0f / 16.0f;
		float ratio9x18 = 9.0f / 18.0f;

		if (ratio < ratio9x16)
		{
			float fov = 60.0f + (ratio9x16 - ratio) / (ratio9x16 - ratio9x18) * 6.0f;
			camera.fieldOfView = fov;
		}
		else
		{
			camera.fieldOfView = 60.0f;
		}
	}

	private void Update()
	{
		Touch[] touches = Input.touches;
#if UNITY_EDITOR
		touches = GenerateTouches();
#endif

		// save screen touch positions
		// and calculate ground touch positions
		for (int i = 0; i < Mathf.Min(touches.Length, 2); i++)
		{
			fingerIds[i] = touches[i].fingerId;
			screenTouchPoses[i] = touches[i].position;
			if (touches[i].phase == TouchPhase.Began)
			{
				oldFingerIds[i] = touches[i].fingerId;
				oldScreenTouchPoses[i] = touches[i].position;
			}
		}

		if (isMove || (touches.Length == 0))
		{
			isDrag = false;
		}
		else if (!isDrag)
		{
			isDrag = CanStartDrag(touches);
			if (isDrag)
			{
				OnStartDrag?.Invoke();
			}
		}

		bool isFingerEquals = false;
		if (touches.Length == 1)
		{
			isFingerEquals = fingerIds[0] == oldFingerIds[0];
		}
		else if (touches.Length == 2)
		{
			isFingerEquals = (fingerIds[0] == oldFingerIds[0]) && (fingerIds[1] == oldFingerIds[1]);
		}

		if (isDrag && isFingerEquals)
		{
			float sensitivity = 2.0f;

			// process drag
			if (touches.Length == 1)
			{
				Vector3 delta = (screenTouchPoses[0] - oldScreenTouchPoses[0]) / Screen.width;
				delta.z = delta.y;
				delta.y = 0.0f;
				delta = Quaternion.Euler(0.0f, yawAngle, 0.0f) * delta;

				destPosition -= delta * 3.0f * sensitivity;
			}
			// process yaw, pitch and zoom
			else if (touches.Length == 2)
			{
				Vector3 touchCenter = Vector3.Lerp(screenTouchPoses[0], screenTouchPoses[1], 0.5f);
				Vector3 oldTouchCenter = Vector3.Lerp(oldScreenTouchPoses[0], oldScreenTouchPoses[1], 0.5f);

				//float yawDelta = (touchCenter.x - oldTouchCenter.x) / Screen.width;
				//destYawAngle += yawDelta * 100.0f * sensitivity;

				float pitchDelta = (touchCenter.y - oldTouchCenter.y) / Screen.width;
				destPitchAngle -= pitchDelta * 30.0f * sensitivity;

				float touchAngle = Mathf.Atan2(screenTouchPoses[1].y - screenTouchPoses[0].y, screenTouchPoses[1].x - screenTouchPoses[0].x);
				float oldTouchAngle = Mathf.Atan2(oldScreenTouchPoses[1].y - oldScreenTouchPoses[0].y, oldScreenTouchPoses[1].x - oldScreenTouchPoses[0].x);
				if ((oldTouchAngle < -Mathf.PI / 2.0f) && (touchAngle > Mathf.PI / 2.0f))
				{
					oldTouchAngle += Mathf.PI * 2.0f;
				}
				if ((oldTouchAngle > Mathf.PI / 2.0f) && (touchAngle < -Mathf.PI / 2.0f))
				{
					oldTouchAngle -= Mathf.PI * 2.0f;
				}

				float yawDelta = touchAngle - oldTouchAngle;
				destYawAngle += yawDelta * Mathf.Rad2Deg * 0.5f * sensitivity;

				float touchDistance = Vector3.Distance(screenTouchPoses[0], screenTouchPoses[1]);
				float oldTouchDistance = Vector3.Distance(oldScreenTouchPoses[0], oldScreenTouchPoses[1]);

				float zoomDelta = (touchDistance - oldTouchDistance) / Screen.width;
				destZoomDistance += zoomDelta * 2.0f * sensitivity;
			}
		}

		// trim drag position
		if (touches.Length == 1)
		{
			destPosition = new Vector3(
				Mathf.Clamp(destPosition.x, minDragPositionX, maxDragPositionX),
				destPosition.y,
				Mathf.Clamp(destPosition.z, minDragPositionZ, maxDragPositionZ));
		}
		else
		{
			destPosition = new Vector3(
				Mathf.Clamp(destPosition.x, minPositionX, maxPositionX),
				destPosition.y,
				Mathf.Clamp(destPosition.z, minPositionZ, maxPositionZ));
		}

		// trim pitch angle and zoom
		if (touches.Length == 2)
		{
			destPitchAngle = Mathf.Clamp(destPitchAngle, minDragPitchAngle, maxDragPitchAngle);
			destZoomDistance = Mathf.Clamp(destZoomDistance, minDragZoomDistance, maxDragZoomDistance);
		}
		else
		{
			destPitchAngle = Mathf.Clamp(destPitchAngle, minPitchAngle, maxPitchAngle);
			destZoomDistance = Mathf.Clamp(destZoomDistance, minZoomDistance, maxZoomDistance);
		}

		// set position
		position = Vector3.Lerp(position, destPosition, Time.deltaTime * 10.0f);
		transform.position = position;

		// set yaw angle
		yawAngle = Mathf.Lerp(yawAngle, destYawAngle, Time.deltaTime * 10.0f);
		transform.localEulerAngles = new Vector3(0.0f, yawAngle, 0.0f);

		// set pitch angle
		pitchAngle = Mathf.Lerp(pitchAngle, destPitchAngle, Time.deltaTime * 10.0f);
		pivot.localEulerAngles = new Vector3(pitchAngle, 0.0f, 0.0f);

		// set zoom
		zoomDistance = Mathf.Lerp(zoomDistance, destZoomDistance, Time.deltaTime * 10.0f);
		camera.transform.localPosition = new Vector3(0.0f, 0.0f, zoomDistance);

		// save touch positions to old one
		oldFingerIds[0] = fingerIds[0];
		oldFingerIds[1] = fingerIds[1];
		oldScreenTouchPoses[0] = screenTouchPoses[0];
		oldScreenTouchPoses[1] = screenTouchPoses[1];
	}

	private bool CanStartDrag(Touch[] touches)
	{
		bool canStartDrag = false;
		if ((touches.Length == 1) && (touches[0].phase == TouchPhase.Began) &&
			//!EventSystem.current.IsPointerOverGameObject(touches[0].fingerId))
			!MyUtils.IsPointerOverUIElement(touches[0].position))
		{
			canStartDrag = true;
		}
		else if ((touches.Length == 2) &&
			(touches[0].phase == TouchPhase.Began) && (touches[1].phase == TouchPhase.Began) &&
			//!EventSystem.current.IsPointerOverGameObject(touches[0].fingerId) &&
			//!EventSystem.current.IsPointerOverGameObject(touches[1].fingerId))
			!MyUtils.IsPointerOverUIElement(touches[0].position) &&
			!MyUtils.IsPointerOverUIElement(touches[1].position))
		{
			canStartDrag = true;
		}
		return canStartDrag;
	}

	private Touch[] GenerateTouches()
	{
		Touch[] touches = new Touch[0];
		int touchesCount = 1;
		bool isZoom = false;
		if (Input.GetKey(KeyCode.LeftAlt))
		{
			touchesCount = 2;
		}
		if (Input.GetKey(KeyCode.LeftControl))
		{
			isZoom = true;
			touchesCount = 2;
		}
		if (Input.GetMouseButtonDown(0))
		{
			touches = new Touch[touchesCount];

			Touch touch = new Touch();
			touch.fingerId = 0;
			touch.phase = TouchPhase.Began;
			touch.position = Input.mousePosition;
			touches[0] = touch;

			if (touchesCount == 2)
			{
				touch = new Touch();
				touch.fingerId = 1;
				touch.phase = TouchPhase.Began;
				if (isZoom)
				{
					touch.position = new Vector2(Screen.width - Input.mousePosition.x, Screen.height - Input.mousePosition.y);
				}
				else
				{
					touch.position = Input.mousePosition + new Vector3(0.0f, -100.0f, 0.0f);
				}
				touches[1] = touch;
			}
		}
		else if (Input.GetMouseButton(0))
		{
			touches = new Touch[touchesCount];

			Touch touch = new Touch();
			touch.fingerId = 0;
			touch.phase = TouchPhase.Moved;
			touch.position = Input.mousePosition;
			touches[0] = touch;

			if (touchesCount == 2)
			{
				touch = new Touch();
				touch.fingerId = 1;
				touch.phase = TouchPhase.Moved;
				if (isZoom)
				{
					touch.position = new Vector2(Screen.width - Input.mousePosition.x, Screen.height - Input.mousePosition.y);
				}
				else
				{
					touch.position = Input.mousePosition + new Vector3(0.0f, -100.0f, 0.0f);
				}
				touches[1] = touch;
			}
		}
		return touches;
	}

	public void MoveTo(Vector3 pos, float yaw, float pitch, float zoom)
	{
		isMove = true;
		oldPosition = destPosition;
		//oldYawAngle = destYawAngle;
		oldPitchAngle = destPitchAngle;
		oldZoomDistance = destZoomDistance;

		while (Mathf.Abs(yaw - destYawAngle) > 180.0f)
		{
			if (yaw > destYawAngle) yaw -= 360.0f;
			else yaw += 360.0f;
		}

		moveTween?.Kill();
		moveFactor = 0.0f;
		moveTween = DOTween.To(() => moveFactor, x => moveFactor = x, 1.0f, 0.4f)
			.SetEase(Ease.Linear)
			.OnUpdate(() =>
			{
				destPosition = Vector3.Lerp(oldPosition, pos, moveFactor);
				//destYawAngle = Mathf.Lerp(oldYawAngle, yaw, moveFactor);
				destPitchAngle = Mathf.Lerp(oldPitchAngle, pitch, moveFactor);
				destZoomDistance = Mathf.Lerp(oldZoomDistance, zoom, moveFactor);
			})
			.OnComplete(() =>
			{
				isMove = false;
			});
	}
}

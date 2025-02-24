using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasAdjuster : MonoBehaviour
{
	[SerializeField] private CanvasScaler scaler;
	[SerializeField] new private Camera camera;

	private Vector2 lastResolution = Vector2.zero;

	private void Start()
	{
		Adjust();
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
		if (Screen.width < Screen.height)
		{
			float ratio = (float)Screen.width / (float)Screen.height;
			float fullHDRatio = 9.0f / 16.0f;

			scaler.referenceResolution = new Vector2(1080.0f, 1920.0f);
			if (ratio < fullHDRatio)
			{
				scaler.matchWidthOrHeight = 0.0f;
			}
			else
			{
				scaler.matchWidthOrHeight = 1.0f;
			}

			if (camera != null) camera.orthographicSize = 9.6f;
		}
		else
		{
			float ratio = (float)Screen.width / (float)Screen.height;
			float fullHDRatio = 16.0f / 9.0f;

			scaler.referenceResolution = new Vector2(1920.0f, 1080.0f);
			if (ratio < fullHDRatio)
			{
				scaler.matchWidthOrHeight = 0.0f;
			}
			else
			{
				scaler.matchWidthOrHeight = 1.0f;
			}

			if (camera != null) camera.orthographicSize = 5.4f;
		}
	}
}

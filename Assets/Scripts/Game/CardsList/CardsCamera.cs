using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsCamera : MonoBehaviour
{
	[SerializeField] new private Camera camera;
	[SerializeField] private Transform anchorContainer;

	private Vector2 lastResolution = Vector2.zero;

	private void Start()
	{
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
		float size = 0.0f;

		if (Screen.height > Screen.width)
		{
			float fullHDRatio = 1920.0f / 1080.0f;
			float currentRatio = (float)Screen.height / (float)Screen.width;
			size = currentRatio * 9.6f / fullHDRatio;
			if (size < 9.6f)
			{
				size = 9.6f;
			}
		}
		else
		{
			float fullHDRatio = 1080.0f / 1920.0f;
			float currentRatio = (float)Screen.height / (float)Screen.width;
			size = currentRatio * 5.4f / fullHDRatio;
			if (size < 5.4f)
			{
				size = 5.4f;
			}
		}

		camera.orthographicSize = size;
		anchorContainer.localPosition = new Vector3(0.0f, -size + 2.1f, 0.0f);
	}
}

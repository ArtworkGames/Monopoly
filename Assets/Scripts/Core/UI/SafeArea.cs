using UnityEngine;

public class SafeArea : MonoBehaviour
{
	private RectTransform rectTransform;

	private Rect lastSafeArea = Rect.zero;

	private void Start()
	{
		rectTransform = GetComponent<RectTransform>();
		Adjust();
	}

	private void FixedUpdate()
	{
		Rect r = Screen.safeArea;
		if (lastSafeArea != r)
		{
			lastSafeArea = r;
			Adjust();
		}
	}

	public void Adjust()
	{
		Rect r = Screen.safeArea;

		if ((r.width == 0) || (r.height == 0)) return;

		if (Screen.width > Screen.height)
		{
			r = new Rect(new Vector2(r.x, 0.0f), new Vector2(r.width, Screen.height));
		}

		Vector2 anchorMin = r.position;
		Vector2 anchorMax = r.position + r.size;

		anchorMin.x /= Screen.width;
		anchorMin.y /= Screen.height;
		anchorMax.x /= Screen.width;
		anchorMax.y /= Screen.height;

		rectTransform.anchorMin = anchorMin;
		rectTransform.anchorMax = anchorMax;
	}
}

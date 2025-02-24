using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetName : MonoBehaviour
{
	private Vector3 position;
	private Vector3 scale;
	private Vector3 rotation;


	private void Awake()
	{
		position = transform.localPosition;
		scale = transform.localScale;
		rotation = transform.localEulerAngles;
	}

	private IEnumerator Start()
	{
		yield return new WaitForEndOfFrame();

		transform.localPosition = position;
		transform.localScale = scale;
		transform.localEulerAngles = rotation;
	}
}

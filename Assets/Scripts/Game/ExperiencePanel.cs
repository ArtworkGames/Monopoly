using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExperiencePanel : MonoBehaviour
{
	[SerializeField] private TMP_Text label;

	private void Start()
	{
		Profile.OnExperienceChanged += OnExperienceChanged;
		OnExperienceChanged();
	}

	private void OnDestroy()
	{
		Profile.OnExperienceChanged -= OnExperienceChanged;
	}

	private void OnExperienceChanged()
	{
		label.text = Profile.experience.ToString();
	}
}

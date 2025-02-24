using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnergyPanel : MonoBehaviour
{
	[SerializeField] private TMP_Text label;

	private void Start()
	{
		Profile.OnEnergyChanged += OnEnergyChanged;
		OnEnergyChanged();
	}

	private void OnDestroy()
	{
		Profile.OnEnergyChanged -= OnEnergyChanged;
	}

	private void OnEnergyChanged()
	{
		label.text = Profile.energy.ToString();
	}
}

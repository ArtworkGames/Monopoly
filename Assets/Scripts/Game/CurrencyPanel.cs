using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyPanel : MonoBehaviour
{
	[SerializeField] private TMP_Text label;

	private void Start()
	{
		Profile.OnCurrencyChanged += OnCurrencyChanged;
		OnCurrencyChanged();
	}

	private void OnDestroy()
	{
		Profile.OnCurrencyChanged -= OnCurrencyChanged;
	}

	private void OnCurrencyChanged()
	{
		label.text = Profile.currency.ToString();
	}
}

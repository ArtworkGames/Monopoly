using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class HourlyReward : MonoBehaviour
{
	[SerializeField] private TMP_Text label;
	[SerializeField] private GameObject button;
	[SerializeField] private Transform content;

	private int secondsToReward = 120;
	private int seconds;

	private bool isTaken;

	private void Start()
	{
		StartCoroutine(Tick());
	}

	private void UpdateLabel()
	{
		int remain = secondsToReward - seconds;
		int min = remain / 60;
		int sec = remain - min * 60;

		if (min > 0)
		{
			label.text = "Reward\nin " + min + "m " + sec + "s";
		}
		else
		{
			label.text = "Reward\nin " + sec + "s";
		}
	}

	private IEnumerator Tick()
	{
		do
		{
			isTaken = false;
			button.SetActive(false);
			seconds = 0;

			do
			{
				UpdateLabel();
				yield return new WaitForSecondsRealtime(1.0f);
				seconds++;
			}
			while (seconds < secondsToReward);

			label.text = "TAKE\nREWARD!";
			button.SetActive(true);
			Blink();

			yield return new WaitUntil(() => isTaken);
		}
		while (true);
	}

	public void OnTakeRewardClick()
	{
		Profile.energy += 10;
		FlyingIcons.ShowEnergy(transform.position);

		isTaken = true;
	}

	private void Blink()
	{
		content.DOScale(0.9f, 0.6f)
			.SetEase(Ease.OutCubic)
			.OnComplete(() =>
			{
				content.DOScale(1.0f, 1.4f)
					.SetEase(Ease.OutElastic)
					.OnComplete(() =>
					{
						if (seconds == secondsToReward)
						{
							Blink();
						}
					});
			});
	}
}

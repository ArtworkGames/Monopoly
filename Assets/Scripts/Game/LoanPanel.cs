using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoanPanel : MonoBehaviour
{
	[SerializeField] private Transform content;
	[SerializeField] private TMP_Text label;

	private bool isShown;
	private Tween showTween;

	private void Start()
	{
		content.localPosition = new Vector3(500.0f, 0.0f, 0.0f);

		Profile.OnLoanChanged += OnLoanChanged;
		OnLoanChanged();
	}

	private void OnDestroy()
	{
		showTween?.Kill();
		Profile.OnLoanChanged -= OnLoanChanged;
	}

	private void OnLoanChanged()
	{
		if (Profile.loan > 0)
		{
			Show();
		}
		else
		{
			Hide();
		}

		label.text = "-" + Profile.loan.ToString() + "$\n(" + Profile.loanCount + "0% = " + Profile.loanInterest + "$)";
	}

	private void Show()
	{
		if (isShown) return;
		isShown = true;
		
		showTween?.Kill();
		showTween = content.DOLocalMoveX(0.0f, 0.3f)
			.SetEase(Ease.OutBack);
	}

	private void Hide()
	{
		if (!isShown) return;
		isShown = false;

		showTween?.Kill();
		showTween = content.DOLocalMoveX(500.0f, 0.3f)
			.SetEase(Ease.OutCubic);
	}

	public void OnClick()
	{
		//GameManager.instance.ShowRepayLoanWindow();
	}
}

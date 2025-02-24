using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsList : MonoBehaviour
{
	public Action<CardData> OnSpinResult;

	public const int itemsCount = 9;
	public const float itemsDistance = 2.4f;

	[SerializeField] private Transform hideContainer;
	[SerializeField] private Transform spinContainer;
	[SerializeField] private GameObject sourceItem;

	private List<CardsListItem> items;
	private CardsListItem selectedItem;
	private int leftItemIndex = 0;

	private bool isSpinning;
	private bool isSpinLastPhase;
	private Coroutine spinCoroutine;
	private Tween spinTween;
	private float oldSpinPosition;
	private float spinPosition;
	private int totalShift;
	private int itemsShift;
	private CardData spinResult;

	private Tween showTween;

	private void Start()
	{
		items = new List<CardsListItem>();
		for (int i = 0; i < itemsCount; i++)
		{
			CardsListItem item = CreateItem(i, (-itemsCount / 2 + i) * itemsDistance);
			item.SetCard(GetNextCard());

			if (i == (itemsCount / 2))
			{
				item.SetSelected(true, 0.0f);
			}
		}
	}

	private void OnDestroy()
	{
		spinTween?.Kill();
		if (spinCoroutine != null) StopCoroutine(spinCoroutine);
	}

	private CardsListItem CreateItem(int index, float x)
	{
		GameObject itemObject = Instantiate<GameObject>(sourceItem, spinContainer, false);
		itemObject.name = "Card " + index;
		itemObject.transform.localPosition = new Vector3(x, 0.0f, 0.0f);

		CardsListItem item = itemObject.GetComponent<CardsListItem>();
		item.index = index;
		items.Add(item);

		return item;
	}

	private CardData GetNextCard(bool isResult = false)
	{
		return CardDeck.GetNextCard(isResult);
	}

	public void Spin()
	{
		if (isSpinning) return;

		spinCoroutine = StartCoroutine(SpinCoroutine());
	}

	private IEnumerator SpinCoroutine()
	{
		// start spinning

		isSpinning = true;
		isSpinLastPhase = false;
		spinPosition = 0.0f;
		oldSpinPosition = 0.0f;

		totalShift = 9;
		itemsShift = 0;

		for (int i = 0; i < items.Count; i++)
		{
			items[i].SetSelected(false);
		}

		bool next = false;
		spinTween = DOTween.To(() => spinPosition, x => spinPosition = x, -itemsDistance * 4, 0.2f)
			.SetEase(Ease.InCubic)
			.OnComplete(() =>
			{
				next = true;
			});
		yield return new WaitUntil(() => next);

		// continue spinning

		do
		{
			next = false;
			totalShift += 12;
			spinTween = DOTween.To(() => spinPosition, x => spinPosition = x, spinPosition - itemsDistance * 12, 0.4f)
				.SetEase(Ease.Linear)
				.OnComplete(() =>
				{
					next = true;
				});
			yield return new WaitUntil(() => next);
		}
		// waiting for server answer
		while (UnityEngine.Random.value < 0.2f);

		// stop spinning

		isSpinLastPhase = true;
		next = false;
		spinTween = DOTween.To(() => spinPosition, x => spinPosition = x, spinPosition - itemsDistance * 5, 1.0f)
			.SetEase(Ease.OutBack)
			.OnComplete(() =>
			{
				next = true;
			});
		yield return new WaitUntil(() => next);

		spinPosition = 0.0f;
		oldSpinPosition = 0.0f;
		isSpinning = false;
		spinContainer.localPosition = Vector3.zero;

		OnSpinResult?.Invoke(spinResult);
	}

	private void Update()
	{
		float delta = spinPosition - oldSpinPosition;
		oldSpinPosition = spinPosition;
		float contentX = spinContainer.localPosition.x + delta;
		while (contentX <= -itemsDistance)
		{
			contentX += itemsDistance;
			ShiftItems();
		}
		spinContainer.localPosition = new Vector3(contentX, 0.0f, 0.0f);
	}

	private void ShiftItems()
	{
		for (int i = 0; i < items.Count; i++)
		{
			items[i].transform.localPosition += new Vector3(-itemsDistance, 0.0f, 0.0f);
		}

		bool isSelectedCard = false;
		itemsShift++;
		if (isSpinLastPhase && (itemsShift == (totalShift - itemsCount / 2)))
		{
			isSelectedCard = true;
		}

		items[leftItemIndex].transform.localPosition = new Vector3(itemsCount / 2 * itemsDistance, 0.0f, 0.0f);
		items[leftItemIndex].ResetContent();
		items[leftItemIndex].SetCard(GetNextCard(isSelectedCard));
		if (isSelectedCard)
		{
			items[leftItemIndex].SetSelected(true);
			selectedItem = items[leftItemIndex];
			spinResult = items[leftItemIndex].card;
			//Debug.Log(items[leftItemIndex].data.ToString());
		}

		leftItemIndex++;
		if (leftItemIndex == items.Count) leftItemIndex = 0;
	}

	public void Show()
	{
		showTween?.Kill();
		showTween = hideContainer.DOLocalMoveY(0.0f, 0.4f)
			.SetEase(Ease.OutBack);
	}

	public void Hide()
	{
		showTween?.Kill();
		showTween = hideContainer.DOLocalMoveY(-5.0f, 0.3f)
			.SetEase(Ease.OutQuad);
	}

	public CardsListItem GetSelectedItem()
	{
		return selectedItem;
	}

	public void Refresh()
	{
		for (int i = 0; i < items.Count; i++)
		{
			items[i].Refresh();
		}
	}
}

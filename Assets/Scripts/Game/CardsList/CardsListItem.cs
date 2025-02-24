using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class CardsListItem : MonoBehaviour
{
	[Serializable]
	public class DistrictUI
	{
		public GameObject content;
		public SpriteRenderer colorRect;
		public TMP_Text title;
		public GameObject landRightsIcon;
		public TMP_Text price;
	}

	[Serializable]
	public class BuildingUI
	{
		public GameObject content;
		public SpriteRenderer colorRect;
		public GameObject[] boughtBuildings;
		public TMP_Text title;
		public SpriteRenderer image;
		public GameObject currencyIcon;
		public GameObject constructionIcon;
		public GameObject cleanIcon;
		public TMP_Text price;
	}

	[Serializable]
	public class RecourceUI
	{
		public GameObject content;
		public TMP_Text title;
		public SpriteRenderer image;
		public Sprite[] sprites;
		public TMP_Text amount;
	}

	[Serializable]
	public class SpecialUI
	{
		public GameObject content;
		public TMP_Text title;
		public SpriteRenderer image;
		public Sprite[] sprites;
	}

	public Transform container;
	public Transform content;
	[SerializeField] private DistrictUI districtUI;
	[SerializeField] private BuildingUI buildingUI;
	[SerializeField] private RecourceUI resourceUI;
	[SerializeField] private SpecialUI specialUI;

	[HideInInspector] public int index;
	[HideInInspector] public CardData card;

	public void Refresh()
	{
		SetCard(card);
	}

	public void ResetContent()
	{
		container.localPosition = Vector3.zero;

		content.localPosition = Vector3.zero;
		content.localScale = Vector3.one;
		content.gameObject.ChangeLayersRecursively("UI");
	}

	public void SetCard(CardData newCard)
	{
		card = newCard;

		switch (card.type)
		{
			case CardType.District:
				districtUI.content.SetActive(true);
				buildingUI.content.SetActive(false);
				resourceUI.content.SetActive(false);
				specialUI.content.SetActive(false);

				districtUI.colorRect.color = DistrictData.GetColor(card.district.color);
				districtUI.title.text = "LAND RIGHTS";
				districtUI.price.text = card.district.activationPrice + "$";
				break;

			case CardType.Building:
				districtUI.content.SetActive(false);
				buildingUI.content.SetActive(true);
				resourceUI.content.SetActive(false);
				specialUI.content.SetActive(false);

				buildingUI.colorRect.color = DistrictData.GetColor(card.building.district.color);
				buildingUI.title.text = card.building.title.ToUpper();

				buildingUI.cleanIcon.SetActive(false);
				buildingUI.image.gameObject.SetActive(false);
				buildingUI.constructionIcon.SetActive(false);
				buildingUI.currencyIcon.SetActive(false);

				bool showBoughtBuildings = false;
				if (card.building.constructionState == ConstructionState.NeedToClean)
				{
					buildingUI.cleanIcon.SetActive(true);
					buildingUI.price.text = "";
				}
				else if ((card.building.constructionState == ConstructionState.NotBuilt) ||
					((card.building.constructionState == ConstructionState.Completed) &&
					(card.building.propertyState == PropertyState.NotOwned)))
				{
					buildingUI.image.gameObject.SetActive(true);
					buildingUI.price.text = card.building.constructionPrice.ToString() + "$";
				}
				else if (card.building.constructionState == ConstructionState.InProgress)
				{
					buildingUI.constructionIcon.SetActive(true);
					buildingUI.price.text = "";
				}
				else if ((card.building.constructionState == ConstructionState.Completed) &&
					(card.building.propertyState != PropertyState.NotOwned))
				{
					showBoughtBuildings = true;
					buildingUI.currencyIcon.SetActive(true);
					buildingUI.price.text = "+" + card.building.district.GetRent() + "$";
				}

				if (!showBoughtBuildings)
				{
					for (int i = 0; i < buildingUI.boughtBuildings.Length; i++)
					{
						buildingUI.boughtBuildings[i].SetActive(false);
					}
				}
				else
				{
					int boughtBuildingsCount = card.building.district.GetCompletedBuildingsCount();
					for (int i = 0; i < buildingUI.boughtBuildings.Length; i++)
					{
						buildingUI.boughtBuildings[i].SetActive((i + 1) <= boughtBuildingsCount);
					}
				}
				break;

			case CardType.Energy:
			case CardType.Currency:
			case CardType.LoanInterest:
			case CardType.RepayLoan:
			case CardType.Robbery:
				districtUI.content.SetActive(false);
				buildingUI.content.SetActive(false);
				resourceUI.content.SetActive(true);
				specialUI.content.SetActive(false);

				resourceUI.title.text = card.title.ToUpper();
				if (card.type == CardType.LoanInterest)
					resourceUI.amount.text = "-" + Profile.loanInterest + "$";
				else if (card.type == CardType.RepayLoan)
					resourceUI.amount.text = "-" + Profile.loan + "$";
				else if (card.type == CardType.Robbery)
					resourceUI.amount.text = "-" + card.amount + "$";
				else
					resourceUI.amount.text = "+" + card.amount;

				if (card.type == CardType.Energy)
					resourceUI.image.sprite = resourceUI.sprites[0];
				else if (card.type == CardType.Currency)
					resourceUI.image.sprite = resourceUI.sprites[1];
				else if ((card.type == CardType.LoanInterest) ||
					(card.type == CardType.RepayLoan))
					resourceUI.image.sprite = resourceUI.sprites[2];
				else if (card.type == CardType.Robbery)
					resourceUI.image.sprite = resourceUI.sprites[3];
				break;

			case CardType.Start:
			case CardType.MiniGame:
			case CardType.LuckyWheel:
			case CardType.Chance:
				districtUI.content.SetActive(false);
				buildingUI.content.SetActive(false);
				resourceUI.content.SetActive(false);
				specialUI.content.SetActive(true);

				specialUI.title.text = card.title.ToUpper();

				if (card.type == CardType.Start)
					specialUI.image.sprite = specialUI.sprites[0];
				else if (card.type == CardType.MiniGame)
					specialUI.image.sprite = specialUI.sprites[1];
				else if (card.type == CardType.LuckyWheel)
					specialUI.image.sprite = specialUI.sprites[2];
				else if (card.type == CardType.Chance)
					specialUI.image.sprite = specialUI.sprites[3];
				break;
		}
	}

	public void SetSelected(bool state, float delay = 1.0f)
	{
		if (state)
		{
			container.DOLocalMoveY(0.7f, 0.2f)
				.SetDelay(delay)
				.SetEase(Ease.OutQuad);
		}
		else
		{
			container.DOLocalMoveY(0.0f, 0.15f)
				.SetEase(Ease.OutCubic);
		}
	}
}

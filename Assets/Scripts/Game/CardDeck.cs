using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDeck
{
	private static List<CardData> cards;

	private static Randomizer indexRandomizer;
	private static Randomizer resultIndexRandomizer;

	private static int cardsCounter = 0;
	private static int regularResultsCounter = 0;
	private static int specialResultsCounter = 0;

	public static void Init()
	{
		cards = new List<CardData>();

		for (int i = 0; i < Profile.districts.Count; i++)
		{
			if (!Profile.districts[i].isActive)
			{
				Profile.districts[i].OnActivated += OnDistrictActivated;
				cards.Add(new CardData()
				{
					chance = 0.0f,
					type = CardType.District,
					district = Profile.districts[i]
				});
			}

			for (int j = 0; j < Profile.districts[i].buildings.Length; j++)
			{
				cards.Add(new CardData()
				{
					chance = 0.0f,
					type = CardType.Building,
					building = Profile.districts[i].buildings[j]
				});
			}
		}

		cards.Add(new CardData() { chance = 0.2f, type = CardType.Energy, title = "Energy", amount = 2 });
		cards.Add(new CardData() { chance = 0.15f, type = CardType.Energy, title = "Energy", amount = 5 });
		//cards.Add(new CardData() { chance = 0.1f, type = CardType.Energy, title = "Energy", amount = 15 });
		//cards.Add(new CardData() { chance = 0.05f, type = CardType.Energy, title = "Energy", amount = 20 });

		cards.Add(new CardData() { chance = 0.4f, type = CardType.Currency, title = "Monopoly bucks", amount = 25 });
		cards.Add(new CardData() { chance = 0.3f, type = CardType.Currency, title = "Monopoly bucks", amount = 50 });
		cards.Add(new CardData() { chance = 0.2f, type = CardType.Currency, title = "Monopoly bucks", amount = 75 });
		cards.Add(new CardData() { chance = 0.1f, type = CardType.Currency, title = "Monopoly bucks", amount = 100 });

		//cards.Add(new CardData() { chance = 2.0f, type = CardType.LoanInterest, title = "Loan interest" });
		//cards.Add(new CardData() { chance = 2.0f, type = CardType.RepayLoan, title = "Repay the loan" });

		//cards.Add(new CardData() { chance = 1.0f, type = CardType.Robbery, title = "Robbery" });

		cards.Add(new CardData() { chance = 0.0f, type = CardType.Start, title = "Start" });
		cards.Add(new CardData() { chance = 0.1f, type = CardType.MiniGame, title = "Mini game" });
		cards.Add(new CardData() { chance = 0.1f, type = CardType.LuckyWheel, title = "Lucky wheel" });
		cards.Add(new CardData() { chance = 0.1f, type = CardType.Chance, title = "Chance" });

		indexRandomizer = new Randomizer(cards.Count, 5);
		resultIndexRandomizer = new Randomizer(cards.Count, 2);

		Profile.OnWaveChanged += OnWaveChanged;
		UpdateCardChances();
	}

	private static void OnDistrictActivated(DistrictData district)
	{
		district.OnActivated -= OnDistrictActivated;
		UpdateCardChances();
	}

	private static void OnWaveChanged()
	{
		UpdateCardChances();
	}

	private static void UpdateCardChances()
	{
		for (int i = 0; i < cards.Count; i++)
		{
			if (cards[i].type == CardType.District)
			{
				if ((cards[i].district.wave <= Profile.wave) &&
					!cards[i].district.isActive)
				{
					cards[i].chance = 2.0f;
				}
				else
				{
					cards[i].chance = 0.0f;
				}
			}

			if (cards[i].type == CardType.Building)
			{
				if ((cards[i].building.district.wave <= Profile.wave) &&
					cards[i].building.district.isActive)
				{
					cards[i].chance = 1.0f;
				}
				else
				{
					cards[i].chance = 0.0f;
				}
			}
		}
	}

	public static CardData GetNextCard(bool isResult = false)
	{
		cardsCounter++;
		if (cardsCounter == ((CardsList.itemsCount / 2) + 1))
		{
			return GetCardsByType(CardType.Start)[0];
		}

		List<int> lastIndexes = indexRandomizer.GetLastIndexes();
		List<int> resultLastIndexes = new List<int>();
		if (isResult)
		{
			regularResultsCounter++;
			if (regularResultsCounter == 5)
			{
				regularResultsCounter = 0;
				return GetSpecialCard();
			}
			resultLastIndexes = resultIndexRandomizer.GetLastIndexes();
		}

		List<CardData> freeCards = new List<CardData>();
		float chanceSum = 0.0f;
		for (int i = 0; i < cards.Count; i++)
		{
			bool canAdd = true;
			//if ((cards[i].type == CardType.Building) &&
			//	((cards[i].building.state == BuildingState.NeedToClean) ||
			//	(cards[i].building.state == BuildingState.InConstruction)))
			//{
			//	canAdd = false;
			//}

			//if (((cards[i].type == CardType.LoanInterest) || (cards[i].type == CardType.RepayLoan)) &&
			//	(Profile.loan == 0))
			//{
			//	canAdd = false;
			//}

			if (isResult &&
				((cards[i].type == CardType.MiniGame) || (cards[i].type == CardType.LuckyWheel) ||
				(cards[i].type == CardType.Chance)))
			{
				canAdd = false;
			}

			if (isResult && (cards[i].type == CardType.District) &&
				(cards[i].district.activationPrice > Profile.currency))
			{
				canAdd = false;
			}

			if (isResult)
			{
				if (canAdd && !lastIndexes.Contains(i) && !resultLastIndexes.Contains(i) && (cards[i].chance > 0.0f))
				{
					freeCards.Add(cards[i]);
					chanceSum += cards[i].chance;
				}
			}
			else
			{
				if (canAdd && !lastIndexes.Contains(i) && (cards[i].chance > 0.0f))
				{
					freeCards.Add(cards[i]);
					chanceSum += cards[i].chance;
				}
			}
		}

		float chance = Random.value * chanceSum;
		float chanceAcc = 0.0f;
		CardData selectedCard = null;

		for (int i = 0; i < freeCards.Count; i++)
		{
			if ((chanceAcc + freeCards[i].chance) >= chance)
			{
				selectedCard = freeCards[i];
				break;
			}
			chanceAcc += freeCards[i].chance;
		}
		if ((chance > chanceAcc) && (selectedCard == null))
		{
			selectedCard = freeCards[freeCards.Count - 1];
		}

		int index = cards.IndexOf(selectedCard);
		indexRandomizer.AddLastIndex(index);
		if (isResult)
		{
			resultIndexRandomizer.AddLastIndex(index);
		}

		return selectedCard;
	}

	private static CardData GetSpecialCard()
	{
		specialResultsCounter++;

		CardData selectedCard = null;
		switch (specialResultsCounter % 3)
		{
			case 0:
				selectedCard = GetCardsByType(CardType.Chance)[0];
				break;
			case 1:
				selectedCard = GetCardsByType(CardType.LuckyWheel)[0];
				break;
			case 2:
				selectedCard = GetCardsByType(CardType.MiniGame)[0];
				break;
		}

		return selectedCard;
	}

	public static List<CardData> GetCardsByType(CardType type)
	{
		List<CardData> selectedCards = new List<CardData>();
		for (int i = 0; i < cards.Count; i++)
		{
			if (cards[i].type == type)
			{
				selectedCards.Add(cards[i]);
			}
		}
		return selectedCards;
	}
}

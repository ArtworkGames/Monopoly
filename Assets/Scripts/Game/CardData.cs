using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
	None = 0,

	District,
	Building,

	Energy,
	Currency,
	LoanInterest,
	RepayLoan,
	Robbery,

	Start,
	LuckyWheel,
	MiniGame,
	Chance,
	Disaster,
};

public class CardData
{
	public float chance;
	public CardType type;
	public DistrictData district;
	public BuildingData building;
	public string title;
	public int amount;
	public bool isNewShown;

	public override string ToString()
	{
		string str = "type=" + type.ToString();
		if (building != null)
		{
			str += ", color=" + building.district.color.ToString();
			str += ", title=" + building.title;
			str += ", price=" + building.constructionPrice;
		}
		if (!string.IsNullOrEmpty(title)) str += ", title=" + title;
		if (amount != 0) str += ", amount=" + amount;
		return str;
	}
}

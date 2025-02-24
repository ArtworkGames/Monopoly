using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonTools
{
	public static string FromStringIntPairs(List<KeyValuePair<string, int>> items)
	{
		string str = "";
		for (int i = 0; i < items.Count; i++)
		{
			str += items[i].Value + ":" + items[i].Key;
			if (i < (items.Count - 1)) str += ",";
		}

		return str;
	}

	public static List<KeyValuePair<string, int>> ToStringIntPairs(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return new List<KeyValuePair<string, int>>();
		}

		List<KeyValuePair<string, int>> pairs = new List<KeyValuePair<string, int>>();
		string[] pairStrs = str.Split(",");
		foreach (string pairStr in pairStrs)
		{
			pairs.Add(ToStringIntPair(pairStr));
		}

		return pairs;
	}

	public static KeyValuePair<string, int> ToStringIntPair(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return new KeyValuePair<string, int>();
		}

		KeyValuePair<string, int> pair = new KeyValuePair<string, int>();
		string[] values = str.Split(':');
		if (int.TryParse(values[0], out int count))
		{
			pair = new KeyValuePair<string, int>(values[1], count);
		}
		else
		{
			Debug.LogError("Can't parse pair " + str);
		}

		return pair;
	}

	public static List<KeyValuePair<string, float>> ToStringFloatPairs(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return new List<KeyValuePair<string, float>>();
		}

		List<KeyValuePair<string, float>> pairs = new List<KeyValuePair<string, float>>();
		string[] pairStrs = str.Split(",");
		foreach (string pairStr in pairStrs)
		{
			pairs.Add(ToStringFloatPair(pairStr));
		}

		return pairs;
	}

	public static KeyValuePair<string, float> ToStringFloatPair(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return new KeyValuePair<string, float>();
		}

		KeyValuePair<string, float> pair = new KeyValuePair<string, float>();
		string[] values = str.Split(':');
		if (float.TryParse(values[0], out float count))
		{
			pair = new KeyValuePair<string, float>(values[1], count);
		}
		else
		{
			Debug.LogError("Can't parse pair " + str);
		}

		return pair;
	}
}

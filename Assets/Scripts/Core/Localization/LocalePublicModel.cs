using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArtworkGames.Localization
{
	public class LocalePublicModel
	{
		private Dictionary<string, string> _strings;

		public void Init(Dictionary<string, string> strings)
		{
			_strings = strings;
		}

		public string GetString(string key)
		{
			if (string.IsNullOrEmpty(key) || (_strings == null))
				return string.Empty;

			if (!_strings.ContainsKey(key))
				return $"Locale [{key}] not found!";

			string str = _strings[key];

			if (string.IsNullOrEmpty(str))
				return $"Locale [{key}] is empty!";

			return str;
		}
	}
}

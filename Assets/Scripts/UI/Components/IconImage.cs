using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace ArtworkGames.DiceValley.UI.Components
{
	public enum IconType { Items };

	[RequireComponent(typeof(Image))]
	public class IconImage : MonoBehaviour
	{
		[SerializeField] private IconType _type;

		public async UniTask LoadIconAsync(string id)
		{
			Image _image = GetComponent<Image>();

			try
			{
				_image.sprite = await Addressables.LoadAssetAsync<Sprite>(GetIconPath(id));
			}
			catch (Exception e)
			{
				Debug.LogError("Error loading icon \"" + GetIconPath(id) + "\": " + e.Message);
			}
		}

		private string GetIconPath(string id)
		{
			return $"Icons/{_type}/{id}.png";
		}
	}
}

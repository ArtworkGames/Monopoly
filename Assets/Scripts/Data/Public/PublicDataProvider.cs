using ArtworkGames.Initialization;
using ArtworkGames.Signals;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ArtworkGames.DiceValley.Data.Public
{
	public class PublicDataProvider : BaseInitializable
	{
		private List<IPublicModel> _models;

		public PublicDataProvider(
			IPublisher<RegisterInitializableSignal> registerPublisher) : base(registerPublisher)
		{
		}

		public override async UniTask InitializeAsync()
		{
			_models = new List<IPublicModel>();

			_models.Add(new ProductsPublicModel());
			_models.Add(new ItemsPublicModel());
			_models.Add(new LevelsPublicModel());
			_models.Add(new BuildingsPublicModel());

			PublicData data = await LoadDataAsync();

			foreach (IPublicModel model in _models)
			{
				model.Init(data);
			}

			int n = 0;
			do
			{
				InitializationProgress = (1.0f / 5.0f) * n;
				await UniTask.WaitForSeconds(0.1f);
				n++;
			}
			while (n <= 5);
		}

		private async UniTask<PublicData> LoadDataAsync()
		{
			PublicData data = new PublicData();

			data.products = await LoadSchemasAsync<ProductPublicSchema>("products");
			data.items = await LoadSchemasAsync<ItemPublicSchema>("items");
			data.levels = await LoadSchemasAsync<LevelPublicSchema>("levels");
			data.buildings = await LoadSchemasAsync<BuildingPublicSchema>("buildings");

			return data;
		}

		private async UniTask<T[]> LoadSchemasAsync<T>(string name)
		{
			try
			{
				TextAsset textAsset = await Addressables.LoadAssetAsync<TextAsset>(GetDataPath(name));
				T[] data = JsonConvert.DeserializeObject<T[]>(textAsset.text);
				return data;
			}
			catch (Exception e)
			{
				Debug.LogError("Error reading asset \"" + GetDataPath(name) + "\": " + e.Message);
			}
			return new T[0];
		}

		private string GetDataPath(string name)
		{
			return $"Data/{name}.json";
		}

		public T Get<T>() where T : class, IPublicModel
		{
			return (T)_models.Find(m => m is T);
		}
	}
}

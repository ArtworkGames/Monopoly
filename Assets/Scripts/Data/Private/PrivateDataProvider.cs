using ArtworkGames.Initialization;
using ArtworkGames.Signals;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ArtworkGames.DiceValley.Data.Private
{
	public class PrivateDataProvider : BaseInitializable
	{
		private List<IPrivateModel> _models;

		public PrivateDataProvider(
			IPublisher<RegisterInitializableSignal> registerPublisher) : base(registerPublisher)
		{
		}

		public override async UniTask InitializeAsync()
		{
			_models = new List<IPrivateModel>();

			_models.Add(new PlayerPrivateModel());
			_models.Add(new ItemsPrivateModel());
			_models.Add(new RewardsPrivateModel());
			_models.Add(new BuildingsPrivateModel());

			PrivateData data = await LoadDataAsync();

			foreach (IPrivateModel model in _models)
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

		private async UniTask<PrivateData> LoadDataAsync()
		{
			PrivateData data = new PrivateData();

			data.player = await LoadSchemaAsync<PlayerPrivateSchema>(Get<PlayerPrivateModel>().Name);
			data.items = await LoadSchemasAsync<ItemPrivateSchema>(Get<ItemsPrivateModel>().Name);
			data.rewards = await LoadSchemasAsync<RewardPrivateSchema>(Get<RewardsPrivateModel>().Name);
			data.buildings = await LoadSchemasAsync<BuildingPrivateSchema>(Get<BuildingsPrivateModel>().Name);

			return data;
		}

		private async UniTask<T> LoadSchemaAsync<T>(string name) where T : class, new()
		{
			try
			{
				if (PlayerPrefs.HasKey(name))
				{
					string text = PlayerPrefs.GetString(name);
					T data = JsonConvert.DeserializeObject<T>(text);
					return data;
				}
			}
			catch (Exception e)
			{
				Debug.LogError("Error reading player prefs \"" + name + "\": " + e.Message);
			}
			return new T();
		}

		private async UniTask<List<T>> LoadSchemasAsync<T>(string name)
		{
			try
			{
				if (PlayerPrefs.HasKey(name))
				{
					string text = PlayerPrefs.GetString(name);
					List<T> data = JsonConvert.DeserializeObject<List<T>>(text);
					return data;
				}
			}
			catch (Exception e)
			{
				Debug.LogError("Error reading player prefs \"" + name + "\": " + e.Message);
			}
			return new List<T>();
		}

		public T Get<T>() where T : class, IPrivateModel
		{
			return (T)_models.Find(m => m is T);
		}

		public void SaveModel<T>() where T : class, IPrivateModel
		{
			IPrivateModel model = Get<T>();
			if ((model == null) || !model.IsDirty) return;

			Debug.Log($"Save [{model.Name}] model");

			string json = JsonConvert.SerializeObject(model.GetData());
			PlayerPrefs.SetString(model.Name, json);
			PlayerPrefs.Save();
			model.ResetDirty();
		}

		public void Reset()
		{
			PlayerPrefs.DeleteAll();
			PlayerPrefs.Save();
		}
	}
}

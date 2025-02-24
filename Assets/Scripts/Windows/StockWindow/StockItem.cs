using ArtworkGames.DiceValley.Data.Public;
using ArtworkGames.DiceValley.Signals;
using ArtworkGames.DiceValley.UI.Components;
using MessagePipe;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace ArtworkGames.DiceValley.Windows.StockWindow
{
	public class StockItem : MonoBehaviour
	{
		public Action<StockItem> OnClick;

		[SerializeField] private IconImage _image;
		[SerializeField] private TMP_Text _title;
		[SerializeField] private TMP_Text _count;
		[SerializeField] private GameObject _searchIcon;

		[HideInInspector] public ItemPublicSchema PublicSchema;
		[HideInInspector] public int Count;

		private IDisposable _subscriptions;

		//private BuildingData productBuilding;

		[Inject]
		public void Construct(
			ISubscriber<StockItemChangedSignal> itemChangedSubscriber)
		{
			DisposableBagBuilder bag = DisposableBag.CreateBuilder();
			itemChangedSubscriber.Subscribe(OnItemChanged).AddTo(bag);
			_subscriptions = bag.Build();
		}

		public void Dispose()
		{
			_subscriptions.Dispose();
		}

		private async void Start()
		{
			if (PublicSchema == null) return;

			_title.text = PublicSchema.id;
			UpdateComponents();

			_image.gameObject.SetActive(false);
			await _image.LoadIconAsync(PublicSchema.id);
			_image.gameObject.SetActive(true);

			//productBuilding = Profile.FindBuilding(productProfile.type);
			//productBuilding.OnConstructionStateChanged += OnBuildingConstructionStateChanged;

		}

		private void OnDestroy()
		{
			OnClick = null;
			PublicSchema = null;
		}

		private void UpdateComponents()
		{
			_count.text = Count.ToString();

			//_title.text = productProfile.title + "\n" + productProfile.count;
			//_searchIcon.SetActive(productBuilding.constructionState == ConstructionState.Completed);
		}

		private void OnItemChanged(StockItemChangedSignal signal)
		{
			if (PublicSchema == null) return;

			if (signal.ItemId.Equals(PublicSchema.id))
			{
				UpdateComponents();
			}
		}

		public void OnItemClick()
		{
			OnClick?.Invoke(this);
		}
	}
}

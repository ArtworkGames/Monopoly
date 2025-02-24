using ArtworkGames.DiceValley.Data.Public;
using ArtworkGames.DiceValley.Signals;
using ArtworkGames.Initialization;
using ArtworkGames.Signals;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArtworkGames.DiceValley.Managers
{
	public class BankManager : BaseInitializable
	{
		private PublicDataProvider _publicDataProvider;
		private IPublisher<RewardSignal> _rewardPublisher;

		private ProductsPublicModel _productsPublicModel;

		protected override List<Type> Dependencies => new List<Type> { typeof(PublicDataProvider) };

		public BankManager(
			IPublisher<RegisterInitializableSignal> registerPublisher,
			PublicDataProvider publicDataProvider,
			IPublisher<RewardSignal> rewardPublisher) : base(registerPublisher)
		{
			_publicDataProvider = publicDataProvider;
			_rewardPublisher = rewardPublisher;
		}

		public override async UniTask InitializeAsync()
		{
			_productsPublicModel = _publicDataProvider.Get<ProductsPublicModel>();
		}

		public ProductPublicSchema GetProduct(string id)
		{
			return _productsPublicModel.GetProduct(id);
		}

		public ProductPublicSchema[] GetProducts(ProductType type)
		{
			return _productsPublicModel.GetProducts(type);
		}

		public void BuyProduct(string id)
		{
			ProductPublicSchema product = GetProduct(id);
			BuyProduct(product);
		}

		public void BuyProduct(ProductPublicSchema product)
		{
			_rewardPublisher.Publish(new RewardSignal(product.Items)
			{
				Type = RewardType.ItemWindow
			});
		}
	}
}

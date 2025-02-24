using System.Linq;

namespace ArtworkGames.DiceValley.Data.Public
{
	public class ProductsPublicModel : IPublicModel
	{
		private ProductPublicSchema[] _products;

		public string Name => "products";

		public void Init(PublicData data)
		{
			_products = data.products;
		}

		public ProductPublicSchema GetProduct(string id)
		{
			return _products.FirstOrDefault(product => product.id == id);
		}

		public ProductPublicSchema[] GetProducts(ProductType type)
		{
			return _products.Where(i => i.type == type).ToArray();
		}
	}
}

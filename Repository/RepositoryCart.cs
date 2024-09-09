using MyGameStore.Repository.Interfaces;
using MyGameStoreModel.Entities;

namespace MyGameStore.Repository;

public class RepositoryCart : IRepositoryCart
{
	private readonly List<GameProduct> gameProducts = [];

	public decimal SumProduct => gameProducts.Sum(gameProduct => gameProduct.Price);

	public void Add(GameProduct gameProduct)
	{
		gameProducts.Add(gameProduct);
	}

	public void Delete(int id)
	{
		var deletingGameProducts = gameProducts.First(gameProduct => gameProduct.Id == id);
		gameProducts.Remove(deletingGameProducts);
	}

	public IEnumerable<GameProduct> GetProducts() =>
		gameProducts;

	public void Clear() =>
		gameProducts.Clear();

}

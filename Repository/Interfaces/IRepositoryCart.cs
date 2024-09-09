using MyGameStoreModel.Entities;
namespace MyGameStore.Repository.Interfaces;

public interface IRepositoryCart
{
	IEnumerable<GameProduct> GetProducts();
	decimal SumProduct { get; }
	void Add(GameProduct gameProduct);
	void Delete(int id);
	void Clear();
}

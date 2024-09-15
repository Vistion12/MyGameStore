using MyGameStoreModel.Entities;

namespace MyGameStore.Repository.Interfaces;

public interface IRepositoryWishList
{
	Task AddAsync(User user, GameProduct gameProduct);
	Task DeleteAsync(int idGameProduct, string idUser);

}

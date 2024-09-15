using Microsoft.EntityFrameworkCore;
using MyGameStore.Repository.Interfaces;
using MyGameStoreModel.Data;
using MyGameStoreModel.Entities;

namespace MyGameStore.Repository;

public class RepositoryWishList(GameShopContext gameShopContext) : IRepositoryWishList
{
	public async Task AddAsync(User User, GameProduct gameProduct)
	{
		var newWishList = new WishList
		{
			Gameproduct = gameProduct,
			user = User
		};

		await gameShopContext.WishList.AddAsync(newWishList);
		await gameShopContext.SaveChangesAsync();
	}

	public async Task DeleteAsync(int idGameProduct, string idUser)
	{
		await gameShopContext.WishList
			.Include(wishlist => wishlist.user)
			.Include(wishlist => wishlist.Gameproduct)
			.Where(wishlist => wishlist.user.Id == idUser && wishlist.Gameproduct.Id == idGameProduct)
			.ExecuteDeleteAsync();
	}
}

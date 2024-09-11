using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGameStore.Repository;
using MyGameStore.Repository.Interfaces;
using MyGameStore.ViewModel;
using MyGameStoreModel.Data;
using MyGameStoreModel.Entities;
using MyGameStoreModel.Repositories;
using MyGameStoreModel.Repositories.Interfaces;
using System.Security.Claims;

namespace MyGameStore.Controllers;

public class GameController(GameShopContext gameShopContext, IGameProductRepository gameProductRepository, IRepositoryCart repositoryCart, IHttpContextAccessor httpContextAccessor) : Controller
{
	public async Task<IActionResult> Details(int id)
	{

		var idUser = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

		var gameProduct = await gameShopContext.GameProducts
			.Include(gameProduct => gameProduct.Genre)
			.Include(gameProduct => gameProduct.ImageUrls)
			.Include(gameProduct => gameProduct.Videos)
			.Include(gameProduct => gameProduct.MinimumSystemRequirements)
			.Include(gameProduct => gameProduct.RecommendedSystemRequirements)
			.FirstAsync(gameProduct => gameProduct.Id == id);

		var WishList = gameShopContext.WishList
			.Include(wishlist => wishlist.user)
			.Include(wishlist => wishlist.Gameproduct)
			.Where(wishlist => wishlist.user.Id == idUser && wishlist.Gameproduct.Id == id);

		var cart = gameShopContext.carts
			.Include(wishlist => wishlist.User)
			.Include(wishlist => wishlist.gameProducts)
			.Where(wishlist => wishlist.User.Id == idUser && wishlist.gameProducts.Contains(gameProduct));

		var wishGameProduct = new WishGameProduct
		{
			gameProduct = gameProduct,

		};


		if (WishList.Any())
		{
			wishGameProduct.ContainsWishGameProducts = true;

		}
		
		if(cart.Any())
		{
			wishGameProduct.ContainsGameProducts = true;
		}


		return View(wishGameProduct);
	}
	public async Task<IActionResult> Add(int id)
	{
		var gameProduct = await gameProductRepository.GetGameProductsAsync(id);

		repositoryCart.Add(gameProduct);

		return Redirect("~/Cart/Index");
	}
	public async Task<IActionResult> AddWishList(int Id)
	{
		var idUser = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
		var User = await gameShopContext.Users.FirstAsync(user => user.Id == idUser);
		var gameProduct = await gameShopContext.GameProducts
			.Include(gameProduct => gameProduct.Genre)
			.Include(gameProduct => gameProduct.ImageUrls)
			.Include(gameProduct => gameProduct.Videos)
			.Include(gameProduct => gameProduct.MinimumSystemRequirements)
			.Include(gameProduct => gameProduct.RecommendedSystemRequirements)
			.FirstAsync(gameProduct => gameProduct.Id == Id);

		var newWishList = new WishList
		{
			Gameproduct = gameProduct,
			user = (User)User
		};
		

		var wishGameProduct = new WishGameProduct
		{
			gameProduct = gameProduct,

		};

		wishGameProduct.ContainsWishGameProducts = true;

		var cart = gameShopContext.carts
			.Include(wishlist => wishlist.User)
			.Include(wishlist => wishlist.gameProducts)
			.Where(wishlist => wishlist.User.Id == idUser && wishlist.gameProducts.Contains(gameProduct));

		if (cart.Any())
		{
			wishGameProduct.ContainsGameProducts = true;
		}

		await gameShopContext.WishList.AddAsync(newWishList);
		await gameShopContext.SaveChangesAsync();

		return View("Details", wishGameProduct); // 
	}

	public async Task<IActionResult> DeleteWishList(int id)
	{
		var idUser = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
		await gameShopContext.WishList
			.Include(wishlist => wishlist.user)
			.Include(wishlist => wishlist.Gameproduct)
			.Where(wishlist=>wishlist.user.Id==idUser && wishlist.Gameproduct.Id==id)
			.ExecuteDeleteAsync();
		var gameProduct = await gameShopContext.GameProducts
			.Include(gameProduct => gameProduct.Genre)
			.Include(gameProduct => gameProduct.ImageUrls)
			.Include(gameProduct => gameProduct.Videos)
			.Include(gameProduct => gameProduct.MinimumSystemRequirements)
			.Include(gameProduct => gameProduct.RecommendedSystemRequirements)
			.FirstAsync(gameProduct => gameProduct.Id == id);

		var cart = gameShopContext.carts
			.Include(wishlist => wishlist.User)
			.Include(wishlist => wishlist.gameProducts)
			.Where(wishlist => wishlist.User.Id == idUser && wishlist.gameProducts.Contains(gameProduct));

		var wishGameProduct = new WishGameProduct
		{
			gameProduct = gameProduct,

		};

		if (cart.Any())
		{
			wishGameProduct.ContainsGameProducts = true;
		}
		return View("Details", wishGameProduct); 

	}
}

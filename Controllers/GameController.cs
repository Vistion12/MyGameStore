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

public class GameController(
	GameShopContext gameShopContext,
	IRepositoryWishList repositoryWishList,
	IGameProductRepository gameProductRepository, 
	IRepositoryCart repositoryCart, 
	IHttpContextAccessor httpContextAccessor) : Controller
{
	public async Task<IActionResult> Details(int id)
	{
		if(httpContextAccessor.HttpContext is null ||
			httpContextAccessor.HttpContext.User is null ||
			httpContextAccessor.HttpContext.User.Identity is null)
		{ 
			return BadRequest(); 
		}

		var gameProduct = await gameShopContext.GameProducts
			.Include(gameProduct => gameProduct.Genre)
			.Include(gameProduct => gameProduct.ImageUrls)
			.Include(gameProduct => gameProduct.Videos)
			.Include(gameProduct => gameProduct.MinimumSystemRequirements)
			.Include(gameProduct => gameProduct.RecommendedSystemRequirements)
			.SingleAsync(gameProduct => gameProduct.Id == id );

		var wishGameProductVM = new WishGameProductVM
		{
			gameProduct = gameProduct,

		};

		if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
		{
			var idUser = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

			wishGameProductVM.ContainsGameProducts =
				await gameShopContext.carts
				.Include(wishlist => wishlist.User)
				.Include(wishlist => wishlist.gameProducts)
				.Where(wishlist =>
				wishlist.User.Id == idUser &&
				wishlist.gameProducts.Contains(gameProduct))
				.AnyAsync();

			wishGameProductVM.ContainsWishGameProducts=
				await gameShopContext.WishList
				.Include(wishlist => wishlist.user)
				.Include(wishlist=> wishlist.Gameproduct)
				.Where(wishlist => 
				wishlist.user.Id ==  idUser &&
				wishlist.Gameproduct.Id== id)
				.AnyAsync();
		}
		return View(wishGameProductVM);		
	}
	public async Task<IActionResult> AddToCart(int id)
	{
		if (httpContextAccessor.HttpContext is null ||
			httpContextAccessor.HttpContext.User is null ||
			httpContextAccessor.HttpContext.User.Identity is null)
		{
			return BadRequest();
		}
		if(!httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
		{
			return RedirectToAction("Login", "Account");
		}
		var gameProduct = await gameProductRepository.GetGameProductsAsync(id);
		repositoryCart.Add(gameProduct);
		return RedirectToAction("Index","Cart");
	}

	public async Task<IActionResult> AddWishList(int id)
	{
		if (httpContextAccessor.HttpContext is null ||
			httpContextAccessor.HttpContext.User is null ||
			httpContextAccessor.HttpContext.User.Identity is null)
		{
			return BadRequest();
		}

		if (!httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
		{
			return RedirectToAction("Login", "Account");
		}

		var idUser = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
		var user = await gameShopContext.Users.SingleAsync(user => user.Id == idUser);
		
		var gameProduct = await gameProductRepository.GetGameProductsAsync(id);

		var newWishList = new WishList
		{
			Gameproduct = gameProduct,
			user = (User)user
		};

		await repositoryWishList.AddAsync((User)user, gameProduct);
		return RedirectToAction("Details", "Game", new { id = id });
		//return RedirectToAction("Index", new RouteValueDictionary(new 
		//	{ 
		//		controller = "Game", 
		//		action = "Details", 
		//		Id = id 
		//	}));

	}

	public async Task<IActionResult> DeleteWishList(int id)
	{
		if (httpContextAccessor.HttpContext is null ||
			httpContextAccessor.HttpContext.User is null ||
			httpContextAccessor.HttpContext.User.Identity is null)
		{
			return BadRequest();
		}

		var idUser = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

		await repositoryWishList.DeleteAsync(id,idUser);
		return RedirectToAction("Details", "Game", new { id = id });
		//return RedirectToAction("Index", new RouteValueDictionary(
		//	new
		//	{
		//		controller = "Game",
		//		action = "Details",
		//		Id = id
		//	}));


	}
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGameStore.Repository;
using MyGameStore.Repository.Interfaces;
using MyGameStoreModel.Data;
using MyGameStoreModel.Repositories;
using MyGameStoreModel.Repositories.Interfaces;

namespace MyGameStore.Controllers;

public class GameController(GameShopContext gameShopContext, IGameProductRepository gameProductRepository, IRepositoryCart repositoryCart)  : Controller
{
	public async Task <IActionResult> Details(int id)
	{
		var gameProduct = await gameShopContext.GameProducts
			.Include(gameProduct=> gameProduct.Genre)
			.Include(gameProduct=> gameProduct.ImageUrls)
			.Include(gameProduct=> gameProduct.Videos)
			.Include(gameProduct => gameProduct.MinimumSystemRequirements)
			.Include(gameProduct => gameProduct.RecommendedSystemRequirements)
			.FirstAsync(gameProduct => gameProduct.Id == id);
		return View(gameProduct);
	}
	public async Task<IActionResult> Add(int id)
	{
		var gameProduct = await gameProductRepository.GetGameProductsAsync(id);

		repositoryCart.Add(gameProduct);

		return Redirect("~/Cart/Index");
	}
}

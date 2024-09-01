using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGameStoreModel.Data;

namespace MyGameStore.Controllers;

public class GameController(GameShopContext gameShopContext) : Controller
{
	public async Task <IActionResult> Details(int id)
	{
		var gameProduct = await gameShopContext.GameProducts
			.Include(gameProduct=> gameProduct.Genre)
			.Include(gameProduct=> gameProduct.ImageUrls)
			.FirstAsync(gameProduct => gameProduct.Id == id);
		return View(gameProduct);
	}
}

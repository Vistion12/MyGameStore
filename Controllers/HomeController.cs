using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGameStoreModel.Data;
using MyGameStoreModel.Repositories.Interfaces;

namespace MyGameStore.Controllers;

public class HomeController(GameShopContext gameShopContext ,IGameProductRepository gameProductRepository) : Controller
{
    public async Task <IActionResult> Index()
    {
        var gameProducts = await gameProductRepository.GetAllGameProductsAsync();
        return View(gameProducts);
    }
    public IActionResult PopularGames()
    {        
        return View();
    }
    public IActionResult Recommendations()
    {        
        return View();
    }
    public async Task <IActionResult> WishList()
    {
       var wishList = await gameShopContext.WishList
            .Include(wishList => wishList.user)
            .Include(wishList => wishList.Gameproduct)
            .Where(wishlist => wishlist.user.Id == "93491a95-ecb3-4b81-9038-daa4b40c0ef0")
            .ToListAsync();
        
        return View(wishList);
    }
}

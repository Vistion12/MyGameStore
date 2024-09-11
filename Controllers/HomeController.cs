using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGameStoreModel.Data;
using MyGameStoreModel.Repositories.Interfaces;
using System.Security.Claims;

namespace MyGameStore.Controllers;

public class HomeController(GameShopContext gameShopContext ,IGameProductRepository gameProductRepository, IHttpContextAccessor httpContextAccessor) : Controller
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

        var idUser = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

        var wishList = await gameShopContext.WishList
            .Include(wishList => wishList.user)
            .Include(wishList => wishList.Gameproduct)
            .Where(wishlist => wishlist.user.Id == idUser)
            .ToListAsync();
        
        return View(wishList);
    }

    public async Task<IActionResult> DeleteWishList(int Id)
    {
        var idUser = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

        var wishlist = await gameShopContext.WishList
            .Include(wishlist => wishlist.user)
            .Include(wishlist => wishlist.Gameproduct)
            .Where(wishlist => wishlist.Gameproduct.Id == Id && wishlist.user.Id == idUser)
            .ExecuteDeleteAsync();

        return RedirectToAction("WishList","Home");
    }
}

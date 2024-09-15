using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGameStore.Extensions;
using MyGameStoreModel.Data;
using MyGameStoreModel.Entities;
using MyGameStoreModel.Repositories.Interfaces;
using System.Security.Claims;

namespace MyGameStore.Controllers;



public class HomeController(GameShopContext gameShopContext,
    IGameProductRepository gameProductRepository,
    IHttpContextAccessor httpContextAccessor) : Controller
{

    private const int countPopGames = 100;

    public async Task<IActionResult> Index()
    {
        var gameProducts = await gameProductRepository.GetAllGameProductsAsync();
        return View(gameProducts);
    }

    //public async Task<IActionResult> Index(string searchString)
    //{
    //    var gameProducts = await gameProductRepository.GetAllGameProductsAsync();
    //    gameProducts= gameProducts.Where(gameProduct=> gameProduct.Title.ToUpper().Contains(searchString.ToUpper())).ToList();
    //    return View(gameProducts);
    //}    

    public async Task<IActionResult> PopularGames()
    {
        //var currentDate = DateTime.UtcNow;
        //var monthAgo = new DateTime(currentDate.Year, currentDate.Month - 1, currentDate.Day);

        var Carts = await gameShopContext.carts
             .Include(cart => cart.gameProducts)
             .Between(cart => cart.DatePurchase, DateTime.UtcNow.Date, DateTime.UtcNow.Date)
             .ToListAsync();
        var dictionary = new Dictionary<int, GameProduct>();
        foreach (var cart in Carts)
        {
            if (dictionary.Count > 100)
            {
                break;
            }
            foreach (var gameProduct in cart.gameProducts)
            {
                if (dictionary.ContainsKey(gameProduct.Id))
                {
                    continue;
                }
                dictionary.Add(gameProduct.Id,gameProduct);
            }

        }

        return View(dictionary);
    }
    public IActionResult Recommendations() // эксперты рекомендовали
    {
        return View();
    }
    public async Task<IActionResult> WishList()
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

        return RedirectToAction("WishList", "Home");
    }
}

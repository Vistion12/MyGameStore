using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyGameStore.Extensions;
using MyGameStore.ViewModel;
using MyGameStoreModel.Data;
using MyGameStoreModel.Entities;
using MyGameStoreModel.Repositories.Interfaces;
using System.Security.Claims;

namespace MyGameStore.Controllers;



public class HomeController(GameShopContext gameShopContext,
    IGameProductRepository gameProductRepository,
    IHttpContextAccessor httpContextAccessor) : Controller
{
    private const int minValueMonth = -1;
    private const int countPopGames = 100;

    public async Task<IActionResult> Index(string GameGenre, string NameSearchString)
    {
		var gameProductGenre = from g in gameShopContext.Genres
						   select g;

		var gameProducts = from g in gameShopContext.GameProducts
                           select g;

        if (!string.IsNullOrEmpty(NameSearchString))
        {
            gameProducts = gameProducts.Where(
                gameProduct => 
                gameProduct.Title.ToUpper().Contains(NameSearchString.ToUpper()));
        }

		if (!string.IsNullOrEmpty(GameGenre))
		{
			gameProducts = gameProducts
                .Include(gameProduct => gameProduct.Genre)
                .Where(gameProduct => 
                            gameProduct.Genre.Where(genre=> 
                                    genre.Title.Contains(GameGenre)).Any());
		}

		var filteredGameProductVM = new FilteredGameProductVM
        { 
            GameGenres = new SelectList(await gameProductGenre.Select(genre=>genre.Title).ToListAsync()) ,
            GameProducts= await gameProducts.ToListAsync() 
        };

        
        return View(filteredGameProductVM);
    }

    //public async Task<IActionResult> Index(string searchString)
    //{
    //    var gameProducts = await gameProductRepository.GetAllGameProductsAsync();
    //    gameProducts= gameProducts.Where(gameProduct=> gameProduct.Title.ToUpper().Contains(searchString.ToUpper())).ToList();
    //    return View(gameProducts);
    //}    

    public async Task<IActionResult> PopularGames()
    {

        var Carts = await gameShopContext.carts
             .Include(cart => cart.gameProducts)
             .Where(cart => 
             cart.DatePurchase >= DateTime.UtcNow.Date.AddMonths(minValueMonth)&&
             cart.DatePurchase <= DateTime.UtcNow.Date)
             .ToListAsync();

        var dictionary = Carts.SelectMany(cart => cart.gameProducts)
            .GroupBy(gameProduct => gameProduct.Id)
            .Take(countPopGames)
            .ToDictionary(group => group.Key, group => group.First());

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

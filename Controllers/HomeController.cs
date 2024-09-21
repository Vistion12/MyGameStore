using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyGameStore.Core;
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
    private const int pageSize = 6;
    private const int minValueMonth = -1;
    private const int countPopGames = 100;

    public async Task<IActionResult> Index(
        string selectedGenreGameProduct,
        string selectedTitleGamePoduct,
		SortGameProductState? sortGameProductState,
        int page=1)
    {
		var gameProductGenre = from g in gameShopContext.Genres
						   select g.Title;

		var gameProducts = from g in gameShopContext.GameProducts
                           select g;

        if (!string.IsNullOrEmpty(selectedTitleGamePoduct))
        {
            gameProducts = gameProducts
                .Where(gameProduct => gameProduct.Title.ToUpper().Contains(selectedTitleGamePoduct.ToUpper()));
        }

		if (!string.IsNullOrEmpty(selectedGenreGameProduct))
		{
            gameProducts = gameProducts
                .Include(gameProduct => gameProduct.Genre)
                .Where(gameProduct =>
                            gameProduct.Genre.Any(genre => genre.Title.Contains(selectedGenreGameProduct)));
		}

        gameProducts = sortGameProductState switch
        {
            SortGameProductState.TitleAsc => gameProducts.OrderBy(gameProduct => gameProduct.Title),
            SortGameProductState.TitleDesc => gameProducts.OrderByDescending(gameProduct => gameProduct.Title),
           _ => gameProducts
        };

        var count = await gameProducts.CountAsync();
        var gameProductsResult = await gameProducts
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var gameProductsVM = new GameProductsVM
        {
            GameProducts = gameProductsResult,
            PageViewModel = new(count, page, pageSize),
            SortGPVM = new(sortGameProductState),
            FilteredGP_VM = new(new(gameProductGenre), selectedGenreGameProduct, selectedTitleGamePoduct)

        };
		return View(gameProductsVM);
    }

       

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

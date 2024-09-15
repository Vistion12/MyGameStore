using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGameStoreModel.Data;
using MyGameStoreModel.Entities;
using System.Security.Claims;

namespace MyGameStore.Controllers;

public class LibraryController(GameShopContext gameShopContext , IHttpContextAccessor httpContextAccessor) : Controller
{
    public async Task <IActionResult> Index()
    {
        if(!httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login","Account");
        }
        var idUser = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var gameProducts = new List<GameProduct>();
       var carts=  gameShopContext.carts
            .Include(cart => cart.User)
            .Include(cart => cart.gameProducts)
            .Where(cart => cart.User.Id == idUser);

        foreach(var item in carts)
        {
            gameProducts.AddRange(item.gameProducts);

		}
        return View(gameProducts);
    }
}

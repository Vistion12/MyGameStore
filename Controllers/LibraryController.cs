using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGameStoreModel.Data;
using System.Security.Claims;

namespace MyGameStore.Controllers;

public class LibraryController(GameShopContext gameShopContext , IHttpContextAccessor httpContextAccessor) : Controller
{
    public async Task <IActionResult> Index()
    {
        if(!httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account");
        }
        var idUser = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

       var cart = await gameShopContext.carts
            .Include(cart => cart.User)
            .Include(cart => cart.gameProducts)
            .Where(cart => cart.User.Id == idUser)
            .FirstAsync();

        
        return View(cart.gameProducts);
    }
}

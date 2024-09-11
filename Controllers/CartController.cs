using Microsoft.AspNetCore.Mvc;
using MyGameStore.Repository;
using MyGameStore.Repository.Interfaces;
using MyGameStore.ViewModel;
using MyGameStoreModel.Data;
using MyGameStoreModel.Entities;
using MyGameStoreModel.Repositories.Interfaces;
using System.Security.Claims;

namespace MyGameStore.Controllers;

public class CartController(GameShopContext gameShopContext, IRepositoryCart repositoryCart, IHttpContextAccessor httpContextAccessor): Controller
{
	public IActionResult Index()
	{
        var products = repositoryCart.GetProducts();

        var cartViewModel = new CartViewModel
        {
            gameProducts = products,
            SumgameProducts = repositoryCart.SumProduct
        };
		return View(cartViewModel);
	}

    public IActionResult Delete(int id)
    {
        repositoryCart.Delete(id);

        return Redirect("/Cart/Index");
    }
    public async Task <IActionResult> PlaceOrder()
    {
        var idUser = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = gameShopContext.Users.Where(u => u.Id == idUser).First();
        var products = repositoryCart.GetProducts();

        var cart = new Cart
        {
            gameProducts = [],
            Sum = repositoryCart.SumProduct,
            User = (User)user,
            DatePurchase=DateTime.Now,
        };
        foreach (var product in products)
        {
            var gameProduct  = gameShopContext.GameProducts.Where(gameProduct => gameProduct.Id == product.Id).First();
            cart.gameProducts.Add(gameProduct);

        }

        await gameShopContext.carts.AddAsync(cart);
        await gameShopContext.SaveChangesAsync();   


        repositoryCart.Clear();

        return RedirectToAction("Index", "Home");
    }

}

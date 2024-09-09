using Microsoft.AspNetCore.Mvc;
using MyGameStore.Repository.Interfaces;
using MyGameStore.ViewModel;
using MyGameStoreModel.Repositories.Interfaces;

namespace MyGameStore.Controllers;

public class CartController(IRepositoryCart repositoryCart): Controller
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
    public IActionResult PlaceOrder()
    {
        var products = repositoryCart.GetProducts();
        repositoryCart.Clear();

        return RedirectToAction("Index", "Home");
    }

}

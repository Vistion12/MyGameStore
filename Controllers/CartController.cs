using Microsoft.AspNetCore.Mvc;
using MyGameStore.Repository.Interfaces;
using MyGameStoreModel.Repositories.Interfaces;

namespace MyGameStore.Controllers;

public class CartController(IGameProductRepository gameProductRepository , IRepositoryCart repositoryCart): Controller
{
	public IActionResult Index()
	{
		var products = repositoryCart.GetProducts();
		return View(products);
	}

    public IActionResult Delete(int id)
    {
        repositoryCart.Delete(id);

        return Redirect("/Cart/Index");
    }

}

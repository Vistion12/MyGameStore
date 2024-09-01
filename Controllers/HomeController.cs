using Microsoft.AspNetCore.Mvc;
using MyGameStoreModel.Repositories.Interfaces;

namespace MyGameStore.Controllers;

public class HomeController(IGameProductRepository gameProductRepository) : Controller
{
    public async Task <IActionResult> Index()
    {
        var gameProducts = await gameProductRepository.GetAllGameProductsAsync();
        return View(gameProducts);
    }
}

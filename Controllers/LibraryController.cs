using Microsoft.AspNetCore.Mvc;

namespace MyGameStore.Controllers;

public class LibraryController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

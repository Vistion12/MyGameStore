using Microsoft.AspNetCore.Mvc;

namespace MyGameStore.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

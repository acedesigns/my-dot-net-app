/* =======================================================
 *
 * Created by anele on 27/08/2025.
 *
 * @anele_ace
 *
 * =======================================================
 */

using Microsoft.AspNetCore.Mvc;

namespace MyApi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Optional: pass some data to the view via ViewData or a model
            ViewData["Title"] = "Welcome to iiDENTIFii";
            return View();
        }
    }
}

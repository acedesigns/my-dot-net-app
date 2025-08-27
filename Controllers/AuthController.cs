/* =======================================================
 *
 * Created by anele on 27/08/2025.
 *
 * @anele_ace
 *
 * =======================================================
 */

using MyApi.Data;
using MyApi.Models;
using MyApi.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;

namespace MyApi.Controllers
{

    public class AuthController(AppDbContext db) : Controller
    {
        private readonly AppDbContext _db = db;

        // GET: /Auth/Register
        public IActionResult Register() => View();

        // POST: /Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Check if email exists
            if (await _db.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("", "Email already registered.");
                return View(model);
            }

            // Hash password (simple example; use stronger hashing like BCrypt in production)
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Login));
        }

        // GET: /Auth/Login
        public IActionResult Login() => View();

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            var claims = new List<Claim> {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MyCookieAuth", principal);

            return RedirectToAction("Index", "Posts");
        }

        // POST: /Auth/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction(nameof(Login));
        }

        // GET: /Auth/AccessDenied
        public IActionResult AccessDenied() => View();
    }
}

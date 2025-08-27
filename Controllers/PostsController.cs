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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyApi.Controllers
{
    public class PostsController(AppDbContext db) : Controller
    {
        private readonly AppDbContext _db = db;

        // GET: /Posts
        public async Task<IActionResult> Index(int page = 1) {
            int pageSize = 4;
            var totalPosts = await _db.Posts.CountAsync();

            var posts = await _db.Posts
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);
            return View(posts);
        }

        // GET: /Posts/Create
        public IActionResult Create() => View();

        // POST: /Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post)
        {
            if (!ModelState.IsValid)
                return View(post);
            Console.WriteLine($"Title: {post.Title}, Content: {post.Content}");

            post.CreatedAt = DateTime.UtcNow;
            _db.Posts.Add(post);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: /Posts/Like/5
        [HttpPost]
        public async Task<IActionResult> Like(int id)
        {
            var post = await _db.Posts.FindAsync(id);
            if (post == null) return NotFound();

            post.Likes = (post.Likes ?? 0) + 1;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

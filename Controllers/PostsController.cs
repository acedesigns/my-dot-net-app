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
using Microsoft.AspNetCore.Authorization;

namespace MyApi.Controllers
{
    
    public class PostsController : Controller
    {
        private readonly AppDbContext _db;

        public PostsController(AppDbContext db)
        {
            _db = db;
        }

        //GET: Post Details
        [Authorize(AuthenticationSchemes = "MyCookieAuth")]
        public async Task<IActionResult> Details(int id)
        {
            var post = await _db.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return NotFound();

            return View(post);
        }

        // GET: /Posts
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 4;
            var totalPosts = await _db.Posts.CountAsync();

            var posts = await _db.Posts
                .Include(p => p.User)
                .Include(p => p.Likes)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);
            return View(posts);
        }

        // GET: /Posts/Create
        [Authorize(AuthenticationSchemes = "MyCookieAuth")]
        public IActionResult Create() => View();

        // POST: /Posts/Create
        [Authorize(AuthenticationSchemes = "MyCookieAuth")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post)
        {
            if (!ModelState.IsValid)
                return View(post);

            post.CreatedAt = DateTime.UtcNow;

            // TODO: replace with your logged-in user ID
            post.UserId = GetCurrentUserId();

            _db.Posts.Add(post);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: /Posts/Like/5
        [Authorize(AuthenticationSchemes = "MyCookieAuth")]
        [HttpPost]
        public async Task<IActionResult> Like(int id)
        {
            var post = await _db.Posts
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return NotFound();

            int userId = GetCurrentUserId(); // get logged-in user ID

            // Prevent liking your own post
            if (post.UserId == userId)
            {
                TempData["Error"] = "You cannot like your own post.";
                return RedirectToAction(nameof(Index));
            }

            // Prevent liking more than once
            bool alreadyLiked = post.Likes.Any(l => l.UserId == userId);
            if (alreadyLiked)
            {
                TempData["Error"] = "You have already liked this post.";
                return RedirectToAction(nameof(Index));
            }

            // Add new like
            var like = new Like
            {
                PostId = post.Id,
                UserId = userId
            };

            _db.Likes.Add(like);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // TODO: Implement your method for retrieving the logged-in user
        private int GetCurrentUserId()
        {
            // Example placeholder - replace with real authentication
            return 1;
        }
    }
}

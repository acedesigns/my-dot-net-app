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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyApi.Controllers
{

    public class PostsController : Controller
    {
        private readonly AppDbContext _db;

        public PostsController(AppDbContext db)
        {
            _db = db;
        }

        private IQueryable<Post> FilterPostsByDate(IQueryable<Post> query, DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue)
                query = query.Where(p => p.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.CreatedAt <= endDate.Value);

            return query;
        }

        // GET: /Posts/ByAuthor/5
        public async Task<IActionResult> ByAuthor(int id, int page = 1)
        {
            const int pageSize = 4;

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            var totalPosts = await _db.Posts.CountAsync(p => p.UserId == id);

            var posts = await _db.Posts
                .Where(p => p.UserId == id)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.AuthorName = user.Username;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);

            return View(posts);
        }

        //GET: Post Details
        public async Task<IActionResult> Details(int id)
        {
            var post = await _db.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return NotFound();

            return View(post);
        }

        // GET: /Posts
        public async Task<IActionResult> Index(PostFilterViewModel filter, int page = 1)
        {
            const int pageSize = 4;

            var query = _db.Posts.Include(p => p.User).Include(p => p.Likes).AsQueryable();

            if (filter.SelectedAuthorId.HasValue)
                query = query.Where(p => p.UserId == filter.SelectedAuthorId.Value);


            if (filter.StartDate.HasValue)
                query = query.Where(p => p.CreatedAt >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(p => p.CreatedAt <= filter.EndDate.Value);

            var totalPosts = await query.CountAsync();

            filter.Posts = await query.OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var authors = await _db.Users.OrderBy(u => u.Username).ToListAsync();

            filter.AuthorsList = authors.Select(a =>
                new SelectListItem { Value = a.Id.ToString(), Text = a.Username }
            ).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);

            return View(filter);
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

            try {
                post.UserId = GetCurrentUserId();
            } catch {
                ModelState.AddModelError("", "Unable to determine logged-in user.");
                return View(post);
            }

            post.CreatedAt = DateTime.UtcNow;

            _db.Posts.Add(post);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: /Posts/Like/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(AuthenticationSchemes = "MyCookieAuth")]
        public async Task<IActionResult> Like(int id)
        {
            var post = await _db.Posts
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return NotFound();

            int userId = GetCurrentUserId();

            if (post.UserId == userId)
            {
                TempData["Error"] = "You cannot like your own post.";
                return RedirectToAction(nameof(Index));
            }

            if (post.Likes.Any(l => l.UserId == userId))
            {
                TempData["Error"] = "You have already liked this post.";
                return RedirectToAction(nameof(Index));
            }

            _db.Likes.Add(new Like { PostId = post.Id, UserId = userId });
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Get Logged In User
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new InvalidOperationException("Unable to determine the current logged-in user.");
            }

            return userId;
        }
        

        [HttpPost]
        [Authorize(AuthenticationSchemes = "MyCookieAuth")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int postId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Comment cannot be empty.";
                return RedirectToAction("Details", new { id = postId });
            }

            int userId;
            try
            {
                userId = GetCurrentUserId();
            }
            catch
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Details", new { id = postId });
            }

            var comment = new Comment
            {
                Content = content,
                CreatedAt = DateTime.UtcNow,
                PostId = postId,
                UserId = userId
            };

            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            return RedirectToAction("Details", new { id = postId });
        }
    }
}

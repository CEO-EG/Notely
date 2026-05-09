using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notely.Models;

namespace Notely.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public AdminController(AppDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        private async Task<bool> IsAdmin()
        {
            var user = await _userManager.GetUserAsync(User);
            return user != null && string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase);
        }

        private async Task<IActionResult?> RequireAdmin()
        {
            if (!await IsAdmin())
                return RedirectToAction("AccessDenied", "Error");

            return null;
        }

        public async Task<IActionResult> Index()
        {
            var guardResult = await RequireAdmin();
            if (guardResult != null)
                return guardResult;

            var model = new AdminDashboardViewModel
            {
                Users = await _context.Users
                    .OrderBy(u => u.Created_at)
                    .ToListAsync(),
                Notes = await _context.Notes
                    .Include(n => n.User)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var guardResult = await RequireAdmin();
            if (guardResult != null)
                return guardResult;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            var notes = await _context.Notes
                .Where(n => n.UserId == user.Id)
                .ToListAsync();

            foreach (var note in notes)
            {
                if (!string.IsNullOrEmpty(note.ImagePath))
                {
                    var imagePath = Path.Combine(
                        _env.WebRootPath,
                        note.ImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
                    );

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
            }

            _context.Notes.RemoveRange(notes);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(user.ProfileImagepath))
            {
                var profilePath = Path.Combine(
                    _env.WebRootPath,
                    user.ProfileImagepath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
                );

                if (System.IO.File.Exists(profilePath))
                {
                    System.IO.File.Delete(profilePath);
                }
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var guardResult = await RequireAdmin();
            if (guardResult != null)
                return guardResult;

            var note = await _context.Notes.FindAsync(id);

            if (note == null)
                return NotFound();

            if (!string.IsNullOrEmpty(note.ImagePath))
            {
                var oldImagePath = Path.Combine(
                    _env.WebRootPath,
                    note.ImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
                );

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

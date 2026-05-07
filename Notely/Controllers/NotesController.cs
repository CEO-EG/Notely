using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notely.Models;

namespace Notely.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotesController(AppDbContext context, UserManager<ApplicationUser> userManager , IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }





        // GET: Notes
        public async Task<IActionResult> Index(string? search)
        {
            var userId = _userManager.GetUserId(User);

            var notes = _context.Notes
                .Where(n => n.UserId == userId || n.State);
            if (!string.IsNullOrEmpty(search))
            {
                notes = notes.Where(n => n.Title.Contains(search));
            }

            return View(await notes.ToListAsync());
        }
        public async Task<IActionResult> Private()
        {
            var userId = _userManager.GetUserId(User);

            var notes = await _context.Notes
                .Where(n => n.UserId == userId && !n.State)
                .ToListAsync();

            return View("Index", notes);
        }

        public async Task<IActionResult> Public()
        {
            var notes = await _context.Notes
                .Where(n => n.State)
                .ToListAsync();

            return View("Index", notes);
        }

        //GET: Notes/Details/id
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var note = await _context.Notes
                .Include(n => n.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            var userId = _userManager.GetUserId(User);

            if (note == null || (note.UserId != userId && !note.State))
                return Unauthorized();

            return View(note);
        }

        // GET: Notes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Notes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Note note)
        {
            // 🔥 ربط النوت بالمستخدم الحالي
            note.UserId = _userManager.GetUserId(User);

            // 🔥 تاريخ الإنشاء
            note.CreatedAt = DateTime.Now;


            if (!ModelState.IsValid)
                return View(note);

            // 🔥 رفع الصورة
            if (note.ImageFile != null)
            {
                var folder = Path.Combine(_env.WebRootPath, "imgs");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid().ToString()
                               + Path.GetExtension(note.ImageFile.FileName);

                var fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await note.ImageFile.CopyToAsync(stream);
                }

                note.ImagePath = "/imgs/" + fileName;
            }

            // 🔥 حفظ النوت
            _context.Notes.Add(note);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Notes/Edit/id
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var note = await _context.Notes.FindAsync(id);
            var userId = _userManager.GetUserId(User);

            if (note == null || note.UserId != userId)
                return Unauthorized();

            return View(note);
        }

        // POST: Notes/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Note note)
        {
            if (id != note.Id) return NotFound();

            var existingNote = await _context.Notes.FindAsync(id);
            var userId = _userManager.GetUserId(User);

            if (existingNote == null || existingNote.UserId != userId)
                return Unauthorized();

            if (ModelState.IsValid)
            {
                existingNote.Title = note.Title;
                existingNote.Content = note.Content;
                existingNote.State = note.State;
                if (note.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "imgs");

                    
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(note.ImageFile.FileName);

                    string filePath = Path.Combine(uploadsFolder, fileName);

                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await note.ImageFile.CopyToAsync(stream);
                    }
                    existingNote.ImagePath = "/imgs/" + fileName;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(note);
        }

        // GET: Notes/Delete/id
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var note = await _context.Notes.FindAsync(id);
            var userId = _userManager.GetUserId(User);

            if (note == null || note.UserId != userId)
                return Unauthorized();

            return View(note);
        }

        // POST: Notes/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            var userId = _userManager.GetUserId(User);

            if (note == null || note.UserId != userId)
                return Unauthorized();

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        
    }
}
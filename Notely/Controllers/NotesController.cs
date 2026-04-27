using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Notely.Models;  

public class NotesController : Controller
{
    private readonly AppDbContext _context;
    public NotesController(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        var notes = await _context.Notes.ToListAsync();
        return View(notes);
    }




    
}

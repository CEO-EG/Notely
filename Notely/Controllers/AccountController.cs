
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Notely.Models;
namespace Notely.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _context;

        public AccountController(
           UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment env,
            AppDbContext contex)
                {
            _userManager = userManager;
            _signInManager = signInManager;
            _env = env;
            _context = contex;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            string? filePath = null;

            if (model.ProfileImage != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "imgs");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfileImage.FileName);

                var fullPath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(stream);
                }

                filePath = "/imgs/" + fileName;
            }

            var user = new ApplicationUser
            {
                
                FullName = model.FirstName + " " + model.LastName,
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                ProfileImagepath = filePath,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
              

                await _signInManager.SignInAsync(user, isPersistent: false);

                TempData["Toast"] = "Welcome to Notely! 🎉";

                return RedirectToAction("Index", "Notes");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                TempData["Toast"] = "Welcome back! 👋";

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                var user = await _userManager.FindByEmailAsync(model.Email);
               


                return RedirectToAction("Index", "Notes");
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult Profile()
        {
            var user = _userManager.GetUserAsync(User).Result;

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var userId = _userManager.GetUserId(User);

            var notes = _context.Notes
                .Where(n => n.UserId == userId)
                .ToList();

            var model = new ProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                ProfileImagePath = user.ProfileImagepath,
                Notes = notes
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string Fullname, string Email, IFormFile? ProfileImage)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login");

            // Update basic info
            user.FullName = Fullname;
            user.Email = Email;
            user.UserName = Email;

            // 🔥 Handle image upload
            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imgs");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfileImage.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImage.CopyToAsync(stream);
                }

                user.ProfileImagepath = "/imgs/" + fileName;
            }

            await _userManager.UpdateAsync(user);

            return RedirectToAction("Profile");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login");

            string userId = user.Id.ToString();

            // get all user notes
            var notes = _context.Notes
                .Where(n => n.UserId == userId)
                .ToList();

            // delete note images
            foreach (var note in notes)
            {
                if (!string.IsNullOrEmpty(note.ImagePath))
                {
                    var imagePath = Path.Combine(
                        _env.WebRootPath,
                        note.ImagePath.TrimStart('/')
                    );

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
            }

            // delete notes
            _context.Notes.RemoveRange(notes);

            await _context.SaveChangesAsync();

            // delete profile image
            if (!string.IsNullOrEmpty(user.ProfileImagepath))
            {
                var profilePath = Path.Combine(
                    _env.WebRootPath,
                    user.ProfileImagepath.TrimStart('/')
                );

                if (System.IO.File.Exists(profilePath))
                {
                    System.IO.File.Delete(profilePath);
                }
            }

            // logout user
            await _signInManager.SignOutAsync();

            // delete user directly from AspNetUsers
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied() => View();




    }
}

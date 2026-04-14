using System.Drawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class AboutController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public AboutController(FullDatabaseContext context, ILogger<AboutController> logger, IWebHostEnvironment webHostEnvironment, IConfiguration configuration) : base(context, logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        private bool ProcessImageFile(About about)
        {
            IFormFile? file = HttpContext.Request.Form.Files["ImageFile"];
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img");
            string publicUploadPath = _configuration["FileUploadSettings:GenericImagesUploadPath"];
            string filePath = Path.Combine(uploadPath, "about.jpg");
            string publicFilePath = Path.Combine(publicUploadPath, "about.jpg");

            if (file != null && file.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ImageFile", "Please upload a JPG file.");
                    return false;
                }
                using (var image = Image.FromStream(file.OpenReadStream()))
                {
                    if (image.Width != 1164 || image.Height != 1011)
                    {
                        ModelState.AddModelError("ImageFile", "Please upload an image with a resolution of 1164px x 1011px.");
                        return false;
                    }
                }

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                if (System.IO.File.Exists(publicFilePath))
                {
                    System.IO.File.Delete(publicFilePath);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                using (var stream = new FileStream(publicFilePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            else
            {
                if (!System.IO.File.Exists(filePath))
                {
                    ModelState.AddModelError("ImageFile", "Please select a valid image file.");
                    return false;
                }

            }
            return true;
        }

        // GET: About/Edit
        public async Task<IActionResult> Edit()
        {
            if (_context.About == null)
            {
                return NotFound();
            }

            var about = await _context.About
                .Include(x => x.Translations)
                .FirstOrDefaultAsync();
            if (about == null)
            {
                return NotFound();
			}
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img");
            string filePath = Path.Combine(uploadPath, "about.jpg");
            if (System.IO.File.Exists(filePath))
            {
	            about.ImagePath = $"\\img\\about.jpg";
            }
			ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(about);
        }

        // POST: About/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, About about)
        {
            if (id != about.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(about);
                    await _context.SaveChangesAsync();

                    if (!ProcessImageFile(about))
                    {
                        ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
                        return View(nameof(Edit), about);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AboutExists(about.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img");
            string filePath = Path.Combine(uploadPath, "about.jpg");
            if (System.IO.File.Exists(filePath))
            {
                about.ImagePath = $"\\img\\about.jpg";
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(about);
        }

        private bool AboutExists(int id)
        {
            return (_context.About?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

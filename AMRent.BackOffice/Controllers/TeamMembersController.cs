using System.Drawing;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class TeamMembersController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public TeamMembersController(FullDatabaseContext context, ILogger<TeamMembersController> logger, IWebHostEnvironment webHostEnvironment, IConfiguration configuration) : base(context, logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        [HttpPost]
        public JsonResult Get(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<TeamMember> recordsTotal = _context.TeamMembers;
                var recordsTotalCount = recordsTotal.Count();
                var recordsFiltered = recordsTotal;

                if (viewModel.order.Any())
                {
                    switch (viewModel.order.First().column)
                    {
                        case 0:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.SortNumber)
                                : recordsFiltered.OrderByDescending(x => x.SortNumber);
                            break;
                        case 1:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Name)
                                : recordsFiltered.OrderByDescending(x => x.Name);
                            break;
                        case 2:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Job)
                                : recordsFiltered.OrderByDescending(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Job);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.search.value))
                {
                    var predicate = PredicateBuilder.New<TeamMember>();
                    predicate = predicate.Or(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Job.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.TeamMemberIndex()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Job = x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Job,
                    SortNumber = x.SortNumber
                }).ToArray();

                int highestSortNumberId = recordsTotal.OrderByDescending(x => x.SortNumber).FirstOrDefault()?.Id ?? 0;
                int lowestSortNumberId = recordsTotal.OrderBy(x => x.SortNumber).FirstOrDefault()?.Id ?? 0;

                if (recordsFilteredPage.Any(x => x.Id == highestSortNumberId))
                {
                    recordsFilteredPage.FirstOrDefault(x => x.Id == highestSortNumberId).IsHighestSortNumber = true;
                }
                if (recordsFilteredPage.Any(x => x.Id == lowestSortNumberId))
                {
                    recordsFilteredPage.FirstOrDefault(x => x.Id == lowestSortNumberId).IsLowestSortNumber = true;
                }

                result = Json(new
                {
                    draw = viewModel.draw,
                    recordsTotal = recordsTotalCount,
                    recordsFiltered = recordsFilteredCount,
                    data = recordsFilteredPage,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return result;
        }

        private bool ProcessImageFile(TeamMember teamMember)
        {
            IFormFile? file = HttpContext.Request.Form.Files["ImageFile"];
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img\\team");
            string publicUploadPath = _configuration["FileUploadSettings:TeamImagesUploadPath"];
            string filePath = Path.Combine(uploadPath, $"{teamMember.Id}.jpg");
            string publicFilePath = Path.Combine(publicUploadPath, $"{teamMember.Id}.jpg");

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
                    if (image.Width != 416 || image.Height != 534)
                    {
                        ModelState.AddModelError("ImageFile", "Please upload an image with a resolution of 416px x 534px.");
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

        // GET: TeamMembers
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: TeamMembers/Create
        public IActionResult Create()
        {
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View();
        }

        // POST: TeamMembers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeamMember teamMember)
        {
            if (ModelState.IsValid)
            {
                teamMember.SortNumber = (short)(await _context.TeamMembers.MaxAsync(x => x.SortNumber) + 1);
                _context.Add(teamMember);
                await _context.SaveChangesAsync();

                if (!ProcessImageFile(teamMember))
                {
                    return View(nameof(Edit), teamMember);
                }

                return RedirectToAction(nameof(Index));
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(teamMember);
        }

        // GET: TeamMembers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TeamMembers == null)
            {
                return NotFound();
            }

            var teamMember = await _context.TeamMembers
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (teamMember == null)
            {
                return NotFound();
            }
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img\\team");
            string filePath = Path.Combine(uploadPath, $"{teamMember.Id}.jpg");
            if (System.IO.File.Exists(filePath))
            {
                teamMember.ImagePath = $"\\img\\team\\{teamMember.Id}.jpg";
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(teamMember);
        }

        // POST: TeamMembers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TeamMember teamMember)
        {
            if (id != teamMember.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teamMember);
                    await _context.SaveChangesAsync();

                    if (!ProcessImageFile(teamMember))
                    {
                        return View(teamMember);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamMemberExists(teamMember.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img\\team");
            string filePath = Path.Combine(uploadPath, $"{teamMember.Id}.jpg");
            if (System.IO.File.Exists(filePath))
            {
                teamMember.ImagePath = $"\\img\\team\\{teamMember.Id}.jpg";
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(teamMember);
        }

        // GET: TeamMembers/SortUp/5
        public async Task<IActionResult> SortUp(int? id)
        {
            if (id == null || _context.TeamMembers == null)
            {
                return NotFound();
            }

            var teamMember = await _context.TeamMembers
                .FirstOrDefaultAsync(x => x.Id == id);
            if (teamMember == null)
            {
                return NotFound();
            }

            if (teamMember.SortNumber > 1)
            {
                var lowerSortNumberTeamMember = await _context.TeamMembers.FirstOrDefaultAsync(x => x.SortNumber == teamMember.SortNumber - 1);
                if (lowerSortNumberTeamMember != null)
                {
                    lowerSortNumberTeamMember.SortNumber++;
                }

                teamMember.SortNumber--;

                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: TeamMembers/SortDown/5
        public async Task<IActionResult> SortDown(int? id)
        {
            if (id == null || _context.TeamMembers == null)
            {
                return NotFound();
            }

            var teamMember = await _context.TeamMembers
                .FirstOrDefaultAsync(x => x.Id == id);
            if (teamMember == null)
            {
                return NotFound();
            }

            var higherSortNumberTeamMember = await _context.TeamMembers.FirstOrDefaultAsync(x => x.SortNumber == teamMember.SortNumber + 1);
            if (higherSortNumberTeamMember != null)
            {
                higherSortNumberTeamMember.SortNumber--;
            }

            teamMember.SortNumber++;

            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }

        // GET: TeamMembers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TeamMembers == null)
            {
                return NotFound();
            }

            var teamMember = await _context.TeamMembers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teamMember == null)
            {
                return NotFound();
            }

            _context.TeamMembers.Remove(teamMember);
            await _context.SaveChangesAsync();

            return View(nameof(Index));
        }

        // POST: TeamMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TeamMembers == null)
            {
                return Problem("Entity set 'FullDatabaseContext.TeamMembers'  is null.");
            }
            var teamMember = await _context.TeamMembers.FindAsync(id);
            if (teamMember != null)
            {
                _context.TeamMembers.Remove(teamMember);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamMemberExists(int id)
        {
            return (_context.TeamMembers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

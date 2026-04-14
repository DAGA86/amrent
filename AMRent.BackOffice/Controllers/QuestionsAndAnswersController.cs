using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class QuestionsAndAnswersController : BaseController
    {
        public QuestionsAndAnswersController(FullDatabaseContext context, ILogger<QuestionsAndAnswersController> logger) : base(context, logger)
        {
        }

        [HttpPost]
        public JsonResult Get(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<QuestionAndAnswer> recordsTotal = _context.QuestionsAndAnswers;
                var recordsTotalCount = recordsTotal.Count();
                var recordsFiltered = recordsTotal;

                if (viewModel.order.Any())
                {
                    switch (viewModel.order.First().column)
                    {
                        case 0:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Question)
                                : recordsFiltered.OrderByDescending(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Question);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.search.value))
                {
                    var predicate = PredicateBuilder.New<QuestionAndAnswer>();
                    predicate = predicate.Or(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Question.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.QuestionAndAnswerIndex()
                {
                    Id = x.Id,
                    Question = x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Question
                }).ToArray();

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

        // GET: QuestionsAndAnswers
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: QuestionsAndAnswers/Create
        public IActionResult Create()
        {
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View();
        }

        // POST: QuestionsAndAnswers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionAndAnswer questionAndAnswer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(questionAndAnswer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(questionAndAnswer);
        }

        // GET: QuestionsAndAnswers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.QuestionsAndAnswers == null)
            {
                return NotFound();
            }

            var questionAndAnswer = await _context.QuestionsAndAnswers
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (questionAndAnswer == null)
            {
                return NotFound();
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(questionAndAnswer);
        }

        // POST: QuestionsAndAnswers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, QuestionAndAnswer questionAndAnswer)
        {
            if (id != questionAndAnswer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(questionAndAnswer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionAndAnswerExists(questionAndAnswer.Id))
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
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(questionAndAnswer);
        }

        // GET: QuestionsAndAnswers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.QuestionsAndAnswers == null)
            {
                return NotFound();
            }

            var questionAndAnswer = await _context.QuestionsAndAnswers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (questionAndAnswer == null)
            {
                return NotFound();
            }

            return View(questionAndAnswer);
        }

        // POST: QuestionsAndAnswers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.QuestionsAndAnswers == null)
            {
                return Problem("Entity set 'FullDatabaseContext.QuestionsAndAnswers'  is null.");
            }
            var questionAndAnswer = await _context.QuestionsAndAnswers.FindAsync(id);
            if (questionAndAnswer != null)
            {
                _context.QuestionsAndAnswers.Remove(questionAndAnswer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionAndAnswerExists(int id)
        {
            return (_context.QuestionsAndAnswers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

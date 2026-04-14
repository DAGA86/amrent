using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using AMRent.Data.Enums;
using AMRent.Data.Models.DataTransfer;
using System.Text;
using UtfUnknown;
using System.Security.Claims;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class PricesController : BaseController
    {
        public PricesController(FullDatabaseContext context, ILogger<PricesController> logger) : base(context, logger)
        {
        }

        [HttpPost]
        public JsonResult Get(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<Price> recordsTotal = _context.Prices;
                var recordsTotalCount = recordsTotal.Count();
                var recordsFiltered = recordsTotal;

                if (viewModel.order.Any())
                {
                    switch (viewModel.order.First().column)
                    {
                        case 0:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Season.SeasonCategory.Name)
                                : recordsFiltered.OrderByDescending(x => x.Season.SeasonCategory.Name);
                            break;
                        case 1:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.CarSegment.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name)
                                : recordsFiltered.OrderByDescending(x => x.CarSegment.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name);
                            break;
                        case 2:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Days)
                                : recordsFiltered.OrderByDescending(x => x.Days);
                            break;
                        case 3:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Value)
                                : recordsFiltered.OrderByDescending(x => x.Value);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.search.value))
                {
                    var predicate = PredicateBuilder.New<Price>();
                    predicate = predicate.Or(x => x.Season.SeasonCategory.Name.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.CarSegment.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.PriceIndex()
                {
                    Id = x.Id,
                    Season = x.Season.SeasonCategory.Name,
                    Segment = x.CarSegment.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name,
                    Days = x.Days,
                    Value = x.Value
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

        public IActionResult Export()
        {
            // Get all the Seasons with SeasonCategory
            Dictionary<int, int> seasonsWithSeasonCategory = _context.Seasons
                .Where(x => x.Prices.Any())
                .OrderBy(x => x.SeasonCategoryId)
                .ThenByDescending(x => x.EndDateUtc)
                .Select(x => new { x.SeasonCategoryId, x.Id })
                .AsEnumerable()
                .ToDictionary(x => x.Id, x => x.SeasonCategoryId);

            // Get only the latest Season per SeasonCategory
            List<int> seasonCategoriesAlreadyConsidered = new();
            List<int> seasonIdsToConsider = new();

            foreach (KeyValuePair<int, int> seasonKeyValuePair in seasonsWithSeasonCategory)
            {
                if (!seasonCategoriesAlreadyConsidered.Contains(seasonKeyValuePair.Value))
                {
                    seasonCategoriesAlreadyConsidered.Add(seasonKeyValuePair.Value);
                    seasonIdsToConsider.Add(seasonKeyValuePair.Key);
                }
            }

            // Get the filtered Seasons
            Dictionary<int, string> seasons = _context.Seasons
                .Where(x => seasonIdsToConsider.Contains(x.Id))
                .OrderBy(x => x.EndDateUtc)
                .Select(x => new { x.Id, x.SeasonCategory.Name })
                .AsEnumerable()
                .ToDictionary(x => x.Id, x => x.Name);

            // Get the number of days
            List<Tuple<int, int>> seasonDays = _context.Prices
                .Where(x => seasonIdsToConsider.Contains(x.SeasonId))
                .OrderBy(x => x.Days)
                .Select(x => new { x.SeasonId, x.Days })
                .Distinct()
                .AsEnumerable()
                .Select(x => new Tuple<int, int>(x.SeasonId, x.Days))
                .OrderBy(x => x.Item1)
                    .ThenBy(x => x.Item2)
                .ToList();

            // Get all the segments
            List<CarSegment> carSegments = _context.CarSegments
                .Include(cs => cs.CarCategory)
                .ThenInclude(cc => cc.Translations.Where(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .OrderBy(cs => cs.CarCategory.Translations.First().Name)
                .ThenBy(cs => cs.Code)
                .ToList();

            // Get the Prices for the filtered Seasons
            List<Price> prices = _context.Prices
                .Include(p => p.CarSegment)
                    .ThenInclude(cs => cs.CarCategory)
                    .ThenInclude(cc => cc.Translations.Where(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(p => p.Season)
                    .ThenInclude(s => s.SeasonCategory)
                .Where(p => seasonIdsToConsider.Contains(p.SeasonId))
                .ToList();

            // Build the CSV
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
                {
                    using (CsvWriter csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture)))
                    {
                        // Header
                        csvWriter.WriteField(string.Empty);
                        csvWriter.WriteField("Última temporada com preços");
                        csvWriter.WriteField("Dias");

                        foreach (CarSegment carSegment in carSegments)
                        {
                            csvWriter.WriteField(carSegment.CarCategory.Translations.First().Name);
                        }
                        csvWriter.NextRecordAsync();

                        // Sub Header
                        csvWriter.WriteField(string.Empty);
                        csvWriter.WriteField(string.Empty);
                        csvWriter.WriteField(string.Empty);

                        foreach (CarSegment carSegment in carSegments)
                        {
                            csvWriter.WriteField(carSegment.Code);
                        }
                        csvWriter.NextRecordAsync();

                        // Data
                        foreach (KeyValuePair<int, string> season in seasons)
                        {
                            foreach (Tuple<int, int> seasonDay in seasonDays.Where(x => x.Item1 == season.Key))
                            {
                                csvWriter.WriteField(season.Value);
                                csvWriter.WriteField($"{prices.FirstOrDefault(x => x.SeasonId == season.Key && x.Days == seasonDay.Item2).Season.StartDateUtc.ToString("dd/MM/yyyy")} - {prices.FirstOrDefault(x => x.SeasonId == season.Key && x.Days == seasonDay.Item2).Season.EndDateUtc.ToString("dd/MM/yyyy")}");
                                csvWriter.WriteField(seasonDay.Item2);
                                List<string> priceList = new();
                                foreach (CarSegment carSegment in carSegments)
                                {
                                    csvWriter.WriteField(prices.FirstOrDefault(x => x.SeasonId == season.Key && x.Days == seasonDay.Item2 && x.CarSegmentId == carSegment.Id)?.Value.ToString("0.00") ?? "");
                                }
                                csvWriter.NextRecordAsync();
                            }
                        }
                    }
                    streamWriter.Flush();
                }
                memoryStream.Position = 0;
                var fileName = $"ExportPrecos_{DateTime.Now::yyyy-MM-dd_HH_mm_ss}.csv";
                _logger.LogInformation("Seasons count: {0}", seasons.Count);
                _logger.LogInformation("SeasonDays count: {0}", seasonDays.Count);
                _logger.LogInformation("Prices count: {0}", prices.Count);
                _logger.LogInformation("CarSegments count: {0}", carSegments.Count);
                return File(memoryStream.ToArray(), "text/csv", fileName);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Import(Data.Models.View.PriceImport priceImport)
        {
            IFormFile? file = HttpContext.Request.Form.Files["file"];
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    try
                    {
                        Encoding encoding = Encoding.UTF8;
                        List<Data.Models.DataTransfer.PriceImport> newPrices = new();

                        char[] possibleDelimiters = { ',', ';' };
                        char delimiter = ',';

                        using (var stream = file.OpenReadStream())
                        {
                            using (var reader = new StreamReader(stream, encoding))
                            {
                                string line = reader.ReadLine();
                                if (line != null)
                                {
                                    foreach (char possibleDelimiter in possibleDelimiters)
                                    {
                                        if (line.Contains(possibleDelimiter))
                                        {
                                            delimiter = possibleDelimiter;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        using (var stream = file.OpenReadStream())
                        {
                            encoding = CharsetDetector.DetectFromStream(stream).Detected.Encoding;
                        }

                        CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
                        switch (delimiter)
                        {
                            case ';':
                                csvConfiguration = new CsvConfiguration(new CultureInfo("pt-PT"));
                                break;
                            case ',':
                            default:
                                csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
                                break;
                        }

                        // ColumnIndex, (CarSegmentId, CarSegmentCode)
                        Dictionary<int, Tuple<int, string>> carSegmentsInFile = new();

                        using (var stream = file.OpenReadStream())
                        {
                            using var reader = new StreamReader(stream, encoding);
                            using var csv = new CsvReader(reader, csvConfiguration);

                            // Ignore the first header (CarCategory)
                            csv.Read();

                            // Read the second header (CarSegment)
                            csv.Read();

                            for (int i = 3; i < csv.Parser.Record.Length; i++)
                            {
                                string? carSegmentCode = csv.GetField<string?>(i) ?? "";
                                if (string.IsNullOrEmpty(carSegmentCode))
                                {
                                    throw new Exception($"Segmento inválido na coluna {i}");
                                }

                                int? carSegmentId = _context.CarSegments.FirstOrDefault(x =>
                                        x.Code == carSegmentCode)?
                                    .Id;
                                if (!carSegmentId.HasValue)
                                {
                                    throw new Exception($"Segmento não existe: {carSegmentCode}");
                                }

                                if (carSegmentsInFile.Any(x => x.Value.Item1 == carSegmentId.Value))
                                {
                                    throw new Exception($"Segmento duplicado: {carSegmentCode}");
                                }

                                carSegmentsInFile.Add(i, new Tuple<int, string>(carSegmentId.Value, carSegmentCode));
                            }

                            // Read the rest of the file

                            while (csv.Read())
                            {
                                string? seasonCategoryName = csv.GetField<string?>(0);
                                if (string.IsNullOrEmpty(seasonCategoryName))
                                {
                                    throw new Exception($"Temporada inválida na linha {csv.Parser.Row}");
                                }

                                int? seasonCategoryId = _context.SeasonCategories
                                    .FirstOrDefault(x => x.Name == seasonCategoryName)?.Id;
                                if (!seasonCategoryId.HasValue)
                                {
                                    throw new Exception($"Temporada não existe: {seasonCategoryName}");
                                }

                                int? days = csv.GetField<int?>(2);
                                if (!days.HasValue)
                                {
                                    throw new Exception($"Número de dias inválido na linha {csv.Parser.Row}");
                                }

                                if (newPrices.Any(x =>
                                        x.SeasonCategoryId == seasonCategoryId.Value && x.Days == days.Value))
                                {
                                    throw new Exception(
                                        $"Linha duplicada. Temporada: {seasonCategoryName}, Dias: {days.Value}");
                                }

                                for (int i = 3; i < csv.Parser.Record.Length; i++)
                                {
                                    string? value = csv.GetField<string?>(i);
                                    if (string.IsNullOrEmpty(value) || !decimal.TryParse(value.Replace(',', '.'), out decimal decimalValue))
                                    {
                                        throw new Exception(
                                            $"Preço inválido. Categoria: {seasonCategoryName}, Dias: {days.Value}, Segmento: {carSegmentsInFile[i].Item2}");
                                    }

                                    newPrices.Add(new Data.Models.DataTransfer.PriceImport()
                                    {
                                        SeasonCategoryId = seasonCategoryId.Value,
                                        CarSegmentId = carSegmentsInFile[i].Item1,
                                        Days = days.Value,
                                        Value = decimalValue
                                    });
                                }
                            }
                        }

                        List<Data.Models.Database.Season> seasonsToUpdate = new();

                        switch (priceImport.Method)
                        {
                            case PriceImportMethods.FromCurrentSeason:
                                seasonsToUpdate = _context.Seasons.Where(x => x.EndDateUtc >= DateTime.Today).ToList();
                                break;
                            case PriceImportMethods.FromNextSeason:
                                seasonsToUpdate = _context.Seasons.Where(x => x.StartDateUtc > DateTime.Today).ToList();
                                break;
                            case PriceImportMethods.OnlyEmpty:
                                seasonsToUpdate = _context.Seasons.Where(x => !x.Prices.Any()).ToList();
                                break;
                        }

                        int[] carSegmentIdsInFile = carSegmentsInFile.Values.Select(x => x.Item1).ToArray();

                        _context.Prices.RemoveRange(_context.Prices.Where(x => seasonsToUpdate.Contains(x.Season) && carSegmentIdsInFile.Contains(x.CarSegmentId)));

                        foreach (Season seasonToUpdate in seasonsToUpdate)
                        {
                            List<Data.Models.DataTransfer.PriceImport> relatedPrices = newPrices
                                .Where(x => x.SeasonCategoryId == seasonToUpdate.SeasonCategoryId).ToList();

                            foreach (PriceImport relatedPrice in relatedPrices)
                            {
                                _context.Prices.Add(new Price()
                                {
                                    SeasonId = seasonToUpdate.Id,
                                    CarSegmentId = relatedPrice.CarSegmentId,
                                    Days = relatedPrice.Days,
                                    Value = relatedPrice.Value,
                                });
                            }
                        }

                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index");

                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("file", ex.Message);
                    }
                }
                else
                {
                    ModelState.AddModelError("file", "Please select a file.");
                }
            }

            return View("Index");
        }

        // GET: Prices
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: Prices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Prices == null)
            {
                return NotFound();
            }

            var price = await _context.Prices
                .Include(x => x.Changes)
                .ThenInclude(y => y.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (price == null)
            {
                return NotFound();
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            ViewBag.Seasons = new SelectList(_context.Seasons.Select(x => new { x.Id, Text = $"{x.SeasonCategory.Name} ({x.StartDateUtc:yyyy-MM-dd} - {x.EndDateUtc:yyyy-MM-dd})" }), "Id", "Text", price.SeasonId);
            ViewBag.Segments = new SelectList(_context.CarSegments.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name }), "Id", "Name", price.CarSegmentId);
            return View(price);
        }

        // POST: Prices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Price price)
        {
            if (id != price.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.UpdateWithTracking(price, Guid.Parse(User.Claims.First(x => x.Type == ClaimTypes.UserData).Value));
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PriceExists(price.Id))
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
            ViewBag.Seasons = new SelectList(_context.Seasons.Select(x => new { x.Id, Text = $"{x.SeasonCategory.Name} ({x.StartDateUtc:yyyy-MM-dd} - {x.EndDateUtc:yyyy-MM-dd})" }), "Id", "Text", price.SeasonId);
            ViewBag.Segments = new SelectList(_context.CarSegments.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name }), "Id", "Name", price.CarSegmentId);
            return View(price);
        }

        // GET: Prices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Prices == null)
            {
                return NotFound();
            }

            var price = await _context.Prices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (price == null)
            {
                return NotFound();
            }

            return View(price);
        }

        // POST: Prices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Prices == null)
            {
                return Problem("Entity set 'FullDatabaseContext.Prices'  is null.");
            }
            var price = await _context.Prices.FindAsync(id);
            if (price != null)
            {
                _context.Prices.Remove(price);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PriceExists(int id)
        {
            return (_context.Prices?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

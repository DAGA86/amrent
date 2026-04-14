using CsvHelper;
using CsvHelper.Configuration;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Enums;
using AMRent.Data.Models.Database;
using System.Globalization;
using System.Linq;
using System.Text;
using UtfUnknown;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class InsuranceLevelsController : BaseController
    {
        public InsuranceLevelsController(FullDatabaseContext context, ILogger<InsuranceLevelsController> logger) : base(context, logger)
        {
        }

        [HttpPost]
        public JsonResult Get(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<InsuranceLevel> recordsTotal = _context.InsuranceLevels;
                var recordsTotalCount = recordsTotal.Count();
                var recordsFiltered = recordsTotal;

                if (viewModel.order.Any())
                {
                    switch (viewModel.order.First().column)
                    {
                        case 0:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name)
                                : recordsFiltered.OrderByDescending(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.search.value))
                {
                    var predicate = PredicateBuilder.New<InsuranceLevel>();
                    predicate = predicate.Or(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.InsuranceLevelIndex()
                {
                    Id = x.Id,
                    Name = x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name
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

        // GET: InsuranceLevels
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult Export()
        {
            List<Tuple<Tuple<int, string>, int>> insuranceLevelDays = _context.InsurancePrices.
                OrderBy(ip => ip.Insurance.InsuranceLevelId)
                    .ThenBy(ip => ip.Days)
                .Select(ip => new { ip.Insurance.InsuranceLevelId, ip.Insurance.InsuranceLevel.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name, ip.Days })
                .Distinct()
                .AsEnumerable()
                .Select(x => new Tuple<Tuple<int, string>, int>(new Tuple<int, string>(x.InsuranceLevelId, x.Name), x.Days))
                .ToList();

            // Get all the segments
            List<CarSegment> carSegments = _context.CarSegments
                .Include(cs => cs.CarCategory)
                    .ThenInclude(cc => cc.Translations.Where(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .OrderBy(cs => cs.CarCategory.Translations.First().Name)
                    .ThenBy(cs => cs.Translations.First().Name)
                .ToList();

            // Get the Insurance Prices
            List<InsurancePrice> insurancePrices = _context.InsurancePrices
                .Include(ip => ip.Insurance)
                    .ThenInclude(i => i.CarSegment)
                        .ThenInclude(cs => cs.CarCategory)
                            .ThenInclude(cc => cc.Translations.Where(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(ip => ip.Insurance)
                    .ThenInclude(i => i.InsuranceLevel)
                        .ThenInclude(il => il.Translations.Where(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .OrderBy(ip => ip.Insurance.InsuranceLevelId)
                    .ThenBy(ip => ip.Days)
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
                        csvWriter.WriteField(string.Empty);

                        foreach (CarSegment carSegment in carSegments)
                        {
                            csvWriter.WriteField(carSegment.CarCategory.Translations.First().Name);
                        }
                        csvWriter.NextRecordAsync();

                        // Sub Header
                        csvWriter.WriteField(string.Empty);
                        csvWriter.WriteField("Dias");

                        foreach (CarSegment carSegment in carSegments)
                        {
                            csvWriter.WriteField(carSegment.Code);
                        }
                        csvWriter.NextRecordAsync();

                        // Data
                        foreach (Tuple<Tuple<int, string>, int> insuranceLevelDay in insuranceLevelDays)
                        {
                            csvWriter.WriteField(insuranceLevelDay.Item1.Item2);
                            csvWriter.WriteField(insuranceLevelDay.Item2);

                            foreach (CarSegment carSegment in carSegments)
                            {
                                InsurancePrice insurancePrice = insurancePrices.FirstOrDefault(ip =>
                                    ip.Insurance.InsuranceLevelId == insuranceLevelDay.Item1.Item1
                                    && ip.Days == insuranceLevelDay.Item2
                                    && ip.Insurance.CarSegmentId == carSegment.Id);

                                csvWriter.WriteField(insurancePrice?.Value);
                            }
                            csvWriter.NextRecordAsync();
                        }
                    }
                    streamWriter.Flush();
                }
                memoryStream.Position = 0;
                var fileName = $"ExportCoberturas_{DateTime.Now::yyyy-MM-dd_HH_mm_ss}.csv";
                return File(memoryStream.ToArray(), "text/csv", fileName);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Import()
        {
            IFormFile? file = HttpContext.Request.Form.Files["file"];
            if (file != null && file.Length > 0)
            {
                try
                {
                    Encoding encoding = Encoding.UTF8;
                    List<Data.Models.DataTransfer.InsurancePriceImport> newInsurancePrices = new();

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

                        for (int i = 2; i < csv.Parser.Record.Length; i++)
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
                            string? insuranceLevelName = csv.GetField<string?>(0);
                            if (string.IsNullOrEmpty(insuranceLevelName))
                            {
                                throw new Exception($"Nível de cobertura inválido na linha {csv.Parser.Row}");
                            }

                            int? InsuranceLevelId = _context.InsuranceLevelTranslations
                                .FirstOrDefault(x => x.Name == insuranceLevelName)?.InsuranceLevelId;
                            if (!InsuranceLevelId.HasValue)
                            {
                                throw new Exception($"Nível de cobertura não existe: {insuranceLevelName}");
                            }

                            int? days = csv.GetField<int?>(1);
                            if (!days.HasValue)
                            {
                                throw new Exception($"Número de dias inválido na linha {csv.Parser.Row}");
                            }

                            if (newInsurancePrices.Any(x =>
                                    x.InsuranceLevelId == InsuranceLevelId.Value && x.Days == days.Value))
                            {
                                throw new Exception(
                                    $"Linha duplicada. Nível de cobertura: {insuranceLevelName}, Dias: {days.Value}");
                            }

                            for (int i = 2; i < csv.Parser.Record.Length; i++)
                            {
                                string? value = csv.GetField<string?>(i);
                                if (string.IsNullOrEmpty(value))
                                {
                                    continue;
                                }
                                if (!decimal.TryParse(value.Replace(',', '.'), out decimal decimalValue))
                                {
                                    throw new Exception(
                                        $"Preço inválido. Nível de cobertura: {insuranceLevelName}, Dias: {days.Value}, Segmento: {carSegmentsInFile[i].Item2}");
                                }

                                newInsurancePrices.Add(new Data.Models.DataTransfer.InsurancePriceImport()
                                {
                                    InsuranceLevelId = InsuranceLevelId.Value,
                                    CarSegmentId = carSegmentsInFile[i].Item1,
                                    Days = days.Value,
                                    Value = decimalValue
                                });
                            }
                        }
                    }

                    int[] carSegmentIdsInFile = carSegmentsInFile.Values.Select(x => x.Item1).ToArray();

                    _context.InsurancePrices.RemoveRange(_context.InsurancePrices.Where(x => carSegmentIdsInFile.Contains(x.Insurance.CarSegmentId)));

                    foreach (Data.Models.DataTransfer.InsurancePriceImport newInsurancePrice in newInsurancePrices)
                    {
                        var insurance = _context.Insurances.Include(i => i.Prices).FirstOrDefault(i =>
                            i.CarSegmentId == newInsurancePrice.CarSegmentId
                            && i.InsuranceLevelId == newInsurancePrice.InsuranceLevelId);

                        if (insurance != null)
                        {
                            insurance.Prices.Add(new()
                            {
                                Days = newInsurancePrice.Days,
                                Value = newInsurancePrice.Value
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

            return View("Index");
        }

        // GET: InsuranceLevels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.InsuranceLevels == null)
            {
                return NotFound();
            }

            var insuranceLevel = await _context.InsuranceLevels
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (insuranceLevel == null)
            {
                return NotFound();
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(insuranceLevel);
        }

        // POST: InsuranceLevels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InsuranceLevel insuranceLevel)
        {
            if (id != insuranceLevel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(insuranceLevel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InsuranceLevelExists(insuranceLevel.Id))
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
            return View(insuranceLevel);
        }

        private bool InsuranceLevelExists(int id)
        {
            return (_context.InsuranceLevels?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

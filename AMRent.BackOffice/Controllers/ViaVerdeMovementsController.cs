using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AMRent.BackOffice.Models;
using AMRent.Data.Contexts;
using AMRent.Data.Enums;
using AMRent.Data.Models.Database;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class ViaVerdeMovementsController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public ViaVerdeMovementsController(FullDatabaseContext context, ILogger<ViaVerdeMovementsController> logger, IWebHostEnvironment webHostEnvironment, IConfiguration configuration) : base(context, logger)
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
                IQueryable<ViaVerdeMovement> recordsTotal = _context.ViaVerdeMovements;
                var recordsTotalCount = recordsTotal.Count();
                var recordsFiltered = recordsTotal;

                if (viewModel.order.Any())
                {
                    switch (viewModel.order.First().column)
                    {
                        case 0:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.TransactionCode)
                                : recordsFiltered.OrderByDescending(x => x.TransactionCode);
                            break;
                        case 1:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.ContractLicencePlate)
                                : recordsFiltered.OrderByDescending(x => x.ContractLicencePlate);
                            break;
                        case 2:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.EquipmentNumber)
                                : recordsFiltered.OrderByDescending(x => x.EquipmentNumber);
                            break;
                        case 3:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.EntryDate)
                                : recordsFiltered.OrderByDescending(x => x.EntryDate);
                            break;
                        case 4:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.ExitDate)
                                : recordsFiltered.OrderByDescending(x => x.ExitDate);
                            break;
                        case 5:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Status)
                                : recordsFiltered.OrderByDescending(x => x.Status);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.search.value))
                {
                    var predicate = PredicateBuilder.New<ViaVerdeMovement>();
                    predicate = predicate.Or(x => x.TransactionCode.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.ContractLicencePlate.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.EquipmentNumber.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.ViaVerdeMovementIndex()
                {
                    Id = x.Id,
                    TransactionCode = x.TransactionCode,
                    LicencePlate = x.ContractLicencePlate,
                    EquipmentNumber = x.EquipmentNumber,
                    EntryDate = x.EntryDate,
                    ExitDate = x.ExitDate,
                    Status = Data.Enums.Generic.GetDescription(x.Status)
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

        // GET: ViaVerdeMovements
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: ViaVerdeMovements/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.ViaVerdeMovements == null)
            {
                return NotFound();
            }

            var viaVerdeMovement = await _context.ViaVerdeMovements
                .FirstOrDefaultAsync(x => x.Id == id);
            if (viaVerdeMovement == null)
            {
                return NotFound();
            }

            ViewBag.ViaVerdeMovementStatus = new SelectList(dCore.Helpers.Enum.GetWithDescription<Data.Enums.ViaVerdeMovementStatus>().Select(x => new SelectListItem
            {
                Text = x.Value,
                Value = x.Key
            }), "Value", "Text", viaVerdeMovement.Status);

            ViewBag.Countries = new SelectList(_context.Countries.Select(x => new
            {
                x.Id,
                x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name
            }), "Id", "Name", viaVerdeMovement.CountryId);

            return View(viaVerdeMovement);
        }

        // POST: ViaVerdeMovements/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ViaVerdeMovement viaVerdeMovement)
        {
            if (id != viaVerdeMovement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (viaVerdeMovement.Status == ViaVerdeMovementStatus.RejectedTow
                    && string.IsNullOrEmpty(viaVerdeMovement.NameOrDeliverySlipNumber))
                {
                    ModelState.AddModelError("DeliverySlipNumber", "Tem de preencher o numero da guia de transporte");
                } else if (viaVerdeMovement.Status == ViaVerdeMovementStatus.RejectedNotPayed &&
                        (string.IsNullOrEmpty(viaVerdeMovement.NameOrDeliverySlipNumber)
                            || string.IsNullOrEmpty(viaVerdeMovement.Address)
                            || string.IsNullOrEmpty(viaVerdeMovement.Town)
                            || string.IsNullOrEmpty(viaVerdeMovement.PostalCode)
                            || string.IsNullOrEmpty(viaVerdeMovement.PostalLocation)
                            || string.IsNullOrEmpty(viaVerdeMovement.IdentificationNumber)
                            || string.IsNullOrEmpty(viaVerdeMovement.CreditCardLast4Digits)
                            || string.IsNullOrEmpty(viaVerdeMovement.AcquirerResultCode)
                            || string.IsNullOrEmpty(viaVerdeMovement.AcquirerResultDescription)
                            || !viaVerdeMovement.CollectionAttemptedDate.HasValue
                            || !viaVerdeMovement.CountryId.HasValue
                        )
                    )
                {
                    if (string.IsNullOrEmpty(viaVerdeMovement.NameOrDeliverySlipNumber))
                    {
                        ModelState.AddModelError("", "Tem de preencher o nome do condutor");
                    }
                    if (string.IsNullOrEmpty(viaVerdeMovement.Address))
                    {
                        ModelState.AddModelError("", "Tem de preencher a morada do condutor");
                    }
                    if (string.IsNullOrEmpty(viaVerdeMovement.Town))
                    {
                        ModelState.AddModelError("", "Tem de preencher a localidade do condutor");
                    }
                    if (string.IsNullOrEmpty(viaVerdeMovement.PostalCode))
                    {
                        ModelState.AddModelError("", "Tem de preencher o código postal do condutor");
                    }
                    if (string.IsNullOrEmpty(viaVerdeMovement.PostalLocation))
                    {
                        ModelState.AddModelError("", "Tem de preencher a localidade postal do condutor");
                    }
                    if (!viaVerdeMovement.CountryId.HasValue)
                    {
                        ModelState.AddModelError("", "Tem de preencher o país do condutor");
                    }
                    if (string.IsNullOrEmpty(viaVerdeMovement.IdentificationNumber))
                    {
                        ModelState.AddModelError("", "Tem de preencher o numero de identificação do condutor");
                    }
                    if (string.IsNullOrEmpty(viaVerdeMovement.CreditCardLast4Digits))
                    {
                        ModelState.AddModelError("", "Tem de preencher os últimos 4 dígitos do cartão de crédtio do condutor");
                    }
                    if (string.IsNullOrEmpty(viaVerdeMovement.AcquirerResultDescription))
                    {
                        ModelState.AddModelError("", "Tem de preencher a descrição de resultado da rejeição");
                    }
                    if (string.IsNullOrEmpty(viaVerdeMovement.AcquirerResultCode))
                    {
                        ModelState.AddModelError("", "Tem de preencher o código de resultado da rejeição");
                    }
                    if (!viaVerdeMovement.CollectionAttemptedDate.HasValue)
                    {
                        ModelState.AddModelError("", "Tem de preencher a data de tentativa de cobrança");
                    }
                }
                else
                {
                    try
                    {
                        viaVerdeMovement.SendToViaVerde = true;
                        _context.Update(viaVerdeMovement);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ViaVerdeMovementExists(viaVerdeMovement.Id))
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
            }

            ViewBag.ViaVerdeMovementStatus = new SelectList(dCore.Helpers.Enum.GetWithDescription<Data.Enums.ViaVerdeMovementStatus>().Select(x => new SelectListItem
            {
                Text = x.Value,
                Value = x.Key
            }), "Value", "Text", viaVerdeMovement.Status);

            ViewBag.Countries = new SelectList(_context.Countries.Select(x => new
            {
                x.Id,
                x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name
            }), "Id", "Name", viaVerdeMovement.CountryId);

            return View(viaVerdeMovement);
        }

        // GET: ViaVerdeMovements/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.ViaVerdeMovements == null)
            {
                return NotFound();
            }

            var viaVerdeMovement = await _context.ViaVerdeMovements
                .FirstOrDefaultAsync(m => m.Id == id);
            if (viaVerdeMovement == null)
            {
                return NotFound();
            }

            _context.ViaVerdeMovements.Remove(viaVerdeMovement);
            await _context.SaveChangesAsync();

            return View(nameof(Index));
        }

        private bool ViaVerdeMovementExists(Guid id)
        {
            return (_context.ViaVerdeMovements?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

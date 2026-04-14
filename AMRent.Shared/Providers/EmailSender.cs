using System;
using System.Globalization;
using System.Text.RegularExpressions;
using dCore.MultiLanguage.Providers;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Models.Database;

namespace AMRent.Shared.Providers
{
    public class EmailSender
    {
        internal readonly Data.Contexts.FullDatabaseContext _context;
        internal readonly dCore.MultiLanguage.Providers.TranslationProvider _translationProvider;
        internal readonly dCore.Communication.Models.SmtpConfiguration _smtpConfiguration;
        internal readonly int _languageId;
        private string _siteUrl = "https://www.amrent.pt";

        public EmailSender(Data.Contexts.FullDatabaseContext context, dCore.MultiLanguage.Providers.TranslationProvider translationProvider, dCore.Communication.Models.SmtpConfiguration smtpConfiguration, int languageId)
        {
            _context = context;
            _translationProvider = translationProvider;
            _smtpConfiguration = smtpConfiguration;
            _languageId = languageId;
        }

        private string ReplaceLinkTags(string input)
        {
            // Define regular expression patterns for the tags
            string linkPattern = @"\[LINK_START\]\[TEXT\](.*?)\[URL\](.*?)\[LINK_END\]";
            input = Regex.Replace(input, linkPattern, match =>
            {
                string urlText = match.Groups[1].Value.Trim();
                string urlHref = match.Groups[2].Value.Trim();
                return $"<a style='color: #d9883c !important;' href='{urlHref}' target='_blank'>{urlText}</a>";
            });

            return input;
        }

        private string ReplacePasswordResetTags(string input, Data.Models.Database.Quotation quotation)
        {
            input = input.Replace("[NAME]", quotation.CustomerName);
            input = input.Replace("[NUMBER]", quotation.Number);
            if (input.Contains("[EMPLOYEE_NAME]"))
            {
                string employeeName = quotation.User != null ?
                    $"{quotation.User.FirstName} {quotation.User.LastName}" :
                    _translationProvider.GetTranslation(_languageId, "Email.EquipaAMRent");

                input = input.Replace("[EMPLOYEE_NAME]", employeeName);
            }

            return input;
        }

        private string ReplaceReservationTags(string input, Data.Models.Database.Reservation reservation)
        {
            input = input.Replace("[NAME]", reservation.DriverName);
            input = input.Replace("[NUMBER]", reservation.Number);
            input = input.Replace("[PAYMENT_URL]", $"{_siteUrl}/Booking/Payment/?reservationNumber={reservation.Number}");
            if (input.Contains("[EMPLOYEE_NAME]"))
            {
                string employeeName = reservation.AssignedUser != null ?
                    $"{reservation.AssignedUser.FirstName} {reservation.AssignedUser.LastName}" :
                    _translationProvider.GetTranslation(_languageId, "Email.EquipaAMRent");

                input = input.Replace("[EMPLOYEE_NAME]", employeeName);
            }

            if (input.Contains("[PAYMENT_INFO]"))
            {
                string paymentInfo = "";
                if (reservation.PaymentStatus == Data.Enums.PaymentStatus.Paid
                    || reservation.PaymentType == Data.Enums.PaymentTypes.PostPaid
                    || reservation.PaymentType == Data.Enums.PaymentTypes.Cash)
                {
                    input.Replace("[PAYMENT_INFO]", "");
                }
                else
                {
                    switch (reservation.PaymentType)
                    {
                        case Data.Enums.PaymentTypes.CreditCard:
                            paymentInfo += _translationProvider.GetTranslation(_languageId, "EmailRegistoReserva.Pagamento.CartaoCredito");
                            break;
                        case Data.Enums.PaymentTypes.MBReference:
                            paymentInfo += _translationProvider.GetTranslation(_languageId, "EmailRegistoReserva.Pagamento.Multibanco");
                            paymentInfo += "<br>";
                            paymentInfo += _translationProvider.GetTranslation(_languageId, "EmailRegistoReserva.Pagamento.Multibanco.Entidade");
                            paymentInfo += $": {reservation.MultibancoEntity}<br>";
                            paymentInfo += _translationProvider.GetTranslation(_languageId, "EmailRegistoReserva.Pagamento.Multibanco.Referencia");
                            paymentInfo += $": {reservation.MultibancoReference}<br>";
                            paymentInfo += _translationProvider.GetTranslation(_languageId, "EmailRegistoReserva.Pagamento.Multibanco.Valor");
                            paymentInfo += $": {reservation.TotalCostOverride ?? reservation.TotalCost}€";
                            break;
                        case Data.Enums.PaymentTypes.MBWay:
                            paymentInfo += _translationProvider.GetTranslation(_languageId, "EmailRegistoReserva.Pagamento.MBWay");
                            break;
                        case Data.Enums.PaymentTypes.BankTransfer:
                            paymentInfo += _translationProvider.GetTranslation(_languageId, "EmailRegistoReserva.Pagamento.TransferenciaBancaria");
                            break;
                    }

                    input = input.Replace("[PAYMENT_INFO]", paymentInfo);
                }
            }

            return input;
        }

        private string ReplaceQuotationTags(string input, Data.Models.Database.Quotation quotation)
        {
            input = input.Replace("[NAME]", quotation.CustomerName);
            input = input.Replace("[NUMBER]", quotation.Number);
            input = input.Replace("[QUOTATION_EXPIRE_DATE]", quotation.ExpireDateTime.ToString("dd/MM/yyyy"));
            if (input.Contains("[EMPLOYEE_NAME]"))
            {
                string employeeName = quotation.User != null ?
                    $"{quotation.User.FirstName} {quotation.User.LastName}" :
                    _translationProvider.GetTranslation(_languageId, "Email.EquipaAMRent");

                input = input.Replace("[EMPLOYEE_NAME]", employeeName);
            }

            return input;
        }

        private string RemoveTemplateSection(string template, string sectionName)
        {
            var pattern = $@"<!--\[\[{sectionName}\]\]-->(.*?)<!--\[\[/{sectionName}\]\]-->";

            return Regex.Replace(template, pattern, string.Empty, RegexOptions.Singleline);
        }

        public static string RemoveHtmlComments(string html)
        {
            return Regex.Replace(html, @"<!--(.*?)-->", string.Empty, RegexOptions.Singleline);
        }

        public static string ProcessTemplateForeachSection<T>(string template, string sectionName, IEnumerable<T> items, Func<T, string, string> processItemContent)
        {
            var pattern = $@"<!--\[\[Foreach:{sectionName}\]\]-->(.*?)<!--\[\[/Foreach:{sectionName}\]\]-->";
            var regex = new Regex(pattern, RegexOptions.Singleline);

            return regex.Replace(template, match =>
            {
                var itemTemplate = match.Groups[1].Value;

                var processedItems = items.Select(item =>
                {
                    var itemContent = itemTemplate;

                    itemContent = processItemContent(item, itemContent);

                    return itemContent;
                });

                return string.Join(Environment.NewLine, processedItems);
            });
        }

        private string BuildReservationSummary(Data.Models.Database.Reservation reservation)
        {
            Data.Models.Database.CarSegment carSegment = _context.CarSegments.AsNoTracking()
                .Include(x => x.CarCategory)
                    .ThenInclude(y => y.Translations.Where(z => z.LanguageId == _languageId))
                .Include(x => x.CarFuel)
                    .ThenInclude(y => y.Translations.Where(z => z.LanguageId == _languageId))
                .Include(x => x.CarGearbox)
                    .ThenInclude(y => y.Translations.Where(z => z.LanguageId == _languageId))
                .FirstOrDefault(x => x.Id == reservation.CarSegmentId);
            Shared.Providers.CostCalculator costCalculatorHelper = new Shared.Providers.CostCalculator(_context, reservation.PickupDateTime, reservation.ReturnDateTime);
            int days = costCalculatorHelper.GetTotalDays();
            string valueWithoutDiscount = (reservation.CarSegmentCost + reservation.PickupCost + reservation.ReturnCost + reservation.InsuranceCost + reservation.Extras?.Sum(x => x.UnitValue * x.Quantity) + reservation.Services?.Sum(x => x.Value) != (reservation.TotalCostOverride ?? reservation.TotalCost)) ? $"<s>{reservation.CarSegmentCost + reservation.PickupCost + reservation.ReturnCost + reservation.InsuranceCost + reservation.Extras?.Sum(x => x.UnitValue * x.Quantity) + reservation.Services?.Sum(x => x.Value)}€</s>&nbsp;&nbsp;" : "";

            string filePath = Path.Combine(AppContext.BaseDirectory, "Templates", "Html", "ReservationSummary.html");
            string reservationSummary = File.ReadAllText(filePath);

            reservationSummary = reservationSummary.Replace("[[CustomerName]]", reservation.BillName ?? reservation.DriverName);
            reservationSummary = reservationSummary.Replace("[[Number]]", reservation.Number);
            reservationSummary = reservationSummary.Replace("[[Date]]", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            reservationSummary = reservationSummary.Replace("[[PickupLocationName]]", reservation.PickupLocationId == -1 ? $"({reservation.PickupLocation.Translations.First().Name}) {reservation.CustomPickupLocationName}" : reservation.PickupLocation.Translations.First().Name);
            reservationSummary = reservationSummary.Replace("[[PickupDateTime]]", $"{reservation.PickupDateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)} {reservation.PickupDateTime.ToString("HH:mm")}");
            reservationSummary = reservationSummary.Replace("[[ReturnLocationName]]", reservation.ReturnLocationId == -1 ? $"({reservation.ReturnLocation.Translations.First().Name}) {reservation.CustomReturnLocationName}" : reservation.ReturnLocation.Translations.First().Name);
            reservationSummary = reservationSummary.Replace("[[ReturnDateTime]]", $"{reservation.ReturnDateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)} {reservation.ReturnDateTime.ToString("HH:mm")}");

            if (!string.IsNullOrEmpty(reservation.FlightNumber))
            {
                reservationSummary = reservationSummary.Replace("[[FlightNumber]]", reservation.FlightNumber);
            }
            else
            {
                reservationSummary = RemoveTemplateSection(reservationSummary, "FlightNumberSection");
            }

            reservationSummary = reservationSummary.Replace("[[SegmentName]]", reservation.CarSegment.Translations.First().Name);
            reservationSummary = reservationSummary.Replace("[[FuelName]]", carSegment.CarFuel.Translations.First().Name);
            reservationSummary = reservationSummary.Replace("[[GearboxName]]", carSegment.CarGearbox.Translations.First().Name);
            reservationSummary = reservationSummary.Replace("[[SegmentSeats]]", carSegment.Seats.ToString());

            if (carSegment.CarCategory.IsCommercial)
            {
                reservationSummary = reservationSummary.Replace("[[LoadingSpaceLength]]", (carSegment.LoadingSpaceLengthInMilimeters.HasValue ? ((decimal)carSegment.LoadingSpaceLengthInMilimeters / 1000) : 0).ToString("0.00"));
                reservationSummary = reservationSummary.Replace("[[LoadingSpaceWidth]]", (carSegment.LoadingSpaceWidthInMilimeters.HasValue ? ((decimal)carSegment.LoadingSpaceWidthInMilimeters / 1000) : 0).ToString("0.00"));
                reservationSummary = reservationSummary.Replace("[[LoadingSpaceHeight]]", (carSegment.LoadingSpaceHeightInMilimeters.HasValue ? ((decimal)carSegment.LoadingSpaceHeightInMilimeters / 1000) : 0).ToString("0.00"));
            }
            else
            {
                reservationSummary = RemoveTemplateSection(reservationSummary, "CommercialMeasuresSection");
            }

            if (!string.IsNullOrEmpty(carSegment.CarCategory.Translations.First().Included))
            {
                reservationSummary = ProcessTemplateForeachSection(reservationSummary, "CategoryCharacteristics", carSegment.CarCategory.Translations.First().Included.Split("\r\n"), (characteristic, characteristicsItemContent) =>
                {
                    characteristicsItemContent = characteristicsItemContent.Replace("CategoryCharacteristicsText", characteristic);
                    return characteristicsItemContent;
                });
            }
            else
            {
                reservationSummary = RemoveTemplateSection(reservationSummary, "CategoryCharacteristicsSection");
            }

            reservationSummary = reservationSummary.Replace("[[SegmentId]]", reservation.CarSegmentId.ToString());
            reservationSummary = reservationSummary.Replace("[[ReservationDays]]", days.ToString());
            reservationSummary = reservationSummary.Replace("[[SegmentAndPickupReturnCost]]", (reservation.TotalCost - reservation.InsuranceCost - reservation.Extras?.Sum(x => x.UnitValue * x.Quantity) - reservation.Services?.Sum(x => x.Value)).ToString());
            reservationSummary = reservationSummary.Replace("[[InsuranceCost]]", reservation.InsuranceCost.ToString());
            reservationSummary = reservationSummary.Replace("[[ExtrasAndServicesCost]]", (reservation.Extras?.Sum(x => x.UnitValue * x.Quantity) + reservation.Services?.Sum(x => x.Value)).ToString());

            if (reservation.HasAdvancePartialPayment)
            {
                reservationSummary = reservationSummary.Replace("[[AdvanceParcialPaymentValue]]", (reservation.AdvancePartialPaymentValue ?? 0).ToString());
                reservationSummary = reservationSummary.Replace("[[LoadingSpaceWidth]]", (carSegment.LoadingSpaceWidthInMilimeters ?? 0).ToString());
                reservationSummary = reservationSummary.Replace("[[LoadingSpaceHeight]]", (carSegment.LoadingSpaceHeightInMilimeters ?? 0).ToString());
            }
            else
            {
                reservationSummary = RemoveTemplateSection(reservationSummary, "AdvanceParcialPaymentSection");
            }

            reservationSummary = reservationSummary.Replace("[[ValueWithoutDiscount]]", valueWithoutDiscount);
            reservationSummary = reservationSummary.Replace("[[TotalCost]]", (reservation.TotalCostOverride ?? reservation.TotalCost).ToString());
            reservationSummary = reservationSummary.Replace("[[InsuranceLevelName]]", reservation.InsuranceLevel.Translations.First().Name);
            reservationSummary = reservationSummary.Replace("[[InsuranceExcess]]", reservation.InsuranceExcess.ToString());

            reservationSummary = ProcessTemplateForeachSection(reservationSummary, "Extras", reservation.Extras, (extra, itemContent) =>
            {
                string extraQuantity = extra.Quantity > 1 ? $" ({extra.Quantity})" : "";
                itemContent = itemContent.Replace("[[ExtraName]]", extra.Extra.Translations.First().Name);
                itemContent = itemContent.Replace("[[ExtraQuantity]]", extraQuantity.ToString());
                itemContent = itemContent.Replace("[[ExtraUnitCost]]", extra.UnitValue == 0 ? "([[Translation:Extra.Incluido]])" : $"{extra.UnitValue.ToString()}€");
                return itemContent;
            });
            reservationSummary = ProcessTemplateForeachSection(reservationSummary, "Services", reservation.Services, (service, itemContent) =>
            {
                itemContent = itemContent.Replace("[[ServiceName]]", service.Service.Translations.First().Name);
                itemContent = itemContent.Replace("[[ServiceCost]]", service.Value.ToString());
                return itemContent;
            });
            reservationSummary = ProcessTemplateForeachSection(reservationSummary, "PickupReturnTemporaryTaxes", reservation.PickupReturnTemporaryTaxes, (pickupReturnTemporaryTax, itemContent) =>
            {
                string pickupReturnTemporaryTaxQuantity = pickupReturnTemporaryTax.Quantity > 1 ? $" ({pickupReturnTemporaryTax.Quantity})" : "";
                itemContent = itemContent.Replace("[[TaxName]]", pickupReturnTemporaryTax.PickupReturnTemporaryTax.Translations.First().Name);
                itemContent = itemContent.Replace("[[TaxQuantity]]", pickupReturnTemporaryTaxQuantity.ToString());
                itemContent = itemContent.Replace("[[TaxUnitCost]]", $"{pickupReturnTemporaryTax.UnitValue.ToString()}€");
                return itemContent;
            });

            List<Data.Models.Database.InsuranceLevel> insuranceLevels = _context.InsuranceLevels.AsNoTracking().Include(x => x.Translations.Where(z => z.LanguageId == _languageId)).OrderBy(x => x.Id).ToList();

            reservationSummary = ProcessTemplateForeachSection(reservationSummary, "InsuranceLevels", insuranceLevels, (insuranceLevel, itemContent) =>
            {
                itemContent = itemContent.Replace("[[InsuranceLevelsInsuranceLevelName]]", insuranceLevel.Translations.First().Name);
                itemContent = itemContent.Replace("[[InsuranceLevelsInsuranceLevelIncludedProtection]]", string.Join("<br>", insuranceLevel?.Translations?.First()?.Included.Split("\n")));
                return itemContent;
            });

            var translationRegex = new Regex(@"\[\[Translation:(.*?)\]\]");

            reservationSummary = translationRegex.Replace(reservationSummary, match =>
            {
                var key = match.Groups[1].Value;
                var translation = _translationProvider.GetTranslation(_languageId, key);
                return translation;
            });

            return reservationSummary;
        }

        private string BuildQuotationSummary(Data.Models.Database.Quotation quotation)
        {
            Shared.Providers.CostCalculator costCalculatorHelper = new Shared.Providers.CostCalculator(_context, quotation.PickupDateTime, quotation.ReturnDateTime);
            int days = costCalculatorHelper.GetTotalDays();

            string filePath = Path.Combine(AppContext.BaseDirectory, "Templates", "Html", "QuotationSummary.html");
            string quotationSummary = File.ReadAllText(filePath);

            quotationSummary = quotationSummary.Replace("[[CustomerName]]", quotation.CustomerName);
            quotationSummary = quotationSummary.Replace("[[Number]]", quotation.Number);
            quotationSummary = quotationSummary.Replace("[[Date]]", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            quotationSummary = quotationSummary.Replace("[[ExpireDate]]", quotation.ExpireDateTime.ToString("dd/MM/yyyy HH:mm"));
            quotationSummary = quotationSummary.Replace("[[PickupLocationName]]", quotation.PickupLocationId == -1 ? $"({quotation.PickupLocation.Translations.First().Name}) {quotation.CustomPickupLocationName}" : quotation.PickupLocation.Translations.First().Name);
            quotationSummary = quotationSummary.Replace("[[PickupDateTime]]", $"{quotation.PickupDateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)} {quotation.PickupDateTime.ToString("HH:mm")}");
            quotationSummary = quotationSummary.Replace("[[ReturnLocationName]]", quotation.ReturnLocationId == -1 ? $"({quotation.ReturnLocation.Translations.First().Name}) {quotation.CustomReturnLocationName}" : quotation.ReturnLocation.Translations.First().Name);
            quotationSummary = quotationSummary.Replace("[[ReturnDateTime]]", $"{quotation.ReturnDateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)} {quotation.ReturnDateTime.ToString("HH:mm")}");

            quotationSummary = ProcessTemplateForeachSection(quotationSummary, "QuotationItems", quotation.QuotationItems, (quotationItem, itemContent) =>
            {
                Data.Models.Database.CarSegment carSegment = _context.CarSegments.AsNoTracking()
                    .Include(x => x.Translations.Where(z => z.LanguageId == _languageId))
                    .Include(x => x.CarCategory)
                        .ThenInclude(y => y.Translations.Where(z => z.LanguageId == _languageId))
                    .Include(x => x.CarFuel)
                        .ThenInclude(y => y.Translations.Where(z => z.LanguageId == _languageId))
                    .Include(x => x.CarGearbox)
                        .ThenInclude(y => y.Translations.Where(z => z.LanguageId == _languageId))
                    .FirstOrDefault(x => x.Id == quotationItem.CarSegmentId);
                string valueWithoutDiscount = (quotationItem.CarSegmentCost + quotationItem.PickupCost + quotationItem.ReturnCost + quotationItem.InsuranceCost + quotationItem.Extras?.Sum(x => x.UnitValue * x.Quantity) + quotationItem.Services?.Sum(x => x.Value) != (quotationItem.TotalCostOverride ?? quotationItem.TotalCost)) ? $"<s>{quotationItem.CarSegmentCost + quotationItem.PickupCost + quotationItem.ReturnCost + quotationItem.InsuranceCost + quotationItem.Extras?.Sum(x => x.UnitValue * x.Quantity) + quotationItem.Services?.Sum(x => x.Value)}€</s>&nbsp;&nbsp;" : "";

                itemContent = itemContent.Replace("[[SegmentName]]", carSegment.Translations.First().Name);
                itemContent = itemContent.Replace("[[FuelName]]", carSegment.CarFuel.Translations.First().Name);
                itemContent = itemContent.Replace("[[GearboxName]]", carSegment.CarGearbox.Translations.First().Name);
                itemContent = itemContent.Replace("[[SegmentSeats]]", carSegment.Seats.ToString());

                if (carSegment.CarCategory.IsCommercial)
                {
                    itemContent = itemContent.Replace("[[LoadingSpaceLength]]", (carSegment.LoadingSpaceLengthInMilimeters.HasValue ? ((decimal)carSegment.LoadingSpaceLengthInMilimeters / 1000) : 0).ToString("0.##"));
                    itemContent = itemContent.Replace("[[LoadingSpaceWidth]]", (carSegment.LoadingSpaceWidthInMilimeters.HasValue ? ((decimal)carSegment.LoadingSpaceWidthInMilimeters / 1000) : 0).ToString("0.##"));
                    itemContent = itemContent.Replace("[[LoadingSpaceHeight]]", (carSegment.LoadingSpaceHeightInMilimeters.HasValue ? ((decimal)carSegment.LoadingSpaceHeightInMilimeters / 1000) : 0).ToString("0.##"));
                }
                else
                {
                    itemContent = RemoveTemplateSection(itemContent, "CommercialMeasuresSection");
                }

                if (!string.IsNullOrEmpty(carSegment.CarCategory.Translations.First().Included))
                {
                    itemContent = ProcessTemplateForeachSection(itemContent, "CategoryCharacteristics", carSegment.CarCategory.Translations.First().Included.Split("\r\n"), (characteristic, characteristicsItemContent) =>
                    {
                        characteristicsItemContent = characteristicsItemContent.Replace("[[CategoryCharacteristicsText]]", characteristic);
                        return characteristicsItemContent;
                    });
                }
                else
                {
                    itemContent = RemoveTemplateSection(itemContent, "CategoryCharacteristicsSection");
                }


                itemContent = itemContent.Replace("[[SegmentId]]", quotationItem.CarSegmentId.ToString());
                itemContent = itemContent.Replace("[[ReservationDays]]", days.ToString());
                itemContent = itemContent.Replace("[[SegmentAndPickupReturnCost]]", (quotationItem.TotalCost - quotationItem.InsuranceCost - quotationItem.Extras?.Sum(x => x.UnitValue * x.Quantity) - quotationItem.Services?.Sum(x => x.Value)).ToString());
                itemContent = itemContent.Replace("[[InsuranceCost]]", quotationItem.InsuranceCost.ToString());
                itemContent = itemContent.Replace("[[ExtrasAndServicesCost]]", (quotationItem.Extras?.Sum(x => x.UnitValue * x.Quantity) + quotationItem.Services?.Sum(x => x.Value)).ToString());
                itemContent = itemContent.Replace("[[ValueWithoutDiscount]]", valueWithoutDiscount);
                itemContent = itemContent.Replace("[[TotalCost]]", (quotationItem.TotalCostOverride ?? quotationItem.TotalCost).ToString());
                itemContent = itemContent.Replace("[[InsuranceLevelName]]", quotationItem.InsuranceLevel.Translations.First().Name);
                itemContent = itemContent.Replace("[[InsuranceExcess]]", quotationItem.InsuranceExcess.ToString());

                itemContent = ProcessTemplateForeachSection(itemContent, "Extras", quotationItem.Extras, (extra, extrasItemContent) =>
                {
                    string extraQuantity = extra.Quantity > 1 ? $" ({extra.Quantity})" : "";
                    extrasItemContent = extrasItemContent.Replace("[[ExtraName]]", extra.Extra.Translations.First().Name);
                    extrasItemContent = extrasItemContent.Replace("[[ExtraQuantity]]", extraQuantity.ToString());
                    extrasItemContent = extrasItemContent.Replace("[[ExtraUnitCost]]", extra.UnitValue == 0 ? "([[Translation:Extra.Incluido]])" : $"{extra.UnitValue.ToString()}€");
                    return extrasItemContent;
                });

                itemContent = ProcessTemplateForeachSection(itemContent, "Services", quotationItem.Services, (service, servicesItemContent) =>
                {
                    servicesItemContent = servicesItemContent.Replace("[[ServiceName]]", service.Service.Translations.First().Name);
                    servicesItemContent = servicesItemContent.Replace("[[ServiceCost]]", service.Value.ToString());
                    return servicesItemContent;
                });

                itemContent = ProcessTemplateForeachSection(itemContent, "PickupReturnTemporaryTaxes", quotationItem.PickupReturnTemporaryTaxes, (pickupReturnTemporaryTax, pickupReturnTemporaryTaxesItemContent) =>
                {
                    string pickupReturnTemporaryTaxQuantity = pickupReturnTemporaryTax.Quantity > 1 ? $" ({pickupReturnTemporaryTax.Quantity})" : "";
                    pickupReturnTemporaryTaxesItemContent = pickupReturnTemporaryTaxesItemContent.Replace("[[TaxName]]", pickupReturnTemporaryTax.PickupReturnTemporaryTax.Translations.First().Name);
                    pickupReturnTemporaryTaxesItemContent = pickupReturnTemporaryTaxesItemContent.Replace("[[TaxQuantity]]", pickupReturnTemporaryTaxQuantity.ToString());
                    pickupReturnTemporaryTaxesItemContent = pickupReturnTemporaryTaxesItemContent.Replace("[[TaxUnitCost]]", $"{pickupReturnTemporaryTax.UnitValue.ToString()}€");
                    return pickupReturnTemporaryTaxesItemContent;
                });

                return itemContent;
            });

            List<Data.Models.Database.InsuranceLevel> insuranceLevels = _context.InsuranceLevels.AsNoTracking()
                .Include(x => x.Translations.Where(z => z.LanguageId == _languageId))
                .Where(x =>  x.Insurances.Any(y => quotation.QuotationItems.Select(z => z.CarSegmentId).Contains(y.CarSegmentId)))
                .OrderBy(x => x.Id).ToList();

            quotationSummary = ProcessTemplateForeachSection(quotationSummary, "InsuranceLevels", insuranceLevels, (insuranceLevel, itemContent) =>
            {
                itemContent = itemContent.Replace("[[InsuranceLevelsInsuranceLevelName]]", insuranceLevel.Translations.First().Name);
                itemContent = itemContent.Replace("[[InsuranceLevelsInsuranceLevelIncludedProtection]]", string.Join("<br>", insuranceLevel?.Translations?.First()?.Included.Split("\n")));
                return itemContent;
            });

            var translationRegex = new Regex(@"\[\[Translation:(.*?)\]\]");

            quotationSummary = translationRegex.Replace(quotationSummary, match =>
            {
                var key = match.Groups[1].Value;
                var translation = _translationProvider.GetTranslation(_languageId, key);
                return translation;
            });

            return quotationSummary;
        }

        private const string EmailHtmlStart = " <html style='width: 100%' width='100%'><body style='width: 100%' width='100%'><table style='width: 100%; padding: 0; border: 0;' width='100%' border='0' cellpadding='25px' cellspacing='0' style='background-color: #fff;'><tr><td><font size='2'><table width='100%' cellpadding='0' cellspacing='0' style='border: 2px solid #d9883c; border-radius: 10px; background-color: #FCFBFB; width: 100%;'><tr><td colspan='5' height='20px' style='height: 20px'></td></tr><tr><td style='width: 20px' width='20px'></td><td><img src='https://www.amrent.pt/img/logo_200.png' /></td><td></td><td></td><td width='20px' style='width: 20px'></td></tr><tr><td colspan='5' style='height: 40px' height='40px'></td></tr><tr><td style='width: 20px' width='20px'></td><td colspan='3'>";
        private const string EmailHtmlEnd = "</td><td style='width: 20px' width='20px'></td></tr><tr><td colspan='5' style='height: 20px' height='20px'></td></tr></table></font></td></tr></table></body></html>";

        public async Task Send(Data.Enums.EmailContentTypes emailType, bool isTestEnvironment, int? reservationQuotationId = null, string[]? adminDestinationAddresses = null, bool sendAsync = true, Guid? userId = null)
        {
            _siteUrl = isTestEnvironment ? "https://testwww.amrent.pt" : "https://www.amrent.pt";
            Data.Models.Database.EmailContentTranslation emailContentTranslation = _context.EmailContents
                .Include(x => x.Translations)
                .FirstOrDefault(x => x.Type == emailType).Translations
                .FirstOrDefault(x => x.LanguageId == _languageId);

            if (emailContentTranslation != null)
            {
                dCore.Communication.Providers.Email emailProvider = new dCore.Communication.Providers.Email();
                List<string> recipientEmailAddresses = new List<string>();
                string subject = $"{(isTestEnvironment ? "[TESTES] - " : "")}{emailContentTranslation.Subject}";
                string content = EmailHtmlStart;
                List<string> bccEmailAddresses = new List<string>();
                List<(byte[] Data, string FileName, string MimeType)> attachments = new();

                Data.Models.Database.EmailContent dbEmailType = _context.EmailContents.FirstOrDefault(x => x.Type == emailType);

                switch (emailType)
                {
                    case Data.Enums.EmailContentTypes.QuotationRegistration:
                    case Data.Enums.EmailContentTypes.QuotationExpiring:

                        Data.Models.Database.Quotation quotation =
                            _context.Quotations
                            .Include(x => x.QuotationItems).ThenInclude(x => x.CarSegment).ThenInclude(x => x.Translations.Where(y => y.LanguageId == _languageId))
                            .Include(x => x.PickupLocation).ThenInclude(x => x.Translations.Where(y => y.LanguageId == _languageId))
                            .Include(x => x.ReturnLocation).ThenInclude(x => x.Translations.Where(y => y.LanguageId == _languageId))
                            .Include(x => x.QuotationItems).ThenInclude(x => x.InsuranceLevel).ThenInclude(x => x.Translations.Where(y => y.LanguageId == _languageId))
                            .Include(x => x.QuotationItems).ThenInclude(x => x.Extras).ThenInclude(x => x.Extra).ThenInclude(x => x.Translations.Where(y => y.LanguageId == _languageId))
                            .Include(x => x.QuotationItems).ThenInclude(x => x.Services).ThenInclude(x => x.Service).ThenInclude(x => x.Translations.Where(y => y.LanguageId == _languageId))
                            .Include(x => x.QuotationItems).ThenInclude(x => x.PickupReturnTemporaryTaxes).ThenInclude(x => x.PickupReturnTemporaryTax).ThenInclude(x => x.Translations.Where(y => y.LanguageId == _languageId))
                            .Include(x => x.User)
                            .FirstOrDefault(x => x.Id == reservationQuotationId);


                        if (emailType == Data.Enums.EmailContentTypes.QuotationRegistration && quotation.Status == Data.Enums.QuotationStatus.Requested)
                        {
                            if (adminDestinationAddresses != null && adminDestinationAddresses.Any())
                            {
                                string adminContent = EmailHtmlStart;
                                adminContent += $"Foi registado um novo pedido de cotação com o número {quotation.Number}. Verifique no backoffice -> <a href='http://{(isTestEnvironment ? "test" : "")}admin.amrent.pt/Quotations/Edit/{quotation.Id}'>Abrir</a>";
                                adminContent += EmailHtmlEnd;
                                await emailProvider.Send(adminDestinationAddresses,
                                    $"{(isTestEnvironment ? "[TESTES] - " : "")}Pedido de cotação - {quotation.Number}",
                                    adminContent,
                                    _smtpConfiguration);
                            }
                        }
                        else
                        {
                            subject = ReplaceQuotationTags(subject, quotation);
                            bccEmailAddresses.Add(quotation.User.EmailAddress);
                            recipientEmailAddresses.Add(quotation.CustomerEmailAddress);

                            foreach (string line in emailContentTranslation.Text.Split("\n"))
                            {
                                string lineWithReplacedTags = ReplaceQuotationTags(line, quotation);
                                lineWithReplacedTags = ReplaceLinkTags(lineWithReplacedTags);
                                content += lineWithReplacedTags;
                                content += "<br>";
                            }

                            if (dbEmailType.SendQuotationReservationSummaryPdf)
                            {
                                string summaryPdfcontent = $"<html><body>{RemoveHtmlComments(BuildQuotationSummary(quotation))}</body></html>";
                                var pdfService = new Shared.Providers.Pdf();
                                byte[] pdfBytes = pdfService.ConvertHtmlToPdf(summaryPdfcontent);
                                attachments.Add((pdfBytes, $"{quotation.Number}.pdf", "application/pdf"));
                            }
                        }


                        break;

                    case Data.Enums.EmailContentTypes.ReservationRegistration:
                    case Data.Enums.EmailContentTypes.ReservationRegistrationFromQuotation:
                    case Data.Enums.EmailContentTypes.PaymentChoiceReservationFromQuotation:
                    case Data.Enums.EmailContentTypes.PaymentConfirmation:
                    case Data.Enums.EmailContentTypes.ReservationCancellation:
                    case Data.Enums.EmailContentTypes.ReservationApproval:
                    case Data.Enums.EmailContentTypes.PaymentFailure:
                    case Data.Enums.EmailContentTypes.ReservationFinished:

                        Data.Models.Database.Reservation reservation =
                            _context.Reservations
                            .Include(x => x.CarSegment).ThenInclude(x => x.Translations.Where(y => y.LanguageId == _languageId))
                            .Include(x => x.PickupLocation).ThenInclude(x => x.Translations.Where(y => y.LanguageId == _languageId))
                            .Include(x => x.ReturnLocation).ThenInclude(x => x.Translations.Where(y => y.LanguageId == _languageId))
                            .Include(x => x.InsuranceLevel).ThenInclude(x => x.Translations.Where(y => y.LanguageId == _languageId))
                            .Include(x => x.Extras).ThenInclude(x => x.Extra).ThenInclude(x => x.Translations.Where(y => y.LanguageId == _languageId))
                            .Include(x => x.Services).ThenInclude(x => x.Service).ThenInclude(x => x.Translations.Where(y => y.LanguageId == _languageId))
                            .Include(x => x.PickupReturnTemporaryTaxes).ThenInclude(x => x.PickupReturnTemporaryTax).ThenInclude(x => x.Translations.Where(y => y.LanguageId == _languageId))
                            .Include(x => x.AssignedUser)
                            .FirstOrDefault(x => x.Id == reservationQuotationId);

                        subject = ReplaceReservationTags(subject, reservation);
                        recipientEmailAddresses.Add(reservation.BillEmailAddress);
                        if (!string.IsNullOrEmpty(reservation.DriverEmailAddress) && reservation.DriverEmailAddress != reservation.BillEmailAddress)
                        {
                            recipientEmailAddresses.Add(reservation.DriverEmailAddress);
                        }
                        if (reservation.AssignedUser != null && emailType != Data.Enums.EmailContentTypes.ReservationFinished)
                        {
                            bccEmailAddresses.Add(reservation.AssignedUser.EmailAddress);
                        }
                        if (emailType == Data.Enums.EmailContentTypes.ReservationApproval)
                        {
                            bccEmailAddresses.Add(_smtpConfiguration.BccAddress);
                        }

                        foreach (string line in emailContentTranslation.Text.Split("\n"))
                        {
                            string lineWithReplacedTags = ReplaceReservationTags(line, reservation);
                            lineWithReplacedTags = ReplaceLinkTags(lineWithReplacedTags);
                            content += lineWithReplacedTags;
                            content += "<br>";
                        }

                        if (emailType == Data.Enums.EmailContentTypes.ReservationRegistration && reservation.Source == Data.Enums.ReservationQuotationSources.W)
                        {
                            if (adminDestinationAddresses != null && adminDestinationAddresses.Any())
                            {
                                string adminContent = EmailHtmlStart;
                                adminContent += $"Foi registada uma nova reserva com o número {reservation.Number}. Verifique no backoffice -> <a href='http://{(isTestEnvironment ? "test" : "")}admin.amrent.pt/Reservations/Edit/{reservation.Id}'>Abrir</a>";
                                adminContent += EmailHtmlEnd;
                                await emailProvider.Send(adminDestinationAddresses,
                                    $"{(isTestEnvironment ? "[TESTES] - " : "")}{dCore.Helpers.Enum.GetDescription(emailType)} - {reservation.Number}",
                                    adminContent,
                                    _smtpConfiguration);
                            }
                        }

                        if (dbEmailType.SendQuotationReservationSummaryPdf)
                        {
                            string summaryPdfcontent = $"<html><body>{RemoveHtmlComments(BuildReservationSummary(reservation))}</body></html>";
                            var pdfService = new Shared.Providers.Pdf();
                            byte[] pdfBytes = pdfService.ConvertHtmlToPdf(summaryPdfcontent);
                            attachments.Add((pdfBytes, $"{reservation.Number}.pdf", "application/pdf"));
                        }
                        break;
                    case Data.Enums.EmailContentTypes.PasswordReset:
                    case Data.Enums.EmailContentTypes.AccountRegistered:
                        var user = _context.Users.FirstOrDefault(x => x.Id == userId);
                        recipientEmailAddresses.Add(user.EmailAddress);
                        switch (emailType)
                        {
                            case Data.Enums.EmailContentTypes.PasswordReset:
                                foreach (string line in emailContentTranslation.Text.Split("\n"))
                                {
                                    content += $"{line.Replace("[PASSWORD_RESET_URL]", $"{_siteUrl}/Account/ResetPassword/?id={userId.ToString()}")}<br>";
                                    content = ReplaceLinkTags(content);
                                }
                                break;
                            case Data.Enums.EmailContentTypes.AccountRegistered:
                                foreach (string line in emailContentTranslation.Text.Split("\n"))
                                {
                                    content += $"{line.Replace("[ACTIVATE_ACCOUNT_URL]", $"{_siteUrl}/Account/Activate/?id={userId.ToString()}")}<br>";
                                    content = ReplaceLinkTags(content);
                                }
                                break;
                        }

                        break;
                    default:
                        break;
                }

                content += EmailHtmlEnd;

                content = RemoveHtmlComments(content);

                await emailProvider.Send(recipientEmailAddresses.ToArray(), subject, content, _smtpConfiguration, bccEmailAddresses.ToArray(), sendAsync, attachments);
            }
        }
        public async Task Send(Data.Enums.InternalEmailContentTypes emailType, bool isTestEnvironment, string[]? adminDestinationAddresses = null, bool sendAsync = true, Guid? userId = null, int[]? changedDataProtectionConsentIds = null)
        {
            _siteUrl = isTestEnvironment ? "https://testwww.amrent.pt" : "https://www.amrent.pt";

            dCore.Communication.Providers.Email emailProvider = new dCore.Communication.Providers.Email();
            List<string> recipientEmailAddresses = new List<string>();
            string subject = "";
            string content = EmailHtmlStart;
            List<string> bccEmailAddresses = new List<string>();

            switch (emailType)
            {
                case Data.Enums.InternalEmailContentTypes.DataProtectionConsentChanged:

                    Data.Models.Database.User user =
                        _context.Users
                        .Include(x => x.DataProtectionConsents).ThenInclude(x => x.DataProtectionConsent).ThenInclude(x => x.Translations.Where(y => y.LanguageId == _languageId))
                        .FirstOrDefault(x => x.Id == userId);

                    subject = $"{(isTestEnvironment ? "[TESTES] - " : "")}Alteração RGPD - {user.Name}";
                    recipientEmailAddresses.AddRange(adminDestinationAddresses);
                    content += $"Os consentimentos RGPD do utilizador {user.Name} foram alterados. Verifique no backoffice -> <a href='http://{(isTestEnvironment ? "test" : "")}admin.amrent.pt/Users/CustomerDetail/{user.Id}'>Abrir</a><br><br>";
                    foreach (var changedDataProtectionConsent in user.DataProtectionConsents.Where(x => changedDataProtectionConsentIds.Contains(x.DataProtectionConsentId)))
                    {
                        string consentValue = changedDataProtectionConsent.HasConsented ? "Sim" : "Não";
                        content += $"{consentValue} : {changedDataProtectionConsent.DataProtectionConsent.Translations.First().Text}";
                    }
                    break;
            }

            content += EmailHtmlEnd;

            content = RemoveHtmlComments(content);

            await emailProvider.Send(recipientEmailAddresses.ToArray(), subject, content, _smtpConfiguration, bccEmailAddresses.ToArray(), sendAsync);
        }
    }
}

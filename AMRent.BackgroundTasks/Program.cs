using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using AMRent.Data.Models.Database;
using AMRent.Shared.Providers.FileParser.Mopr.Parsers;
using AMRent.Shared.Providers.FileParser;
using System.Transactions;
using System.Text;

namespace AMRent.BackgroundTasks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            NLog.LogManager.Configuration = new NLogLoggingConfiguration(configuration.GetSection("NLog"));

            using var serviceProvider = new ServiceCollection()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddNLog();
                })
                .BuildServiceProvider();

            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            logger.LogDebug("App starting...");

            try
            {
                DbContextOptionsBuilder<Data.Contexts.FullDatabaseContext> optionsBuilder = new();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

                using (var context = new Data.Contexts.FullDatabaseContext(optionsBuilder.Options))
                {
                    // Automatically close reservations that are past the return date
                    if (args.Contains("AutomaticReservationClose"))
                    {
                        logger.LogInformation($"Started Automatic Reservation Close");

                        var reservationsReadyToClose = context.Reservations
                            .Where(x =>
                                x.Status == Data.Enums.ReservationStatus.Confirmed
                                && x.ReturnDateTime < DateTime.UtcNow
                            ).ToList();

                        if (reservationsReadyToClose.Any())
                        {
                            var smtpConfiguration = configuration.GetSection("SmtpConfiguration").Get<dCore.Communication.Models.SmtpConfiguration>();
                            DbContextOptionsBuilder<dCore.MultiLanguage.Contexts.TranslationContext> translationContextOptionsBuilder = new();
                            translationContextOptionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                            using (dCore.MultiLanguage.Contexts.TranslationContext translationContext = new(translationContextOptionsBuilder.Options))
                            {
                                dCore.MultiLanguage.Providers.TranslationProvider _translationProvider = new(translationContext);

                                foreach (var reservation in reservationsReadyToClose)
                                {
                                    logger.LogInformation($"Reservation {reservation.Number} (id {reservation.Id})");
                                    reservation.Status = Data.Enums.ReservationStatus.Finished;

                                    context.SaveChanges();
                                    logger.LogInformation($"Set Finished status");

                                    Shared.Providers.EmailSender emailSender = new Shared.Providers.EmailSender(context, _translationProvider, smtpConfiguration, reservation.LanguageId);
                                    emailSender.Send(Data.Enums.EmailContentTypes.ReservationFinished, configuration["Environment"] == "Test", reservation.Id, sendAsync: false);
                                    logger.LogInformation($"Email sent");

                                    Thread.Sleep(10000);
                                }
                            }
                        }
                        logger.LogInformation($"Finished Automatic Reservation Close");
                    }

                    // Send email warning for quotation reaching expire date (run only once a day)
                    if (args.Contains("QuotationExpiringEmail"))
                    {
                        logger.LogInformation($"Started Quotation Expiring Email");

                        Data.Models.Database.Setting setting = context.Settings.FirstOrDefault(x => x.Key == "QuotationExpiringReminderDays");

                        if (int.TryParse(setting?.Value, out int daysAnticipation))
                        {
                            var excludedStatus = new List<Data.Enums.QuotationStatus>
                            {
                                Data.Enums.QuotationStatus.Finished,
                                Data.Enums.QuotationStatus.Cancelled
                            };
                            var quotationsExpiring = context.Quotations
                                .Where(x =>
                                    x.Status == Data.Enums.QuotationStatus.Registered
                                    && x.ExpireDateTime.AddDays(-(daysAnticipation)) < DateTime.UtcNow
                                    && x.ExpireDateTime.AddDays(-(daysAnticipation - 1)) > DateTime.UtcNow
                                    && !excludedStatus.Contains(x.Status)
                                ).ToList();

                            if (quotationsExpiring.Any())
                            {
                                var smtpConfiguration = configuration.GetSection("SmtpConfiguration").Get<dCore.Communication.Models.SmtpConfiguration>();
                                DbContextOptionsBuilder<dCore.MultiLanguage.Contexts.TranslationContext> translationContextOptionsBuilder = new();
                                translationContextOptionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                                using (dCore.MultiLanguage.Contexts.TranslationContext translationContext = new(translationContextOptionsBuilder.Options))
                                {
                                    dCore.MultiLanguage.Providers.TranslationProvider _translationProvider = new(translationContext);

                                    foreach (var quotation in quotationsExpiring)
                                    {
                                        logger.LogInformation($"Quotation {quotation.Number} (id {quotation.Id})");

                                        Shared.Providers.EmailSender emailSender = new Shared.Providers.EmailSender(context, _translationProvider, smtpConfiguration, quotation.LanguageId);
                                        emailSender.Send(Data.Enums.EmailContentTypes.QuotationExpiring, configuration["Environment"] == "Test", quotation.Id, sendAsync: false);
                                        logger.LogInformation($"Email sent");

                                        Thread.Sleep(10000);
                                    }
                                }
                            }
                        }
                        logger.LogInformation($"Finished Quotation Expiring Email");
                    }

                    // Get Via Verde MOPR file and integrate data
                    if (args.Contains("ViaVerdeMopr"))
                    {
                        logger.LogInformation($"Started Via Verde MOPR");

                        string basePath = AppContext.BaseDirectory;
                        string path = Path.Combine(basePath, "Files", "IN", "MOPR");
                        Directory.CreateDirectory(path);

                        var viaVerdeConfiguration = configuration.GetSection("ViaVerde").Get<Data.Models.Configuration.ViaVerde>();

                        Shared.Providers.Sftp sftpProvider = new Shared.Providers.Sftp(viaVerdeConfiguration.SftpAddress, viaVerdeConfiguration.SftpPort, viaVerdeConfiguration.SftpUsername, viaVerdeConfiguration.SftpPassword);

                        foreach (string fileName in sftpProvider.ListFiles("Mopr"))
                        {
                            string downloadedFilePathName = sftpProvider.DownloadFile(fileName, path);
                            var parsers = new Dictionary<string, object>
                            {
                                { "H", new MoprHeaderParser() },
                                { "1", new MoprDataRecordParser() },
                                { "F", new MoprTrailerParser() }
                            };

                            var processor = new FileProcessor(parsers);
                            var lines = File.ReadAllLines(downloadedFilePathName, Encoding.Latin1);
                            var records = processor.ProcessFile(lines);

                            foreach (var record in records)
                            {
                                if (record is Shared.Providers.FileParser.Mopr.Models.MoprDataRecord)
                                {
                                    var moprDataRecord = (Shared.Providers.FileParser.Mopr.Models.MoprDataRecord)record;
                                    context.ViaVerdeMovements.Add(new()
                                    {
                                        ManufacturerCode = moprDataRecord.ManufacturerCode,
                                        EquipmentNumber = moprDataRecord.EquipmentNumber,
                                        ContractLicencePlate = moprDataRecord.ContractLicencePlate,
                                        TariffClass = moprDataRecord.TariffClass,
                                        ExitTollCode = moprDataRecord.ExitTollCode,
                                        ExitTollName = moprDataRecord.ExitTollName,
                                        ExitDate = moprDataRecord.ExitDate,
                                        EntryTollCode = moprDataRecord.EntryTollCode,
                                        EntryTollName = moprDataRecord.EntryTollName,
                                        EntryDate = moprDataRecord.EntryDate,
                                        TariffInCents = moprDataRecord.TariffInCents,
                                        TransactionCode = moprDataRecord.TransactionCode,
                                        VatCode = moprDataRecord.VatCode,
                                        ServiceCode = moprDataRecord.ServiceCode,
                                        SendToViaVerde = true
                                    });
                                }
                            }

                            context.ProcessedFiles.Add(new Data.Models.Database.ProcessedFile()
                            {
                                FileName = fileName,
                                ProcessedAt = DateTime.UtcNow
                            });

                            sftpProvider.DeleteFile("Mopr", fileName);

                            context.SaveChanges();
                        }
                        logger.LogInformation($"Finished Via Verde MOPR");
                    }

                    // Produce and send Via Verde MERF file
                    if (args.Contains("ViaVerdeMerf"))
                    {
                        logger.LogInformation($"Started Via Verde MERF");

                        string basePath = AppContext.BaseDirectory;
                        string path = Path.Combine(basePath, "Files", "OUT", "MERF");
                        Directory.CreateDirectory(path);

                        var viaVerdeMovements = context.ViaVerdeMovements.Include(x => x.Country).Where(x => x.SendToViaVerde).ToList();

                        if (viaVerdeMovements.Any())
                        {
                            var viaVerdeConfiguration = configuration.GetSection("ViaVerde").Get<Data.Models.Configuration.ViaVerde>();

                            StringBuilder merfFileStringBuilder = new StringBuilder();

                            string fileName = $"MERF{viaVerdeConfiguration.RentACarCode}{viaVerdeConfiguration.ViaVerdeCode}{DateTime.UtcNow:yyyyMMdd}000";
                            string previousFileName = $"MERF{viaVerdeConfiguration.RentACarCode}{viaVerdeConfiguration.ViaVerdeCode}{DateTime.UtcNow.AddDays(-1):yyyyMMdd}000";

                            merfFileStringBuilder.AppendLine($"H {fileName} {DateTime.UtcNow.AddDays(-1):yyyyMMdd} 000 {DateTime.UtcNow:yyyyMMdd} 000 {viaVerdeConfiguration.FileSpecificationVersion}");

                            foreach (var viaVerdeMovement in viaVerdeMovements)
                            {
                                merfFileStringBuilder.Append($"1");
                                merfFileStringBuilder.Append($"{viaVerdeMovement.TransactionCode.PadRight(12, ' ')}");
                                switch (viaVerdeMovement.Status)
                                {
                                    case Data.Enums.ViaVerdeMovementStatus.Registered:
                                    case Data.Enums.ViaVerdeMovementStatus.Validated:
                                    default:
                                        viaVerdeMovement.ResultCode = "000";
                                        break;
                                    case Data.Enums.ViaVerdeMovementStatus.RejectedTow:
                                        viaVerdeMovement.ResultCode = "009";
                                        break;
                                    case Data.Enums.ViaVerdeMovementStatus.RejectedNotPayed:
                                        viaVerdeMovement.ResultCode = "005";
                                        break;
                                }
                                merfFileStringBuilder.Append($"{viaVerdeMovement.ResultCode}");
                                if (viaVerdeMovement.Status == Data.Enums.ViaVerdeMovementStatus.Registered || viaVerdeMovement.Status == Data.Enums.ViaVerdeMovementStatus.Validated)
                                {
                                    merfFileStringBuilder.AppendLine();
                                    continue;
                                }
                                merfFileStringBuilder.Append($"{viaVerdeMovement.NameOrDeliverySlipNumber.PadRight(40, ' ')}");
                                if (viaVerdeMovement.Status == Data.Enums.ViaVerdeMovementStatus.RejectedTow)
                                {
                                    merfFileStringBuilder.AppendLine();
                                    continue;
                                }
                                merfFileStringBuilder.Append($"{viaVerdeMovement.Address.PadRight(60, ' ')}");
                                merfFileStringBuilder.Append($"{viaVerdeMovement.Town.PadRight(30, ' ')}");
                                merfFileStringBuilder.Append($"{viaVerdeMovement.PostalCode.PadRight(8, ' ')}");
                                merfFileStringBuilder.Append($"{viaVerdeMovement.PostalLocation.PadRight(30, ' ')}");
                                merfFileStringBuilder.Append($"{viaVerdeMovement.Country.Iso3166NumberCode.PadRight(3, ' ')}");
                                if (viaVerdeMovement.Country.Alpha2Code == "PT")
                                {
                                    merfFileStringBuilder.Append($"NF{viaVerdeMovement.IdentificationNumber.PadRight(18, ' ')}");
                                }
                                else
                                {
                                    merfFileStringBuilder.Append($"DL{viaVerdeMovement.IdentificationNumber.PadRight(18, ' ')}");
                                }
                                merfFileStringBuilder.Append($"{viaVerdeMovement.CreditCardLast4Digits.PadRight(20, ' ')}");
                                merfFileStringBuilder.Append($"{viaVerdeMovement.CollectionAttemptedDate.Value.ToString("yyyyMMddHHmmss").PadRight(14, ' ')}");
                                merfFileStringBuilder.Append($"{viaVerdeMovement.AcquirerResultCode.PadRight(15, ' ')}");
                                merfFileStringBuilder.Append($"{viaVerdeMovement.AcquirerResultDescription.PadRight(128, ' ')}");

                                merfFileStringBuilder.AppendLine();
                            }

                            merfFileStringBuilder.AppendLine($"F{viaVerdeMovements.Count.ToString().PadLeft(6, '0')}");

                            var merfText = merfFileStringBuilder.ToString();
                            File.WriteAllText(Path.Combine(path, $"{fileName}.txt"), merfText, Encoding.Latin1);

                            using (var sftpProvider = new Shared.Providers.Sftp(viaVerdeConfiguration.SftpAddress, viaVerdeConfiguration.SftpPort, viaVerdeConfiguration.SftpUsername, viaVerdeConfiguration.SftpPassword))
                            {
                                sftpProvider.UploadFile($"{path}/{fileName}.txt", "Merf");
                            }

                            foreach (var viaVerdeMovement in viaVerdeMovements)
                            {
                                viaVerdeMovement.SendToViaVerde = false;
                            }

                            context.SaveChanges();
                        }
                        logger.LogInformation($"Finished Via Verde MERF");
                    }
                }
                logger.LogDebug("Process finished!");
            }
            catch (Exception exception)
            {
                // NLog: catch setup errors
                logger.LogError(exception, "Stopped program because of exception");
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
                logger.LogDebug("App stopped!");
            }
        }
    }
}

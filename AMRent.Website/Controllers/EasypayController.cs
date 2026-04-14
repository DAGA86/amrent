using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using AMRent.Data.Contexts;

namespace AMRent.Website.Controllers
{
    public class EasypayController : BaseController
    {
        private readonly IConfiguration _configuration;

        public EasypayController(ILogger<HomeController> logger, FullDatabaseContext context, dCore.MultiLanguage.Providers.TranslationProvider translationProvider, IConfiguration configuration) : base(logger, context, translationProvider)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public JsonResult GenericNotification([FromBody] Models.EasyPay.GenericNotification genericNotification)
        {

            if (_configuration["Environment"] == "Test")
                _logger.LogInformation(JsonSerializer.Serialize(genericNotification));

            Data.Models.Database.Reservation reservation = _context.Reservations.FirstOrDefault(x => x.ExternalPaymentReference == genericNotification.Id && x.Number == genericNotification.Key);
            if (reservation == null)
            {
                return new JsonResult("Not Found");
            }

            reservation.PaymentStatus = Data.Enums.PaymentStatus.Paid;
            _context.SaveChanges();

            dCore.Communication.Models.SmtpConfiguration smtpConfiguration = _configuration.GetSection("SmtpConfiguration").Get<dCore.Communication.Models.SmtpConfiguration>();
            Shared.Providers.EmailSender emailSender = new Shared.Providers.EmailSender(_context, _translationProvider, smtpConfiguration, reservation.LanguageId);
            emailSender.Send(Data.Enums.EmailContentTypes.PaymentConfirmation, _configuration["Environment"] == "Test", reservation.Id);

            return new JsonResult("OK");
        }

        [HttpGet]
        public IActionResult CreditCardDetails(string t_key, string id)
        {
            if (string.IsNullOrEmpty(t_key) || string.IsNullOrEmpty(id))
            {
                if (_configuration["Environment"] == "Test")
                    _logger.LogError("Error: Not enough params");

                return BadRequest("Error: Not enough params");
            }

            if (_configuration["Environment"] == "Test")
                _logger.LogInformation($"t_key: {t_key}     - id: {id}");

            // Fetch your account id from your authentication mechanism
            string accountId = _configuration["Easypay:AccountId"];

            if (accountId != id)
            {
                return BadRequest("Error: Data mismatch");
            }

            // Connection string for your database
            string connectionString = "Server=localhost;Database=your_database;User Id=my_user;Password=my_password;";

            try
            {
                //using (SqlConnection connection = new SqlConnection(connectionString))
                //{
                //    connection.Open();

                    // Query to fetch payment details
                    string query = $"SELECT * FROM easypay_notifications_table WHERE t_key = '{t_key}'";

                    //using (SqlCommand command = new SqlCommand(query, connection))
                    //{
                    //    using (SqlDataReader reader = command.ExecuteReader())
                    //    {
                    //        if (reader.Read())
                    //        {
                                // Build XML response
                                string xml = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>\n";
                                xml += "<get_detail>\n";
                                xml += "<ep_status>ok</ep_status>\n";
                                xml += "<ep_message>success</ep_message>\n";
                                xml += "<ep_value>" /*+ reader["ep_value"]*/ + "</ep_value>\n";
                                xml += "<t_key>" /*+ reader["t_key"]*/ + "</t_key>\n";
                                xml += "<order_info>\n";
                                xml += "<total_taxes>" /*+ reader["taxes"]*/ + "</total_taxes>\n";
                                xml += "<total_including_taxes>" /*+ reader["total"]*/ + "</total_including_taxes>\n";
                                xml += "<bill_name>" /*+ reader["billing_first_name"]*/ + " " /*+ reader["billing_last_name"]*/ + "</bill_name>\n";
                                xml += "<shipp_name>" /*+ reader["shipping_first_name"]*/ + " " /*+ reader["shipping_last_name"]*/ + "</shipp_name>\n";
                                xml += "<bill_address_1>" /*+ reader["billing_address_1"]*/ + "</bill_address_1>\n";
                                xml += "<shipp_adress_1>" /*+ reader["shipping_address_1"]*/ + "</shipp_adress_1>\n";
                                xml += "<bill_address_2>" /*+ reader["billing_address_2"]*/ + "</bill_address_2>\n";
                                xml += "<shipp_adress_2>" /*+ reader["shipping_address_2"]*/ + "</shipp_adress_2>\n";
                                xml += "<bill_city>" /*+ reader["billing_city"]*/ + "</bill_city>\n";
                                xml += "<shipp_city>" /*+ reader["shipping_city"]*/ + "</shipp_city>\n";
                                xml += "<bill_zip_code>" /*+ reader["billing_postcode"]*/ + "</bill_zip_code>\n";
                                xml += "<shipp_zip_code>" /*+ reader["shipping_postcode"]*/ + "</shipp_zip_code>\n";
                                xml += "<bill_country>" /*+ reader["billing_country"]*/ + "</bill_country>\n";
                                xml += "<shipp_country>" /*+ reader["shipping_country"]*/ + "</shipp_country>\n";
                                xml += "</order_info>\n";
                                xml += "<order_detail>\n";

                                // You may need to adjust this part based on your actual implementation to get order items
                                xml += "<item>\n";
                                xml += "<item_description>" + "Sample Item" + "</item_description>\n";
                                xml += "<item_quantity>" + "1" + "</item_quantity>\n";
                                xml += "<item_total>" + "10.00" + "</item_total>\n";
                                xml += "</item>\n";

                                xml += "</order_detail>\n";
                                xml += "</get_detail>\n";

                                return Content(xml, "application/xml");
                            //}
                            //else
                            //{
                            //    return NotFound("Error: No payment found");
                            //}
                    //    }
                    //}
                //}
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        [HttpGet]
        public IActionResult CreditCardForward(string e, string r, string v, string? k, string s, string t_key)
        {
            if (string.IsNullOrEmpty(e) || string.IsNullOrEmpty(r) || string.IsNullOrEmpty(v) || string.IsNullOrEmpty(s) || string.IsNullOrEmpty(t_key))
            {
                if (_configuration["Environment"] == "Test")
                    _logger.LogError("Error: Not enough params");

                return BadRequest("Error: Not enough params");
            }

            if (_configuration["Environment"] == "Test")
                _logger.LogInformation($"e: {e}     - r: {r}     - v: {v}     - s: {s}     - t_key: {t_key}     - k: {k ?? ""}");

            try
            {
                Data.Models.Database.Reservation reservation =
                    _context.Reservations.FirstOrDefault(x => x.Number == t_key);
                if (reservation != null)
                {
                    dCore.Communication.Models.SmtpConfiguration smtpConfiguration = _configuration.GetSection("SmtpConfiguration").Get<dCore.Communication.Models.SmtpConfiguration>();
                    Shared.Providers.EmailSender emailSender = new Shared.Providers.EmailSender(_context, _translationProvider, smtpConfiguration, reservation.LanguageId);
                    if (s != "ok")
                    {
                        reservation.PaymentStatus = Data.Enums.PaymentStatus.Failed;
                        _context.SaveChanges();

                        emailSender.Send(Data.Enums.EmailContentTypes.PaymentFailure, _configuration["Environment"] == "Test", reservation.Id);

                        return BadRequest("Payment failed.");
                    }

                    reservation.PaymentStatus = Data.Enums.PaymentStatus.Paid;
                    _context.SaveChanges();

                    emailSender.Send(Data.Enums.EmailContentTypes.PaymentConfirmation, _configuration["Environment"] == "Test", reservation.Id);

                    return RedirectToAction("CreditCardConfirm", "Booking", new {reservationNumber = t_key});
                }
                return BadRequest($"Error: No reservation found with Number {t_key}");
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }
    }
}
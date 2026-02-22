using LeMarconnes.DTOs;
using LeMarconnes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using System.Text.Json;

namespace LeMarconnes.Controllers
{
    public class ReservationsController : BaseUserController
    {
        // GET: Reservations/Create/{id}
        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Accounts");

            using var client = GetAuthorizedClient();

            // 1. Haal Accommodatie op
            var accResponse = await client.GetAsync($"{_apiBaseUrl}/Accommodations/{id}");
            if (!accResponse.IsSuccessStatusCode) return NotFound();
            var accDto = await accResponse.Content.ReadFromJsonAsync<AccommodationDto>();

            // 2. Haal bestaande reserveringen op via het enkelvoudige endpoint
            var resResponse = await client.GetAsync($"{_apiBaseUrl}/Reservation");
            List<string> blockedDates = new List<string>();

            if (resResponse.IsSuccessStatusCode)
            {
                var jsonString = await resResponse.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonString);

                foreach (var element in doc.RootElement.EnumerateArray())
                {
                    // VEILIGE ACCOMMODATION ID PARSING (Checkt op Number of Object)
                    int resAccId = 0;
                    if (element.TryGetProperty("accommodationId", out var accProp))
                    {
                        if (accProp.ValueKind == JsonValueKind.Object)
                            resAccId = accProp.GetProperty("id").GetInt32();
                        else if (accProp.ValueKind == JsonValueKind.Number)
                            resAccId = accProp.GetInt32();
                    }

                    // VEILIGE STATUS PARSING (Voorkomt 'StartObject' error)
                    int statusValue = 0;
                    if (element.TryGetProperty("status", out var statusProp))
                    {
                        if (statusProp.ValueKind == JsonValueKind.Object)
                            statusValue = statusProp.TryGetProperty("id", out var sId) ? sId.GetInt32() : 0;
                        else if (statusProp.ValueKind == JsonValueKind.Number)
                            statusValue = statusProp.GetInt32();
                    }

                    // Alleen data blokkeren voor DEZE kamer/gîte als status niet 'Geannuleerd' (2) is
                    if (resAccId == id && statusValue != 2)
                    {
                        if (element.TryGetProperty("checkInDate", out var checkInProp) &&
                            element.TryGetProperty("checkOutDate", out var checkOutProp))
                        {
                            DateTime checkIn = checkInProp.GetDateTime();
                            DateTime checkOut = checkOutProp.GetDateTime();

                            var totalDays = (checkOut - checkIn).Days;
                            for (int i = 0; i < totalDays; i++)
                            {
                                blockedDates.Add(checkIn.AddDays(i).ToString("yyyy-MM-dd"));
                            }
                        }
                    }
                }
            }

            var viewModel = new ReservationCreateViewModel
            {
                AccommodationId = id,
                AccommodationName = accDto?.Name ?? "Accommodatie",
                RatePerNight = accDto?.RatePerNight ?? 0,
                MaxCapacity = accDto?.Capacity ?? 0,
                IsGite = accDto?.IsGite ?? false,
                IsHotelRoom = accDto?.IsHotelRoom ?? false,
                BlockedDatesJson = JsonSerializer.Serialize(blockedDates.Distinct().ToList()),
                TouristTax = 2.50m
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection form)
        {
            var culture = System.Globalization.CultureInfo.InvariantCulture;
            var paymentPlan = form["paymentPlan"].ToString();

            // Veiligheid: Gebruik een fallback "0" als velden leeg zijn om FormatException te voorkomen
            var checkInRaw = form["CheckInDate"].ToString();
            var checkOutRaw = form["CheckOutDate"].ToString();
            var guestsRaw = string.IsNullOrEmpty(form["NumberOfGuests"]) ? "1" : form["NumberOfGuests"].ToString();
            var taxRaw = string.IsNullOrEmpty(form["TouristTax"]) ? "0" : form["TouristTax"].ToString().Replace(",", ".");
            var discRaw = string.IsNullOrEmpty(form["Discount"]) ? "0" : form["Discount"].ToString();

            // Parsing
            var checkIn = DateTime.Parse(checkInRaw);
            var checkOut = DateTime.Parse(checkOutRaw);
            var nights = (checkOut - checkIn).Days;
            var guests = int.Parse(guestsRaw);
            var rate = 125.00m;
            var taxRate = decimal.Parse(taxRaw, culture);

            var totalAmount = (nights * rate) + (nights * taxRate);
            var payments = new List<object>();

            if (paymentPlan == "split")
            {
                var half = totalAmount / 2;
                payments.Add(new { amount = half, paymentDate = DateTime.Now });
                payments.Add(new { amount = half, paymentDate = checkIn });
            }
            else
            {
                payments.Add(new { amount = totalAmount, paymentDate = DateTime.Now });
            }

            var reservationDto = new
            {
                userId = HttpContext.Session.GetString("UserId"),
                accommodationId = int.Parse(form["AccommodationId"]!),
                checkInDate = checkInRaw,
                checkOutDate = checkOutRaw,
                numberOfGuests = guests,
                touristTax = taxRate,
                discount = decimal.Parse(discRaw, culture),
                status = 1,
                user = (object?)null,
                accommodation = (object?)null,
                payments = payments
            };

            using var client = new HttpClient();
            var token = HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsJsonAsync(
                "https://lemarconnes-api-d7hgf2emb3cebbb3.westeurope-01.azurewebsites.net/api/Reservation",
                reservationDto);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.PaymentPlan = paymentPlan;

                // Gebruik het volledige pad naar de view
                return View("~/Views/Reservations/Thanks.cshtml", paymentPlan);
            }

            var error = await response.Content.ReadAsStringAsync();
            return Content($"Fout bij opslaan: {error}");
        }

        [HttpGet]
        public IActionResult Thanks(string plan)
        {
            ViewBag.PaymentPlan = plan;
            return View();
        }
    }
}
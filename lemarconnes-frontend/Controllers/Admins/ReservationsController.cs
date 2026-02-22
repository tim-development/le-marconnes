using LeMarconnes.DTOs;
using LeMarconnes.DTOs.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;

namespace LeMarconnes.Controllers.Admin
{
    public class ReservationsController : BaseAdminController
    {
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        [HttpGet, Route("Admins/Reservations")]
        public async Task<IActionResult> Index(string? filter)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            var client = GetAuthorizedClient();
            var list = await client.GetFromJsonAsync<List<AdminReservationDto>>($"{_apiBaseUrl}/Reservation", _options) ?? new List<AdminReservationDto>();

            if (!string.IsNullOrEmpty(filter))
            {
                list = filter.ToLower() switch
                {
                    "pending" => list.Where(x => x.Status?.Id == 1).ToList(),
                    "confirmed" => list.Where(x => x.Status?.Id == 3).ToList(),
                    "cancelled" => list.Where(x => x.Status?.Id == 2).ToList(),
                    _ => list
                };
            }

            ViewBag.CurrentFilter = filter;
            return View("~/Views/Admins/Reservations/Index.cshtml", list.OrderByDescending(x => x.Id).ToList());
        }

        [HttpGet, Route("Admins/Reservations/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var item = await GetFullReservationDetails(id);
            if (item == null) return NotFound();

            return View("~/Views/Admins/Reservations/Details.cshtml", item);
        }

        [HttpGet, Route("Admins/Reservations/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await GetFullReservationDetails(id);
            if (item == null) return NotFound();

            return View("~/Views/Admins/Reservations/Delete.cshtml", item);
        }

        [HttpPost, Route("Admins/Reservations/Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            var client = GetAuthorizedClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/Reservation/{id}");

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Reservering succesvol verwijderd.";
            else
                TempData["Error"] = "Verwijderen mislukt.";

            return RedirectToAction(nameof(Index));
        }

        private async Task<AdminReservationDto?> GetFullReservationDetails(int id)
        {
            if (!IsAdmin()) return null;

            var client = GetAuthorizedClient();

            try
            {
                // STAP 1: Haal de basis reservering op (Hier mist de prijs nog)
                var res = await client.GetFromJsonAsync<AdminReservationDto>($"{_apiBaseUrl}/Reservation/{id}", _options);
                if (res == null || res.Accommodation == null) return res;

                int accId = res.Accommodation.Id;

                // STAP 2: Controleer het type via de algemene Accommodation endpoint
                var accResponse = await client.GetAsync($"{_apiBaseUrl}/Accommodation/{accId}");
                if (accResponse.IsSuccessStatusCode)
                {
                    var accBase = await accResponse.Content.ReadFromJsonAsync<AccommodationDto>(_options);
                    if (accBase != null)
                    {
                        // STAP 3: Haal de RatePerNight op bij de specifieke bron (Gite of HotelRoom)
                        string subEndpoint = accBase.IsGite ? "Gite" : "HotelRoom";
                        var detailResponse = await client.GetAsync($"{_apiBaseUrl}/{subEndpoint}/{accId}");

                        if (detailResponse.IsSuccessStatusCode)
                        {
                            var detail = await detailResponse.Content.ReadFromJsonAsync<AdminAccommodationSummaryDto>(_options);
                            if (detail != null)
                            {
                                // VUL HET GAT: Voeg de prijs toe aan het model
                                res.Accommodation.RatePerNight = detail.RatePerNight;
                            }
                        }
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Fout in LeMarconnes: {ex.Message}");
                return null;
            }
        }
    }
}
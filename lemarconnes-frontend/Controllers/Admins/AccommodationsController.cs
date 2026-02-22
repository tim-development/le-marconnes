using LeMarconnes.DTOs;
using LeMarconnes.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using BedDto = LeMarconnes.DTOs.BedDto;

namespace LeMarconnes.Controllers.Admin
{
    public class AccommodationsController : BaseAdminController
    {
        [HttpGet, Route("Admins/Accommodations")]
        public async Task<IActionResult> Index(string? filter)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            using var client = GetAuthorizedClient();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var list = await client.GetFromJsonAsync<List<AccommodationDto>>($"{_apiBaseUrl}/Accommodations", options)
                        ?? new List<AccommodationDto>();

            if (filter == "gite")
                list = list.Where(x => x.IsGite).ToList();
            else if (filter == "hotel")
                list = list.Where(x => x.IsHotelRoom).ToList();

            ViewBag.CurrentFilter = filter;
            return View("~/Views/Admins/Accommodations/Index.cshtml", list);
        }

        [Route("Admins/Accommodations/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            using var client = GetAuthorizedClient();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var baseItem = await client.GetFromJsonAsync<AccommodationDto>($"{_apiBaseUrl}/Accommodations/{id}", options);
            if (baseItem == null) return NotFound();

            string url = baseItem.IsGite ? $"{_apiBaseUrl}/Gites/{id}" : $"{_apiBaseUrl}/HotelRooms/{id}";
            var viewModel = await client.GetFromJsonAsync<AccommodationDetailViewModel>(url, options);

            return View("~/Views/Admins/Accommodations/Details.cshtml", viewModel);
        }

        [HttpGet, Route("Admins/Accommodations/Create")]
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            using var client = GetAuthorizedClient();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var bedDtos = await client.GetFromJsonAsync<List<BedDto>>($"{_apiBaseUrl}/Beds", options)
                          ?? new List<BedDto>();

            var viewModel = new AccommodationCreateViewModel
            {
                SelectedType = "hotel",
                BedTypes = bedDtos.Select(b => new BedSelectionItem
                {
                    BedId = b.Id,
                    BedName = b.Type,
                    Quantity = 0
                }).ToList()
            };

            return View("~/Views/Admins/Accommodations/Create.cshtml", viewModel);
        }

        [HttpPost, Route("Admins/Accommodations/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AccommodationCreateViewModel model)
        {
            // --- HET TEAM EASTER EGG ---
            if (model.Name == "We proudly present for your viewing pleasure!" &&
                model.Description == "Het team: Les Codeurs de Marconnes!")
            {
                return View("~/Views/Suprise/EasterEgg.cshtml");
            }

            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            if (!ModelState.IsValid)
            {
                return View("~/Views/Admins/Accommodations/Create.cshtml", model);
            }

            using var client = GetAuthorizedClient();

            var selectedBeds = model.BedTypes
                .Where(b => b.Quantity > 0)
                .Select(b => new {
                    bedId = b.BedId,
                    quantity = b.Quantity
                }).ToList();

            string endpoint = model.SelectedType == "gite" ? "/Accommodations/gite" : "/Accommodations/hotelroom";
            object payload = model.SelectedType == "gite" ? (object)new
            {
                name = model.Name,
                description = model.Description,
                capacity = model.Capacity,
                ratePerNight = model.RatePerNight,
                isHotelRoom = false,
                isGite = true,
                accommodationType = "Gite",
                entireProperty = model.EntireProperty,
                garden = model.Garden,
                parkingAvailable = model.ParkingAvailable,
                accommodationBeds = selectedBeds
            } : new
            {
                name = model.Name,
                description = model.Description,
                capacity = model.Capacity,
                ratePerNight = model.RatePerNight,
                isHotelRoom = true,
                isGite = false,
                accommodationType = "HotelRoom",
                roomNumber = model.RoomNumber ?? 0,
                privateBathroom = model.PrivateBathroom,
                accommodationBeds = selectedBeds
            };

            var response = await client.PostAsJsonAsync($"{_apiBaseUrl}{endpoint}", payload);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Accommodatie succesvol toegevoegd aan LeMarconnes!";
                return RedirectToAction(nameof(Index));
            }

            var errorMsg = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"API Fout: {errorMsg}");
            return View("~/Views/Admins/Accommodations/Create.cshtml", model);
        }

        [HttpGet, Route("Admins/Accommodations/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            using var client = GetAuthorizedClient();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var baseItem = await client.GetFromJsonAsync<AccommodationDto>($"{_apiBaseUrl}/Accommodations/{id}", options);
            if (baseItem == null) return NotFound();

            string url = baseItem.IsGite ? $"{_apiBaseUrl}/Gites/{id}" : $"{_apiBaseUrl}/HotelRooms/{id}";
            var details = await client.GetFromJsonAsync<AccommodationDetailViewModel>(url, options);
            if (details == null) return NotFound();

            var allBeds = await client.GetFromJsonAsync<List<BedDto>>($"{_apiBaseUrl}/Beds", options) ?? new List<BedDto>();
            ViewBag.AllBeds = allBeds;

            var model = new AccommodationEditViewModel
            {
                Id = details.Id,
                Name = details.Name,
                Description = details.Description,
                Capacity = details.Capacity,
                RatePerNight = details.RatePerNight,
                SelectedType = details.IsGite ? "gite" : "hotel",
                RoomNumber = details.RoomNumber,
                PrivateBathroom = details.PrivateBathroom,
                EntireProperty = details.EntireProperty,
                Garden = details.Garden,
                ParkingAvailable = details.ParkingAvailable,
                BedTypes = allBeds.Select(b => new BedSelectionItem
                {
                    BedId = b.Id,
                    BedName = b.Type,
                    Quantity = details.AccommodationBeds?.FirstOrDefault(ab => ab.BedId == b.Id)?.Quantity ?? 0
                }).ToList()
            };

            return View("~/Views/Admins/Accommodations/Edit.cshtml", model);
        }

        [HttpPost, Route("Admins/Accommodations/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AccommodationEditViewModel model)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");
            using var client = GetAuthorizedClient();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            string endpoint = model.SelectedType == "gite" ? $"/Gites/{id}" : $"/HotelRooms/{id}";
            object accPayload = model.SelectedType == "gite" ? (object)new
            {
                id = id,
                name = model.Name,
                description = model.Description,
                capacity = model.Capacity,
                ratePerNight = model.RatePerNight,
                entireProperty = model.EntireProperty,
                garden = model.Garden,
                parkingAvailable = model.ParkingAvailable
            } : new
            {
                id = id,
                name = model.Name,
                description = model.Description,
                capacity = model.Capacity,
                ratePerNight = model.RatePerNight,
                roomNumber = model.RoomNumber ?? 0,
                privateBathroom = model.PrivateBathroom
            };

            var response = await client.PutAsJsonAsync($"{_apiBaseUrl}{endpoint}", accPayload);

            if (response.IsSuccessStatusCode)
            {
                var allAccommodationBeds = await client.GetFromJsonAsync<List<AccommodationBedDto>>($"{_apiBaseUrl}/AccommodationBeds", options)
                                           ?? new List<AccommodationBedDto>();

                var currentAccBeds = allAccommodationBeds.Where(b => b.AccommodationId == id).ToList();

                foreach (var bedItem in model.BedTypes)
                {
                    var existingRecord = currentAccBeds.FirstOrDefault(b => b.BedId == bedItem.BedId);
                    var bedPayload = new { id = existingRecord?.Id ?? 0, accommodationId = id, bedId = bedItem.BedId, quantity = bedItem.Quantity };

                    if (existingRecord != null)
                        await client.PutAsJsonAsync($"{_apiBaseUrl}/AccommodationBeds/{existingRecord.Id}", bedPayload);
                    else if (bedItem.Quantity > 0)
                        await client.PostAsJsonAsync($"{_apiBaseUrl}/AccommodationBeds", bedPayload);
                }

                TempData["SuccessMessage"] = $"De wijzigingen voor '{model.Name}' zijn succesvol opgeslagen.";
                return RedirectToAction(nameof(Index));
            }

            var errorBody = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"API Fout: {errorBody}");
            ViewBag.AllBeds = await client.GetFromJsonAsync<List<BedDto>>($"{_apiBaseUrl}/Beds", options) ?? new List<BedDto>();
            return View("~/Views/Admins/Accommodations/Edit.cshtml", model);
        }

        [HttpGet, Route("Admins/Accommodations/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            using var client = GetAuthorizedClient();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var item = await client.GetFromJsonAsync<AccommodationDto>($"{_apiBaseUrl}/Accommodations/{id}", options);

            return View("~/Views/Admins/Accommodations/Delete.cshtml", item);
        }

        [HttpPost, Route("Admins/Accommodations/Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            using var client = GetAuthorizedClient();

            await client.DeleteAsync($"{_apiBaseUrl}/AccommodationBeds/ByAccommodation/{id}");
            var response = await client.DeleteAsync($"{_apiBaseUrl}/Accommodations/{id}");

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Accommodatie succesvol verwijderd.";
            else
                TempData["Error"] = "Verwijderen mislukt. Controleer op actieve reserveringen.";

            return RedirectToAction("Index");
        }
    }
}
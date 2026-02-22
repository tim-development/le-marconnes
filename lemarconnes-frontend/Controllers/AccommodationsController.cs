using LeMarconnes.DTOs;
using LeMarconnes.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;


namespace LeMarconnes.Controllers
{
    public class AccommodationsController: BaseUserController
    {
        [HttpGet]
        public async Task<IActionResult> Index(string? filter)
        {

            using var client = new HttpClient();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var list = await client.GetFromJsonAsync<List<AccommodationDto>>($"{_apiBaseUrl}/Accommodations", options)
                       ?? new List<AccommodationDto>();

            if (filter == "gite")
                list = list.Where(x => x.IsGite).ToList();
            else if (filter == "hotel")
                list = list.Where(x => x.IsHotelRoom).ToList();

            ViewBag.CurrentFilter = filter;
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            using var client = new HttpClient();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // 1. Haal basis info op
            var baseItem = await client.GetFromJsonAsync<AccommodationDto>($"{_apiBaseUrl}/Accommodations/{id}", options);
            if (baseItem == null) return NotFound();

            // 2. Haal de volledige details op met het juiste type
            string url = baseItem.IsGite ? $"{_apiBaseUrl}/Gites/{id}" : $"{_apiBaseUrl}/HotelRooms/{id}";
            var viewModel = await client.GetFromJsonAsync<AccommodationDetailViewModel>(url, options);

            return View(viewModel);
        }
    }
}

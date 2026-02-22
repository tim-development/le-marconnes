using LeMarconnes.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace LeMarconnes.Controllers
{
    public class AccountsController : BaseAdminController
    {
        // GET: /Accounts/Login
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            using var client = new HttpClient();
            try
            {
                var response = await client.PostAsJsonAsync($"{_apiBaseUrl}/Auth/login", new { email, password });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    if (result != null)
                    {
                        HttpContext.Session.SetString("Token", result.Token);
                        HttpContext.Session.SetString("IsLoggedIn", "true");
                        HttpContext.Session.SetString("IsAdmin", result.User.IsAdmin.ToString().ToLower());
                        HttpContext.Session.SetString("UserName", result.User.Name);
                        HttpContext.Session.SetString("UserId", result.User.Id);

                        if (result.User.IsAdmin)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["DebugError"] = errorContent;
                TempData["DebugStatus"] = (int)response.StatusCode;
                ViewBag.Error = "Inloggen mislukt. Controleer uw gegevens.";
            }
            catch (Exception ex)
            {
                TempData["DebugError"] = ex.Message;
                ViewBag.Error = "Fout bij verbinden met de API.";
            }
            return View();
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string name, string email, string password, string phoneNumber, string address)
        {
            using var client = new HttpClient();
            var newUser = new
            {
                id = Guid.NewGuid().ToString(),
                name,
                email,
                phoneNumber = phoneNumber ?? "",
                address = address ?? "",
                isAdmin = false,
                password
            };

            try
            {
                var response = await client.PostAsJsonAsync($"{_apiBaseUrl}/Auth/register", newUser);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Account succesvol aangemaakt! U kunt nu inloggen.";
                    return RedirectToAction("Login");
                }

                TempData["DebugError"] = await response.Content.ReadAsStringAsync();
                ViewBag.Error = "Registratie mislukt.";
            }
            catch (Exception ex) { TempData["DebugError"] = ex.Message; }
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // --- ADMIN DASHBOARD ---

        // GET: /Accounts/Index
        [Route("Admins/Index")]
        [Route("Accounts/Index")]
        public IActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            return View("~/Views/Admins/Index.cshtml");
        }

        // --- GEBRUIKERS BEHEER (ADMINS) ---

        [HttpGet, Route("Admins/Users/Create")]
        public IActionResult CreateUser()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View("~/Views/Admins/Users/Create.cshtml");
        }

        [HttpPost, Route("Admins/Users/Create")]
        public async Task<IActionResult> CreateUser(string name, string email, string password, string phoneNumber, string address, bool isAdmin)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            using var client = GetAuthorizedClient();
            var newUser = new
            {
                id = Guid.NewGuid().ToString(),
                name,
                email,
                phoneNumber = phoneNumber ?? "",
                address = address ?? "",
                isAdmin,
                password
            };

            var response = await client.PostAsJsonAsync($"{_apiBaseUrl}/Auth/register", newUser);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Nieuwe gebruiker succesvol toegevoegd.";
                return RedirectToAction("Index");
            }

            TempData["DebugError"] = await response.Content.ReadAsStringAsync();
            ViewBag.Error = "Fout bij aanmaken van de gebruiker.";
            return View("~/Views/Admins/Users/Create.cshtml");
        }
    }
}
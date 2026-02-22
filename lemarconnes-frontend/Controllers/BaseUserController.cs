using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace LeMarconnes.Controllers
{
    public class BaseUserController : Controller
    {
        //protected readonly string _apiBaseUrl = "https://lemarconnes-api.duckdns.org/api";
        protected readonly string _apiBaseUrl = "https://lemarconnes-api-d7hgf2emb3cebbb3.westeurope-01.azurewebsites.net/api";


        protected HttpClient GetAuthorizedClient()
        {
            var client = new HttpClient();
            var token = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }

        protected bool IsAdmin()
        {
            return HttpContext.Session.GetString("IsAdmin") == "true";
        }
    }
}
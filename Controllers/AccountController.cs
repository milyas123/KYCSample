using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using KYC_AzaKaw_WebApp.Entities;
using KYC_AzaKaw_WebApp.Extentions;
using KYC_AzaKaw_WebApp.Helpers;
using KYC_AzaKaw_WebApp.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KYC_AzaKaw_WebApp.Controllers
{
    public class AccountController : Controller
    {
        const string currentUserInfo = "_currentUserInfo";
        const string authToken = "_authToken";

        CustomerAPI _custAPI = new CustomerAPI();

        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]

        public IActionResult Register() 
        {
           
            return View();
        }

        [HttpPost]
        public IActionResult Register(Customer obj)
        {
                HttpClient client = _custAPI.Initialize();
                string stringData = JsonConvert.SerializeObject(obj);
                var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync("api/Customer", contentData).Result;

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Message = "Success"; 
                }
                else { 
                    ViewBag.Message = "Error";
                }
            return View();
        }

        [HttpPost]
        public IActionResult Login(Auth obj)
        {
            HttpClient client = _custAPI.Initialize();
            string stringData = JsonConvert.SerializeObject(obj);
            var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync("api/UserAuth", contentData).Result;

            if (response.IsSuccessStatusCode)
            {
                ViewBag.Message = "Success";
                var userObject = JsonConvert.DeserializeObject<User>(response.Content.ReadAsStringAsync().Result);

                if (userObject != null && userObject.Token != null)
                {
                    HttpContext.Session.SetString(authToken, userObject.Token.ToString());
                    HttpContext.Session.SetString(currentUserInfo, userObject.Id.ToString());
                    //Save token in session object
                }
                //Create the identity for the user  
                var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, userObject.Id.ToString())
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            }
            else {
                ViewBag.Message = "Error";
                return View();
            }
            return RedirectToAction("GetUpload", "Upload");
        }

        [NonAction]
        private static HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                var ms = new MemoryStream();
                SerializeJsonIntoStream(content, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            return httpContent;
        }

        [NonAction]
        public static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            using (var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None })
            {
                var js = new JsonSerializer();
                js.Serialize(jtw, value);
                jtw.Flush();
            }
        }
    }
}
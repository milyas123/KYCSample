using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using KYC_AzaKaw_WebApp.Entities;
using KYC_AzaKaw_WebApp.Extentions;
using KYC_AzaKaw_WebApp.Helpers;
using KYC_AzaKaw_WebApp.Model;
using KYC_AzaKaw_WebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KYC_AzaKaw_WebApp.Controllers
{
    [Authorize]
    public class UploadController : Controller
    {
        CustomerAPI _custAPI = new CustomerAPI();
        const string currentUserInfo = "_currentUserInfo";
        const string authToken = "_authToken";

        [HttpGet] 
        public IActionResult GetUpload()
        {
            HttpClient client = _custAPI.Initialize();
            var authorToken = HttpContext.Session.GetString(authToken);
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + authorToken);
            var result = client.GetAsync("api/UploadFile").Result;
            var mrzInfos = JsonConvert.DeserializeObject<List<MrzInfo>>(result.Content.ReadAsStringAsync().Result); 
            ViewBag.DataSource = mrzInfos; 
            return View("UploadFile");
        }

        [HttpPost("FileUpload")]
        public IActionResult UploadFile(List<IFormFile> files) 
        { 
            HttpClient client = _custAPI.Initialize();

            var file = files[0];
            if (file != null && file.Length > 0)
            {

                byte[] data;
                using (var br = new BinaryReader(file.OpenReadStream()))
                    data = br.ReadBytes((int)file.OpenReadStream().Length);

                var authorToken = HttpContext.Session.GetString(authToken);
                var currUserId = HttpContext.Session.GetString(currentUserInfo);

                ByteArrayContent bytes = new ByteArrayContent(data);
                MultipartFormDataContent multiContent = new MultipartFormDataContent();
                multiContent.Add(bytes, "file", file.FileName);

                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + authorToken);
                var result = client.PostAsync("api/UploadFile", multiContent).Result;
                //JsonConvert.DeserializeObject<MrzInfo>(result.Content.ReadAsStringAsync().Result).Substring(1, result.Content.ReadAsStringAsync().Result).Length - 2);
                var mrzInfos = JsonConvert.DeserializeObject<List<MrzInfo>>(result.Content.ReadAsStringAsync().Result);

                //  var mrzInfos = JsonConvert.DeserializeObject<MrzInfo>(result.Content.ReadAsStringAsync().Result);
                ViewBag.DataSource = mrzInfos;

                if (result.IsSuccessStatusCode)
                {
                    ViewBag.Message = "Success";
                }
                else {
                    ViewBag.Message = "Error";
                }

                return View("UploadFile");
            } 
            return Ok();
        }


        [HttpGet]
        public IActionResult Logout()
        {

            HttpClient client = _custAPI.Initialize();
            var authorToken = HttpContext.Session.GetString(authToken);
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + authorToken);
            var result = client.GetAsync("api/UserAuth").Result; 

            HttpContext.Session.Remove("token"); 
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
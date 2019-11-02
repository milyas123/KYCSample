using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KYC_AzaKaw_Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace KYC_AzaKaw_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiStatusController : Controller
    {
         

        [HttpGet]
        public IActionResult Get()
        { 
            return   Ok("****************KYC API is Up****************");
        }
    }
}
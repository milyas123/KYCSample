using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KYC_AzaKaw_Core.Model;
using KYC_AzaKaw_Core.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KYC_AzaKaw_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public UserAuthController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Post([FromBody]Auth auth)
        {

            var user = _customerRepository.ValidateCustomer(auth.Email, auth.Password);

            if (user == null)
                return BadRequest(new { message = "Invalid Credentials" });

            return Ok(user);
        }

        [Authorize]
        [HttpGet]
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync();  
            return Ok();
        }
    }
}
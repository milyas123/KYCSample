using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KYC_AzaKaw_Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KYC_AzaKaw_Core.Model;
using System.Transactions;

namespace KYC_AzaKaw_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var products = _customerRepository.GetCustomers();
            return new OkObjectResult(products);
        }

        // POST api/customer  
        [HttpPost]
        public IActionResult Post([FromBody]Customer customer)
        {
            using (var scope = new TransactionScope())
            {
                var custResult = _customerRepository.GetCustomers().FirstOrDefault(a => a.Email.ToUpper().Equals(customer.Email.ToUpper()));
             
                if(custResult == null)
                {
                    _customerRepository.InsertCustomer(customer);
                    scope.Complete();
                    return CreatedAtAction(nameof(Get), new { id = customer.CustomerId }, customer);
                }
                else {
                    return BadRequest(); 
                }
            }
        } 


        // GET: Customer
        public ActionResult Index()
        {
            return View();
        } 
    }
}
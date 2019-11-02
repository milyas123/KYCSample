using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using KYC_AzaKaw_Core.DbContexts;
using KYC_AzaKaw_Core.Entities;
using KYC_AzaKaw_Core.Helpers;
using KYC_AzaKaw_Core.Model;  
using Microsoft.IdentityModel.Tokens;

namespace KYC_AzaKaw_Core.Repositories
{
    public class CustomerRepository :ICustomerRepository
    {
        private readonly CustomerContext _dbContext;
         

        public CustomerRepository(CustomerContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Customer> GetCustomers()
        {
            return _dbContext.Customers.ToList();
        }


        public void InsertCustomer(Customer customer)
        {
            customer.CreatedDate = DateTime.Now;
            _dbContext.Add(customer);
            Save();
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public User ValidateCustomer(string email, string password)
        {
            var customer = _dbContext.Customers.SingleOrDefault(x => x.Email == email && x.Password == password);
           
            // return null if user not found
            if (customer == null)
                return null;

            User user = new User()
            {
                Id = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email
            };

            // authentication successful so generate jwt token
 
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] { 
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            };
            var tokeOptions = new JwtSecurityToken(
                issuer: "http://localhost:54016",
                audience: "http://localhost:54016",
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

            user.Token = tokenString;

            return user;
        }
    }


}

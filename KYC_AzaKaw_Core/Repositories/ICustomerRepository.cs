using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KYC_AzaKaw_Core.Entities;
using KYC_AzaKaw_Core.Model; 
namespace KYC_AzaKaw_Core.Repositories
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetCustomers();
        void InsertCustomer(Customer product);

        User ValidateCustomer(string email,string password);
        void Save();
    }
}

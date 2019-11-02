using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KYC_AzaKaw_Core.Model;
using Microsoft.EntityFrameworkCore;

namespace KYC_AzaKaw_Core.DbContexts
{ 
    public class CustomerContext  : DbContext
    {
        public CustomerContext(DbContextOptions<CustomerContext> options) : base(options)
        {
        }
        public DbSet<MrzInfo> MrzInfos { get; set; }
        public DbSet<Customer> Customers { get; set; } 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
     }
   
}

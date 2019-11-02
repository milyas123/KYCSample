using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace KYC_AzaKaw_WebApp.Helpers
{
    public class CustomerAPI
    {
        public HttpClient Initialize()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:54016");
            return client;
        }
    }
}

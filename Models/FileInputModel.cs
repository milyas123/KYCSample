using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KYC_AzaKaw_WebApp.Models
{
    public class FileInputModel
    {
        public IFormFile File { get; set; }
        public string Param { get; set; }

    }
}

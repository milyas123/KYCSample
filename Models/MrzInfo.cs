using KYC_AzaKaw_WebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KYC_AzaKaw_WebApp.Models
{
    public class MrzInfo
    {
         
        public int MrzId { get; set; }
        //Foreign key for Standard
         
        public int CustomerId { get; set; } 
        public virtual Customer Customer { get; set; }
        public string MrzType { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Checksum { get; set; }
        public string ChecksumVerified { get; set; }
        public string DocumentType { get; set; }
        public string DocumentSubtype { get; set; }
        public string IssuingCountry { get; set; }
        public string DocumentNumber { get; set; }
        public string DocumentNumberVerified { get; set; }
        public string DocumentNumberCheck { get; set; }
        public string Nationality { get; set; }
        public string BirthDate { get; set; }
        public string BirthDateVerified { get; set; }
        public string BirthDateCheck { get; set; }
        public string Sex { get; set; }
        public string ExpiryDate { get; set; }
        public string ExpiryDateVerified { get; set; }
        public string ExpiryDateCheck { get; set; }
        public string FileName { get; set; }
        public string FileNameUnique { get; set; }
        public Boolean isKYCVerified { get; set; }
        public string AdditionalInfo { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KYC_AzaKaw_Core.Entities;
using KYC_AzaKaw_Core.Model;
namespace KYC_AzaKaw_Core.Repositories
{ 
    public interface IUploadRepository
    {
        IEnumerable<MrzInfo> GetMrzInfos();
        IEnumerable<MrzInfo> InsertMrzInfo(MrzInfo mrzInfo);
        void DeleteMrzInfo(int mrzID);
    }
}

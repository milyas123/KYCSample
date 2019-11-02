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
    public class UploadRepository : IUploadRepository
    {
        private readonly CustomerContext _dbContext;
        public UploadRepository(CustomerContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<MrzInfo> GetMrzInfos()
        {
            return _dbContext.MrzInfos.ToList();
        }


        public IEnumerable<MrzInfo> InsertMrzInfo(MrzInfo mrzInfo)
        {
            mrzInfo.CreatedDate = DateTime.Now;
            _dbContext.Add(mrzInfo); 
            Save();
            return _dbContext.MrzInfos.ToList();
        }

        public void DeleteMrzInfo(int id)
        {
            var mrzInfo = _dbContext.MrzInfos.Find(id);
            _dbContext.MrzInfos.Remove(mrzInfo);
            Save();
        }
        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}

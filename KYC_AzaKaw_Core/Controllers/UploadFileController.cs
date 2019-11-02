using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using KYC_AzaKaw_Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Abbyy.CloudSdk.V2.Client;
using KYC_AzaKaw_Core.AbbyOCRUtiilites; 
using System.IO;
using Abbyy.CloudSdk.Demo.Core.EventArgs; 
 
using Microsoft.AspNetCore.Hosting;
using KYC_AzaKaw_Core.Helpers;
using System.Data;
using KYC_AzaKaw_Core.Model;
using Trulioo.Client.V1;
using Trulioo.Client.V1.Model;
using Newtonsoft.Json;
using System.Globalization;

namespace KYC_AzaKaw_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        private readonly IUploadRepository _uploadRepository;
        private readonly ICustomerRepository _icustomerRepository;
        ILoggerManager _logger;
        private IHostingEnvironment _hostingEnvironment;

        public UploadFileController(IUploadRepository uploadRepository, ICustomerRepository icustomerRepository, IHostingEnvironment environment,  ILoggerManager logger)
        {
            _uploadRepository = uploadRepository;
            _hostingEnvironment = environment;
              _logger = logger;
            _icustomerRepository = icustomerRepository;
    }

        [Authorize]
        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> Post(IFormFile file)
        {

            List<string> restURL = new List<string>();
            #region SavingFile
            _logger.LogInfo("Uploading file success - Started");
            var filePath = string.Empty;
            
            var strActualFileName = Path.GetFileNameWithoutExtension(file.FileName);
            var fileExt = Path.GetExtension(file.FileName);
            var appendString = DateTime.Now.ToString("yyyyddmmHHssmmm");
            string strUniquename = strActualFileName + "_" + appendString + "" + fileExt; 
            var uploads = @"C:/AzaKawSampleKYC/KYCUploads";
            if (!Directory.Exists(uploads)) {
                _logger.LogInfo("Creating Upload Directory - Start");
                Directory.CreateDirectory(uploads);
                _logger.LogInfo("Creating Upload Directory - Done");
            }
            if (file.Length > 0)
            {
                filePath = Path.Combine(uploads, strUniquename); 

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);

                }
            }
            strActualFileName = strActualFileName + "" + fileExt; 
            _logger.LogInfo("Uploading file success - Done");
            #endregion

            #region ReadingMRZWithAbbyOCR 
            _logger.LogInfo("Reading MRZ Inforation with Abby Cloud OCR - Started");
            if (System.IO.File.Exists(@filePath))
            {
                using (var ocrClient = OCRHelper.GetOcrClientWithRetryPolicy())
                {
                    restURL = await OCRHelper.ProcessImageAsync(ocrClient, @filePath);
                    //new List<string>() { "https://ocrsdk.blob.core.windows.net/files/8ebe4855-1071-4ed0-95b6-de2334536a85.result?sv=2012-02-12&se=2019-11-02T21%3A00%3A00Z&sr=b&si=downloadResults&sig=VFoM9QfONhmpc4GjokEIEDDtIIDd9fRDY8t%2Bkvdu%2Fuo%3D" };
                    //await OCRHelper.ProcessImageAsync(ocrClient, @filePath);
                }
            }
            else {
                _logger.LogWarn("Could not find the uploaded file");
                 throw new ArgumentException( $"Some thing went wrong, Please contact administrator (view logs)");
            } 
            _logger.LogInfo("Reading MRZ Inforation with Abby Cloud OCR - Done");
            #endregion

            #region ReadingMRZOutput
            _logger.LogInfo("Reading MRZ Resulted Output - Start");
            DataSet ds = new DataSet();
            WebRequest request = WebRequest.Create(restURL[0].ToString());
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            WebRequest requestXML = WebRequest.Create(restURL[0].ToString());
            request.Method = "GET";
            using (var response = (System.Net.HttpWebResponse)request.GetResponse())
            {
                // get correct charset and encoding from the server's header
                Encoding encodingXML;
                try
                {
                    encodingXML = Encoding.GetEncoding(response.CharacterSet);
                }
                catch
                {
                    encodingXML = Encoding.UTF8;
                }

                using (var rdr = new StreamReader(response.GetResponseStream(), encoding))
                {
                    ds.ReadXml(rdr);
                }
            }

            MrzInfo objMRZ = OCRHelper.DeserializeMRZInfo(ds.Tables[1]);
            objMRZ.CustomerId = int.Parse(User.Claims.FirstOrDefault().Value);
            objMRZ.FileName = strActualFileName;
            objMRZ.FileNameUnique = strUniquename;
            _logger.LogInfo("Reading MRZ Resulted Output - Done");
            #endregion

            #region KYCVerification
            _logger.LogInfo("Verifying User KYC Details with Trulioo - Start");
            //Example Username: JoeNapoli_API_Demo, Example Password: 05uZuPRCyPi!6 
            bool isKYCVerified = await IsKYCVerified();
            _logger.LogInfo("Verifying User KYC Details with Trulioo - End");
            #endregion
            #region SavingMRZInfo 
            _logger.LogInfo("Saving MRZ Info - Start");
            objMRZ.isKYCVerified = isKYCVerified;
            objMRZ.AdditionalInfo = JsonConvert.SerializeObject(objMRZ);
            _uploadRepository.InsertMrzInfo(objMRZ);
            var allMRzInfo = _uploadRepository.GetMrzInfos();
            _logger.LogInfo("Saving MRZ Info - End");
            #endregion
            return Ok(allMRzInfo); 
        }

        [NonAction]
        public async Task<Boolean> IsKYCVerified() {

            try {
                // Even though having Global exception Handling, this try Block implemented to catch the exception and to not stop the remaining process
                //However, KYC verificaiton can also be done later for the saved entry
                VerifyResult respVerifyResult = new VerifyResult();
                using (var truliooClient = new HttpClient())
                {
                    truliooClient.BaseAddress = new Uri("https://gateway.trulioo.com");
                    truliooClient.DefaultRequestHeaders.Add("x-trulioo-api-key", "196ac06958fa024bc58b724a0e79cf16");
                    var result = await truliooClient.GetAsync("/trial/connection/v1/testauthentication");

                    using (HttpContent content = result.Content)
                    {
                        Task<string> result1 = content.ReadAsStringAsync();
                        var res = result1.Result;
                    }

                    if (result.IsSuccessStatusCode)
                    {
                        _logger.LogInfo("Trulioo Successfully Authenticated");
                    }
                    else {
                        _logger.LogInfo("Trulioo Authentication Failed");
                        return false;
                    }
                    //Since trial version of Trulioo is Used, it has limited access only for Asutrallina ID verification. 
                    //Dummy Australlian ID details are used to verify KYC. However, this can be refined with actual KYC request with purchased trulioo plan.    
                    VerifyRequest idDetails = new VerifyRequest()
                    {
                        AcceptTruliooTermsAndConditions = true,
                        CleansedAddress = false,
                        ConfigurationName = "Identity Verification",
                        ConsentForDataSources = new string[]
                     {
                         "Visa Verification"
                     },

                        CountryCode = "AU",
                        DataFields = new DataFields()
                        {
                            PersonInfo = new PersonInfo()
                            {
                                FirstGivenName = "John",
                                MiddleName = "Henry",
                                FirstSurName = "Smith",
                                DayOfBirth = 5,
                                MonthOfBirth = 3,
                                YearOfBirth = 1983,
                                Gender = "M"
                            },
                            Location = new Location()
                            {
                                BuildingNumber = "10",
                                UnitNumber = "3",
                                StreetName = "Lawford",
                                StreetType = "st",
                                Suburb = "Doncaster",
                                StateProvinceCode = "VIC",
                                PostalCode = "3108"
                            },
                            Communication = new Communication()
                            {
                                Telephone = "03 9896 8785",
                                EmailAddress = "testpersonAU@gdctest.com"
                            },
                            Passport = new Passport()
                            {
                                Number = "N1236548"
                            }
                        }
                    };
                    string stringData = JsonConvert.SerializeObject(idDetails);
                    var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
                    var idDetailsResults = await truliooClient.PostAsync("/trial/verifications/v1/verify", contentData);

                    if (idDetailsResults.IsSuccessStatusCode)
                    {

                        using (HttpContent content = idDetailsResults.Content)
                        {
                            Task<string> resultID = content.ReadAsStringAsync();
                            var strKYCResult = resultID.Result;
                            respVerifyResult = JsonConvert.DeserializeObject<VerifyResult>(strKYCResult);
                        }

                        _logger.LogInfo("Verifying User KYC Details with Trulioo - Done");
                        if (respVerifyResult.Record.RecordStatus.Equals("match"))
                            return true;
                        else
                            return false;
                    }
                    else {
                        _logger.LogInfo("Received Invalid Status Code from Trulioo");
                        return false;
                    }

                     
                } 
            }
            catch (Exception ex)
            {
                _logger.LogError("Technical glitch while doing Trulioo KYC Verification " + ex.ToString());
                return false;
            }
        }

         
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            var allMRzInfo = _uploadRepository.GetMrzInfos().OrderBy(x => x.CreatedDate).ToList();
             return Ok(allMRzInfo);
        }


    }
}
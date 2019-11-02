using Abbyy.CloudSdk.Demo.Core;
using Abbyy.CloudSdk.Demo.Core.EventArgs;
using Abbyy.CloudSdk.V2.Client;
using Abbyy.CloudSdk.V2.Client.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using KYC_AzaKaw_Core.AbbyOCRUtiilites;
using Polly; 
using Polly.Extensions.Http;
using KYC_AzaKaw_Core.Model;
using System.Data;

namespace KYC_AzaKaw_Core.Helpers
{
    public class OCRHelper
    { 
        private const string ApplicationId = @"eda2b839-3717-48aa-80b5-fb500fd78909";
        private const string Password = @"E2EXXLkQnGXthipqNBfd5FDR"; 
        private static ServiceProvider _serviceProvider;
        private static HttpClient _httpClient; 
        private const string ServiceUrl = "https://cloud-eu.ocrsdk.com";

        private static int _retryCount = 3;
        private static int _delayBetweenRetriesInSeconds = 3;
        private static string _httpClientName = "OCR_HTTP_CLIENT";

        private static readonly AuthInfo AuthInfo = new AuthInfo
        {
            Host = ServiceUrl,
            ApplicationId = ApplicationId,
            Password = Password
        };


        public static async Task<List<string>> ProcessImageAsync(IOcrClient ocrClient,string strFilePath)
        {
            AuthInfo AuthInfo = new AuthInfo
            {
                Host = "https://cloud-eu.ocrsdk.com",
                ApplicationId = @"eda2b839-3717-48aa-80b5-fb500fd78909",
                Password = @"E2EXXLkQnGXthipqNBfd5FDR"
            };
            var parameters = ProcessingParamsBuilder.GetMrzProcessingParams();
            string filePath = strFilePath;

            using (var fileStream = new FileStream(@filePath, FileMode.Open))
            {
                var taskInfo = await ocrClient.ProcessMrzAsync(
                    parameters,
                    fileStream,
                    Path.GetFileName(filePath),
                    waitTaskFinished: true);

                return taskInfo.ResultUrls;
            }
        }

        public static IOcrClient GetOcrClientWithRetryPolicy()
        {
            // Create service collection and configure our services
            var services = ConfigureServices();
            // Generate a provider
            _serviceProvider = services.BuildServiceProvider();

            var httpClientFactory = _serviceProvider.GetService<IHttpClientFactory>();
            _httpClient = httpClientFactory.CreateClient(_httpClientName);

            return new OcrClient(_httpClient);
        }

        private static ServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            //Configure HttpClientFactory with retry handler
            services.AddHttpClient(_httpClientName, conf =>
            {
                conf.BaseAddress = new Uri(AuthInfo.Host);
                //increase the default value of timeout for the duration of retries
                conf.Timeout = conf.Timeout + TimeSpan.FromSeconds(_retryCount * _delayBetweenRetriesInSeconds);
            })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    PreAuthenticate = true,
                    Credentials = new NetworkCredential(AuthInfo.ApplicationId, AuthInfo.Password)
                })
                //Add  custom HttpClientRetryPolicyHandler with polly
                .AddHttpMessageHandler(() => new HttpClientRetryPolicyHandler(GetRetryPolicy()));

            //or you can use Microsoft.Extensions.DependencyInjection Polly extension
            //.AddPolicyHandler(GetRetryPolicy());
            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions.HandleTransientHttpError()
                //Condition - what kind of request errors should we repeat
                .OrResult(r => r.StatusCode == HttpStatusCode.GatewayTimeout)
                .WaitAndRetryAsync(
                    _retryCount,
                    sleepDurationProvider => TimeSpan.FromSeconds(_delayBetweenRetriesInSeconds),
                    (exception, calculatedWaitDuration, retries, context) =>
                    {
                        Console.WriteLine($"Retry {retries} for policy with key {context.PolicyKey}");
                    }
                )
                .WithPolicyKey("WaitAndRetryAsync_For_GatewayTimeout_504__StatusCode");
        }

        public static MrzInfo DeserializeMRZInfo(DataTable dtMRZ)
        {
            var retList = new MrzInfo();

            for (int i = 0; i < dtMRZ.Rows.Count; i++)
            {
                var row = dtMRZ.Rows[i];
                switch (row["type"]) {
                    case "MRZType":
                        retList.MrzType = row["value"].ToString();
                        break;

                    case "MrzType":
                        retList.MrzType = row["value"].ToString();
                        break;
                    case "Line1":
                        retList.Line1 = row["value"].ToString();
                        break;
                    case "Line2":
                        retList.Line2 = row["value"].ToString();
                        break;
                    case "Checksum":
                        retList.Checksum = row["value"].ToString();
                        break;
                    case "ChecksumVerified":
                        retList.ChecksumVerified = row["value"].ToString();
                        break;
                    case "DocumentType":
                        retList.DocumentType = row["value"].ToString();
                        break;
                    case "DocumentSubtype":
                        retList.DocumentSubtype = row["value"].ToString();
                        break;
                    case "IssuingCountry":
                        retList.IssuingCountry = row["value"].ToString();
                        break;
                    case "DocumentNumber":
                        retList.DocumentNumber = row["value"].ToString();
                        break;
                    case "DocumentNumberVerified":
                        retList.DocumentNumberVerified = row["value"].ToString();
                        break;
                    case "DocumentNumberCheck":
                        retList.DocumentNumberCheck = row["value"].ToString();
                        break;
                    case "Nationality":
                        retList.Nationality = row["value"].ToString();
                        break;
                    case "BirthDate":
                        retList.BirthDate = row["value"].ToString();
                        break;
                    case "BirthDateVerified":
                        retList.BirthDateVerified = row["value"].ToString();
                        break;
                    case "BirthDateCheck":
                        retList.BirthDateCheck = row["value"].ToString();
                        break;
                    case "Sex":
                        retList.Sex = row["value"].ToString();
                        break;
                    case "ExpiryDate":
                        retList.ExpiryDate = row["value"].ToString();
                        break;
                    case "ExpiryDateVerified":
                        retList.ExpiryDateVerified = row["value"].ToString();
                        break;
                    case "ExpiryDateCheck":
                        retList.ExpiryDateCheck = row["value"].ToString();
                        break; 

                }

            } 
            return retList;
        }

    }
}
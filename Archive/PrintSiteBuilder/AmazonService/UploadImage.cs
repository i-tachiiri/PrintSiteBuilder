using FikaAmazonAPI.Services;
using FikaAmazonAPI;
using PrintSiteBuilder.Interfaces;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using FikaAmazonAPI.AmazonSpApiSDK.Models.Upload;
using FikaAmazonAPI.Parameter.Upload;
using FikaAmazonAPI.Utils;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace PrintSiteBuilder.AmazonService
{
    public class UploadImage
    {
        public Auth authService;
        public AmazonCredential credential;
        public AmazonConnection connection;
        public UploadService service;
        public UploadService uploadService;
        public IPrint2 iPrint;
        public string SellerId;
        public string IssueLocale;
        public string MarketPlaceId;
        public UploadImage(IPrint2 iPrint)
        {
            authService = new Auth();
            credential = authService.Credential;
            connection = authService.Connection;
            service = new UploadService(credential);
            SellerId = credential.SellerID;
            MarketPlaceId = credential.MarketPlace.ID;
            IssueLocale = "ja-JP";
            this.iPrint = iPrint;
        }
        public async Task UploadProductImageAsync(string filePath, string sku)
        {
            var parameter = new ParameterCreateUploadDestinationForResource();
            parameter.marketplaceIds = new List<string> { MarketPlace.Japan.ID };
            parameter.contentType = "image/png";
            parameter.contentMD5 = CreateMd5(filePath);
            parameter.resource = ""; //アップロード先の作成？
            service.CreateUploadDestinationForResourceAsync(parameter);
        }
        private string CreateMd5(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hash = md5.ComputeHash(stream);
                    return Convert.ToBase64String(hash);
                }
            }
        }

    }
}

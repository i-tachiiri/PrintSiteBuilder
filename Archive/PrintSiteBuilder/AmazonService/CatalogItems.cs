using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FikaAmazonAPI.Services;
using FikaAmazonAPI;
using FikaAmazonAPI.Utils;

using Newtonsoft.Json.Linq;
using RestSharp;
using FikaAmazonAPI.Parameter.CatalogItems;
using FikaAmazonAPI.AmazonSpApiSDK.Models.CatalogItems.V20220401;
using static FikaAmazonAPI.Utils.Constants;

namespace PrintSiteBuilder.AmazonService
{
    public class CatalogItems
    {
        private readonly string _region = "fe";
        private readonly string _marketplaceId = "A1VC38T7YXB528"; // 日本のマーケットプレイスID
        private readonly string _asin;
        public Auth authService;
        public AmazonCredential credential;
        public AmazonConnection connection;
        public CatalogItemService service;
        public CatalogItems()
        {
            authService = new Auth();
            credential = authService.Credential;
            connection = authService.Connection;
            service = new CatalogItemService(credential);
        }
        public async Task<Item> GetCatalogItem()
        {
            var parameter = new ParameterGetCatalogItem();
            parameter.marketplaceIds = new List<string> { MarketPlace.Japan.ID };
            parameter.ASIN = "B0D986DNL6";
            parameter.includedData = new List<IncludedData>
            {
                IncludedData.attributes,
                IncludedData.dimensions,
                IncludedData.identifiers,
                IncludedData.images,
                IncludedData.productTypes,
                IncludedData.relationships,
                IncludedData.salesRanks,
                IncludedData.summaries,
                //IncludedData.vendorDetails
            };
            return await service.GetCatalogItem202204Async(parameter);
        }
        /*
        "class ItemSummaryByMarketplace {
          MarketplaceId: A1VC38T7YXB528
          Brand: テンプリ
          BrowseClassification: class ItemBrowseClassification {
          DisplayName: 学習帳・練習帳
          ClassificationId: 4095902051
        }

          Color: 
          ItemClassification: BASEPRODUCT
          ItemName: 1桁の足し算のプリント (繰り上がり有)
          Manufacturer: テンプリ
          ModelNumber: 
          PackageQuantity: 1
          PartNumber: 100002
          Size: A4
          Style: 繰り上がり有
          WebsiteDisplayGroup: office_product_display_on_website
        }
        "
         */
    }
}

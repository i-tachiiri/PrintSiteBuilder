using System;
using System.Windows.Forms;
using FikaAmazonAPI;
using FikaAmazonAPI.Parameter.FulFillmentInbound;
using FikaAmazonAPI.Parameter.Order;
using FikaAmazonAPI.Parameter.Report;
using FikaAmazonAPI.Utils;
using static FikaAmazonAPI.Utils.Constants;
using FikaAmazonAPI.Services;
using Google.Apis.Auth.OAuth2;
using FikaAmazonAPI.AmazonSpApiSDK.Models.FulfillmentInbound;
using System.Text.Json;

namespace PrintSiteBuilder.AmazonService.Archive
{
    public class TuterialService
    {
        public Auth authService;
        public AmazonCredential credential;
        public AmazonConnection connection;
        public TuterialService()
        {
            authService = new Auth();
            credential = authService.Credential;
            connection = authService.Connection;
        }
        public void GetOrderList()
        {
            ParameterOrderList serachOrderList = new ParameterOrderList();
            serachOrderList.CreatedAfter = DateTime.UtcNow.AddMinutes(-600000);
            serachOrderList.OrderStatuses = new List<OrderStatuses>();
            serachOrderList.OrderStatuses.Add(OrderStatuses.Canceled);
            var orders = connection.Orders.GetOrders
            (
                 new ParameterOrderList
                 {
                     TestCase = TestCase200
                 }
            );
            var c = 1;
        }
        public void GetReports()
        {
            var parameters = new ParameterReportList();
            parameters.pageSize = 100;
            parameters.reportTypes = new List<ReportTypes>();
            parameters.reportTypes.Add(ReportTypes.GET_AFN_INVENTORY_DATA);
            parameters.marketplaceIds = new List<string>();
            parameters.marketplaceIds.Add(MarketPlace.Japan.ID);
            var reports = connection.Reports.GetReports(parameters);
            var c = 1;
        }
        public async Task GetCatalogItem()
        {
            var data = await connection.CatalogItem.GetCatalogItem202204Async(
                new FikaAmazonAPI.Parameter.CatalogItems.ParameterGetCatalogItem
                {
                    ASIN = "B0D986DNL6",
                    includedData = new[] { IncludedData.attributes,
                                                   IncludedData.salesRanks,
                                                   IncludedData.summaries,
                                                   IncludedData.productTypes,
                                                   IncludedData.relationships,
                                                   IncludedData.dimensions,
                                                   IncludedData.identifiers,
                                                   IncludedData.attributes,
                                                   IncludedData.images }
                });
            var jsonData = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

            var c = 1;
            /*

             */
        }
    }
}

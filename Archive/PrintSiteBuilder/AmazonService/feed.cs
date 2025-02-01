using System;
using System.Windows.Forms;
using FikaAmazonAPI;
using FikaAmazonAPI.Parameter.Order;
using FikaAmazonAPI.Parameter.Report;
using FikaAmazonAPI.Utils;
using static FikaAmazonAPI.Utils.Constants;
using FikaAmazonAPI.Services;
using Google.Apis.Auth.OAuth2;
using System.Text.Json;
using FikaAmazonAPI.AmazonSpApiSDK.Models.FulfillmentInbound;
using FikaAmazonAPI.AmazonSpApiSDK.Models.Feeds;
using FikaAmazonAPI.Parameter.FulFillmentInbound;
using FikaAmazonAPI.Parameter.Feed;
using FikaAmazonAPI.ConstructFeed.Messages;
using FikaAmazonAPI.ConstructFeed;

namespace PrintSiteBuilder.AmazonService
{
    public class feed
    {
        public Auth authService;
        public AmazonCredential credential;
        public AmazonConnection connection;
        public FeedService service;
        public feed()
        {
            authService = new Auth();
            credential = authService.Credential;
            connection = authService.Connection;
            service = new FeedService(credential);
        }
        public async Task<IList<Feed>> GetFeeds()
        {
            //feed=送信データ。この関数はfeedの送信履歴を確認するもの
            var parameter = new ParameterGetFeed();
            parameter.feedTypes = new List<FeedType>() { FeedType.POST_PRODUCT_DATA };
            return await service.GetFeedsAsync(parameter);
        }
        public async Task ChangePrice()
        {
            ConstructFeedService createDocument = new ConstructFeedService("{SellerID}", "1.02");
            var list = new List<InventoryMessage>();
            list.Add(new InventoryMessage()
            {
                SKU = "82010312061.22...",
                Quantity = 2,
                FulfillmentLatency = "11",
            });
            createDocument.AddInventoryMessage(list);
            var xml = createDocument.GetXML();
            var feedID = connection.Feed.SubmitFeed(xml, FeedType.POST_INVENTORY_AVAILABILITY_DATA);
            Thread.Sleep(1000 * 30);
            var feedOutput = connection.Feed.GetFeed(feedID);
            var outPut = connection.Feed.GetFeedDocument(feedOutput.ResultFeedDocumentId);
            var reportOutpit = outPut.Url;
            var processingReport = connection.Feed.GetFeedDocumentProcessingReport(outPut.Url);

        }
        /*public async Task<CreateFeedResult> CreateFeed()
        {
            var parameter = new CreateFeedSpecification( "1",new List<string> { MarketPlace.Japan.ID},  );
            return await service.CreateFeedAsync(parameter);
        }*/
    }
}

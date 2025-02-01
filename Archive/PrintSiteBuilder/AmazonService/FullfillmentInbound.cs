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
using FikaAmazonAPI.Parameter.FulFillmentInbound;

namespace PrintSiteBuilder.AmazonService
{
    public class FullfillmentInbound
    {
        public Auth authService;
        public AmazonCredential credential;
        public AmazonConnection connection;
        public FulFillmentInboundService service;
        public FullfillmentInbound()
        {
            authService = new Auth();
            credential = authService.Credential;
            connection = authService.Connection;
            service = new FulFillmentInboundService(credential);            
        }
        public async Task<InboundShipmentResult> UpdateInboundShipment()
        {
            var request = new InboundShipmentRequest();
            var ShipmentId = "FBA15F2GWC1M";
            request.MarketplaceId = MarketPlace.Japan.ID;
            request.InboundShipmentItems = new InboundShipmentItemList();
            request.InboundShipmentItems.Add(new InboundShipmentItem(ShipmentId, "IP-06CC-YL9S", "X0017VMFJF",1,1,1));
            return await service.UpdateInboundShipmentAsync(ShipmentId, request);
        }
        public async Task<InboundShipmentItemList> GetShipmentItemsAsync()
        {
            var parameter = new ParameterGetShipmentItems();
            parameter.LastUpdatedAfter = DateTime.Now.AddMonths(-1);
            parameter.LastUpdatedBefore = DateTime.Now;
            parameter.MarketplaceId = MarketPlace.Japan.ID;
            parameter.QueryType = QueryType.DATE_RANGE;
            return await service.GetShipmentItemsAsync(parameter);
        }
        /*
        "class InboundShipmentItem {
          ShipmentId: FBA15F268D7Y
          SellerSKU: IP-06CC-YL9S
          FulfillmentNetworkSKU: X0017VMFJF
          QuantityShipped: 5
          QuantityReceived: 5
          QuantityInCase: 0
          ReleaseDate: 
          PrepDetailsList: class PrepDetailsList {
          FikaAmazonAPI.AmazonSpApiSDK.Models.FulfillmentInbound.PrepDetailsList
        }
         */

        public async Task<GetShipmentItemsResult> GetShipmentItemsByShipmentId()
        {
            return await service.GetShipmentItemsByShipmentIdAsync("FBA15F268D7Y");
        }
        /*
        {class InboundShipmentItem {
          ShipmentId: FBA15F268D7Y
          SellerSKU: IP-06CC-YL9S
          FulfillmentNetworkSKU: X0017VMFJF
          QuantityShipped: 5
          QuantityReceived: 5
          QuantityInCase: 0
          ReleaseDate: 
          PrepDetailsList: class PrepDetailsList {
          FikaAmazonAPI.AmazonSpApiSDK.Models.FulfillmentInbound.PrepDetailsList
        }
         */
        public async Task<CreateInboundShipmentPlanResult> CreateInboundShipmentPlanAsync()
        {
            var address = new Address("伊藤駿", "東町1-29-8(17)", "", "日本","小金井市", "東京都", "JP", "184-0011");
            var inboundShipmentPlanItemListRequest = new InboundShipmentPlanRequestItemList();
            inboundShipmentPlanItemListRequest.Add(new InboundShipmentPlanRequestItem("IP-06CC-YL9S", "B0D986DNL6", Condition.NewItem, 5, 1, null));
            var request = new CreateInboundShipmentPlanRequest(address,LabelPrepPreference.SELLERLABEL,"JP","", inboundShipmentPlanItemListRequest);
            return await service.CreateInboundShipmentPlanAsync(request);
        }
        /*
        "class InboundShipmentPlan {
          ShipmentId: FBA15F2GWC1M
          DestinationFulfillmentCenterId: KIX6
          ShipToAddress: class Address {
          Name: KIX6
          AddressLine1: 東海岸町20-1
          AddressLine2: 
          DistrictOrCounty: 
          City: 尼崎市
          StateOrProvinceCode: 兵庫県
          CountryCode: JP
          PostalCode: 660-8582
        }

          LabelPrepType: SELLERLABEL
          Items: class InboundShipmentPlanItemList {
          FikaAmazonAPI.AmazonSpApiSDK.Models.FulfillmentInbound.InboundShipmentPlanItemList
        }

          EstimatedBoxContentsFee: 
        }
        "

         */


        public async Task<GetShipmentsResult> GetShipmentsAsync()
        {
            var parameterGetShipment = new ParameterGetShipments();
            parameterGetShipment.MarketplaceId = MarketPlace.Japan.ID;
            parameterGetShipment.ShipmentIdList = new List<string> { "FBA15F268D7Y" };
            return await service.GetShipmentsAsync(parameterGetShipment);
        }

        /*
        {class InboundShipmentInfo {
          ShipmentId: FBA15F268D7Y
          ShipmentName: FBA STA (2024/08/04 10:40)-TYO9
          ShipFromAddress: class Address {
          Name: 伊藤駿
          AddressLine1: 1-29-8(17)
          AddressLine2: 
          DistrictOrCounty: 
          City: 小金井市東町
          StateOrProvinceCode: Tokyo
          CountryCode: JP
          PostalCode: 1840011
        }

          DestinationFulfillmentCenterId: TYO9
          ShipmentStatus: RECEIVING
          LabelPrepType: SELLERLABEL
          AreCasesRequired: 
          ConfirmedNeedByDate: 
          BoxContentsSource: INTERACTIVE
          EstimatedBoxContentsFee: 
        }
        }
        */

        
    }

}

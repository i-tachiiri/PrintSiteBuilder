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
using FikaAmazonAPI.AmazonSpApiSDK.Models.FulfillmentInboundv20240320;
using FikaAmazonAPI.Parameter.FulFillmentInbound.v20240320;

namespace PrintSiteBuilder.AmazonService.Archive
{
    public class FullfillmentInboundService20240320
    {
        public Auth authService;
        public AmazonCredential credential;
        public AmazonConnection connection;
        public FulFillmentInboundServicev20240320 service;
        public FullfillmentInboundService20240320()
        {
            authService = new Auth();
            credential = authService.Credential;
            connection = authService.Connection;
            service = new FulFillmentInboundServicev20240320(credential);
        }

        public async Task<List<InboundPlanSummary>> GetShipmentLabels()  //段ボールに貼り付けるやつのダウンロードURLを取得できる
        {
            //unauthorized
            //var inboundPlanId = "3BSN8O7C";
            //var shipmentID = "FBA15F268D7Y";
            var parameter = new ParameterGetListInboundPlans();
            return await service.GetListInboundPlansAsync(parameter);
        }
        public async Task<List<InboundPlanSummary>> GetListInboundPlans()  //段ボールに貼り付けるやつのダウンロードURLを取得できる
        {
            //unauthorized
            var parameter = new ParameterGetListInboundPlans();
            return await service.GetListInboundPlansAsync(parameter);
        }
        public async Task<CreateInboundPlanResponse> CreateInboundPlan()
        {
            //unauthorized
            var contactInformation = new ContactInformation("admin@tachiiri.com", "伊藤駿", "07010505738");
            var itemInput = new ItemInput(null, 0, null, "P100004", PrepOwner.SELLER, 5);
            var address = new Address("", "東京都", "小金井市", "", "ja", "伊藤駿", "1840011");
            var createNewInboundPlanRequest = new CreateInboundPlanRequest(contactInformation, new List<string> { MarketPlace.Japan.ID }, new List<ItemInput> { itemInput }, null, address);
            return await service.CreateInboundPlanAsync(createNewInboundPlanRequest);
        }
        /*public async Task<GetShipmentsResult> GetInboundGuidence()
        {
            var parameterGetShipment = new ParameterGetShipments();
            parameterGetShipment.MarketplaceId = MarketPlace.Japan.ID;
            parameterGetShipment.ShipmentIdList = new List<string> { "FBA15F268D7Y" };
            var aaa = new FulFillmentInboundService(credential);
            return await aaa.GetShipmentsAsync(parameterGetShipment);
        }*/

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

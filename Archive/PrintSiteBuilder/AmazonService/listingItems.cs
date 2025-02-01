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
using FikaAmazonAPI.Parameter.ListingItem;
using FikaAmazonAPI.AmazonSpApiSDK.Models.ListingsItems;
using PrintSiteBuilder.Interfaces;
using Newtonsoft.Json;

namespace PrintSiteBuilder.AmazonService
{
    public class listingItems
    {
        public Auth authService;
        public AmazonCredential credential;
        public AmazonConnection connection;
        public ListingsItemService service;
        public IPrint2 iPrint;
        public string SellerId;
        public string IssueLocale;
        public string MarketPlaceId;
        public listingItems(IPrint2 iPrint)
        {
            authService = new Auth();
            credential = authService.Credential;
            connection = authService.Connection;
            service = new ListingsItemService(credential);
            SellerId = credential.SellerID;
            MarketPlaceId = credential.MarketPlace.ID;
            IssueLocale = "ja-JP";
            this.iPrint = iPrint;
        }
        public async Task<Item> GetListingItem()
        {
            try
            {
                var parameter = new ParameterGetListingsItem();
                parameter.sellerId = SellerId;
                parameter.marketplaceIds = new List<string>() { MarketPlaceId };
                parameter.sku = iPrint.Sku;
                parameter.issueLocale = IssueLocale;
                parameter.includedData = new List<ListingsIncludedData>
                {
                    ListingsIncludedData.FulfillmentAvailability,
                    ListingsIncludedData.Issues,
                    ListingsIncludedData.Attributes,
                    ListingsIncludedData.Summaries,
                    ListingsIncludedData.Offers,
                    //ListingsIncludedData.Procurement
                };
                //parameter.TestCase = "false";
                return await service.GetListingsItemAsync(parameter);
            }
            catch (Exception ex) 
            { 
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]sku {iPrint.Sku} is not registered.");
                return new Item();
            }
        }
        public async Task<ListingsItemSubmissionResponse> PutListingsItem()
        {
            var parameter = new ParameterPutListingItem();

            parameter.sellerId = SellerId;
            parameter.marketplaceIds = new List<string> { MarketPlace.Japan.ID };
            parameter.sku = iPrint.Sku;
            parameter.issueLocale = IssueLocale;
            
            parameter.listingsItemPutRequest = new FikaAmazonAPI.Parameter.ListingItem.ListingsItemPutRequest();
            parameter.listingsItemPutRequest.productType = "BLANK_BOOK";
            parameter.listingsItemPutRequest.attributes = await CreateAttributes();
            Console.WriteLine($"Request URI: /listings/2021-08-01/items/{parameter.sellerId}/{parameter.sku}");
            Console.WriteLine($"Request Body: {JsonConvert.SerializeObject(parameter.listingsItemPutRequest)}");
            var response = await service.PutListingsItemAsync(parameter);

            // Log response details
            Console.WriteLine($"Response Body: {JsonConvert.SerializeObject(response)}");
            return response;
        }
        private async Task<object> CreateAttributes()
        {
            var MarketPlaceId = MarketPlace.Japan.ID;
            var PrintId = iPrint.PrintId;//"P100001";
            var BrandName = "テンプリ";
            var Description = iPrint.Description; //"1桁の足し算のプリントです。10問×30枚=300問が収録されています。 QRコードからプリントの参照や再印刷ができる専用のWEBページが開けます。繰り返しの学習や、学校・塾、兄弟姉妹等の複数人での学習用にも適しています。";
            var ItemName = iPrint.PrintName;//"sample_item";
            var Keywords = iPrint.Keywords; //"プリント 算数 足し算 計算 3歳 4歳 5歳 6歳 1年生 2年生 100マス計算 繰り上がり 問題集";
            return new
            {
                
                product_description = new[] { new { value = Description, marketplace_id = MarketPlaceId } },
                manufacturer = new[] { new { value = BrandName, marketplace_id = MarketPlaceId } },
                supplier_declared_dg_hz_regulation = new[] { new { value = "not_applicable", marketplace_id = MarketPlaceId } },
                part_number = new[] { new { value = PrintId, marketplace_id = MarketPlaceId } },
                color = new[] { new { value = "", marketplace_id = MarketPlaceId } },
                bullet_point = new[] { new { value = "", marketplace_id = MarketPlaceId } },
                item_name = new[] { new { value = ItemName, marketplace_id = MarketPlaceId } },
                country_of_origin = new[] { new { value = "JP", marketplace_id = MarketPlaceId } },
                recommended_browse_nodes = new[] { new { value = "4095902051", marketplace_id = MarketPlaceId } },
                is_exclusive_product = new[] { new { value = "FALSE", marketplace_id = MarketPlaceId } },
                brand = new[] { new { value = BrandName, marketplace_id = MarketPlaceId } },
                condition_type = new[] { new { value = "new_new", marketplace_id = MarketPlaceId } },
                merchant_shipping_group = new[] { new { value = "legacy-template-id", marketplace_id = MarketPlaceId } },
                merchant_suggested_asin = new[] { new { value = $"ASIN{PrintId.Substring(0, 6)}", marketplace_id = MarketPlaceId } },
                batteries_required = new[] { new { value = false, marketplace_id = MarketPlaceId } },
                item_package_weight = new[] { new { unit = "kilograms", value = 0.6, marketplace_id = MarketPlaceId } },
                fulfillment_availability = new[] { new { fulfillment_channel_code = "AMAZON_JP", quantity = 0, marketplace_id = MarketPlaceId } },
                generic_keyword = new[] { new { value = Keywords, marketplace_id = MarketPlaceId } },
                purchasable_offer = new[]
                {
                    new
                    {
                        currency = "JPY",
                        start_at = new{value = DateTime.Now},
                        our_price = new[]{new{schedule = new[]{ new { value_with_tax = 1000.0 } } } },
                        marketplace_id = MarketPlaceId
                    }
                },
                item_package_dimensions = new[]
                {
                    new
                    {
                        width = new{unit = "centimeters",value = 22.0},
                        length = new{unit = "centimeters",value = 30},
                        height = new {unit = "centimeters",value = 0.5},
                        marketplace_id = MarketPlaceId
                    }
                },
            };
        }


    }
}
/*
public async Task<ListingsItemSubmissionResponse> PutListingsItem()
{
    // 1. パラメーターのインスタンスを作成
    var parameter = new ParameterPutListingItem
    {
        MarketplaceIds = new List<string> { "MARKETPLACE_ID" },  // 例: "A1PA6795UKMFR9" (Amazon DE)
        Sku = "SAMPLE_SKU",  // 商品のSKU
        ProductType = "PRODUCT_TYPE",  // 商品タイプを指定
        ListingsItem = new ListingsItemRequest
        {
            Attributes = new Dictionary<string, IList<AttributeValue>>
            {
                // 商品タイトル
                { "title", new List<AttributeValue> { new AttributeValue { Value = "Sample Product Title" } } },
                
                // ブランド
                { "brand", new List<AttributeValue> { new AttributeValue { Value = "Sample Brand" } } },
                
                // 商品説明
                { "description", new List<AttributeValue> { new AttributeValue { Value = "This is a sample product description." } } },

                // 他の属性をここに追加
            }
        },
        ConditionType = "new_new",  // 商品の状態（例: 新品）
        ItemCondition = new Condition
        {
            ConditionType = "new_new"
        },
        FulfillmentAvailability = new List<FulfillmentAvailability>
        {
            new FulfillmentAvailability
            {
                FulfillmentChannelCode = "DEFAULT",  // フルフィルメントチャネル (FBAやFBMなど)
                Quantity = 100  // 在庫数
            }
        }
    };

    // 2. パラメーターを使って商品を登録または更新
    return await service.PutListingsItemAsync(parameter);
}

 */

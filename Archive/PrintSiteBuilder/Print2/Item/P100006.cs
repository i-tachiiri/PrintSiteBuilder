using PrintSiteBuilder.Interfaces;
using PrintSiteBuilder.SiteItem;
using PrintSiteBuilder.GoogleService.Drive;
using PrintSiteBuilder.Models.General;
using PrintSiteBuilder.Print2.Template;
using PrintSiteBuilder.AmazonService;
using Newtonsoft.Json.Linq;

namespace PrintSiteBuilder.Print2.Item
{
    public class P100006 : IPrint2
    {
        public Dictionary<string, string> PrintId_CategoryName { get; private set; }
        public int PagesCount { get; private set; }
        public string PrintSlideId { get; private set; }
        public string CoverSlideId { get; private set; }
        public string AmazonSlideId { get; private set; }

        public string PrintId { get; private set; }
        public string PrintName { get; private set; }
        public string Uuid { get; private set; }
        public string FnSku { get; private set; }
        public PathConfig path { get; private set; }
        public int TemplateNumber { get; private set; }
        public IPrintType PrintType { get; private set; }
        public string Description { get; private set; }
        public string Keywords { get; private set; }
        public int Score { get; private set; }
        public string Sku { get; private set; }
        public string Asin { get; private set; }
        public bool IsTemplateSet { get; private set; }
        public List<List<string>> QaPairs;
        public List<List<string>> RandomPairs;
        public TemplateFolder templateFolder;
        public Task<List<Google.Apis.Drive.v3.Data.File>> SlideList;
        public SlidesConfig slidesConfig;
        public Json2 json = new Json2();
        public Item2 item = new Item2();

        public async Task InitializeAsync(string s)
        {
            TemplateNumber = 1;
            PagesCount = 30;
            PrintType = new P100マス計算(this);
            PrintId = GetType().Name.Replace("P", "");
            Sku = $"{PrintType.SkuHeader}-{PrintId.Substring(0, 2)}-{PrintId.Substring(2, 4)}";
            var catelogItems = new listingItems(this);
            var catalogItem = await catelogItems.GetListingItem();
            if(catalogItem.Summaries != null)
            {
                FnSku = catalogItem.Summaries[0].FnSku;
                Asin = catalogItem.Summaries[0].Asin;
                PrintName = catalogItem.Summaries[0].ItemName;
                var DescriptionResult = catalogItem.Attributes["product_description"] as JArray;
                Description = DescriptionResult[0]["value"].ToString();
                var KeywordResult = catalogItem.Attributes["generic_keyword"] as JArray;
                Keywords = KeywordResult[0]["value"].ToString();
            }

            path = new PathConfig(PrintId);
            if (!File.Exists(path.PrintSlideConfig))
            {
                var template = new TemplateFolder(this);
                await template.CopyTemplate();
                slidesConfig = await item.GetSlidesConfig(this);
                json.SerializeSlidesConfig(slidesConfig, this);
            }
            slidesConfig = json.DeserializeSlidesConfig(this);
            CoverSlideId = slidesConfig.CoverSlideConfig.SlideId;
            AmazonSlideId = slidesConfig.AmazonSlideConfig.SlideId;
            PrintSlideId = slidesConfig.PrintSlideConfig.SlideId;
            
            Uuid = PrintSlideId.Substring(0, 12);            
        }

        public List<List<List<string>>> GetQuestionLists()
        {
            return  new List<List<List<string>>>()
            {
                GetQuestions1(),
                //GetQuestions1(),
            };
        }
        public List<List<string>> GetQuestions1()
        {
            var questions = new List<List<string>>();
            var NumberPairs = new List<List<int>>();
            Random random = new Random();
            List<int> vericalNumbers = Enumerable.Range(1, 10).OrderBy(x => random.Next()).ToList();
            List<int> horizonalNumbers = Enumerable.Range(1, 10).OrderBy(x => random.Next()).ToList();
            questions.Add(new List<string>
            {
                "×",
                horizonalNumbers[0].ToString(),
                horizonalNumbers[1].ToString(),
                horizonalNumbers[2].ToString(),
                horizonalNumbers[3].ToString(),
                horizonalNumbers[4].ToString(),
                horizonalNumbers[5].ToString(),
                horizonalNumbers[6].ToString(),
                horizonalNumbers[7].ToString(),
                horizonalNumbers[8].ToString(),
                horizonalNumbers[9].ToString(),
                "×",
                Guid.NewGuid().ToString()
            });
            for (var i = 0; i < vericalNumbers.Count; i++)
            {
                questions.Add(new List<string>
                {
                    vericalNumbers[i].ToString(),
                    (horizonalNumbers[0] * vericalNumbers[i]).ToString(),
                    (horizonalNumbers[1] * vericalNumbers[i]).ToString(),
                    (horizonalNumbers[2] * vericalNumbers[i]).ToString(),
                    (horizonalNumbers[3] * vericalNumbers[i]).ToString(),
                    (horizonalNumbers[4] * vericalNumbers[i]).ToString(),
                    (horizonalNumbers[5] * vericalNumbers[i]).ToString(),
                    (horizonalNumbers[6] * vericalNumbers[i]).ToString(),
                    (horizonalNumbers[7] * vericalNumbers[i]).ToString(),
                    (horizonalNumbers[8] * vericalNumbers[i]).ToString(),
                    (horizonalNumbers[9] * vericalNumbers[i]).ToString(),
                    vericalNumbers[i].ToString(),
                });
            }
            return questions;
        }
    }
}

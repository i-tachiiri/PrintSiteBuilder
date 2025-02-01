using BlogDomain.Entity;
using Google.Apis.Auth.OAuth2;
using Google.Apis.SearchConsole.v1;
using Google.Apis.SearchConsole.v1.Data;
using Google.Apis.Services;

namespace GoogleSearchConsoleLibrary
{
    public class GoogleSearchConsoleService
    {
        private async Task<SearchConsoleService> GetSearchConsoleServiceAsync()
        {
            string credentialPath = Path.Combine(Directory.GetCurrentDirectory(), "client_secret.json");
            var credential = GoogleCredential.FromFile(credentialPath).CreateScoped(SearchConsoleService.Scope.WebmastersReadonly);
            return new SearchConsoleService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
            });
        }
        public async Task<List<SearchEntity>> GetSearchAnalyticsDataAsync(DateTime FromDate,int Period)
        {
            var service = await GetSearchConsoleServiceAsync();
            var request = service.Searchanalytics.Query(new SearchAnalyticsQueryRequest
            {
                StartDate = FromDate.ToString("yyyy-MM-dd"),
                EndDate = FromDate.AddDays(Period).ToString("yyyy-MM-dd"),
                Dimensions = new List<string> { "page" , "query" }, // ページとクエリでグループ化
                RowLimit = 25000
            }, "sc-domain:tachiiri.com"); 

            var response = await request.ExecuteAsync();

            var SearchEntities = new List<SearchEntity>();
            foreach (var row in response.Rows)
            {
                var page = row.Keys[0];   // ページURL
                var query = row.Keys[1];
                var Impressions = row.Impressions;
                var Position = Math.Round((Decimal)row.Position,1);
                var clicks = row.Clicks;  // クリック数
                if(clicks > 0 && page.Contains("tachiiri.com/blog"))
                {
                    Uri uri = new Uri(page);
                    string path = uri.AbsolutePath; 
                    string PageId = System.IO.Path.GetFileName(path).Replace(".html","");
                    var entity = new SearchEntity();
                    entity.FromDate = FromDate;
                    entity.PageId = PageId;
                    entity.Query = query;
                    entity.Period = Period;
                    entity.Impression = (Double)Impressions;
                    entity.Position = (Double)Position;
                    entity.Click = clicks ?? 0;
                    SearchEntities.Add(entity);
                }                
            }
            return SearchEntities;
        }
    }
    
}

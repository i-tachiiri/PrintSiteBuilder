using BlogDomain.Entity;
using BlogDomain.Services;
using GoogleSearchConsoleLibrary;
using MysqlLibrary.Repository.Print;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogGenerator.Services
{
    public class SearchEntityMapper
    {
        private GoogleSearchConsoleService consoleLibrary;
        private MysqlSearchRepository mysqlSearchRepository;
        private Logger logger;
        public SearchEntityMapper(GoogleSearchConsoleService consoleLibrary, MysqlSearchRepository mysqlSearchRepository, Logger logger) 
        {
            this.consoleLibrary = consoleLibrary;
            this.mysqlSearchRepository = mysqlSearchRepository;
            this.logger = logger;
        }
        public async Task UpsertSearchEntities()
        {
            var FromDate = DateTime.Now.AddDays(-30);
            var SearchEntities = new List<SearchEntity>();
            while(FromDate < DateTime.Now.AddDays(-3))
            {
                logger.LoopLog("get console");
                var DailyRank = await consoleLibrary.GetSearchAnalyticsDataAsync(FromDate, 1);
                SearchEntities.AddRange(DailyRank);
                FromDate = FromDate.AddDays(1);
            }
            await mysqlSearchRepository.UpsertCosineListAsync(SearchEntities);

        }
    }
}

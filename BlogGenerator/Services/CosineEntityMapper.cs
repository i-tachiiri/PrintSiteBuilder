using BlogDomain.Entity;
using BlogDomain.Services;
using BlogGenerator.Application.Services;
using MysqlLibrary.Repository;
using MysqlLibrary.Repository.Print;

namespace BlogGenerator.Services
{

    public class CosineEntityMapper
    {
        private MysqlBlogRepository blogRepository;
        private MysqlCosineRepository cosineRepository;
        private TextVectorCalculator textVectorCalculator;
        private Logger logger;
        public CosineEntityMapper(MysqlBlogRepository blogRepository, MysqlCosineRepository cosineRepository,TextVectorCalculator textVectorCalculator, Logger logger)
        {
            this.blogRepository = blogRepository;
            this.cosineRepository = cosineRepository;
            this.textVectorCalculator = textVectorCalculator;
            this.logger = logger;
        }
        public async Task Calc()
        {
            var BlogEntities = await blogRepository.GetPublishedAsync();
            var CosineEntities = new List<CosineEntity>();

            var Corpus = BlogEntities.Select(x => textVectorCalculator.DivideWords(x.Content)).ToList();
            var TitleVectors = BlogEntities.ToDictionary(x => x.PageId, x => textVectorCalculator.CalcTextVector2(x.Title, Corpus));
            var TextVectors = BlogEntities.ToDictionary(x => x.PageId, x => textVectorCalculator.CalcTextVector2(x.Content, Corpus));

            for (int i = 0; i < BlogEntities.Count; i++)
            {
                var baseEntity = BlogEntities[i];

                for (int j = i + 1; j < BlogEntities.Count; j++) // 計算は1回だけ
                {
                    var targetEntity = BlogEntities[j];
                    var titleSimilarity = textVectorCalculator.CalculateCosineSimilarity(TitleVectors[baseEntity.PageId], TitleVectors[targetEntity.PageId]);
                    var textSimilarity = textVectorCalculator.CalculateCosineSimilarity(TextVectors[baseEntity.PageId], TextVectors[targetEntity.PageId]);

                    // A-B エントリを保存
                    CosineEntities.Add(new CosineEntity
                    {
                        BaseId = baseEntity.PageId,
                        TargetId = targetEntity.PageId,
                        BaseTitle = baseEntity.Title,
                        TargetTitle = targetEntity.Title,
                        TitleSimilarity = titleSimilarity,
                        TextSimilarity = textSimilarity,
                        Score = (titleSimilarity + textSimilarity) / 2,
                        LastUpdated = DateTime.Now
                    });

                    // B-A エントリを保存
                    CosineEntities.Add(new CosineEntity
                    {
                        BaseId = targetEntity.PageId,
                        TargetId = baseEntity.PageId,
                        BaseTitle = targetEntity.Title,
                        TargetTitle = baseEntity.Title,
                        TitleSimilarity = titleSimilarity,  // 同じ類似度を使う
                        TextSimilarity = textSimilarity,    // 同じ類似度を使う
                        Score = (titleSimilarity + textSimilarity) / 2,
                        LastUpdated = DateTime.Now
                    }); 
                }
            }
            await cosineRepository.UpsertCosineListAsync(CosineEntities);
        }
    }
}

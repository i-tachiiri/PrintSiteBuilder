using BlogDomain.Entity;
using MathDomain.Entity;
using Microsoft.EntityFrameworkCore;
using MysqlLibrary.Config;

namespace MysqlLibrary.Repository
{
    public class MysqlBlogRepository
    {
        private readonly PrintDbContext _context;

        public MysqlBlogRepository(PrintDbContext context)
        {
            _context = context;
        }
        // 全件のブログ記事を取得する
        public async Task<List<BlogEntity>> GetAllAsync()
        {
            return await _context.Blog.ToListAsync();
        }
        public async Task<List<string>> GetLatestPostIdsAsync()
        {
            return await _context.Blog
                .FromSqlRaw(@"SELECT PageId FROM printsitebuilder.m_blog order by CreatedTime desc LIMIT 10")
                .Select(x => x.PageId)
                .ToListAsync();
        }
        // PageIdで1件のブログ記事を取得する
        public async Task<BlogEntity> GetByPageIdAsync(string pageId)
        {
            return await _context.Blog.FirstOrDefaultAsync(b => b.PageId == pageId);
        }

        // 複数のPageIdによるブログ記事を取得する
        public async Task<List<BlogEntity>> GetByPageIdsAsync(List<string> pageIds)
        {
            return await _context.Blog.Where(b => pageIds.Contains(b.PageId)).ToListAsync();
        }
        // LastUrlが存在する（nullでない）ブログ記事を取得する
        public async Task<List<BlogEntity>> GetHasLastUrlAsync()
        {
            return await _context.Blog
                .Where(b => !string.IsNullOrEmpty(b.LastUrl))
                .ToListAsync();
        }
        public async Task<List<BlogEntity>> GetHasDirectoryAsync()
        {
            return await _context.Blog
                .Where(b => !string.IsNullOrEmpty(b.Directory))
                .ToListAsync();
        }
        public async Task<List<BlogEntity>> GetPublishedAsync()
        {
            return await _context.Blog
                .Where(b => b.Status == "公開中")
                .ToListAsync();
        }
        // Directory列でブログ記事を検索する（複数件取得）
        public async Task<List<BlogEntity>> GetByDirectoryAsync(string directory)
        {
            return await _context.Blog.Where(b => b.Directory == directory).ToListAsync();
        }

        // ブログ記事をアップデートする（PageIdで検索し、それを基に更新する）
        public async Task UpdateBlogAsync(BlogEntity entity)
        {
            var existingEntity = await _context.Blog.FirstOrDefaultAsync(b => b.PageId == entity.PageId);

            if (existingEntity != null)
            {
                _context.Entry(entity).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
        }

        // PageIdが存在すればアップデート、存在しなければ作成する（アップサート機能）
        public async Task UpsertMathListAsync(List<sQuestionEntity> entities)
        {
            foreach (var entity in entities)
            {
                var existingEntity = await _context.sQuestion.FirstOrDefaultAsync(b => b.PageId == entity.PageId);

                if (existingEntity == null)
                {
                    await _context.sQuestion.AddAsync(entity);
                }
                else
                {
                    entity.Id = existingEntity.Id;
                    _context.Entry(existingEntity).CurrentValues.SetValues(entity);

                }
            }
            if (_context.ChangeTracker.HasChanges())
            {
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpsertBlogListAsync(List<BlogEntity> entities)
        {
            foreach (var entity in entities)
            {
                var existingEntity = await _context.Blog.FirstOrDefaultAsync(b => b.PageId == entity.PageId);

                if (existingEntity == null)
                {
                    await _context.Blog.AddAsync(entity);
                }
                else
                {
                    entity.Id = existingEntity.Id;
                    _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                }
            }
            if (_context.ChangeTracker.HasChanges())
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}

using ImageMagick;
using MathDomain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySqlConnector;
using MysqlLibrary.Config;

namespace MysqlLibrary.Repository.Print
{
    public class MysqlSQuestionRepository
    {
        private readonly PrintDbContext _context;
        private string tableName = "s_question";


        public MysqlSQuestionRepository(PrintDbContext context)
        {
            _context = context;
        }
        public async Task<List<sQuestionEntity>> GetAllAsync()
        {
            return await _context.sQuestion.ToListAsync();
        }
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
        public async Task DeleteByPageAsync(string PageId)
        {
            var sql = $"DELETE FROM {tableName} WHERE {nameof(PageId)} = @{nameof(PageId)}";
            var parameters = new[]
            {
                new MySqlParameter("@PageId", PageId),
            };
            await ExecuteSql(sql, parameters);
        }
        public async Task DeleteByPagesAsync(List<string> PageIds)
        {
            if (PageIds == null || !PageIds.Any()) return;
            var parameterNames = PageIds.Select((id, index) => $"@PageId{index}").ToArray();
            var sql = $"DELETE FROM {tableName} WHERE PageId IN ({string.Join(", ", parameterNames)})";
            var parameters = PageIds.Select((id, index) => new MySqlParameter($"@PageId{index}", id)).ToArray();
            await ExecuteSql(sql, parameters);
        }

        public async Task DeleteByPageAndQuestionAsync(string PageId, string QuestionId)
        {
            var sql = $"DELETE FROM {tableName} WHERE {nameof(PageId)} = @{nameof(PageId)} and {nameof(QuestionId)} = @{nameof(PageId)}";
            var parameters = new[]
            {
                    new MySqlParameter("@PageId", PageId),
                    new MySqlParameter("@QuestionId", QuestionId)
                };
            await ExecuteSql(sql, parameters);

        }
        public async Task InsertEntityAsync(List<sQuestionEntity> entities)
        {
            var properties = typeof(sQuestionEntity).GetProperties().Where(p => p.GetValue(entities[0]) != null).ToList();
            var columnNames = properties.Select(p => p.Name).ToList();
            var sql = $"INSERT INTO {tableName} ({string.Join(", ", columnNames)}) VALUES";
            var parameters = new List<MySqlParameter>();
            foreach (var entity in entities)
            {
                var ColumnId = Guid.NewGuid().ToString().Replace("-", "");
                var parameterNames = properties.Select(p => $"@{p.Name}{ColumnId}").ToList();
                sql += $"({string.Join(", ", parameterNames)}),";
                parameters.AddRange(properties.Select(p => new MySqlParameter($"@{p.Name}{ColumnId}", p.GetValue(entity) ?? DBNull.Value)).ToList());
            }
            sql = sql.TrimEnd(',') + ";";
            await ExecuteSql(sql, parameters.ToArray());
        }
        public async Task ExecuteSql(string sql, MySqlParameter[] parameters)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Database.ExecuteSqlRawAsync(sql, parameters);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"エラー: {ex.Message}");
            }
        }
    }
}

using ImageMagick;
using MathDomain.Entity;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using MysqlLibrary.Config;
using System.Text;


namespace MysqlLibrary.Repository
{
    public class MysqlQuestionRepository
    {
        private readonly PrintDbContext _context;
        private string tableName = "m_question";

        public MysqlQuestionRepository(PrintDbContext context)
        {
            _context = context;
        }
        public async Task<List<mQuestionEntity>> GetAllAsync()
        {
            return await _context.Question.OrderBy(x => x.QuestionId).ThenBy(x => x.ValueId).ToListAsync();
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
        public async Task DeleteByPageAndQuestionAsync(string PageId,string QuestionId)
        {
            var sql = $"DELETE FROM {tableName} WHERE {nameof(PageId)} = @{nameof(PageId)} and {nameof(QuestionId)} = @{nameof(PageId)}";
            var parameters = new[]
            {
                    new MySqlParameter("@PageId", PageId),
                    new MySqlParameter("@QuestionId", QuestionId)
                }; 
            await ExecuteSql(sql,parameters);

        }
        public async Task InsertEntityAsync(List<mQuestionEntity> entities)
        {
            if (!entities.Any()) return;

            var properties = typeof(mQuestionEntity).GetProperties().Where(p => p.GetValue(entities[0]) != null).ToList();
            var columnNames = properties.Select(p => p.Name).ToList();

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"INSERT INTO {tableName} ({string.Join(", ", columnNames)}) VALUES ");
            var parameters = new List<MySqlParameter>();

            foreach (var entity in entities)
            {
                var columnId = Guid.NewGuid().ToString("N"); 
                var parameterNames = properties.Select(p => $"@{p.Name}{columnId}").ToList();
                sqlBuilder.Append($"({string.Join(", ", parameterNames)}),");

                parameters.AddRange(properties.Select(p =>
                    new MySqlParameter($"@{p.Name}{columnId}", p.GetValue(entity) ?? DBNull.Value)
                ));
            }

            // 最後のカンマを削除してSQL文を完成
            sqlBuilder.Length--;
            sqlBuilder.Append(";");
            await ExecuteSql(sqlBuilder.ToString(), parameters.ToArray());
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

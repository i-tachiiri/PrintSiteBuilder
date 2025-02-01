using MathDomain.Entity;
using MathDomain.Services;
using MysqlLibrary.Repository;
using NotionLibrary.Repository;
using NotionLibrary.Services;

namespace MathGenerator.Services
{
    public class MathEntityMapper
    {
        private NotionMathRepository notionMathRepostory;
        private NotionDatabaseConverter databaseConverter;
        private MysqlMathRepository mysqlMathRepository;
        private MathLogger logger;
        public MathEntityMapper(NotionMathRepository notionMathRepostory, NotionDatabaseConverter databaseConveter, MysqlMathRepository mysqlMathRepository, MathLogger logger)
        {
            this.databaseConverter = databaseConveter;
            this.notionMathRepostory = notionMathRepostory;
            this.mysqlMathRepository = mysqlMathRepository;
            this.logger = logger;
        }
        public async Task UpsertMathEntitiesAsync()
        {
            var results = await notionMathRepostory.GetAllAsync();
            var Entities = new List<mMathEntity>();
            foreach (var record in results.EnumerateArray())
            {
                var entity = new mMathEntity();
                entity.PrintName = databaseConverter.GetTitleValue(record, "print_name");
                entity.PageId = databaseConverter.GetId(record);
                entity.ClassName = databaseConverter.GetSelectValue(record, "class");
                //entity.FunctionName = databaseConverter.GetSelectValue(record, "function");
                entity.Prefix = databaseConverter.GetRichTextValue(record, "prefix");
                entity.HasFixedValue = databaseConverter.GetSelectValue(record, "has_fixed_value") == "固定値";
                entity.IsOrdered = databaseConverter.GetSelectValue(record, "is_ordered") == "不可逆";
                entity.HasCarry = HasCarry(databaseConverter.GetSelectValue(record, "process_type"));
                entity.HasBorrow = HasBorrow(databaseConverter.GetSelectValue(record, "process_type"));
                entity.MaxValueI = databaseConverter.GetIntValue(record, "max_value_i");
                entity.MinValueI = databaseConverter.GetIntValue(record, "min_value_i");
                entity.MaxValueJ = databaseConverter.GetIntValue(record, "max_value_j");
                entity.MinValueJ = databaseConverter.GetIntValue(record, "min_value_j");
                entity.FixedValue = databaseConverter.GetIntValue(record, "fixed_value");
                entity.MaxAnswer = databaseConverter.GetIntValue(record, "max_answer");
                entity.MinAnswer = databaseConverter.GetIntValue(record, "min_answer");
                Entities.Add(entity);
            }
            await mysqlMathRepository.UpsertMathListAsync(Entities);
        }
        private bool? HasCarry(string ProcessType)
        {
            switch(ProcessType)
            {
                case "HasCarry":
                    return true;
                case "NoCarry":
                    return false;
                default: 
                    return null;
            }                
        }
        private bool? HasBorrow(string ProcessType)
        {
            switch (ProcessType)
            {
                case "HasBorrow":
                    return true;
                case "NoBorrow":
                    return false;
                default:
                    return null;
            }
        }
    }
}

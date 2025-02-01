using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
namespace MathDomain.Entity
{
#if DEBUG
    [Table("m_math")]
#elif RELEASE
    [Table("m_math")]
#endif
    [PrimaryKey(nameof(Id))]
    public class mMathEntity
    {
        public int Id { get; set; }
        public string PrintName { get; set; }
        public string PageId { get; set; }
        public string ClassName { get; set; }
        //public string FunctionName { get; set; }
        public string Prefix { get; set; }
        public bool HasFixedValue { get; set; }
        public bool IsOrdered { get; set; }
        public bool? HasCarry { get; set; }
        public bool? HasBorrow { get; set; }
        public int MaxValueI { get; set; }
        public int MinValueI { get; set; }
        public int MaxValueJ { get; set; }
        public int MinValueJ { get; set; }
        public int FixedValue { get; set; }
        public int MaxAnswer { get; set; }
        public int MinAnswer { get; set; }
        public bool IsOutOfRange(int x)
        {
            return x >= MaxAnswer || x <= MinAnswer;
        }



    }

}

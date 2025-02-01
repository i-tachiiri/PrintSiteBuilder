

namespace MathGenerator.DataTransferObject
{
    public class AdditionWithFixedValue
    {
        public string PageId { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }
        public int FixedValue { get; set; }
        public int MaxAnswer { get; set; }
        public int MinAnswer { get; set; }
    }
}

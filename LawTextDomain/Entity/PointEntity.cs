

namespace LawTextDomain.Entity
{
    public class PointEntity
    {
        public string LawName { get; set; }
        public string Caption { get; set; } // （〇〇）
        public string LawId { get; set; } // 第〇条など
        public string TextType { get; set; } // 第〇条など
        public string Content { get; set; } // テキスト内容
    }

}

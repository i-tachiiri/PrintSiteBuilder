using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathGenerator.DataTransferObject
{
    public class QuestionObject
    {
        public string Value {  get; set; }
        public string Context { get; set; }
        public int ValueId { get; set; }
        public bool HasNext { get; set; }
        public bool IsQuestion {  get; set; }

    }
}

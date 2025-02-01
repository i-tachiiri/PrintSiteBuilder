using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder.Models.Print
{
    public class CellConfig
    {
        public int RowNumber { get;  }
        public int ColumnNumber { get;  }
        public string Value { get; }
        public List<int> AnswerColumn { get; }
        public List<int> AnswerRow { get; }
        public CellConfig(int rowNumber,int columnNumber,string value,List<int> answerColumn)
        {
            RowNumber = rowNumber;
            ColumnNumber = columnNumber;
            Value = value;    
            AnswerColumn = answerColumn;
        }
        public CellConfig(int rowNumber, int columnNumber, string value, List<int> answerColumn, List<int> answerRow)
        {
            RowNumber = rowNumber;
            ColumnNumber = columnNumber;
            Value = value;
            AnswerColumn = answerColumn;
            AnswerRow = answerRow;
        }
    }
}

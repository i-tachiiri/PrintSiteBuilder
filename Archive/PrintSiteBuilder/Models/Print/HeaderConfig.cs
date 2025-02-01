using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder.Models.Print
{
    public class HeaderConfig
    {
        public string PrintName {  get;  }
        public string PrintType { get;  }
        public int PrintNumber { get;  }
        public int Score { get;  }
        public int PageIndex { get;  }
        public HeaderConfig(string printType, string printName, int printNumber,int score,int pageIndex)
        {
            PrintType = printType;
            PrintName = printName;
            PrintNumber = printNumber;
            Score = score;
            PageIndex = pageIndex;
        }
            
    }
}

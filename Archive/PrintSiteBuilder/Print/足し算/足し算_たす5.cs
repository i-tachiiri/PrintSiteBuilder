using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintSiteBuilder.Interfaces;
using PrintSiteBuilder.Models.Print;
using PrintSiteBuilder.Utilities;

namespace PrintSiteBuilder.Print.足し算
{
    public class 足し算_たす5 : IPrint
    {
        private List<int> answerIndex = new List<int> { 4, 10 };
        public int PagesCount { get; private set; }
        private int Score = 10;
        private string PrintName = "足し算(たす5)";
        public string PresentationId { get; private set; }
        public 足し算_たす5()
        {
            PagesCount = 100;
            PresentationId = "1PRZjOQ_p7TKrWqR_fhu0rlymouOfw9S1wnLfdOofvkU";
        }

        public List<PrintConfig> GetPrintConfigs()
        {
            var headerConfigs = GetHeaderConfigs();
            var printConfigs = new List<PrintConfig>();
            foreach (var headerConfigGroup in headerConfigs.GroupBy(item => item.PrintNumber))   //プリント番号別にグループ化して問題と回答をセットに
            {
                var cellConfigs = GetCellConfigs(headerConfigGroup.First().PrintNumber);
                foreach (var headerConfig in headerConfigGroup)
                {
                    var printConfig = new PrintConfig(headerConfig, cellConfigs);
                    printConfigs.Add(printConfig);
                }
            }
            return printConfigs;
        }
        public List<HeaderConfig> GetHeaderConfigs()
        {
            var headerConfigs = new List<HeaderConfig>();

            for (var i = 0; i < PagesCount; i++)
            {
                // Question Config
                var questionPrintType = "問題";
                var questionPrintName = $"{PrintName}-{(i + 1).ToString("D3")}";
                var questionPrintNumber = i;
                var questionScore = Score;
                var questionPageIndex = i;
                HeaderConfig questionConfig = new HeaderConfig(questionPrintType, questionPrintName, questionPrintNumber, questionScore, questionPageIndex);
                headerConfigs.Add(questionConfig);

                // Answer Config
                var answerPrintType = "回答";
                var answerPrintName = $"{PrintName}-{(i + 1).ToString("D3")}";
                var answerPrintNumber = i;
                var answerScore = Score;
                var answerPageIndex = i + PagesCount;
                HeaderConfig answerConfig = new HeaderConfig(answerPrintType, answerPrintName, answerPrintNumber, answerScore, answerPageIndex);
                headerConfigs.Add(answerConfig);
            }

            return headerConfigs;
        }

        public List<CellConfig> GetCellConfigs(int PrintNumber)
        {
            var Coordinates = new List<List<List<int>>>
            {
                new List<List<int>>{new List<int>{0,0},new List<int>{0,1},new List<int>{0,2},new List<int>{0,3},new List<int>{0,4} },
                new List<List<int>>{new List<int>{1,0},new List<int>{1,1},new List<int>{1,2},new List<int>{1,3},new List<int>{1,4}},
                new List<List<int>>{new List<int>{2,0},new List<int>{2,1},new List<int>{2,2},new List<int>{2,3},new List<int>{2,4}},
                new List<List<int>>{new List<int>{3,0},new List<int>{3,1},new List<int>{3,2},new List<int>{3,3},new List<int>{3,4}},
                new List<List<int>>{new List<int>{4,0},new List<int>{4,1},new List<int>{4,2},new List<int>{4,3},new List<int>{4,4}},
                new List<List<int>>{new List<int>{0,6},new List<int>{0,7},new List<int>{0,8},new List<int>{0,9},new List<int>{0,10}},
                new List<List<int>>{new List<int>{1,6},new List<int>{1,7},new List<int>{1,8},new List<int>{1,9},new List<int>{1,10}},
                new List<List<int>>{new List<int>{2,6},new List<int>{2,7},new List<int>{2,8},new List<int>{2,9},new List<int>{2,10}},
                new List<List<int>>{new List<int>{3,6},new List<int>{3,7},new List<int>{3,8},new List<int>{3,9},new List<int>{3,10}},
                new List<List<int>>{new List<int>{4,6},new List<int>{4,7},new List<int>{4,8},new List<int>{4,9},new List<int>{4,10} }
            };
            List<List<string>> pairs = new List<List<string>>();  //Coordinatesより多い前提
            var maxNumber = PrintNumber < 10 ? 10 : PrintNumber;
            for (int i = 1; i <= maxNumber; i++)
            {
                var j = 5;
                List<string> pair = new List<string> { i.ToString(), "+", j.ToString(), "=", (i + j).ToString() };
                pairs.Add(pair);
            }
            var random = new Random();
            var randomNumbers = new HashSet<int>();
            while (randomNumbers.Count < Coordinates.Count)  //10問以上じゃないと無限ループ
            {
                randomNumbers.Add(random.Next(pairs.Count));
            }
            var values = new List<List<string>>();
            foreach (var index in randomNumbers)
            {
                values.Add(pairs[index]);
            }
            var CellConfigs = new List<CellConfig>();
            for (var i = 0; i < values.Count(); i++)
            {
                for (var j = 0; j < values[i].Count(); j++)
                {
                    var RowNumber = Coordinates[i][j][0];
                    var ColumnNumber = Coordinates[i][j][1];
                    var Value = values[i][j];
                    var answerColumn = answerIndex;
                    var cellConfig = new CellConfig(RowNumber, ColumnNumber, Value, answerColumn);
                    CellConfigs.Add(cellConfig);
                }
            }
            return CellConfigs;
        }
    }
}

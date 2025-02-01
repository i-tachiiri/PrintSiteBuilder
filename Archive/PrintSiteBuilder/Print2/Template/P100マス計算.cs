using ImageMagick;
using PrintSiteBuilder.Interfaces;
using PrintSiteBuilder.Models.Print;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintSiteBuilder.SiteItem;
using PrintSiteBuilder.GoogleService.Drive;
using PrintSiteBuilder.Models.General;
using iTextSharp.awt.geom;

namespace PrintSiteBuilder.Print2.Template
{
    public class P100マス計算 : IPrintType
    {
        public IPrint2 iPrint { get; private set; }
        public int Score { get; private set; }
        public string SkuHeader { get; private set; }

        public P100マス計算(IPrint2 iPrint)
        {
            this.iPrint = iPrint;
            Score = 100;
            SkuHeader = "PM";
        }
        private List<int> answerColumnIndex = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private List<int> answerRowIndex = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private List<List<List<int>>> Coordinates = new List<List<List<int>>>
            {
                new List<List<int>>{ new List<int> { 0, 0 }, new List<int>{0,1},new List<int>{0,2},new List<int>{0,3},new List<int>{0,4},new List<int>{0,5},new List<int>{0,6},new List<int>{0,7},new List<int>{0,8},new List<int>{0,9},new List<int>{0,10},new List<int>{0,11} },
                new List<List<int>>{ new List<int> { 1, 0 }, new List<int>{1,1},new List<int>{1,2},new List<int>{1,3},new List<int>{1,4},new List<int>{1,5},new List<int>{1,6},new List<int>{1,7},new List<int>{1,8},new List<int>{1,9},new List<int>{1,10} ,new List<int>{1,11}},
                new List<List<int>>{ new List<int> { 2, 0 }, new List<int>{2,1},new List<int>{2,2},new List<int>{2,3},new List<int>{2,4},new List<int>{2,5},new List<int>{2,6},new List<int>{2,7},new List<int>{2,8},new List<int>{2,9},new List<int>{2,10} ,new List<int>{2,11}},
                new List<List<int>>{ new List<int> { 3, 0 }, new List<int>{3,1},new List<int>{3,2},new List<int>{3,3},new List<int>{3,4},new List<int>{3,5},new List<int>{3,6},new List<int>{3,7},new List<int>{3,8},new List<int>{3,9},new List<int>{3,10} ,new List<int>{3,11}},
                new List<List<int>>{ new List<int> { 4, 0 }, new List<int>{4,1},new List<int>{4,2},new List<int>{4,3},new List<int>{4,4},new List<int>{4,5},new List<int>{4,6},new List<int>{4,7},new List<int>{4,8},new List<int>{4,9},new List<int>{4,10},new List<int>{4,11} },
                new List<List<int>>{ new List<int> { 5, 0 }, new List<int>{5,1},new List<int>{5,2},new List<int>{5,3},new List<int>{5,4},new List<int>{5,5},new List<int>{5,6},new List<int>{5,7},new List<int>{5,8},new List<int>{5,9},new List<int>{5,10} ,new List<int>{5,11}},
                new List<List<int>>{ new List<int> { 6, 0 }, new List<int>{6,1},new List<int>{6,2},new List<int>{6,3},new List<int>{6,4},new List<int>{6,5},new List<int>{6,6},new List<int>{6,7},new List<int>{6,8},new List<int>{6,9},new List<int>{6,10} ,new List<int>{6,11}},
                new List<List<int>>{ new List<int> { 7, 0 }, new List<int>{7,1},new List<int>{7,2},new List<int>{7,3},new List<int>{7,4},new List<int>{7,5},new List<int>{7,6},new List<int>{7,7},new List<int>{7,8},new List<int>{7,9},new List<int>{7,10} ,new List<int>{7,11}},
                new List<List<int>>{ new List<int> { 8, 0 }, new List<int>{8,1},new List<int>{8,2},new List<int>{8,3},new List<int>{8,4},new List<int>{8,5},new List<int>{8,6},new List<int>{8,7},new List<int>{8,8},new List<int>{8,9},new List<int>{8,10} ,new List<int>{8,11}},
                new List<List<int>>{ new List<int> { 9, 0 }, new List<int>{9,1},new List<int>{9,2},new List<int>{9,3},new List<int>{9,4},new List<int>{9,5},new List<int>{9,6},new List<int>{9,7},new List<int>{9,8},new List<int>{9,9},new List<int>{9,10},new List<int>{9,11} },
                new List<List<int>>{ new List<int> { 10, 0 }, new List<int>{10,1},new List<int>{10,2},new List<int>{10,3},new List<int>{10,4},new List<int>{10,5},new List<int>{10,6},new List<int>{10,7},new List<int>{10,8},new List<int>{10,9},new List<int>{10,10},new List<int>{10,11} },

            };
        public List<PrintConfig2> GetPrintConfigs()
        {
            var headerConfigs = GetHeaderConfigs();
            var printConfigs = new List<PrintConfig2>();
            foreach (var headerConfigGroup in headerConfigs.GroupBy(item => item.PrintNumber))   //プリント番号別にグループ化して問題と回答をセットに
            {
                var cellConfigs = GetCellConfigs();
                foreach (var headerConfig in headerConfigGroup)
                {
                    var printConfig = new PrintConfig2(headerConfig, cellConfigs, iPrint.PrintId);
                    printConfigs.Add(printConfig);
                }
            }
            return printConfigs;
        }
        public List<HeaderConfig> GetHeaderConfigs()
        {
            var headerConfigs = new List<HeaderConfig>();

            for (var i = 0; i < iPrint.PagesCount; i++)
            {
                // Question Config
                var questionPrintType = "問題";
                var questionPrintName = $"{iPrint.PrintName.Replace("のプリント","")}-{(i + 1).ToString("D3")}";
                var questionPrintNumber = i + 1;
                var questionScore = iPrint.PrintType.Score;
                var questionPageIndex = i;
                HeaderConfig questionConfig = new HeaderConfig(questionPrintType, questionPrintName, questionPrintNumber, questionScore, questionPageIndex);
                headerConfigs.Add(questionConfig);

                // Answer Config
                var answerPrintType = "回答";
                var answerPrintName = $"{iPrint.PrintName.Replace("のプリント", "")}-{(i + 1).ToString("D3")}";
                var answerPrintNumber = i + 1;
                var answerScore = iPrint.PrintType.Score;
                var answerPageIndex = i + iPrint.PagesCount;
                HeaderConfig answerConfig = new HeaderConfig(answerPrintType, answerPrintName, answerPrintNumber, answerScore, answerPageIndex);
                headerConfigs.Add(answerConfig);
            }

            return headerConfigs;
        }
        public List<CellConfig> GetCellConfigs()
        {
            var CellConfigs = new List<CellConfig>();
            var Questions = GetQaPairs();
            //if (Questions.Count < Score) Console.WriteLine("問題数が足りません。");
            for (var i = 0; i < Coordinates.Count(); i++)
            {
                for (var j = 0; j < Coordinates[i].Count(); j++)
                {
                    var RowNumber = Coordinates[i][j][0];
                    var ColumnNumber = Coordinates[i][j][1];
                    var Value = Questions[i][j];
                    var answerColumn = answerColumnIndex;
                    var answerRow = answerRowIndex;
                    var cellConfig = new CellConfig(RowNumber, ColumnNumber, Value, answerColumn, answerRow);
                    CellConfigs.Add(cellConfig);
                }
            }
            return CellConfigs;
        }
        public List<List<string>> GetQaPairs()
        {

            var QuestionsList = iPrint.GetQuestionLists();
            var Questions = new List<List<string>>();
            foreach (var question in QuestionsList)
            {
                Questions = Questions.Concat(question).ToList();
            }
            var random = new Random();
            return Questions;//.OrderBy(x => random.Next()).ToList(); ;//GetRandomPairs(Questions);
        }
    }
}

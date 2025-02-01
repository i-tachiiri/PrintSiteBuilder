using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PrintSiteBuilder.Interfaces;
using PrintSiteBuilder.Models.Print;
using PrintSiteBuilder.Utilities;

namespace PrintSiteBuilder.Print.数の性質
{
    public class 約数 : IPrint
    {
        private List<int> answerIndex = new List<int> { 7,8,9,10,11 };
        List<List<List<int>>> Coordinates = new List<List<List<int>>>
            {
                new List<List<int>>{new List<int>{0,0},new List<int>{3,7} },
                new List<List<int>>{new List<int>{4,0},new List<int>{7,7}},
                new List<List<int>>{new List<int>{8,0},new List<int>{11,7}},
            };
        public int PagesCount { get; private set; }
        private int Score = 3;
        private string PrintName = "約数";
        public string PresentationId { get; private set; }
        private Calcs calcs = new Calcs();
        public 約数()
        {
            PresentationId = "1MIWQY8cM56d1fHwfkM2ynoLU08f06MQ_Z567bhgzDLY";
            PagesCount = 100;
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
            List<List<string>> InputPairs = new List<List<string>>();  //Coordinatesより多い前提
            var Primes = new List<int> { 23, 37, 61 };  //素数はいくつか混ぜる
            for(var i=10;i<=200;i++)
            {
                if (calcs.IsPrime(i) && !Primes.Contains(i)) continue;
                if (i > 100 && i % 10 != 0) continue;
                var divisor = calcs.Get約数(i);
                InputPairs.Add(new List<string> { $"{i}の約数をすべて答えてください。", string.Join(",",divisor) });
                InputPairs.Add(new List<string> { $"{i}を割ってあまりの出ない整数をすべて答えてください。", string.Join(",", divisor) });
                InputPairs.Add(new List<string> { $"{i}の約数は何個ですか？", $"{divisor.Count().ToString()}個" });
            }
            var random = new Random();
            var randomNumbers = new HashSet<int>();
            while (randomNumbers.Count < Coordinates.Count) 
            {
                randomNumbers.Add(random.Next(InputPairs.Count));
            }
            var values = new List<List<string>>();
            foreach (var index in randomNumbers)
            {
                values.Add(InputPairs[index]);
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

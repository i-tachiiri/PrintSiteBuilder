using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using PrintSiteBuilder.Interfaces;
using PrintSiteBuilder.Models.Print;
using PrintSiteBuilder.Utilities;

namespace PrintSiteBuilder.Print.数の性質
{
    public class 公約数_最大公約数 : IPrint
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
        private string PrintName = "公約数・最大公約数";
        public string PresentationId { get; private set; }
        private Calcs calcs = new Calcs();
        public 公約数_最大公約数()
        {
            PresentationId = "10Qr-mwc8aF6xX3ZfX0OTlxFv36ZM3Cw9voK5H_cjf5Q";
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
            for (var i = 4; i <= 120; i++)
            {
                for (var j = 4; j < i; j++)
                {
                    var 公約数 = calcs.Get公約数(i, j);
                    var 最大公約数 = calcs.Get最大公約数(i, j);
                    if (calcs.IsPrime(i) || calcs.IsPrime(j)) continue;
                    if (公約数.Count < 3) continue;
                    if (最大公約数 == 1 || 最大公約数 == i || 最大公約数 == j) continue;
                    InputPairs.Add(new List<string> { $"({i},{j})の公約数をすべて答えてください。", string.Join(",", 公約数) });
                    InputPairs.Add(new List<string> { $"({i},{j})の最大公約数を答えてください。", 最大公約数.ToString() });
                    InputPairs.Add(new List<string> { $"{i}と{j}の両方を割り切れる数をすべて答えてください。", string.Join(",", 公約数) });
                    InputPairs.Add(new List<string> { $"{i}と{j}の両方を割り切れる数のうち、最も大きな数を答えてください。", 最大公約数.ToString() });
                    var 二桁の公約数 = 公約数.Where(num => num >= 10);
                    if (二桁の公約数.Count() >= 2)
                    {
                        InputPairs.Add(new List<string> { $"{i}と{j}の両方を割り切れる2桁の数をすべて答えてください。", string.Join(",", 二桁の公約数) });
                    }
                    for (var k = 4; k < j; k++)
                    {
                        var 三つの公約数 = calcs.Get公約数(i, j, k);
                        var 三つの最大公約数 = calcs.Get最大公約数(i, j, k);
                        if (calcs.IsPrime(i) || calcs.IsPrime(j) || calcs.IsPrime(k)) continue;
                        if (三つの公約数.Count < 3) continue;
                        if (三つの最大公約数 == 1 || 三つの最大公約数 == i || 三つの最大公約数 == j || 三つの最大公約数 == k) continue;
                        InputPairs.Add(new List<string> { $"({i},{j},{k})の公約数をすべて答えてください。", string.Join(",", 三つの公約数) });
                        InputPairs.Add(new List<string> { $"({i},{j},{k})の最大公約数を答えてください。", 三つの公約数.Max().ToString() });

                    }

                    for (int a = 2; a <= 9; a++)
                    {
                        var 余りの公約数 = new List<int>();
                        for (int x = a + 1; x <= Math.Min(i, j); x++)
                        {
                            if (i % x == a && j % x == a)
                            {
                                余りの公約数.Add(x);
                            }
                        }
                        if (余りの公約数.Count < 3) continue;
                        InputPairs.Add(new List<string> { $"{i}と{j}のどちらを割っても{a}あまる数をすべて答えてください。", string.Join(",", 余りの公約数) });
                        InputPairs.Add(new List<string> { $"{i}と{j}のどちらを割っても{a}あまる数のうち、最大の数を答えてください。", 余りの公約数.Max().ToString() });
                        InputPairs.Add(new List<string> { $"{i}と{j}のどちらを割っても{a}あまる数のうち、最小の数を答えてください。", 余りの公約数.Min().ToString() });
                    }
                }
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

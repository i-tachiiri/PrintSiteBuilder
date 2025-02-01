using PrintSiteBuilder.Interfaces;
using PrintSiteBuilder.Models.Print;
using PrintSiteBuilder.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder.Print.割り算
{
    public class 三つの分数の割り算 : IPrint
    {
        private string PrintName;
        private List<int> answerIndex = new List<int> { 10 };
        private List<List<List<int>>> Coordinates = new List<List<List<int>>>
            {
                new List<List<int>>{new List<int>{0,1},new List<int>{1,1},new List<int>{1,2},new List<int>{0,3},new List<int>{1,3},new List<int>{0,4},new List<int>{0,5},new List<int>{1,5}, new List<int> { 4, 9 }, new List<int>{4,10},new List<int>{5,10} },
                new List<List<int>>{new List<int>{6,1},new List<int>{7,1},new List<int>{7,2},new List<int>{6,3},new List<int>{7,3},new List<int>{6,4},new List<int>{6,5},new List<int>{7,5}, new List<int> { 10, 9 }, new List<int>{10,10},new List<int>{11,10} },
                new List<List<int>>{new List<int>{12,1},new List<int>{13,1},new List<int>{13,2},new List<int>{12,3},new List<int>{13,3},new List<int>{12,4},new List<int>{12,5},new List<int>{13,5}, new List<int> { 16, 9 }, new List<int>{16,10},new List<int>{17,10} },
                new List<List<int>>{new List<int>{18,1},new List<int>{19,1},new List<int>{19,2},new List<int>{18,3},new List<int>{19,3},new List<int>{18,4},new List<int>{18,5},new List<int>{19,5}, new List<int> { 22, 9 }, new List<int>{22,10},new List<int>{23,10} },
                new List<List<int>>{new List<int>{24,1},new List<int>{25,1},new List<int>{25,2},new List<int>{24,3},new List<int>{25,3},new List<int>{24,4},new List<int>{24,5},new List<int>{25,5},new List<int> { 28, 9 }, new List<int>{28,10},new List<int>{29,10} },
            };
        private Calcs calcs = new Calcs();
        public int PagesCount { get; private set; }
        public string PresentationId { get; private set; }
        private int Score;
        public List<List<string>> QaPairs;
        public List<List<string>> RandomPairs;
        public 三つの分数の割り算()
        {
            PresentationId = "1HfwPWIFJJ9RHPt0ymhXgYcrighDOx86Pn2yppJwBFfo";
            PrintName = "三つの分数の割り算";
            PagesCount = 100;
            Score = Coordinates.Count;
        }
        public List<PrintConfig> GetPrintConfigs()
        {
            var headerConfigs = GetHeaderConfigs();
            var printConfigs = new List<PrintConfig>();
            foreach (var headerConfigGroup in headerConfigs.GroupBy(item => item.PrintNumber))   //プリント番号別にグループ化して問題と回答をセットに
            {
                var cellConfigs = GetCellConfigs();
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
        public List<CellConfig> GetCellConfigs()
        {
            var CellConfigs = new List<CellConfig>();
            var Questions = GetQaPairs();
            if (Questions.Count < Score) MessageBox.Show("問題数が足りません。");
            for (var i = 0; i < Coordinates.Count(); i++)
            {
                for (var j = 0; j < Coordinates[i].Count(); j++)
                {
                    var RowNumber = Coordinates[i][j][0];
                    var ColumnNumber = Coordinates[i][j][1];
                    var Value = Questions[i][j];
                    var answerColumn = answerIndex;
                    var cellConfig = new CellConfig(RowNumber, ColumnNumber, Value, answerColumn);
                    CellConfigs.Add(cellConfig);
                }
            }
            return CellConfigs;
        }
        public List<List<string>> GetQaPairs()
        {
            var QuestionsList = new List<List<List<string>>>()
            {
                GetQuestions1(),
            };

            var Questions = new List<List<string>>();
            foreach (var question in QuestionsList)
            {
                Questions = Questions.Concat(question).ToList();
            }

            return GetRandomPairs(Questions);
        }
        public List<List<string>> GetRandomPairs(List<List<string>> questions)
        {
            var random = new Random();
            var questionIndex = new HashSet<string>();
            var randomPairs = new List<List<string>>();

            var shuffledQuestions = questions.OrderBy(x => random.Next()).ToList();

            foreach (var question in shuffledQuestions)
            {
                if (questionIndex.Add(question[question.Count-1]))
                {
                    randomPairs.Add(question);
                }
            }
            return randomPairs;
        }
        public List<List<string>> GetQuestions1()
        {
            var questions = new List<List<string>>();
            var AllFractions = new List<List<int>>();
            for(var i=1;i<50;i++)
            {
                for(var j=i+1;j<50;j++)
                {
                    if (calcs.Get最大公約数(i, j) != 1) continue;
                    AllFractions.Add(new List<int> { i, j });
                }
            }
            var random = new Random();
            //var AllFractionPairs = GetAllFractionPairs(AllFractions, 4);
            //この時点では分数の掛け算として処理
            int 約分回数 = 0;
            int 試行回数 = 0;
            int 問題番号 = 0;
            while (試行回数 <1000000 && questions.Count<5)
            {
                約分回数 = 0;
                試行回数++;
                var pair = AllFractions.OrderBy(x => random.Next()).Take(3).ToList();
                var 分子1 = pair[0][0];
                var 分母1 = pair[0][1];
                var 分子2 = pair[1][0];
                var 分母2 = pair[1][1];
                var 分子3 = pair[2][0];
                var 分母3 = pair[2][1];
                if (calcs.Get最大公約数(分子1, 分母2) != 1) 約分回数++;
                if (calcs.Get最大公約数(分子1, 分母3) != 1) 約分回数++;
                if (calcs.Get最大公約数(分子2, 分母1) != 1) 約分回数++;
                if (calcs.Get最大公約数(分子2, 分母3) != 1) 約分回数++;
                if (calcs.Get最大公約数(分子3, 分母1) != 1) 約分回数++;
                if (calcs.Get最大公約数(分子3, 分母2) != 1) 約分回数++;
                if (約分回数 >= 3)
                {
                    var 計算記号 = new List<string> { "÷", "÷", "÷", "✕" };
                    var 計算記号1 = "÷";
                    var 計算記号2 =計算記号[random.Next(計算記号.Count)];
                    var 計算記号3 = 計算記号[random.Next(計算記号.Count)];
                    var 割後分子1 = pair[0][0];
                    var 割後分母1 = pair[0][1];
                    var 割後分子2 = 計算記号1 == "÷" ? 分母2 : 分子2;
                    var 割後分母2 = 計算記号1 == "÷" ? 分子2 : 分母2;
                    var 割後分子3 = 計算記号2 == "÷" ? 分母3 : 分子3;
                    var 割後分母3 = 計算記号2 == "÷" ? 分子3 : 分母3;
                    var 最大公約数 = calcs.Get最大公約数(分子1 * 分子2 * 分子3 , 分母1 * 分母2 * 分母3 );
                    var 回答分子 = 分子1 * 分子2 * 分子3  / 最大公約数;
                    var 回答分母 = 分母1 * 分母2 * 分母3  / 最大公約数;
                    if (回答分子 > 30 || 回答分母 > 30) continue;
                    問題番号++;
                    questions.Add(new List<string>
                    {
                        $"{割後分子1}",
                        $"{割後分母1}",
                        $"{計算記号1}",
                        $"{割後分子2}",
                        $"{割後分母2}",
                        $"{計算記号2}",
                        $"{割後分子3}",
                        $"{割後分母3}",
                        $"答：",
                        $"{回答分子}",
                        $"{回答分母}",
                        $"{問題番号}"
                    });
                }
            };
            return questions; //基本ない想定
        }

        //再帰的に配列の全てのペアを取得
        public List<List<List<int>>> GetAllFractionPairs(List<List<int>> allFractions, int pairCount)
        {
            var combinations = new List<List<List<int>>>();
            GetCombinationsRecursive(allFractions, pairCount, 0, new List<List<int>>(), combinations);
            return combinations;
        }

        private void GetCombinationsRecursive(List<List<int>> allFractions, int pairCount, int start, List<List<int>> currentCombination, List<List<List<int>>> combinations)
        {
            if (currentCombination.Count == pairCount)
            {
                combinations.Add(new List<List<int>>(currentCombination));
                return;
            }

            for (int i = start; i < allFractions.Count; i++)
            {
                currentCombination.Add(allFractions[i]);
                GetCombinationsRecursive(allFractions, pairCount, i + 1, currentCombination, combinations);
                currentCombination.RemoveAt(currentCombination.Count - 1); // Backtrack
            }
        }
    }
        
}

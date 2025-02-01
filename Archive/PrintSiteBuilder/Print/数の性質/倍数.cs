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
    public class 倍数 : IPrint
    {
        private string PrintName;
        private List<int> answerIndex = new List<int> { 7, 8, 9, 10, 11 };
        private List<List<List<int>>> Coordinates = new List<List<List<int>>>
            {
                new List<List<int>>{new List<int>{0,0},new List<int>{3,7} },
                new List<List<int>>{new List<int>{4,0},new List<int>{7,7}},
                new List<List<int>>{new List<int>{8,0},new List<int>{11,7}},
            };
        private Calcs calcs = new Calcs();
        public int PagesCount { get; private set; }
        public string PresentationId { get; private set; }
        private int Score;
        public 倍数()
        {
            PresentationId = "1CZjklz5HE3oDp-hh_JQZANTM2Gi9w9SKCcPjIySf8jI";
            PrintName = "倍数";
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
            var RandomPairs = GetRandomPairs(Questions);
            for (var i = 0; i < Coordinates.Count(); i++)
            {
                for (var j = 0; j < Coordinates[i].Count(); j++)
                {
                    var RowNumber = Coordinates[i][j][0];
                    var ColumnNumber = Coordinates[i][j][1];
                    var Value = RandomPairs[i][j];
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
                GetQuestions2(),
                GetQuestions3()
            };

            var Questions = new List<List<string>>();
            foreach (var question in QuestionsList)
            {
                Questions = Questions.Concat(question).ToList();
            }

            return GetRandomPairs(Questions);
        }
        public List<List<string>> GetQuestions1()
        {
            var questions = new List<List<string>>();
            for(var i=4;i<25;i++)
            {
                questions.Add(new List<string>()
                {
                    $"{i}の倍数を小さい順に3つ答えてください。",
                    $"{i},{i*2},{i*3}",
                    "1"
                });
            }
            return questions;
        }
        public List<List<string>> GetQuestions2()
        {
            var questions = new List<List<string>>();
            for (var i = 4; i < 25; i++)
            {
                for (var j = 50; j < 200; j++)
                {
                    if (j % i == 0) continue;
                    questions.Add(new List<string>()
                    {
                        $"{i}で割り切れる数のうち、{j}に最も近い数を答えてください。",
                        $"{Math.Floor((decimal)j/i) * i}",
                        $"2"
                    });
                }
            }
            return questions;
        }
        public List<List<string>> GetQuestions3()
        {
            var questions = new List<List<string>>();
            for (var i = 4; i < 25; i++)
            {
                var Answer = Math.Floor((decimal)99 / i) - Math.Floor((decimal)9 / i);
                questions.Add(new List<string>()
                {
                        $"2桁の整数のうち、{i}の倍数は何個ありますか？",
                        $"{Answer.ToString()}個",
                        $"3"
                });
            }
            return questions;
        }

        public List<List<string>> GetRandomPairs(List<List<string>> questions)
        {
            var random = new Random();
            var uniqueThirdElements = new HashSet<string>();
            var randomPairs = new List<List<string>>();

            var shuffledQuestions = questions.OrderBy(x => random.Next()).ToList();

            foreach (var question in shuffledQuestions)
            {
                if (uniqueThirdElements.Add(question[2]))
                {
                    randomPairs.Add(question);
                }
            }
            return randomPairs;
        }
    }
}

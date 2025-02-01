using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintSiteBuilder.Interfaces;
using PrintSiteBuilder.Models.Print;
using PrintSiteBuilder.Utilities;

namespace PrintSiteBuilder.Print.引き算
{
    public class 一桁の引き算 : IPrint
    {
        private string PrintName;
        private List<int> answerIndex = new List<int> { 4, 10 };
        private List<List<List<int>>> Coordinates = new List<List<List<int>>>
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
        private Calcs calcs = new Calcs();
        public int PagesCount { get; private set; }
        public string PresentationId { get; private set; }
        private int Score;
        public List<List<string>> QaPairs;
        public List<List<string>> RandomPairs;
        public 一桁の引き算()
        {
            PresentationId = "1NxPP3H7R3sjG1VmBjfsXGHGhoRxEmHWhweglLDSwLk8";
            PrintName = "一桁の引き算";
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
                if (questionIndex.Add(question[question.Count - 1]))
                {
                    randomPairs.Add(question);
                }
            }
            return randomPairs;
        }
        public List<List<string>> GetQuestions1()
        {
            var questions = new List<List<string>>();
            var NumberPairs = new List<List<int>>();
            for (var i = 1; i < 10; i++)
            {
                for (var j = 1; j <= i; j++)
                {
                    NumberPairs.Add(new List<int> { i, j });
                }
            }
            foreach (var pair in NumberPairs)
            {
                questions.Add(new List<string>
                {
                    pair[0].ToString(),
                    "-",
                    pair[1].ToString(),
                    "=",
                    (pair[0] - pair[1]).ToString(),
                    Guid.NewGuid().ToString()
                });
            }
            return questions;
        }
    }
}

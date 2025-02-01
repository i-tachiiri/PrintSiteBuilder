/*using ImageMagick;
using PrintSiteBuilder.Interfaces;
using PrintSiteBuilder.Models.Print;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintSiteBuilder.SiteItem;
using PrintSiteBuilder.Models.General;

namespace PrintSiteBuilder.Print2.Item
{
    public class 一桁の足し算_100マス計算 : IPrint2
    {
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
        public Dictionary<string, string> PrintId_CategoryName { get; private set; }
        public int PagesCount { get; private set; }
        public string PrintSlideId { get; private set; }
        public string CoverSlideId { get; private set; }
        public string AmazonSlideId { get; private set; }
        public string PrintId { get; private set; }
        public string PrintName { get; private set; }
        public string Uuid { get; private set; }
        public string Barcode { get; private set; }

        public PathConfig path { get; private set; }
        public int TemplateNumber { get; private set; }

        private int Score;
        public List<List<string>> QaPairs;
        public List<List<string>> RandomPairs;
        public SlidesConfig slidesConfig;
        public Json2 json = new Json2();
        /*public 一桁の足し算_100マス計算()
        {
            PrintId = "100001";
            CoverSlideId = "1B--tQR4RoBqnh8VloFR5y1jEVc0e2GnpNLVEc6OPU18";
            AmazonSlideId = "1_jUbevyPQGhXjyKsUUmj7fOidcJGVPPP7aNNddAclVE";
            PrintSlideId = "129dbifHApktdVQ2Xcipyu5VxGZN-n5fn4IOiWJJzEu8";
            PrintName = "1桁の足し算(100マス計算)";
            Uuid = "6e3fa849";
            PagesCount = 50;
            Score = 100;
            path = new PathConfig(PrintId);
            PrintId_CategoryName = new Dictionary<string, string>
            {
                {"100001","プリント" }
            };
        }
        public async Task InitializeAsync(string PrintName)
        {
            TemplateNumber = 1;
            PagesCount = 50;
            Score = 100;
            PrintId = GetType().Name.Replace("P", "");
            this.PrintName = PrintName;
            path = new PathConfig(PrintId);
            if (!File.Exists(path.PrintSlideConfig))
            {
                var item = new Item2();
                slidesConfig = await item.GetSlidesConfig(this);
                json.SerializeSlidesConfig(slidesConfig, this);
            }
            slidesConfig = json.DeserializeSlidesConfig(this);
            CoverSlideId = slidesConfig.CoverSlideConfig.SlideId;
            AmazonSlideId = slidesConfig.AmazonSlideConfig.SlideId;
            PrintSlideId = slidesConfig.PrintSlideConfig.SlideId;
            Uuid = "6e3fa849";  //100003はもう設定しちゃったので固定値
            Barcode = "test";
        }

        public List<PrintConfig2> GetPrintConfigs()
        {
            var headerConfigs = GetHeaderConfigs();
            var printConfigs = new List<PrintConfig2>();
            foreach (var headerConfigGroup in headerConfigs.GroupBy(item => item.PrintNumber))   //プリント番号別にグループ化して問題と回答をセットに
            {
                var cellConfigs = GetCellConfigs();
                foreach (var headerConfig in headerConfigGroup)
                {
                    var printConfig = new PrintConfig2(headerConfig, cellConfigs, PrintId);
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
                var questionPrintNumber = i + 1;
                var questionScore = Score;
                var questionPageIndex = i;
                HeaderConfig questionConfig = new HeaderConfig(questionPrintType, questionPrintName, questionPrintNumber, questionScore, questionPageIndex);
                headerConfigs.Add(questionConfig);

                // Answer Config
                var answerPrintType = "回答";
                var answerPrintName = $"{PrintName}-{(i + 1).ToString("D3")}";
                var answerPrintNumber = i + 1;
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
            var QuestionsList = new List<List<List<string>>>()
            {
                GetQuestions1(),
                //GetQuestions1(),
            };

            var Questions = new List<List<string>>();
            foreach (var question in QuestionsList)
            {
                Questions = Questions.Concat(question).ToList();
            }

            return Questions;//GetRandomPairs(Questions);
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
            Random random = new Random();
            List<int> vericalNumbers = Enumerable.Range(1, 10).OrderBy(x => random.Next()).ToList();
            List<int> horizonalNumbers = Enumerable.Range(1, 10).OrderBy(x => random.Next()).ToList();
            questions.Add(new List<string>
            {
                "+",
                horizonalNumbers[0].ToString(),
                horizonalNumbers[1].ToString(),
                horizonalNumbers[2].ToString(),
                horizonalNumbers[3].ToString(),
                horizonalNumbers[4].ToString(),
                horizonalNumbers[5].ToString(),
                horizonalNumbers[6].ToString(),
                horizonalNumbers[7].ToString(),
                horizonalNumbers[8].ToString(),
                horizonalNumbers[9].ToString(),
                "+",
                Guid.NewGuid().ToString()
            });
            for (var i = 0; i < vericalNumbers.Count; i++)
            {
                questions.Add(new List<string>
                {
                    vericalNumbers[i].ToString(),
                    (vericalNumbers[i] + horizonalNumbers[0]).ToString(),
                    (vericalNumbers[i] + horizonalNumbers[1]).ToString(),
                    (vericalNumbers[i] + horizonalNumbers[2]).ToString(),
                    (vericalNumbers[i] + horizonalNumbers[3]).ToString(),
                    (vericalNumbers[i] + horizonalNumbers[4]).ToString(),
                    (vericalNumbers[i] + horizonalNumbers[5]).ToString(),
                    (vericalNumbers[i] + horizonalNumbers[6]).ToString(),
                    (vericalNumbers[i] + horizonalNumbers[7]).ToString(),
                    (vericalNumbers[i] + horizonalNumbers[8]).ToString(),
                    (vericalNumbers[i] + horizonalNumbers[9]).ToString(),
                    vericalNumbers[i].ToString(),
                });
            }
            return questions;
        }
    }
}*/

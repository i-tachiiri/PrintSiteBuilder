using System;
using System.Collections.Generic;
using System.Globalization;
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
    /*Question3以降を作成中*/
    public class 公倍数の文章題 : IPrint
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
        public List<List<string>> QaPairs;
        public List<List<string>> RandomPairs;
        public 公倍数の文章題()
        {
            PresentationId = "1EXbSJb3792V-RG2qjWh7Hw50ytAl5seluDRfaJ48nCI";
            PrintName = "公倍数の文章題";
            PagesCount = 10;
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
                GetQuestions3(),
                GetQuestions4(),
                GetQuestions5(),
                GetQuestions6(),
                GetQuestions7(),
                GetQuestions8()
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
        public List<List<string>> GetQuestions1()
        {
            var questions = new List<List<string>>();
            for(var i=4;i<25;i++)
            {
                for(var j=i+1;j<25;j++)
                {
                    for(var k=j+1;k<25;k++)
                    {
                        var 最小公倍数 = calcs.Get最小公倍数(i,j,k);
                        if (最小公倍数 > 100) continue;
                        if (j % i == 0|| k%j == 0|| k%i == 0) continue;
                        var random = new Random();
                        var hours = new List<int> { 6, 7, 8, 9, 10, 11 };
                        var time = new DateTime(1900, 1, 1, hours[random.Next(hours.Count)], 0, 0);
                        var items = new List<List<string>>
                        {
                            new List<string>{"バス停","町","バス","台"},
                            new List<string>{"駅","駅","電車","本"},
                            new List<string>{"空港","国","飛行機","便"},
                            new List<string>{"倉庫","社","トラック","台"},
                            new List<string>{"港","港","船","隻"},
                        };
                        var item = items[random.Next(items.Count)];
                        questions.Add(new List<string>()
                        {
                            $"ある{item[0]}からA{item[1]}行きの{item[2]}が{i}分おき、B{item[1]}行きの{item[2]}が{j}分おき、C{item[1]}行きの{item[2]}が{k}分おきに出発します。午前{time.ToString("H時")}に3つの{item[2]}が同時に出発しました。次に3{item[3]}が同時に出発するのは何時何分ですか？",
                            $"{time.AddMinutes(最小公倍数).ToString("H時mm分")}",
                            "1"
                        });
                    }
                }
            }
            return questions;
        }
        public List<List<string>> GetQuestions2()
        {
            var questions = new List<List<string>>();
            for (var i = 4; i < 25; i++)
            {
                for (var j = i + 1; j < 25; j++)
                {
                    for (var k = j + 1; k < 25; k++)
                    {
                        var 最小公倍数 = calcs.Get最小公倍数(i, j, k);
                        if (最小公倍数 > 100) continue;
                        if (j % i == 0 || k % j == 0 || k % i == 0) continue;
                        var random = new Random();
                        var fromHour = new List<int> { 8, 9, 10, 11 };
                        var fromTime = new DateTime(1900, 1, 1, fromHour[random.Next(fromHour.Count)], 0, 0);
                        var fromTimeString = fromTime.ToString("tt H時", new CultureInfo("ja-JP"));
                        var endHour = new List<int> { 3,4,5,6 };
                        var endTime = new DateTime(1900, 1, 1, endHour[random.Next(endHour.Count)], 0, 0);
                        var endTimeString = endTime.ToString("tt H時", new CultureInfo("ja-JP"));
                        var Period = endTime - fromTime; 
                        var items = new List<List<string>>
                        {
                            new List<string>{"バス停","町","バス","台"},
                            new List<string>{"駅","駅","電車","本"},
                            new List<string>{"空港","国","飛行機","便"},
                            new List<string>{"倉庫","社","トラック","台"},
                            new List<string>{"港","港","船","隻"},
                        };
                        var item = items[random.Next(items.Count)];
                        questions.Add(new List<string>()
                        {
                            $"ある{item[0]}からA{item[1]}行きの{item[2]}が{i}分おき、B{item[1]}行きの{item[2]}が{j}分おき、C{item[1]}行きの{item[2]}が{k}分おきに出発します。午前{fromTime.ToString("H時")}に3つの{item[2]}が同時に出発しました。{fromTimeString}から{endTimeString}までに、何回3{item[3]}が同時に出発しますか？",
                            $"{Period.Minutes/最小公倍数+1}",
                            "2"
                        });
                    }
                }
            }
            return questions;
        }
        public List<List<string>> GetQuestions3()
        {
            var questions = new List<List<string>>();
            for (var i = 4; i < 25; i++)
            {
                for (var j = i + 1; j < 25; j++)
                {
                    for (var k = j + 1; k < 25; k++)
                    {
                        var 公倍数 = calcs.Get公倍数(i, j, k,100);
                        if (公倍数.Count < 3) continue;
                        if (k % j == 0 || j % i == 0 || k%i ==0) continue;
                        questions.Add(new List<string>()
                        {
                            $"({i},{j},{k})の公倍数を小さい順に3つ答えてください。",
                            $"{公倍数[0]},{公倍数[1]},{公倍数[2]}",
                            "3"
                        });
                    }
                }
            }
            return questions;
        }
        public List<List<string>> GetQuestions4()
        {
            var questions = new List<List<string>>();
            for (var i = 4; i < 25; i++)
            {
                for (var j = i + 1; j < 25; j++)
                {
                    var 公倍数 = calcs.Get公倍数(i, j,  100);
                    if (公倍数.Count < 3) continue;
                    if ( j % i == 0) continue;
                    questions.Add(new List<string>()
                    {
                        $"({i},{j})の公倍数を小さい順に3つ答えてください。",
                        $"{公倍数[0]},{公倍数[1]},{公倍数[2]}",
                        "3"
                    });                    
                }
            }
            return questions;
        }
        public List<List<string>> GetQuestions5()
        {
            var questions = new List<List<string>>();
            for (var i = 4; i < 25; i++)
            {
                for (var j = i + 1; j < 25; j++)
                {
                    for (var k = j + 1; k < 25; k++)
                    {
                        var 最小公倍数 = calcs.Get最小公倍数(i, j, k);
                        if (最小公倍数 > 100) continue;
                        if (k % j == 0 || j % i == 0 || k % i == 0) continue;
                        questions.Add(new List<string>()
                        {
                            $"{i},{j},{k}のいずれでも割り切れる数の中で、最も小さな数を答えてください。",
                            $"{最小公倍数}",
                            "1"
                        });
                    }
                }
            }
            return questions;
        }
        public List<List<string>> GetQuestions6()
        {
            var questions = new List<List<string>>();
            for (var i = 4; i < 25; i++)
            {
                for (var j = i + 1; j < 25; j++)
                {
                    var 最小公倍数 = calcs.Get最小公倍数(i, j);
                    if (最小公倍数 > 100) continue;
                    if ( j % i == 0) continue;
                    questions.Add(new List<string>()
                    {
                        $"{i}でも{j}でも割り切れる数の中で、最も小さな数を答えてください。",
                        $"{最小公倍数}",
                        "2"
                    });
                }
            }
            return questions;
        }
        public List<List<string>> GetQuestions7()
        {
            var questions = new List<List<string>>();
            for (var i = 4; i < 25; i++)
            {
                for (var j = i + 1; j < 25; j++)
                {
                    for (var k = j + 1; k < 25; k++)
                    {
                        var 最小公倍数 = calcs.Get最小公倍数(i, j, k);
                        if (最小公倍数 > 100) continue;
                        for(var l=最小公倍数 + 1;l<200;l++)
                        {
                            if (l % 最小公倍数 == 0) continue;
                            if (l / 最小公倍数 < 2) continue;
                            if (k % j == 0 || j % i == 0 || k % i == 0) continue;
                            questions.Add(new List<string>()
                            {
                                $"{i},{j},{k}のいずれでも割り切れて、{l}より大きい数の中で、最も小さな数を答えてください。",
                                $"{Math.Ceiling((decimal)l/最小公倍数)*最小公倍数}",
                                "7"
                            });
                        }
                    }
                }
            }
            return questions;
        }
        public List<List<string>> GetQuestions8()
        {
            var questions = new List<List<string>>();
            for (var i = 4; i < 25; i++)
            {
                for (var j = i + 1; j < 25; j++)
                {
                    for (var a = 3; a < 10; a++)
                    {
                        var Variables = new List<int>(); // ここで初期化
                        for (var x = 11; x < 200; x++)
                        {
                            if (x % i != a || x % j != a) continue;
                            if ( j % i == 0) continue;
                            Variables.Add(x);
                            if (Variables.Count == 3)
                            {
                                Variables.Sort(); // 小さい順にソート
                                questions.Add(new List<string>()
                                {
                                    $"{i}で割っても{j}で割っても{a}余る数を、小さい順に3つ答えてください。",
                                    $"{Variables[0]},{Variables[1]},{Variables[2]}",
                                    "8"
                                });
                                break; // 3つ見つけたので次のaへ
                            }                            
                        }
                        if (Variables.Count == 3) break;
                    }
                }
            }
            return questions;
        }



    }
}

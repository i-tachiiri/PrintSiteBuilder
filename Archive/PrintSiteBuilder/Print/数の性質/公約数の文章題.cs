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
    public class 公約数の文章題 : IPrint
    {
        private List<int> answerIndex = new List<int> { 7, 8, 9, 10, 11 };
        private List<List<List<int>>> Coordinates = new List<List<List<int>>>
            {
                new List<List<int>>{new List<int>{0,0},new List<int>{3,7} },
                new List<List<int>>{new List<int>{4,0},new List<int>{7,7}},
                new List<List<int>>{new List<int>{8,0},new List<int>{11,7}},
            };
        private List<List<string>> ItemPairs = new List<List<string>>
            {
                new List<string> { "鉛筆", "本", "消しゴム", "個" },
                new List<string> { "ノート", "冊", "ペン", "本" },
                new List<string> { "お菓子", "個", "ジュース", "本" },
                new List<string> { "シール", "枚", "メモ帳", "冊" },
                new List<string> { "リンゴ", "個", "バナナ", "本" },
                new List<string> { "ボール", "個", "なわとび", "本" },
                new List<string> { "本", "冊", "しおり", "枚" },
                new List<string> { "色鉛筆", "本", "画用紙", "枚" },
                new List<string> { "折り紙", "枚", "ハサミ", "本" },
                new List<string> { "サンドイッチ", "個", "ジュース", "本" },
                new List<string> { "クリップ", "個", "ホッチキス", "本" },
                new List<string> { "クレヨン", "本", "スケッチブック", "冊" },
                new List<string> { "マーカー", "本", "ホワイトボード", "枚" },
                new List<string> { "パズル", "個", "絵本", "冊" },
                new List<string> { "タオル", "枚", "歯ブラシ", "本" },
                new List<string> { "ハンカチ", "枚", "ポケットティッシュ", "個" },
                new List<string> { "マウス", "個", "キーボード", "本" },
            };
        private List<string> CutItems = new List<string> { "画用紙", "ポスター", "紙", "板", "布", "段ボール", "発泡スチロール" };
        private List<List<string>> PlaceItems = new List<List<string>>
            {
                new List<string>{"木を植え","木","本" },
                new List<string>{"看板を立て", "看板","枚"},
                new List<string>{"電柱を立て","電柱","本"},
                new List<string>{"ベンチを置き","ベンチ","個"},
                new List<string>{"桜の木を植え" ,"桜の木","本"}
            };
        private List<List<string>> ItemTrios = new List<List<string>>
            {
                new List<string> { "鉛筆", "本", "消しゴム", "個", "ノート", "冊" },
                new List<string> { "消しゴム", "個", "ノート", "冊" , "ペン", "本" },
                new List<string> { "シール", "枚", "メモ帳", "冊", "ペン", "本" },
                new List<string> { "リンゴ", "個", "バナナ", "本","メロン","個" },
                new List<string> { "色鉛筆", "本", "画用紙", "枚","クレヨン","本" },
                new List<string> { "折り紙", "枚", "ハサミ", "本", "シール", "枚"},
                new List<string> { "クレヨン", "本", "スケッチブック", "冊", "シール", "枚" },
                new List<string> { "タオル", "枚", "歯ブラシ", "本","コップ","個" },
                new List<string> { "ハンカチ", "枚", "ポケットティッシュ", "個" },
                new List<string> { "マウス", "個", "キーボード", "本","イヤホン","本" },
            };

        public int PagesCount { get; private set; }
        private int Score;
        private string PrintName = "公約数の文章題";
        public string PresentationId { get; private set; }
        private Calcs calcs = new Calcs();
        public 公約数の文章題()
        {
            PresentationId = "15zWPmZFV87OxDRvtiRQ7nVppGENyFV8kSFLLnOt6JuE";
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
            var InputPairs = GetQaPairs();
            var RandomPairs = GetRandomPairs(InputPairs);
            var CellConfigs = new List<CellConfig>();
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
            var TwoNumbers = GetTwoNumbers();
            var InputPairs = new List<List<string>>();
            for (var i = 0; i < Score; i++)
            {
                var random = new Random();
                var IndexPair = TwoNumbers[random.Next(TwoNumbers.Count)];
                var a = IndexPair[0];
                var b = IndexPair[1];
                var GreatestCommonDivisor2 = calcs.Get最大公約数(a, b);
                var ItemPair = ItemPairs[random.Next(ItemPairs.Count)];
                var CutItem = CutItems[random.Next(CutItems.Count)];
                var PlaceItem = PlaceItems[random.Next(PlaceItems.Count)];
                InputPairs.Add(new List<string> 
                { 
                    $"{ItemPair[0]}が{a}{ItemPair[1]}、{ItemPair[2]}が{b}{ItemPair[3]}あります。この2つをできるだけ多くの人に、余らないよう分けたいです。{ItemPair[0]}と{ItemPair[2]}を何人に、いくつずつ配れますか？",                     
                    $"{GreatestCommonDivisor2}人に{ItemPair[0]}を{a / GreatestCommonDivisor2}{ItemPair[1]}・{ItemPair[2]}を{b / GreatestCommonDivisor2}{ItemPair[3]}ずつ",
                    "1"
                });
                InputPairs.Add(new List<string> 
                { 
                    $"縦が{a}cm、横が{b}cmの{CutItem}があります。画用紙を同じ大きさの、できるだけ大きな正方形に切り分けます。正方形の枚数と、1辺の長さを答えてください。", 
                    $"{a / GreatestCommonDivisor2 * b / GreatestCommonDivisor2}枚・{GreatestCommonDivisor2}cm",
                    "2"
                });
                InputPairs.Add(new List<string> { 
                    $"縦が{a}m、横が{b}mの土地の周りに、等間隔で{PlaceItem[0]}たいです。土地の4つの角には{PlaceItem[0]}るものとします。{PlaceItem[1]}は少なくとも何{PlaceItem[2]}必要ですか？", 
                    $"{(a + b) * 2 / GreatestCommonDivisor2}{PlaceItem[2]}",
                    "3"
                });
            }

            var ThreeNumbers = GetThreeNumbers();
            for (var i = 0; i < Score; i++)
            {
                var random = new Random();
                var IndexPair = ThreeNumbers[random.Next(ThreeNumbers.Count)];
                var a = IndexPair[0];
                var b = IndexPair[1];
                var c = IndexPair[2];
                var GreatestCommonDivisor2 = calcs.Get最大公約数(a, b, c); var ItemPair = ItemPairs[random.Next(ItemPairs.Count)];
                var ItemTrio = ItemTrios[new Random().Next(CutItems.Count)];
                InputPairs.Add(new List<string>
                {
                    $"{ItemTrio[0]}が{a}{ItemTrio[1]}、{ItemTrio[2]}が{b}{ItemTrio[3]}、{ItemTrio[4]}が{c}{ItemTrio[5]}あります。この3つをできるだけ多くの人に、余らないよう分けたいです。{ItemTrio[0]}・{ItemTrio[2]}・{ItemTrio[4]}を何人に、いくつずつ配れますか？",
                    $"{GreatestCommonDivisor2}人に{ItemTrio[0]}を{a / GreatestCommonDivisor2}{ItemTrio[1]}・{ItemTrio[2]}を{b / GreatestCommonDivisor2}{ItemTrio[3]}・{ItemTrio[4]}を{c/ GreatestCommonDivisor2}{ItemTrio[5]}ずつ",
                    "4"
                });
            }
            return InputPairs;
        }
        private List<List<string>> GetRandomPairs(List<List<string>> InputPairs)
        {
            var random = new Random();
            var RandomPairs = new List<List<string>>();
            var AppearedPairIndex = new HashSet<int>();
            var AppearedQuestionIndex = new HashSet<string>();
            while (RandomPairs.Count < Score)
            {
                int index = random.Next(InputPairs.Count);
                if (!AppearedPairIndex.Contains(index) && !AppearedQuestionIndex.Contains(InputPairs[index][2]))
                {
                    RandomPairs.Add(InputPairs[index]);
                    AppearedPairIndex.Add(index);
                    AppearedQuestionIndex.Add(InputPairs[index][0]);
                }
            }
            return RandomPairs;
        }
        private List<List<int>> GetTwoNumbers()
        {
            var IndexPairs = new List<List<int>>();
            for (var i = 4; i <= 120; i++)
            {
                for (var j = 4; j < i; j++)
                {
                    var CommonDivisor2 = calcs.Get公約数(i, j);
                    var GreatestCommonDivisor2 = calcs.Get最大公約数(i, j);
                    if (calcs.IsPrime(i) || calcs.IsPrime(j)) continue;
                    if (CommonDivisor2.Count < 3) continue;
                    if (GreatestCommonDivisor2 == 1 || GreatestCommonDivisor2 == i || GreatestCommonDivisor2 == j) continue;
                    IndexPairs.Add(new List<int> { i, j });
                }
            }
            return IndexPairs;
        }
        private List<List<int>> GetThreeNumbers()
        {
            var IndexPairs = new List<List<int>>();
            for (var i = 4; i <= 120; i++)
            {
                for (var j = 4; j < i; j++)
                {
                    for (var k = 4; k < j; k++)
                    {
                        var CommonDivisor = calcs.Get公約数(i, j,k);
                        var GreatestCommonDivisor = calcs.Get最大公約数(i, j,k);
                        if (calcs.IsPrime(i) || calcs.IsPrime(j) || calcs.IsPrime(k)) continue;
                        if (CommonDivisor.Count < 3) continue;
                        if (GreatestCommonDivisor == 1 || GreatestCommonDivisor == i || GreatestCommonDivisor == j || GreatestCommonDivisor == k) continue;
                        IndexPairs.Add(new List<int> { i, j,k });
                    }
                }
            }
            return IndexPairs;
        }



    }
}

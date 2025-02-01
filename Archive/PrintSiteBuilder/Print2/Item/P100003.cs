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
using PrintSiteBuilder.Print2.Template;
using System.ComponentModel;

namespace PrintSiteBuilder.Print2.Item
{
    public class P100003 : IPrint2
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
        public string FnSku { get; private set; }
        public IPrintType PrintType { get; private set; }
        public PathConfig path { get; private set; }
        public int TemplateNumber { get; private set; }
        public string Description { get; private set; }
        public string Keywords {  get; private set; }
        public string Sku { get; private set; }
        public string Asin { get; private set; }


        public List<List<string>> QaPairs;
        public List<List<string>> RandomPairs;
        public TemplateFolder templateFolder;
        public Task<List<Google.Apis.Drive.v3.Data.File>> SlideList;
        public SlidesConfig slidesConfig;
        public Json2 json = new Json2();

        public async Task InitializeAsync(string PrintName)
        {
            Description = "1桁の足し算のプリントです。10問×30枚=300問が収録されています。 QRコードからプリントの参照や再印刷ができる専用のWEBページが開けます。繰り返しの学習や、学校・塾、兄弟姉妹等の複数人での学習用にも適しています。";
            Keywords = "プリント 算数 足し算 計算 3歳 4歳 5歳 6歳 1年生 2年生 100マス計算 繰り上がり 問題集";
            TemplateNumber = 1;
            PagesCount = 30;
            PrintId = GetType().Name.Replace("P", "");
            PrintType = new P100マス計算(this);
            this.PrintName = PrintName;
            path = new PathConfig(PrintId);
            if (!File.Exists(path.PrintSlideConfig))
            {
                var item = new Item2();
                var template = new TemplateFolder(this);
                await template.CopyTemplate();
                slidesConfig = await item.GetSlidesConfig(this);
                json.SerializeSlidesConfig(slidesConfig, this);
            }
            slidesConfig = json.DeserializeSlidesConfig(this);
            CoverSlideId = slidesConfig.CoverSlideConfig.SlideId;
            AmazonSlideId = slidesConfig.AmazonSlideConfig.SlideId;
            PrintSlideId = slidesConfig.PrintSlideConfig.SlideId;
            Uuid = "je32at4z";  //100003はもう設定しちゃったので固定値
            FnSku = "X0017VMFJF";
            Sku = $"{PrintType.SkuHeader}-{PrintId.Substring(0, 2)}-{PrintId.Substring(2, 4)}";
            Asin = "B0D986DNL6";
        }

        public List<List<List<string>>> GetQuestionLists()
        {
            return new List<List<List<string>>>()
            {
                GetQuestions1(),
                //GetQuestions1(),
            };
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
}

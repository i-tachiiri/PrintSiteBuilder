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

namespace PrintSiteBuilder.Print2.Item
{
    public class P100004 : IPrint2
    {
        public Dictionary<string, string> PrintId_CategoryName { get; private set; }
        public int PagesCount { get; private set; }
        public string PrintSlideId { get; private set; }
        public string CoverSlideId { get; private set; }
        public string AmazonSlideId { get; private set; }

        public string PrintId { get; private set; }
        public string PrintName { get; private set; }
        public string Uuid { get; private set; }
        public string FnSku { get; private set; }
        public PathConfig path { get; private set; }
        public int TemplateNumber { get; private set; }
        public IPrintType PrintType { get; private set; }
        public string Description { get; private set; }
        public string Keywords { get; private set; }
        public int Score { get; private set; }
        public string Sku { get; private set; }
        public List<List<string>> QaPairs;
        public List<List<string>> RandomPairs;
        public TemplateFolder templateFolder;
        public Task<List<Google.Apis.Drive.v3.Data.File>> SlideList;
        public SlidesConfig slidesConfig;
        public Json2 json = new Json2();
        public Item2 item = new Item2();
        public string Asin { get; private set; }

        public async Task InitializeAsync(string PrintName)
        {
            Description = "1桁の足し算のプリントです。10問×30枚=300問が収録されています。 QRコードからプリントの参照や再印刷ができる専用のWEBページが開けます。繰り返しの学習や、学校・塾、兄弟姉妹等の複数人での学習用にも適しています。";
            Keywords = "プリント 算数 足し算 計算 3歳 4歳 5歳 6歳 1年生 2年生 100マス計算 繰り上がり 問題集";
            TemplateNumber = 1;
            PagesCount = 30;
            PrintType = new P100マス計算(this);
            PrintId = GetType().Name.Replace("P", "");
            this.PrintName = PrintName;
            path = new PathConfig(PrintId);
            if (!File.Exists(path.PrintSlideConfig))
            {
                var template = new TemplateFolder(this);
                await template.CopyTemplate();
                slidesConfig = await item.GetSlidesConfig(this);
                json.SerializeSlidesConfig(slidesConfig, this);
            }
            slidesConfig = json.DeserializeSlidesConfig(this);
            CoverSlideId = slidesConfig.CoverSlideConfig.SlideId;
            AmazonSlideId = slidesConfig.AmazonSlideConfig.SlideId;
            PrintSlideId = slidesConfig.PrintSlideConfig.SlideId;
            Uuid = PrintSlideId.Substring(0, 12);
            FnSku = "X0017VMFJF";
            Sku = "AB-CDEF-GHIJ";//$"{PrintType.SkuHeader}-{PrintId.Substring(0,2)}-{PrintId.Substring(2,4)}";
            Asin = "B0D986DNL6";
        }

        public List<List<List<string>>> GetQuestionLists()
        {
            return  new List<List<List<string>>>()
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

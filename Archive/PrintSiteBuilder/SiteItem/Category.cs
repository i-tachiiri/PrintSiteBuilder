using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintSiteBuilder.Models.General;
using static System.Windows.Forms.LinkLabel;

namespace PrintSiteBuilder.SiteItem
{
    public class Category
    {
        public CategoryConfig GetCategoryConfig(string CategoryName)
        {
            var CategoryConfigPaths = Directory.GetFiles(GlobalConfig.GroupDir, $"{CategoryName}.csv", SearchOption.AllDirectories);
            if (CategoryConfigPaths.Count() == 0)
            {
                return new CategoryConfig();
            }
            var CategoryConfigPath = CategoryConfigPaths[0];
            FileInfo fileInfo = new FileInfo(CategoryConfigPath);
            DirectoryInfo parentDirectory = fileInfo.Directory;
            var categoryConfig = ReadCategoryConfigCsv(CategoryConfigPath);
            categoryConfig.GroupName = parentDirectory.Name;
            return categoryConfig;
        }
        public CategoryConfig ReadCategoryConfigCsv(string CategoryConfigPath)
        {
            var categoryConfig = new CategoryConfig();
            var Lines = File.ReadAllLines(CategoryConfigPath);
            foreach (var line in Lines)
            {
                var row = line.Trim().Split(",");
                if (row.Count() > 1)
                {
                    var key = row[0];
                    var value = row[1];
                    if (key == "itemName")
                    {
                        categoryConfig.CategoryName = value;
                    }
                    else if (key == "title")
                    {
                        categoryConfig.Title = value;
                    }
                    else if (key == "description")
                    {
                        categoryConfig.Description = value;
                    }
                    else if (key == "Q1")
                    {
                        categoryConfig.Q1 = value;
                    }
                    else if (key == "A1")
                    {
                        categoryConfig.A1 = value;
                    }
                    else if (key == "Q2")
                    {
                        categoryConfig.Q2 = value;
                    }
                    else if (key == "A2")
                    {
                        categoryConfig.Q1 = value;
                    }
                    else if (key == "Q3")
                    {
                        categoryConfig.Q3 = value;
                    }
                    else if (key == "A3")
                    {
                        categoryConfig.A3 = value;
                    }
                    else
                    {
                        ;
                    }
                }
            }
            return categoryConfig;
        }
        public void CreateCategoryConfig()
        {
            var SingleSvgPaths = Directory.GetFiles(GlobalConfig.SvgDir, "*.svg");
            var GroupSvgPaths = Directory.GetFiles(GlobalConfig.SvgGroupDir, "*.svg");
            var ItemPaths = SingleSvgPaths.Concat(GroupSvgPaths);
            var CategoryNames = ItemPaths.Select(path => Path.GetFileNameWithoutExtension(path)).Select(path => path.Split("-")[0]).ToList();
            foreach (var categoryName in CategoryNames)
            {
                var Results = Directory.GetFiles($@"{GlobalConfig.GroupDir}", $@"{categoryName}.csv", SearchOption.AllDirectories);
                if (Results.Count() == 0)
                {
                    CreateItemCsv(categoryName);
                }
            }
        }
        public void CreateDocConfig()
        {
            var ItemPaths = Directory.GetFiles(GlobalConfig.DocMdDir, "*.md");
            var CategoryNames = ItemPaths.Select(path => Path.GetFileNameWithoutExtension(path)).Select(path => path.Split("-")[0]).ToList();
            foreach (var categoryName in CategoryNames)
            {
                var Results = Directory.GetFiles($@"{GlobalConfig.DocGroupDir}", $@"{categoryName}.csv", SearchOption.AllDirectories);
                if (Results.Count() == 0)
                {
                    var filePath = $@"{GlobalConfig.DocGroupDir}\00_00_新着\{categoryName}.csv";
                    File.Create(filePath);
                }
            }
        }
        public bool IsGroupConfigCreated()
        {
            var SvgConfigs = Directory.GetFiles($@"{GlobalConfig.GroupDir}\00_00_新着", "*.csv");
            foreach (var itemPath in SvgConfigs)
            {
                var fileInfo = new FileInfo(itemPath);
                if (fileInfo.LastWriteTime > DateTime.Now.AddSeconds(-10))
                {
                    return true;
                }
            }
            var DocConfigs = Directory.GetFiles($@"{GlobalConfig.DocGroupDir}\00_00_新着", "*.csv");
            foreach (var itemPath in DocConfigs)
            {
                var fileInfo = new FileInfo(itemPath);
                if (fileInfo.LastWriteTime > DateTime.Now.AddSeconds(-10))
                {
                    return true;
                }
            }
            return false;
        }
        public void CreateItemCsv(string ItemName)
        {
            var filePath = $@"{GlobalConfig.GroupDir}\00_00_新着\{ItemName}.csv";
            var lines = new List<string>
                    {
                        $"itemName,{ItemName}",
                        $"title,「{ItemName}」のプリント",
                        $"description,「{ItemName}」のプリントです。印刷・ダウンロードしてお使い下さい。",
                        $"Q1,",
                        $"A1,",
                        $"Q2,",
                        $"A2,",
                        $"Q3,",
                        $"A3,",
                    };
            File.WriteAllLines(filePath, lines);
        }
        public void CreateCategoryConfigByHtml(DataTable StatusTable)
        {
            var Keys = new HashSet<string>();
            StatusTable = StatusTable.AsEnumerable().Where(dr => dr["Do"] == "").CopyToDataTable();
            foreach (DataRow status in StatusTable.Rows)
            {
                Keys.Add(status.Field<string>("ItemName").Split("-")[0]);
            }
            foreach (var item in Keys)
            {
                var Results = Directory.GetFiles($@"{GlobalConfig.GroupDir}", $@"{item}.csv", SearchOption.AllDirectories);
                if (Results.Count() == 0)
                {
                    var filePath = $@"{GlobalConfig.GroupDir}\新着\{item}.csv";
                    var lines = new List<string>
                    {
                        $"itemName,{item}",
                        $"title,「{item}」のプリント",
                        $"description,「{item}」のプリントです。印刷・ダウンロードしてお使い下さい。",
                        $"Q1,",
                        $"A1,",
                        $"Q2,",
                        $"A2,",
                        $"Q3,",
                        $"A3,",
                    };
                    File.WriteAllLines(filePath, lines);
                }
            }
        }

    }
}

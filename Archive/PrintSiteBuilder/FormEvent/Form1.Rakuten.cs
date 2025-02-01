using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder.FormEvent
{
    public partial class Form1
    {
		private void RakutenItemName_Leave(object sender, EventArgs e)
		{
			var item = new Item();
			//var itemNames = item.GetItemNames(GlobalConfig.RakutenDir,"txt");

			var itemPaths = Directory.GetFiles(GlobalConfig.RakutenDir, $"{RakutenItemName.Text}.txt", SearchOption.AllDirectories);
			if (itemPaths.Count() > 0)
			{
				RakutenHtml.Text = File.ReadAllText(itemPaths[0]);
				RakutenRegister.Text = "変更";
			}
			else
			{
				RakutenHtml.Text = "";
				RakutenRegister.Text = "新規登録";
			}
		}

		private void RakutenItemName_DropDown(object sender, EventArgs e)
		{
			RakutenItemName.Items.Clear();
			var searchResults = Directory.GetFiles(GlobalConfig.RakutenDir, $@"*{RakutenItemName.Text}*.txt", SearchOption.AllDirectories).Select(path => Path.GetFileNameWithoutExtension(path)).ToHashSet();
			foreach (var searchResult in searchResults)
			{
				RakutenItemName.Items.Add(searchResult);
			}
		}

		private void RakutenHtml_Leave(object sender, EventArgs e)
		{
			dataGridView.DataSource = new DataTable();
			var GroupDirectories = Directory.GetDirectories(GlobalConfig.GroupDir).Select(path => Path.GetFileName(path));
			foreach (var groupDirectory in GroupDirectories)
			{
				var RakutenCategoryDirectory = $@"{GlobalConfig.RakutenDir}\{groupDirectory}";
				Directory.CreateDirectory(RakutenCategoryDirectory);
			}
			ShowRakutenStatus();
		}

		private void RakutenRegister_Click(object sender, EventArgs e)
		{
			var ItemRows = (List<RakutenConfig>)dataGridView.DataSource;
			string htmlContent = RakutenHtml.Text;
			string pattern = @"(?<=<br>).*?(?=</a>)";
			string replacement = RakutenItemName.Text;
			string content = Regex.Replace(htmlContent, pattern, replacement);
			foreach (var Item in ItemRows)
			{
				var TextFilePath = $@"{GlobalConfig.RakutenDir}\{Item.GroupName}\{RakutenItemName.Text}.txt";
				if (!Item.HasItem)
				{
					File.Delete(TextFilePath);
				}
				else
				{
					File.WriteAllText(TextFilePath, content);
				}
			}
			ShowRakutenStatus();
		}
		private void ShowRakutenStatus()
		{
			var RakutenDirectories = Directory.GetDirectories(GlobalConfig.RakutenDir);
			var ConfigList = new List<RakutenConfig>();
			foreach (var rakutenDirectory in RakutenDirectories)
			{
				var rakutenConfig = new RakutenConfig();
				rakutenConfig.HasItem = Directory.GetFiles(rakutenDirectory, $@"{RakutenItemName.Text}.txt").Length > 0;
				rakutenConfig.GroupName = Path.GetFileName(rakutenDirectory);
				rakutenConfig.Content = rakutenConfig.HasItem ? File.ReadAllText($@"{rakutenDirectory}\{RakutenItemName.Text}.txt") : "";
				ConfigList.Add(rakutenConfig);
			}
			dataGridView.DataSource = ConfigList;
			dataGridView.Columns[0].Width = 50;
			dataGridView.Columns[1].Width = 150;
			dataGridView.Columns[2].Width = 500;
		}

	}
}

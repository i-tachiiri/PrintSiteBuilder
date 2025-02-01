

using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using SpreadSheetLibrary.Config;
using CoincheckDomain.Config;
namespace SpreadSheetLibrary.Repository
{
    public class PublicTradeSheetRepository
    {
        private readonly SheetsService sheetsService;
        private readonly string sheetId = "10FjOr_oQI5wE8hczuw6cHKYIT0xPkyWlhFZQSHaZ_O8";
        private List<string> sheetNames = CoincheckConstants.TradingPairs.SelectMany(pair => new[] { $"s_{pair}", $"b_{pair}" }).ToList();

        public PublicTradeSheetRepository(SheetConnecter sheetConnecter)
        {
            sheetsService = sheetConnecter.GetSheetsService();
            //EnsureSheetsExist();
        }
        private void EnsureSheetsExist()
        {
            // 現在のシート名を取得
            var spreadsheet = sheetsService.Spreadsheets.Get(sheetId).Execute();
            var existingSheetNames = spreadsheet.Sheets.Select(sheet => sheet.Properties.Title).ToList();

            // 存在しないシートを追加
            var requests = new List<Request>();

            foreach (var sheetName in sheetNames)
            {
                if (!existingSheetNames.Contains(sheetName))
                {
                    requests.Add(new Request
                    {
                        AddSheet = new AddSheetRequest
                        {
                            Properties = new SheetProperties
                            {
                                Title = sheetName
                            }
                        }
                    });
                }
            }
            if (requests.Any())
            {
                var batchRequest = new BatchUpdateSpreadsheetRequest { Requests = requests };
                sheetsService.Spreadsheets.BatchUpdate(batchRequest, sheetId).Execute();
            }
        }
        /// <summary>
        /// スプレッドシートの指定した範囲の値を取得する
        /// </summary>
        /// <param name="range">シートの範囲 (例: "Sheet1!A1:C10")</param>
        /// <returns>範囲内のセルの値をリストとして返す</returns>
        public IList<IList<object>> GetValues(string range)
        {
            var request = sheetsService.Spreadsheets.Values.Get(sheetId, range);
            ValueRange response = request.Execute();
            return response.Values;
        }
        public void UpdateValues<T>(List<T> entityList)
        {
            var properties = typeof(T).GetProperties();
            var header = properties.Select(p => p.Name).ToList();

            // ヘッダー行をリストの最初に追加
            var values = new List<IList<object>> { header.Cast<object>().ToList() };

            // 各エンティティのプロパティ値を取得し、行として追加
            foreach (var entity in entityList)
            {
                var row = properties.Select(p => p.GetValue(entity)?.ToString() ?? "").ToList();
                values.Add(row.Cast<object>().ToList());
            }

            // データサイズに基づいて動的に範囲（range）を計算
            string sheetName = "all"; // 必要に応じてシート名を指定
            int rows = values.Count;
            int columns = header.Count;
            string range = $"{sheetName}!A1:{GetColumnLetter(columns)}{rows}";

            // 古いデータをクリア
            ClearSheetRange(sheetName);

            // データをスプレッドシートに書き込む
            var body = new ValueRange { Values = values };
            var updateRequest = sheetsService.Spreadsheets.Values.Update(body, sheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            updateRequest.Execute();
        }


        /// <summary>
        /// カラム番号を文字列に変換（例: 1 -> A, 26 -> Z, 27 -> AA）
        /// </summary>
        private string GetColumnLetter(int columnNumber)
        {
            var columnLetter = "";
            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnLetter = Convert.ToChar(65 + modulo) + columnLetter;
                columnNumber = (columnNumber - modulo) / 26;
            }
            return columnLetter;
        }

        /// <summary>
        /// 指定したシート範囲のデータをクリア
        /// </summary>
        private void ClearSheetRange(string sheetName)
        {
            var clearRequest = new ClearValuesRequest();
            var range = $"{sheetName}!A1:Z1000";  // 必要な範囲を指定
            var request = sheetsService.Spreadsheets.Values.Clear(clearRequest, sheetId, range);
            request.Execute();
        }

    }
}

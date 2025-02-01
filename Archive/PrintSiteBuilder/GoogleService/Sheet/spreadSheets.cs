using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder.GoogleService.Sheet
{
    public class spreadSheets
    {
        public GoogleApi googleApi;
        public string SheetId;
        public string SheetName;
        public SheetsService SheetsService;
        public Spreadsheet spreadSheet;
        public Google.Apis.Sheets.v4.Data.Sheet workSheet;
        public spreadSheets(string sheetId, string sheetName)
        {
            googleApi = new GoogleApi();
            SheetId = sheetId;
            SheetName = sheetName;
            SheetsService = googleApi.GetSheetsService();
            spreadSheet = SheetsService.Spreadsheets.Get(SheetId).Execute();
            workSheet = spreadSheet.Sheets.FirstOrDefault(sheet => sheet.Properties.Title == SheetName);
        }
        public IList<IList<object>> GetValues()
        {
            ValueRange response = SheetsService.Spreadsheets.Values.Get(SheetId, SheetName).Execute();
            return response.Values;
        }
        /*public void DeleteValues(int StartIndex,int EndIndex)
        {
            DeleteDimensionRequest request = new DeleteDimensionRequest();
            request.Range = new DimensionRange();
            request.Range.StartIndex = StartIndex;
            request.Range.EndIndex = EndIndex;
            SpreadsheetsResource.ValuesResource .DeleteRequest deleteRequest = service.Spreadsheets.Values.Delete(request, spreadsheetId, range);
            deleteRequest.Execute();
        }*/
    }
}

using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Slides.v1;
using Google.Apis.Slides.v1.Data;
using PrintSiteBuilder.Models.Print;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace PrintSiteBuilder.GoogleService.Slide
{
    public class PrintSlidePage
    {
        public Page slidePage;
        public List<PageElement> tableElements;
        public PageElement header;
        public PageElement content;
        private DriveService driveService;
        public PrintSlidePage(Page page)
        {
            slidePage = page;
            tableElements = GetTableElements();
            header = GetHeaderTable();
            content = GetSecondTable();
        }


        public List<PageElement> GetTableElements()
        {
            return slidePage.PageElements.Where(element => element.Table != null).ToList();
        }
        public PageElement GetHeaderTable()
        {
            return tableElements.OrderBy(element => element.Transform.TranslateY).FirstOrDefault();
        }
        public PageElement GetSecondTable()
        {
            return tableElements.OrderBy(element => element.Transform.TranslateY).LastOrDefault();
        }
        public List<Request> GetUpdateCellRequest(CellConfig cellConfig, bool IsEmptyCell)
        {
            var requests = new List<Request>();
            var value = IsEmptyCell ? " " : cellConfig.Value;
            requests.Add(InsertTableText(content, cellConfig.RowNumber, cellConfig.ColumnNumber, value));
            requests.Add(DeleteTableText(content, cellConfig.RowNumber, cellConfig.ColumnNumber));
            requests.Add(InsertTableText(content, cellConfig.RowNumber, cellConfig.ColumnNumber, value));
            return requests;
        }
        private bool IsCellEmpty(PageElement element, int RowNumber, int ColumnNumber)
        {
            var cell = element.Table.TableRows[RowNumber].TableCells[ColumnNumber];
            if (cell.Text != null && cell.Text.TextElements != null)
            {
                return !cell.Text.TextElements.Any(element => element.TextRun != null && !string.IsNullOrEmpty(element.TextRun.Content.Trim()));
            }
            else
            {
                return true;
            }
        }
        public Request DeleteTableText(PageElement element, int RowNumber, int ColumnNumber)
        {
            var deleteRequest = new Request();
            deleteRequest.DeleteText = new DeleteTextRequest();
            deleteRequest.DeleteText.ObjectId = element.ObjectId;
            deleteRequest.DeleteText.CellLocation = new TableCellLocation();
            deleteRequest.DeleteText.CellLocation.RowIndex = RowNumber;
            deleteRequest.DeleteText.CellLocation.ColumnIndex = ColumnNumber;
            deleteRequest.DeleteText.TextRange = new Google.Apis.Slides.v1.Data.Range();
            deleteRequest.DeleteText.TextRange.Type = "ALL";
            return deleteRequest;
        }
        public Request InsertTableText(PageElement element, int RowNumber, int ColumnNumber, string insertText)
        {
            var insertRequest = new Request();
            insertRequest.InsertText = new InsertTextRequest();
            insertRequest.InsertText.ObjectId = element.ObjectId;
            insertRequest.InsertText.Text = insertText;
            insertRequest.InsertText.CellLocation = new TableCellLocation();
            insertRequest.InsertText.CellLocation.RowIndex = RowNumber;
            insertRequest.InsertText.CellLocation.ColumnIndex = ColumnNumber;
            return insertRequest;
        }
        public List<Request> GetUpdateHeaderRequest(string PrintType, string PrintTitle, int ProblemCount)
        {
            var requests = new List<Request>();
            requests.AddRange(GetUpdatePrintTypeRequest(PrintType));
            requests.AddRange(GetUpdateTitleRequest(PrintTitle));
            requests.AddRange(GetUpdateScoreRequest(ProblemCount));
            return requests;
        }
        public List<Request> GetUpdateTitleRequest(string PrintTitle)
        {
            var requests = new List<Request>();
            requests.Add(DeleteTableText(header, 0, 1));
            requests.Add(InsertTableText(header, 0, 1, PrintTitle));
            return requests;
        }
        public List<Request> GetUpdatePrintTypeRequest(string PrintType)
        {
            var requests = new List<Request>();
            requests.Add(DeleteTableText(header, 0, 0));
            requests.Add(InsertTableText(header, 0, 0, PrintType));
            return requests;
        }
        public List<Request> GetUpdateScoreRequest(int ProblemCount)
        {
            var requests = new List<Request>();
            requests.Add(DeleteTableText(header, 0, 2));
            requests.Add(InsertTableText(header, 0, 2, $" / {ProblemCount}"));
            return requests;
        }
    }
}

namespace PrintSiteBuilder2409.IExternal
{
    public interface IExplorer
    {
        public Task DownloadFromUrl(string Url, string outputPath);
        public void CreateQr(string PrintId, int PageNumber, string QrDir, string Uuid);
    }
}

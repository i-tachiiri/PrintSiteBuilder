using BlogDomain.Config;
using ExplorerLibrary.Repository;
using ImageMagick;

namespace BlogGenerator.Services
{
    public class CoverGenerator
    {
        private ExplorerRepository repository;
        public CoverGenerator(ExplorerRepository repository) 
        {
            this.repository = repository;
        }
        public async Task ConvertToWebp()
        {
            try
            {
                var BaseImagePaths = repository.GetAllVectors();
                foreach (var BaseImagePath in BaseImagePaths)
                {
                    using (var originalImage = new MagickImage(BaseImagePath, DomainConstants.Webp.settings))
                    {
                        using (var resizedImage = (MagickImage)originalImage.Clone())
                        {
                            var Width = resizedImage.Width > resizedImage.Height ? 640 : 0;
                            var Height = resizedImage.Width < resizedImage.Height ? 640 : 0;
                            resizedImage.Quality = DomainConstants.Webp.quality;
                            resizedImage.Density = new Density(DomainConstants.Webp.density);
                            resizedImage.Depth = DomainConstants.Webp.depth;
                            resizedImage.FilterType = DomainConstants.Webp.filterType;
                            resizedImage.Resize((uint)Width, (uint)Height);
                            string outputPath = $@"{DomainConstants.Explorer.BlogWebpFolder}\{Path.GetFileNameWithoutExtension(BaseImagePath)}.{DomainConstants.Webp.extention}";
                            resizedImage.Write(outputPath, DomainConstants.Webp.format);
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("convert format error");
            }
        }
    }
}

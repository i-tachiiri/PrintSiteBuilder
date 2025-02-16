using GoogleSlideLibrary.Repository.Page;
using GoogleSlideLibrary.Repository.Print;
using TempriDomain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace PrintGenerater.Factories
{
    public class PrintFactory
    {
        private readonly IServiceProvider serviceProvider;
        public PrintFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider; // Use DI container to manage instances
        }
        public IPrintEntity CreateInstance(int printId)
        {
            return printId switch
            {
                100007 => serviceProvider.GetRequiredService<Print100007>(),
                _ => throw new ArgumentException("Invalid print ID")
            };
        }
    }
}

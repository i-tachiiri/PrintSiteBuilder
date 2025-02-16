using GoogleSlideLibrary.Repository.Page;
using GoogleSlideLibrary.Repository.Print;
using TempriDomain.Interfaces;

namespace PrintGenerater.Factories
{
    public class PrintFactory
    {
        public static IPrint CreatePrint(int printId)
        {
            return printId switch
            {
                100007 => new Print100007(),
                _ => throw new ArgumentException("Invalid print ID")
            };
        }
    }
}

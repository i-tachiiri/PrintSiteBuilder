using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintSiteBuilder.GoogleService.Drive;
using PrintSiteBuilder.Print2.Item;

namespace PrintSiteBuilder.Interfaces
{
    public static class PrintFactory2
    {
        public static readonly Dictionary<string, Type> ClassNameWithClass = new Dictionary<string, Type>
        {
            { "100マス計算「1桁の足し算」のプリント", typeof(P100003) },
            { "[検証]100マス計算「1桁の足し算」のプリント", typeof(P100004) },
            { "100マス計算「10までの引き算」のプリント", typeof(P100005) },
            { "100マス計算「1桁のかけ算」のプリント", typeof(P100006) },
        };

        public static async Task<IPrint2> GetPrintClass(string type)
        {
            if (ClassNameWithClass.TryGetValue(type, out var contentType))
            {
                var instance = (IPrint2)Activator.CreateInstance(contentType);
                await instance.InitializeAsync(type);
                return instance;
            }
            throw new ArgumentException("PrintFactoryにクラスを追加していません。");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintSiteBuilder.Print.かけ算;
using PrintSiteBuilder.Print.九九;
using PrintSiteBuilder.Print.割り算;
using PrintSiteBuilder.Print.引き算;
using PrintSiteBuilder.Print.数の性質;
using PrintSiteBuilder.Print.足し算;

namespace PrintSiteBuilder.Interfaces
{
    public static class PrintFactory
    {
        public static readonly Dictionary<string, Type> ClassNameWithClass = new Dictionary<string, Type>
    {
        { "一桁の足し算(ランダム)", typeof(一桁の足し算_ランダム) },
        { "一桁の足し算(繰り上がり有)", typeof(一桁の足し算_繰り上がり有) },
        { "一桁の足し算(繰り上がり無)", typeof(一桁の足し算_繰り上がり無) },
        { "足し算(たす1)", typeof(足し算_たす1) },
        { "足し算(たす2)", typeof(足し算_たす2) },
        { "足し算(たす3)", typeof(足し算_たす3) },
        { "足し算(たす3まで)", typeof(足し算_たす3まで) },
        { "足し算(たす4)", typeof(足し算_たす4) },
        { "足し算(たす5)", typeof(足し算_たす5) },
        { "足し算(たす5まで)", typeof(足し算_たす5まで) },
        { "足し算(たす6)", typeof(足し算_たす6) },
        { "足し算(たす7)", typeof(足し算_たす7) },
        { "足し算(たす8)", typeof(足し算_たす8) },
        { "足し算(たす9)", typeof(足し算_たす9) },
        { "足し算(たす10)", typeof(足し算_たす10) },
        { "足し算(たす10まで)", typeof(足し算_たす10まで) },
        { "足し算(たす11)", typeof(足し算_たす11) },
        { "足し算(たす12)", typeof(足し算_たす12) },
        { "足し算(たす13)", typeof(足し算_たす13) },
        { "足し算(たす14)", typeof(足し算_たす14) },
        { "足し算(たす15)", typeof(足し算_たす15) },
        { "足し算(たす16)", typeof(足し算_たす16) },
        { "足し算(たす17)", typeof(足し算_たす17) },
        { "足し算(たす18)", typeof(足し算_たす18) },
        { "足し算(たす19)", typeof(足し算_たす19) },
        { "足し算(たす20)", typeof(足し算_たす20) },
        { "約数", typeof(約数) },
        { "公約数・最大公約数", typeof(公約数_最大公約数) },
        { "公約数の文章題", typeof(公約数の文章題) },
        { "倍数", typeof(倍数) },
        { "公倍数・最小公倍数", typeof(公倍数_最小公倍数) },
        { "四つの分数の割り算", typeof(四つの分数の割り算) },
        { "三つの分数の割り算", typeof(三つの分数の割り算) },
        { "二つの分数の割り算", typeof(二つの分数の割り算) },
        { "分数の割り算テンプレ", typeof(分数の割り算テンプレ) },
        { "一桁の引き算", typeof(一桁の引き算) },
        {  "20までの引き算(繰り下がり無)", typeof(二十までの引き算_繰り下がり無) },
        {  "20までの引き算(繰り下がり有)", typeof(二十までの引き算_繰り下がり有) },
        {  "引き算(ひく1)", typeof(引き算_ひく1) },
        {  "引き算(ひく2)", typeof(引き算_ひく2) },
        {  "引き算(ひく3)", typeof(引き算_ひく3) },
        {  "引き算(ひく3まで)", typeof(引き算_ひく3まで) },
        {  "引き算(ひく5まで)", typeof(引き算_ひく5まで) },
        {  "引き算(ひく10まで)", typeof(引き算_ひく10まで) },
        {  "一桁の足し算(100マス計算)", typeof(一桁の足し算_100マス計算) },
        {  "20までの引き算(100マス計算)", typeof(二十までの引き算_100マス計算) },
        {  "筆算(一桁の足し算)", typeof(筆算_一桁の足し算) },
        {  "筆算(二桁の足し算)", typeof(筆算_二桁の足し算) },
        {  "筆算(三桁の足し算)", typeof(筆算_三桁の足し算) },
        {  "九九(1の段)", typeof(九九_1の段) },
        {  "九九(2の段)", typeof(九九_2の段) },
        {  "九九(3の段)", typeof(九九_3の段) },
        {  "九九(4の段)", typeof(九九_4の段) },
        {  "九九(5の段)", typeof(九九_5の段) },
        {  "九九(6の段)", typeof(九九_6の段) },
        {  "九九(7の段)", typeof(九九_7の段) },
        {  "九九(8の段)", typeof(九九_8の段) },
        {  "九九(9の段)", typeof(九九_9の段) },
        {  "2桁と1桁のかけ算", typeof(二桁と一桁のかけ算) },
        {  "3桁と1桁のかけ算", typeof(三桁と一桁のかけ算) },
        {  "4桁と1桁のかけ算", typeof(四桁と一桁のかけ算) },
    };

        public static IPrint GetPrintClass(string type)
        {
            if (ClassNameWithClass.TryGetValue(type, out var contentType))
            {
                return (IPrint)Activator.CreateInstance(contentType);
            }
            throw new ArgumentException("PrintFactoryにクラスを追加していません。");
        }
    }
}

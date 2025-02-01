using System.Data;
using System.DirectoryServices;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System;
using PrintSiteBuilder.Utilities;
using PrintSiteBuilder.SiteItem;
using PrintSiteBuilder.Print.数の性質;
using PrintSiteBuilder.Interfaces;
using PdfSharp.Drawing.BarCodes;
using PrintSiteBuilder.Models.General;
using PrintSiteBuilder.GoogleService.Drive;
using PrintSiteBuilder.GoogleService.Slide;
using PrintSiteBuilder.AmazonService;
using FikaAmazonAPI.Services;
using PrintSiteBuilder.Archive;

namespace PrintSiteBuilder
{
    public partial class PrintGenerator : Form
    {
        private IPrint2 iPrint;
        public PrintGenerator()
        {
            InitializeComponent();
#if !DEBUG
            this.BackColor = SystemColors.ActiveCaption;
            tabPage1.BackColor = SystemColors.ActiveCaption;
#endif
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            FactoryClasses2.DataSource = PrintFactory2.ClassNameWithClass.Keys.Reverse().ToList();
            FactoryClasses2.Text = PrintFactory2.ClassNameWithClass.Keys.Reverse().ToList().FirstOrDefault();
        }
        private async void TestExecute(object sender, EventArgs e)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]test start...");
            var iPrint = await PrintFactory2.GetPrintClass(FactoryClasses2.Text);
            var json = new Json2();

            var catelogItems = new listingItems(iPrint);
            var xxx = await catelogItems.PutListingsItem();

            var a = 1;
        }

        private async void RenewButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]Googleスライド取得中...");
            var iPrint = await PrintFactory2.GetPrintClass(FactoryClasses2.Text);
            var item = new Item2();
            var json = new Json2();
            if (IsRegisterItem.Checked)
            {
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]register item to amazon...");
                var listingItem = new listingItems(iPrint);
                await listingItem.PutListingsItem();
            }
            if (IsSyncTemplate.Checked)
            {
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]update config...");
                var pages = new SlidePages(iPrint.PrintSlideId);
                pages.SyncTemplateSlide(iPrint);
            }
            if (IsBarcodeExport.Checked)
            {
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]create barcode...");
                var barcode = new barcode();
                barcode.GenerateBarcode(iPrint);
            }
            var slidePages = new PrintSlidePages(iPrint.PrintSlideId);
            ItemsConfig itemsConfig;
            if (IsUpdateConfig2.Checked)
            {
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]create config_slide...");
                itemsConfig = item.GetItemsConfig(IsUpdateAllConfig.Checked, iPrint);
                json.SerializeItemsConfig(itemsConfig, iPrint);
            }
            if (IsUpdateSlide2.Checked)
            {
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]update slide...");
                slidePages.UpdateSlide(iPrint);
            }
            if (IsCoverUpdate.Checked)
            {
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]update cover...");

            }
            if (IsExportSvg2.Checked)
            {
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]export svg...");
                var slide = new ExportSlide();
                await slide.ExportPrintImages(iPrint, "svg");
            }
            if (IsUpdateConfig2.Checked)
            {
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]update config...");
                itemsConfig = item.GetItemsConfig(IsUpdateAllConfig.Checked, iPrint);
                json.SerializeItemsConfig(itemsConfig, iPrint);
            }
            if (!File.Exists(iPrint.path.PrintConfig))
            {
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]stopped because of no item config.");
                return;
            }
            itemsConfig = json.DeserializeItemsConfig(iPrint);
            var image = new Image2(itemsConfig, iPrint);
            var pdf = new Pdf(itemsConfig, iPrint);
            var php = new Php(itemsConfig, iPrint);
            var uuid = new Uuid(iPrint);
            if (IsExportLogo.Checked)
            {
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]export logo...");
                image.CreateQr(true, itemsConfig);
                image.CreateAttachedLogoAndQr(true, itemsConfig);
            }
            if (IsCreateSvg.Checked)
            {
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]create svg...");
                image.CreateSvgs();
            }
            if (IsExportImage.Checked)
            {
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]export image...");
                image.CreateImages(true);
            }
            if (IsExportPdf.Checked)
            {
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]export image...");
                pdf.CreatePdf();
            }
            if (IsExportApplePdf.Checked)
            {
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]export image...");
                pdf.CreatePdfForApple();
            }
            if (IsExportAnswer4Pdf.Checked)
            {
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]create answer svg...");
                pdf.CreateGroupedImage();
            }
            if (IsPhpExport.Checked)
            {
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]export php...");
                php.CreatePhps(true);
            }
            if (IsExportUuid.Checked)
            {
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]export uuid...");
                uuid.CreateUuidFiles();
            }
            if (IsCoverUpdate.Checked)
            {
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]update cover...");
                var coverSlide = new CoverSlidePages(iPrint.CoverSlideId);
                await coverSlide.UpdateCoverSlide(iPrint);
            }
            if (IsAmazonImageUpdate.Checked)
            {
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]update amazon slide...");
                var amazonSlide = new AmazonSlidePages(iPrint.AmazonSlideId);
                await amazonSlide.UpdateAndExportAmazonSlide(iPrint);
            }
            if (IsGoodsExport.Checked)
            {
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]export goods pdf...");
                pdf.CreateQuestionGoods();
            }
            if (IsFtpUpload2.Checked)
            {
                var bat = new Bat();
                bat.RunBat2("ftp-sync-template.txt", true, iPrint.PrintId);
            }


            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]完了！");
            MessageBox.Show($"[{DateTime.Now.ToString("HH:mm:ss")}]完了！");
        }

        private void ButtonTest_Click(object sender, EventArgs e)
        {

        }

        private void OpenSlidePrint(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = $"https://docs.google.com/presentation/d/{iPrint.PrintSlideId}",
                UseShellExecute = true
            });
        }

        private async void SelectPrint(object sender, EventArgs e)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]sync class...");
            RenewButton.Enabled = false;
            iPrint = await PrintFactory2.GetPrintClass(FactoryClasses2.Text);

            var IsGoodsRegisterDone = string.IsNullOrEmpty(iPrint.FnSku);
            var IsSetTemplateDone = 





            RenewButton.Enabled = !File.Exists($@"{iPrint.path.PrintGoodsDir}\goods.pdf");
            SearchWords.Text = iPrint.Keywords;
            ItemName.Text = iPrint.PrintName;
            MakerNumber.Text = iPrint.PrintId;
            SalesPrice.Text = "1000";// iPrint.Price;
            PrintDescription.Text = iPrint.Description;
            Sku.Text = iPrint.Sku;
            Asin.Text = iPrint.Asin;
            Fnsku.Text = iPrint.FnSku;
        }
        private async void StatusCheck()
        {
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]check status...");
            
        }

        private void Check1(object sender, EventArgs e)
        {
            IsBarcodeExport.Checked = check1.Checked;
            IsSyncTemplate.Checked = check1.Checked;
        }

        private void Check2(object sender, EventArgs e)
        {
            IsUpdateSlide2.Checked = check2.Checked;
            IsExportSvg2.Checked = check2.Checked;
            IsUpdateConfig2.Checked = check2.Checked;
        }

        private void Check3(object sender, EventArgs e)
        {
            IsExportLogo.Checked = check3.Checked;
            IsCreateSvg.Checked = check3.Checked;
            IsExportImage.Checked = check3.Checked;
            IsExportPdf.Checked = check3.Checked;
            IsExportApplePdf.Checked = check3.Checked;
            IsExportAnswer4Pdf.Checked = check3.Checked;
            IsPhpExport.Checked = check3.Checked;
            IsExportUuid.Checked = check3.Checked;
            IsFtpUpload2.Checked = check3.Checked;
        }

        private void Check4(object sender, EventArgs e)
        {
            IsCoverUpdate.Checked = check4.Checked;
            IsAmazonImageUpdate.Checked = check4.Checked;
            IsGoodsExport.Checked = check4.Checked;
        }
        private void CheckIgnoreLock(object sender, EventArgs e)
        {
            RenewButton.Enabled = IsIgnoreLock.Checked;
        }

        private void AmazonInventoryLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = $"https://sellercentral-japan.amazon.com/myinventory/inventory?fulfilledBy=all&page=1&pageSize=25&sort=date_created_desc&status=all",
                UseShellExecute = true
            });
        }
    }
}

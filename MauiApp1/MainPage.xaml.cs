
using MysqlLibrary.Repository.Crypto;
/*using OxyPlot.Axes;
using OxyPlot.Maui.Skia;
using OxyPlot.Series;
using OxyPlot;*/


namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        private MysqlPublicTradeRepository mysqlPublicTradeRepository;
        public MainPage()
        {
            InitializeComponent();
            //LoadData();
        }
        /*private async void LoadData()
        {
            // PlotModelの設定
            var plotModel = new PlotModel { Title = "Sample Line Chart" };

            // X軸とY軸の設定
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X Axis" });
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Y Axis" });

            // データシリーズの設定
            var lineSeries = new LineSeries
            {
                Title = "Data Series",
                MarkerType = MarkerType.Circle
            };

            // データの追加
            lineSeries.Points.Add(new DataPoint(0, 3));
            lineSeries.Points.Add(new DataPoint(1, 5));
            lineSeries.Points.Add(new DataPoint(2, 7));
            lineSeries.Points.Add(new DataPoint(3, 4));
            lineSeries.Points.Add(new DataPoint(4, 8));
            lineSeries.Points.Add(new DataPoint(5, 6));
            lineSeries.Points.Add(new DataPoint(6, 2));

            // PlotModelにシリーズを追加
            plotModel.Series.Add(lineSeries);

            // PlotViewにPlotModelをバインド
            //plotView.Model = plotModel;
        }*/
    }
}

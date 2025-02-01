using PrintSiteBuilder2409.Services;
namespace PrintSiteBuilder2409
{
    public partial class Form1 : Form
    {
        private readonly P100004 p100マス計算;
        public Form1(P100004 p100マス計算)
        {
            InitializeComponent();
            this.p100マス計算 = p100マス計算;
        }

        private async void RenewButton_Click(object sender, EventArgs e)
        {
            //var xxx = await TemplateMasterService.GetTemplateByIdAsync("T100001");
            await p100マス計算.RenewExerciseMaster();
            //dataGridView1.DataSource = new List<object> { xxx };
            MessageBox.Show("done!");
        }
    }
}

using PrintSiteBuilder2409.Services;
namespace PrintSiteBuilder2409
{
    public partial class Form1 : Form
    {
        private readonly P100004 p100�}�X�v�Z;
        public Form1(P100004 p100�}�X�v�Z)
        {
            InitializeComponent();
            this.p100�}�X�v�Z = p100�}�X�v�Z;
        }

        private async void RenewButton_Click(object sender, EventArgs e)
        {
            //var xxx = await TemplateMasterService.GetTemplateByIdAsync("T100001");
            await p100�}�X�v�Z.RenewExerciseMaster();
            //dataGridView1.DataSource = new List<object> { xxx };
            MessageBox.Show("done!");
        }
    }
}

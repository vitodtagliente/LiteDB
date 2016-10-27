using System;
using System.Windows.Forms;
using LiteDB;
using LiteDB.Forms;

namespace DebugApplication
{
    public partial class AdapterForm : Form
    {
        LiteFormAdapter<Prova> adapter;

        public AdapterForm()
        {
            InitializeComponent();

            adapter = new LiteFormAdapter<Prova>();
            adapter.BindControl("Nome", txt_nome);
            adapter.BindControl("Id", numericUpDown1);
            adapter.BindControl("Data", dateTimePicker1);
        }

        private void AdapterForm_Load(object sender, EventArgs e)
        {
            var db = LiteDatabase.singleton;
            var model = db.Find<Prova>(1);

            adapter.Read(model);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var model = adapter.Write();
            MessageBox.Show(model.ToString());
        }
    }
}

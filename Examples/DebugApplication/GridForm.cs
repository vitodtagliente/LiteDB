using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiteDB;

namespace DebugApplication
{
    public partial class GridForm : Form
    {
        public GridForm()
        {
            InitializeComponent();
        }

        private void GridForm_Load(object sender, EventArgs e)
        {
            var db = LiteDatabase.singleton;

            var dataTable = new DataTable();
            var sqlAdatper = new SQLiteDataAdapter("SELECT * FROM " + LiteSchema<Prova>.Name(), db.Connection);
            sqlAdatper.Fill(dataTable);
            dataGridView1.DataSource = dataTable;

            sqlAdatper.Dispose();
            
        }
    }
}

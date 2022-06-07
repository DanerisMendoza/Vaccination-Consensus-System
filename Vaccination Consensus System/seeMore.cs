using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Vaccination_Consensus_System
{
    public partial class seeMore : Form
    {
       MySqlConnection connection = new MySqlConnection("datasource=localhost;port=3306; database=vaccination_system; username=root;password=");
        
        public seeMore(int key)
        {
            InitializeComponent();
            connection.Open();
            string LoadUserGrid = "select * from vaccination_system.boosterdose where linkedid = '"+key+"'";
            MySqlCommand command = new MySqlCommand(LoadUserGrid, connection);
            DataTable dataSet = new DataTable();
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
            dataAdapter.Fill(dataSet);
            BindingSource bSource = new BindingSource();
            bSource.DataSource = dataSet;
            dataGridView1.DataSource = bSource;
            dataAdapter.Update(dataSet);
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[2].Visible = false;
            }
            connection.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}

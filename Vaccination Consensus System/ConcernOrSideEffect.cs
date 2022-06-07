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
    public partial class ConcernOrSideEffect : Form
    {
        public ConcernOrSideEffect()
        {
            InitializeComponent();
            loadConcernSideEffectGrid();
            dataGridView1.ReadOnly = true;
        }
        bool allowQuery = false;

        public void loadConcernSideEffectGrid() {
            MySqlConnection connection = new MySqlConnection("datasource=localhost;port=3306;username=root;password=");
            connection.Open();
            string LoadUserGrid = "select * from vaccination_system.concernorsideeffect";
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
            }
            connection.Close();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            dataGridView1.ReadOnly = false;
            allowQuery = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //add
            if (allowQuery == true)
            {
                try
                {
                    MySqlConnection connection1 = new MySqlConnection("datasource=localhost;port=3306;username=root;password=''");
                    connection1.Open();

                    string insertQuery = "INSERT INTO vaccination_system.concernorsideeffect(NAME,CONCERN) VALUES('" + dataGridView1.CurrentRow.Cells[1].Value.ToString() + "','" + dataGridView1.CurrentRow.Cells[2].Value.ToString() + "');";
                    MySqlCommand command = new MySqlCommand(insertQuery, connection1);
                    if (command.ExecuteNonQuery() == 1)
                    {
                        MessageBox.Show("Query Executed");
                    }
                    else
                    {
                        MessageBox.Show("Query Not Executed");
                    }
                    connection1.Close();
                }
                catch (Exception) { MessageBox.Show("Database error!"); }
            }
            else {
                MessageBox.Show("Please Unlock first!");
                return;
            }
    }

        private void button11_Click(object sender, EventArgs e)
        {
            dataGridView1.ReadOnly = true;
            allowQuery = false;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (allowQuery == true)
            {
                MySqlConnection connection1 = new MySqlConnection("datasource=localhost;port=3306;username=root;password=''");
                connection1.Open();
                string deleteQuery = "delete from vaccination_system.concernorsideeffect where id = '" + dataGridView1.CurrentRow.Cells[0].Value.ToString() + "'  ";
                MySqlCommand command = new MySqlCommand(deleteQuery, connection1);
                if (command.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Query Executed");
                    loadConcernSideEffectGrid();
                }
                else
                {
                    MessageBox.Show("Query Not Executed");
                }
                connection1.Close();
            }
            else {
                MessageBox.Show("Please unlock first!");
                return;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (allowQuery == true)
            {
                MySqlConnection connection1 = new MySqlConnection("datasource=localhost;port=3306;username=root;password=''");
                connection1.Open();

                String updateQuery = "UPDATE vaccination_system.concernorsideeffect SET name = '" + dataGridView1.CurrentRow.Cells[1].Value.ToString() + "', concern = '" + dataGridView1.CurrentRow.Cells[2].Value.ToString() + "'WHERE id = '" + dataGridView1.CurrentRow.Cells[0].Value.ToString() + "'";
                MySqlCommand command = new MySqlCommand(updateQuery, connection1);
                if (command.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Query Executed");
                }
                else
                {
                    MessageBox.Show("Query Not Executed");
                }
                connection1.Close();

            }
            else
            {
                MessageBox.Show("Please unlock first!");
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (allowQuery == true)
            {
                MySqlConnection connection1 = new MySqlConnection("datasource=localhost;port=3306;username=root;password=''");
                connection1.Open();
                string deleteQuery = "delete from vaccination_system.concernorsideeffect ";
                MySqlCommand command = new MySqlCommand(deleteQuery, connection1);
                command.ExecuteNonQuery();            
                loadConcernSideEffectGrid();
                connection1.Close();
            }
            else
            {
                MessageBox.Show("Please unlock first!");
                return;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            admin ad = new admin();
            ad.Show();
            this.Hide();
        }
}

    }
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
using System.IO;

namespace Vaccination_Consensus_System
{
    public partial class admin : Form
    {
        public admin()
        {
            InitializeComponent();         
            datagrid();
            pieGraph();
            pieGraph2();
            updateTotalPopulation();         
            barangayPopulationGrid();
            totalVaccinated();
            defaultPic();
      
        }
        int partiallyVaccinated = 0, fullyVaccinated = 0, assumptionPopulation = 0;
        int barangay = 1;
        int barangayPopulation = 0;
        Boolean tbExist = false;
        int uv2;
        byte[] image1;
        MySqlConnection connection = new MySqlConnection("datasource=localhost;port=3306; database=vaccination_system; username=root;password=;Convert Zero Datetime=True");
       
        public void defaultPic() {
            if (dgv1.Rows.Count == 0)
                return;
            string findPic = "select *  from users where fname = '" + dgv1.Rows[0].Cells[3].Value.ToString() + "';";
            connection.Open();
            MySqlCommand command = new MySqlCommand(findPic, connection);
            DataTable table = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter(command);
            da.Fill(table);
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
            MySqlDataReader reader;
            int i = Convert.ToInt32(table.Rows.Count.ToString());
            if (i == 0)
            {
                connection.Close();
                return;
            }
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                tbExist = true;
                if (table.Rows[0][17] is System.DBNull)
                {
                    pictureBox1.Image = null;
                    da.Dispose();
                    connection.Close();
                    return;
                }
                else
                {
                    byte[] img = (byte[])table.Rows[0][17];
                    MemoryStream ms = new MemoryStream(img);
                    pictureBox1.Image = Image.FromStream(ms);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    
                }
            }
            da.Dispose();
            connection.Close();
        }
        public void assignValue() {
            connection.Open();
            string countQueryPartiallyVaccinated = "select count(isFullyVaccinated) from vaccination_system.users where (isFullyVaccinated = 0) and barangay = '" + barangay + "';";

            string countQueryFullyVaccinated = "select count(isFullyVaccinated) from vaccination_system.users where (isFullyVaccinated = 1) and barangay = '" + barangay + "';";
            MySqlCommand command = new MySqlCommand(countQueryFullyVaccinated, connection);
            fullyVaccinated = Convert.ToInt32(command.ExecuteScalar());

            command = new MySqlCommand(countQueryPartiallyVaccinated, connection);
            partiallyVaccinated = Convert.ToInt32(command.ExecuteScalar());

            string countThisBarangayPopulation = "select barangayPopulationForThisBarangay from vaccination_system.barangayspopulation where barangay = '" + barangay + "';";
            command = new MySqlCommand(countThisBarangayPopulation, connection);
            barangayPopulation = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
        }

        
        public void totalVaccinated() {

            string countQueryFullyVaccinated = "select count(isFullyVaccinated) from vaccination_system.users where (isFullyVaccinated = 1);";
            string countPartially = "select count(isFullyVaccinated) from vaccination_system.users where (isFullyVaccinated = 0);";

            connection.Open();
            MySqlCommand command = new MySqlCommand(countQueryFullyVaccinated, connection);
            fullyVaccinated = Convert.ToInt32(command.ExecuteScalar());

            command = new MySqlCommand(countPartially, connection);
            partiallyVaccinated = Convert.ToInt32(command.ExecuteScalar());
           
            int total = partiallyVaccinated + fullyVaccinated;
            numberOfPeopleWithVaccine.Text = Convert.ToString(total);
            connection.Close();
        }      
            //total population
            private void updateTotalPopulation() {
            MySqlConnection connection = new MySqlConnection("datasource=localhost;port=3306;username=root;password=");
            connection.Open();
            //population for each barangay
            string countEachBarangayPopulation = "select sum(barangaypopulationforthisbarangay) from vaccination_system.barangayspopulation;";
            MySqlCommand command = new MySqlCommand(countEachBarangayPopulation, connection);
            int sumOfEachBarangayPopulation = Convert.ToInt32(command.ExecuteScalar());          
            //total population            
            string countQueryAssumptionPopulation = "update vaccination_system.totalPopulation set tp = '"+sumOfEachBarangayPopulation+"'";
            command = new MySqlCommand(countQueryAssumptionPopulation, connection);
            assumptionPopulation = Convert.ToInt32(command.ExecuteScalar());
            totalPopulation.Text = "Total Population for all barangay: "+Convert.ToString(sumOfEachBarangayPopulation);            
            connection.Close();
        }
        //pie graph 1
        private void pieGraph()
        {
                connection.Close();
                connection.Open();
                //queries
                string countQueryFullyVaccinated = "select count(isFullyVaccinated) from vaccination_system.users where (isFullyVaccinated = 1);";
                string countQueryPartiallyVaccinated = "select count(isFullyVaccinated) from vaccination_system.users where (isFullyVaccinated = 0) ;";
                string countQueryAssumptionPopulation = "select tp from vaccination_system.totalpopulation;";
                
                //execution
                MySqlCommand command = new MySqlCommand(countQueryFullyVaccinated, connection);
                fullyVaccinated = Convert.ToInt32(command.ExecuteScalar());

                command = new MySqlCommand(countQueryPartiallyVaccinated, connection);
                partiallyVaccinated = Convert.ToInt32(command.ExecuteScalar());

                command = new MySqlCommand(countQueryAssumptionPopulation, connection);
                assumptionPopulation= Convert.ToInt32(command.ExecuteScalar());
                               
                    //assigning graph value
                    chart1.Series["s1"].Points.Clear();
               
                    int uv2 = assumptionPopulation - (partiallyVaccinated + fullyVaccinated);
                    chart1.Series["s1"].Points.AddXY("unvaccinated", uv2);
                    uv.Text = "Unvaccinated: " + uv2;
                    if (uv2 == 0) {
                        chart1.Series["s1"].Points[0].IsVisibleInLegend = false;
                        chart1.Series["s1"].Points[0].Label = " ";
                    }
                               
                    chart1.Series["s1"].Points.AddXY("partially vaccinated", partiallyVaccinated);
                    pv.Text = "Partially Vaccinated: "+partiallyVaccinated;
                    if (partiallyVaccinated == 0) {
                        chart1.Series["s1"].Points[1].IsVisibleInLegend = false;
                        chart1.Series["s1"].Points[1].Label = " ";
                        pv.Text = "Partially Vaccinated: 0";
                    }
                             
                    chart1.Series["s1"].Points.AddXY("fully vaccinated", fullyVaccinated);
                    fv.Text = "Fully Vaccinated: " + fullyVaccinated;
                    if (fullyVaccinated == 0)
                    {
                        chart1.Series["s1"].Points[2].IsVisibleInLegend = false;
                        chart1.Series["s1"].Points[2].Label = " ";
                        fv.Text = "Fully Vaccinated: 0";
                    }                                                
                totalPopulation.Text = "Total Population for all barangay: " + Convert.ToString(assumptionPopulation);
                connection.Close();
            
        }
        private void pieGraph2(){
            MySqlConnection connection = new MySqlConnection("datasource=localhost;port=3306;username=root;password=");
            connection.Open();         
            //query declaration           
            string countQueryFullyVaccinated = "select count(isFullyVaccinated) from vaccination_system.users where (isFullyVaccinated = 1) and barangay = '" + barangay + "';";
            string countQueryPartiallyVaccinated = "select count(isFullyVaccinated) from vaccination_system.users where (isFullyVaccinated = 0) and barangay = '" + barangay + "';";
            string countThisBarangayPopulation = "select barangayPopulationForThisBarangay from vaccination_system.barangayspopulation where barangay = '" + barangay + "';";
            //mysql execution
            MySqlCommand command = new MySqlCommand(countQueryFullyVaccinated, connection);
            fullyVaccinated = Convert.ToInt32(command.ExecuteScalar());

            command = new MySqlCommand(countQueryPartiallyVaccinated, connection);
            partiallyVaccinated = Convert.ToInt32(command.ExecuteScalar());

            command = new MySqlCommand(countThisBarangayPopulation, connection);
            barangayPopulation = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
                     
            chart2.Series["s2"].Points.Clear();          
            int uv2 = barangayPopulation - (fullyVaccinated + partiallyVaccinated); 
            chart2.Series["s2"].Points.AddXY("unvaccinated", uv2);
            uvg2.Text = "Unvaccinated: " + uv2;
            if (uv2 == 0)
            {
              chart2.Series["s2"].Points[0].IsVisibleInLegend = false;
              chart2.Series["s2"].Points[0].Label = " ";
            }
           
                chart2.Series["s2"].Points.AddXY("partially vaccinated", partiallyVaccinated);
                pvg2.Text = "Partially Vaccinated: " + partiallyVaccinated;
                if (partiallyVaccinated == 0) {
                    chart2.Series["s2"].Points[1].IsVisibleInLegend = false;
                    chart2.Series["s2"].Points[1].Label = " ";
                }
                
                chart2.Series["s2"].Points.AddXY("fully vaccinated", fullyVaccinated);
                fvg2.Text = "Fully Vaccinated: " + fullyVaccinated;
                if (fullyVaccinated == 0)
                {
                    chart2.Series["s2"].Points[2].IsVisibleInLegend = false;
                    chart2.Series["s2"].Points[2].Label = " ";
                }                                              
                totalPopulationForThisBarangay.Text = "Total Population for This barangay: "+Convert.ToString(barangayPopulation);
        }
                       
        public void datagrid(){                       
                connection.Open();
                string LoadUserGrid = "select * from vaccination_system.users ";              
                MySqlCommand command = new MySqlCommand(LoadUserGrid, connection);
                DataTable dataSet = new DataTable();
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                dataAdapter.Fill(dataSet);
                BindingSource bSource = new BindingSource();
                bSource.DataSource = dataSet;
                dgv1.DataSource = bSource;
                dataAdapter.Update(dataSet);
                int i = Convert.ToInt32(dataSet.Rows.Count.ToString());
                foreach (DataGridViewRow row in dgv1.Rows)
                {
                    dgv1.Columns[0].Visible = false;
                    dgv1.Columns[17].Visible = false;
                }             
                //datagrid of booster dose
                if (i == 0) {
                    connection.Close();
                    return;
                }
                string loadBoosterDose = "select * from boosterdose where linkedid = '" + dgv1.Rows[0].Cells[15].Value.ToString() + "'";               
                command = new MySqlCommand(loadBoosterDose, connection);
                dataSet = new DataTable();
                dataAdapter = new MySqlDataAdapter(command);
                dataAdapter.Fill(dataSet);
                bSource = new BindingSource();
                bSource.DataSource = dataSet;
                dgv2.DataSource = bSource;
                dataAdapter.Update(dataSet);
                foreach (DataGridViewRow row in dgv2.Rows)
                {
                    dgv2.Columns[0].Visible = false;
                    dgv2.Columns[2].Visible = false;
                }
                connection.Close();                    
        }
        private void barangayPopulationGrid() { 
            //total population for each barangay grid
               
                connection.Open();
                string LoadPopulationEach = "select * from vaccination_system.barangaysPopulation";
                MySqlCommand command = new MySqlCommand(LoadPopulationEach, connection);
                DataTable dataSet = new DataTable();
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                dataAdapter.Fill(dataSet);
                BindingSource bSource = new BindingSource();
                bSource.DataSource = dataSet;
                dataGridView1.DataSource = bSource;
                dataAdapter.Update(dataSet);
                connection.Close();
        }      
     
        private void button2_Click(object sender, EventArgs e)
        {
            pieGraph();
        }

        private void barangayValueChange(object sender, EventArgs e)
        {                  
            connection.Open();
            barangay = Convert.ToInt32(numericUpDown1.Value);
            //query declaration           
            string countQueryFullyVaccinated = "select count(doseCount) from vaccination_system.users where (doseCount = 2 or vaccineType = 'Johnson and Johnsons Janssen') and barangay = '" + barangay+"';";
            string countQueryPartiallyVaccinated = "select count(doseCount) from vaccination_system.users where doseCount = 1 and vaccineType != 'Johnson and Johnsons Janssen' and barangay = '" + barangay + "' ;";
            string countThisBarangayPopulation = "select barangayPopulationForThisBarangay from vaccination_system.barangayspopulation where barangay = '" + barangay + "';";
            //mysql execution
            MySqlCommand command = new MySqlCommand(countQueryFullyVaccinated, connection);
            fullyVaccinated = Convert.ToInt32(command.ExecuteScalar());

            command = new MySqlCommand(countQueryPartiallyVaccinated, connection);
            partiallyVaccinated = Convert.ToInt32(command.ExecuteScalar());

            command = new MySqlCommand(countThisBarangayPopulation, connection);
            barangayPopulation = Convert.ToInt32(command.ExecuteScalar());
            uv2 = (barangayPopulation-(partiallyVaccinated + fullyVaccinated));
           
            uvg2.Text = "Unvaccinated: " + uv2;
            if (barangayPopulation<=0)
            {
                uvg2.Text = "Unvaccinated: 0";
            }
            pvg2.Text = "Partially Vaccinated: " + partiallyVaccinated;
            fvg2.Text = "Fully Vaccinated: " + fullyVaccinated;
            connection.Close();
            pieGraph2();         
        }    
        private void setGraphButton2(object sender, EventArgs e)
        {
                assignValue();         
                //checkIfPopulationForThisBarangayExist update value if population exist                                      
                connection.Open();
                string checkIfbarangayExist = "select barangaypopulationforthisbarangay from vaccination_system.barangayspopulation where barangay = '" + barangay + "'; ";
                MySqlCommand command = new MySqlCommand(checkIfbarangayExist, connection);                                                        
                DataTable dataSet = new DataTable();
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                dataAdapter.Fill(dataSet);
                int i = Convert.ToInt32(dataSet.Rows.Count.ToString());
                //if barangay population dont exist
                if (i == 0)
                {                  
                    int newPopulation = Convert.ToInt32(graph2PopulationSet.Text);
                    int limit = partiallyVaccinated + fullyVaccinated;
                    if(newPopulation >= limit){      
                    string createBarangayPopulation = "insert into vaccination_system.barangayspopulation(barangay,barangayPopulationForThisBarangay) value('" + barangay + "','" + Convert.ToInt32(graph2PopulationSet.Text) + "')  ";
                    command = new MySqlCommand(createBarangayPopulation, connection);
                    command.ExecuteNonQuery();              
                    pieGraph();
                    pieGraph2();
                    updateTotalPopulation();
                    }
                    else
                    {
                        MessageBox.Show("Population should be greater than or equal fully Vaccinated + partially Vaccinated!");
                        connection.Close();
                        return;
                    }
                }
                else
                {
                    int newPopulation = Convert.ToInt32(graph2PopulationSet.Text);
                    int limit = partiallyVaccinated + fullyVaccinated;
                    if (newPopulation >= limit)
                    {
                        string changePopulationForThisBarangay = "update vaccination_system.barangayspopulation set barangayPopulationForThisBarangay = '" + newPopulation + "' where barangay = '" + barangay + "'; ";
                        command = new MySqlCommand(changePopulationForThisBarangay, connection);
                        command.ExecuteNonQuery();
                        updateTotalPopulation();
                        pieGraph();
                    }
                    else {
                        MessageBox.Show("Population should be greater than or equal fully Vaccinated + partially Vaccinated!");
                        connection.Close();
                        return;
                    }
                }             
                pieGraph2();
                updateTotalPopulation();
                connection.Close();
                barangayPopulationGrid();
         
            if (barangayPopulation == (partiallyVaccinated+fullyVaccinated))
            {
                uvg2.Text = "Unvaccinated: 0";
            }
            pieGraph();
            pieGraph2();
        }

        private void logout(object sender, EventArgs e)
        {
            Login lgn = new Login();
            lgn.Show();
            this.Hide();
        }

        private void admin_Load(object sender, EventArgs e)
        {
            totalVaccinated();
            datagrid();        
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (textBox1.Text == textBox2.Text)
            {
                connection.Open();
                string changeAdminPass = "update vaccination_system.admin set adminPassword = '" + textBox2.Text + "' where adminName = 'admin'; ";
                MySqlCommand command = new MySqlCommand(changeAdminPass, connection);
                command.ExecuteNonQuery();
                if (command.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Password Successfully Change");
                }
                else
                {
                    MessageBox.Show("Unsuccessfull");
                }
                connection.Close();

            }
            else
            {
                MessageBox.Show("Please make password same!");
                return;
            }            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ConcernOrSideEffect cos = new ConcernOrSideEffect();
            cos.Show();
            this.Hide();
        }
     
        private void textBox3_TextChanged(object sender, EventArgs e)
        {           
            MySqlDataAdapter da = null;
            DataTable dt;
            connection.Open();
            da = new MySqlDataAdapter("Select * from vaccination_system.users where (fname like'" + textBox3.Text + "%')", connection);               
            dt = new DataTable();
            da.Fill(dt);
            dgv1.DataSource = dt;
            if (dgv1.CurrentCell == null)
            {
                pictureBox1.Image = null;
                connection.Close(); 
                return;
            }
            //pic
            string findPic = "select *  from users where fname = '" + dgv1.CurrentRow.Cells[3].Value.ToString() + "';";
            MySqlCommand command = new MySqlCommand(findPic, connection);
            DataTable table = new DataTable();
            da = new MySqlDataAdapter(command);
            da.Fill(table);
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                if (table.Rows[0][17] is System.DBNull)
                {
                    pictureBox1.Image = null;
                    da.Dispose();
                    connection.Close();
                    return;
                }
                else
                {
                    byte[] img = (byte[])table.Rows[0][17];
                    MemoryStream ms = new MemoryStream(img);
                    pictureBox1.Image = Image.FromStream(ms);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                }
            }
            da.Dispose();
            connection.Close();      
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
      
        private void click(object sender, EventArgs e)
        {
            datagridChange();  
        }

        private void datagridChange() {
            
            string findPic = "select *  from vaccination_system.users where fname = '" + dgv1.CurrentRow.Cells[3].Value.ToString() + "';";
            connection.Open();
            MySqlCommand command = new MySqlCommand(findPic, connection);
            DataTable table = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter(command);
            da.Fill(table);
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                if (table.Rows[0][17] is System.DBNull)
                {
                    pictureBox1.Image = null;
                }
                else
                {
                    byte[] img = (byte[])table.Rows[0][17];
                    MemoryStream ms = new MemoryStream(img);
                    pictureBox1.Image = Image.FromStream(ms);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                }
            }
            else
            {
                pictureBox1.Image = null;
            }
            reader.Close();

            //datagrid of booster dose
            string LoadUserGrid = "select * from vaccination_system.boosterdose where linkedid = '" + dgv1.CurrentRow.Cells[15].Value.ToString() + "'";
            command = new MySqlCommand(LoadUserGrid, connection);
            DataTable dataSet = new DataTable();
            dataAdapter = new MySqlDataAdapter(command);
            dataAdapter.Fill(dataSet);
            BindingSource bSource = new BindingSource();
            bSource.DataSource = dataSet;
            dgv2.DataSource = bSource;
            dataAdapter.Update(dataSet);
            foreach (DataGridViewRow row in dgv2.Rows)
            {
                dgv2.Columns[0].Visible = false;
                dgv2.Columns[2].Visible = false;
            }

            da.Dispose();
            connection.Close();
        }
        private void dgv1_KeyUp(object sender, KeyEventArgs e)
        {
            datagridChange();   
        }

        private void button14_Click(object sender, EventArgs e)
        {
            connection.Open();
            MySqlDataAdapter da = null;
            DataTable dt;
            //da = new MySqlDataAdapter("Select * from users where (firstDoseDate between '" + dateTimePicker1.Value.ToString("MM/dd/yyyy") + "' and '" + dateTimePicker2.Value.ToString("MM/dd/yyyy") + "') or (secondDoseDate between '" + dateTimePicker1.Value.ToString("MM/dd/yyyy") + "' and '" + dateTimePicker2.Value.ToString("MM/dd/yyyy") + "') ", connection);                
            //da = new MySqlDataAdapter("Select * from users where (firstDoseDate >= '" + dateTimePicker1.Value.ToString("MM/dd/yyyy") + "' and firstDoseDate <='" + dateTimePicker2.Value.ToString("MM/dd/yyyy") + "') or (secondDoseDate >= '" + dateTimePicker1.Value.ToString("MM/dd/yyyy") + "' and secondDoseDate <= '" + dateTimePicker2.Value.ToString("MM/dd/yyyy") + "') ", connection);                
            //da = new MySqlDataAdapter("Select * from users where (firstDoseDate between @date1 and @date2) or (secondDoseDate between @date1 and @date2) ", connection);
            da = new MySqlDataAdapter("Select * from users where (firstDoseDate between @date1 and @date2) or (secondDoseDate between @date1 and @date2)", connection);
            //da.SelectCommand.Parameters.AddWithValue("@date1", dateTimePicker1.Value.ToString("MM/dd/yyyy"));
            //da.SelectCommand.Parameters.AddWithValue("@date2", dateTimePicker2.Value.ToString("MM/dd/yyyy"));
            da.SelectCommand.Parameters.AddWithValue("@date1", dateTimePicker1.Value.ToString("yyyy/MM/dd"));
            da.SelectCommand.Parameters.AddWithValue("@date2", dateTimePicker2.Value.ToString("yyyy/MM/dd"));
            dt = new DataTable();
            da.Fill(dt);
            dgv1.DataSource = dt;
            connection.Close();
            if (dgv1.CurrentCell == null)
                pictureBox1.Image = null;
            else defaultPic();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dgv1.CurrentRow.Cells[15].Value == null || tbExist == false)
                return;
            else
            {
                int key = Convert.ToInt32(dgv1.CurrentRow.Cells[15].Value);
                editUserInfo eu = new editUserInfo(key);
                eu.Show();
                this.Close();
                //this.Hide();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            editUserInfo eu = new editUserInfo(0);
            eu.Show();
            this.Hide();
        }
    }
}

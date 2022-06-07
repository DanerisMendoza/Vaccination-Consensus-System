using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MySql.Data.MySqlClient;

namespace Vaccination_Consensus_System
{
    public partial class Form1 : Form
    {   
        public Form1()
        {
            InitializeComponent();
            vaccineLoad();
            connection.Close();
            
        }
        DateTime dt = DateTime.Now;
        string dateOfFirstDose, dateOfSecondDose;
        string bday = "";
        int age1;
        bool dp;
        byte[] img;
    
        int daysInterval, doseNeed1, id;
        string vaccineType;

        MySqlConnection connection = new MySqlConnection("datasource=localhost;port=3306;username=root;password=''");
        
        public void vaccineLoad()
        {
            connection.Open();
            String selectQuery = "select * from vaccination_system.tblvaccine";
            MySqlCommand command = new MySqlCommand(selectQuery, connection);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string vaccine = reader.GetString("vaccineType");             
                cbVaccine.Items.Add(reader.GetString("vaccineType"));
            }
            reader.Close();
            connection.Close();
          

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            // BUTTON CLEAR
            pictureBox2.Image = null;
            barangayCb.Text = "0";
            txtContactno.Text = string.Empty;
            txtEmailadd.Text = string.Empty;
            txtFirstname.Text = string.Empty;
            txtLastname.Text = string.Empty;
            txtMiddlename.Text = string.Empty;
            txtSuffix.Text = string.Empty;
            txtUsername.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtPassword2.Text = string.Empty;
            address.Text = string.Empty;
            lbStatus.SelectedIndex = -1;
            rdoMale.Checked = false;
            rdoFemale.Checked = false;
            cbPriogroup.SelectedIndex = -1;
            cbVaccine.SelectedIndex = -1;
            DaysIntervalTxt.Text = string.Empty;
            DoseNeedTxt1.Text = string.Empty;
        }
        private void cbVaccine_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            
            
            
            vaccine.Text = cbVaccine.Text.ToString();
            string selectDaysInterval = "select vaccineDayInterval from vaccination_system.tblvaccine where vaccineType = '" + cbVaccine.Text + "';";
            string doseNeed = "select doseNeed from vaccination_system.tblvaccine where vaccineType = '" + cbVaccine.Text + "';";
            string getId = "select id from vaccination_system.tblvaccine where vaccineType = '" + cbVaccine.Text + "';";
            
            MySqlConnection connection = new MySqlConnection("datasource=localhost;port=3306;username=root;password=");
            connection.Open();

            MySqlCommand command = new MySqlCommand(doseNeed, connection);
            doseNeed1 = Convert.ToInt32(command.ExecuteScalar());
            if (doseNeed1 != 0)
            DoseNeedTxt1.Text= doseNeed1.ToString();
            
            command = new MySqlCommand(selectDaysInterval, connection);
            daysInterval = Convert.ToInt32(command.ExecuteScalar());
            if (daysInterval != 0)
            {
                DaysIntervalTxt.Text = daysInterval.ToString();
                //set the interval            
                dateOfSecondDose = dt.AddDays(daysInterval).ToString("yyyy/MM/dd");
            }

            if (doseNeed1 == 1)
            {
                DaysIntervalTxt.Text = string.Empty;
                DaysIntervalTxt.Visible = false;
            }
            else {
                
                DaysIntervalTxt.Visible = true;
            }

            
            command = new MySqlCommand(getId, connection);
            id = Convert.ToInt32(command.ExecuteScalar());
           
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            // VALIDATION

            if (txtFirstname.Text == String.Empty)
            {
                MessageBox.Show("Please input your First Name!");
                txtFirstname.Focus();
                return;
            }
            if (txtMiddlename.Text == String.Empty)
            {
                MessageBox.Show("Please input your Middle Name!");
                txtMiddlename.Focus();
                return;
            }

            if (txtLastname.Text == String.Empty)
            {
                MessageBox.Show("Please input your Last Name!");
                txtLastname.Focus();
                return;
            }

            if (txtUsername.Text == String.Empty)
            {
                MessageBox.Show("Please input your Username!");
                txtUsername.Focus();
                return;
            }

            if (txtPassword.Text == String.Empty)
            {
                MessageBox.Show("Please input your Password!");
                txtPassword.Focus();
                return;
            }
            if (txtPassword2.Text == String.Empty)
            {
                MessageBox.Show("Please confirm your Password!");
                txtPassword.Focus();
                return;
            }
            if (dp == false)
            {
                MessageBox.Show("Please input your birthday!");
                dbo.Focus();
                return;
            }
            if (txtEmailadd.Text == String.Empty)
            {
                MessageBox.Show("Please input your Email Address!");
                txtEmailadd.Focus();
                return;
            }
            if (address.Text == String.Empty)
            {
                MessageBox.Show("Please input your Address!");
                address.Focus();
                return;
            }

            if (txtContactno.Text == String.Empty)
            {
                MessageBox.Show("Please input your Number!");
                txtContactno.Focus();
                return;
            }

            if (rdoMale.Checked == false && rdoFemale.Checked == false)
            {
                MessageBox.Show("Please input select your Gender!");
                genderPanel.Focus();
                return;
            }

            if (lbStatus.SelectedIndex == -1)
            {
                MessageBox.Show("Please select your Civil Status!");
                lbStatus.Focus();
                return;
            }

            if (cbPriogroup.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Priority Group!");
                cbPriogroup.Focus();
                return;
            }
            if (cbVaccine.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Type of Vaccine!");
                cbVaccine.Focus();
                return;
            }
            if (barangayCb.Value.ToString() == "")
            {
                MessageBox.Show("Please select barangay!");
                return;
            }
          

            //SETTING VARIABLES VALUE

            string fname = txtFirstname.Text + " " + txtMiddlename.Text + " " + txtLastname.Text + " " + txtSuffix.Text;
            
            
            //username and pass
               
            string username = txtUsername.Text;
            string password;
            if (txtPassword.Text == txtPassword2.Text)
            {
                password = txtPassword2.Text;
            }
            else {
                MessageBox.Show("Please input the same password");
                return;
            }
            
            // RADIO BUTTON GENDER

            string gender3 = string.Empty;

            if (rdoMale.Checked == true)
            {
                gender3 = "Male";
            }
            else if (rdoFemale.Checked == true)
            {
                gender3 = "Female";
            }
           
         
            string status3 = lbStatus.Text;
            string emailadd3 = txtEmailadd.Text;
            string contactno3 = txtContactno.Text;
            int barangay = Convert.ToInt32(barangayCb.Text);
            dateOfFirstDose = dt.ToString("yyyy/MM/dd");
            //dateOfFirstDose = dt.ToShortDateString();
            string prioGroup = cbPriogroup.Text;
            string vaccineType = cbVaccine.Text;
            int doseCount = 1;
            string address1 = address.Text;
        
            //vaccine profile pic
            if (pictureBox2.Image != null)
            {
                MemoryStream ms = new MemoryStream();
                pictureBox2.Image.Save(ms, pictureBox2.Image.RawFormat);
                img = ms.ToArray();
            }
            
           //check if same user exist     
                MySqlConnection connection = new MySqlConnection("datasource=localhost;port=3306;username=root;password=");
                string select = "select * from vaccination_system.users where username = '" + username + "' or fname = '"+fname+"' ";
                connection.Open();            
                MySqlCommand command = new MySqlCommand(select, connection);
                DataTable dataSet = new DataTable();
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                dataAdapter.Fill(dataSet);
                int i = Convert.ToInt32(dataSet.Rows.Count.ToString());
                //acount dont exist
                if (i == 0)
                {
                    //PASSING VALUE
                    if (Convert.ToInt32(DoseNeedTxt1.Text) == 1)
                        dateOfSecondDose = "";
                    Vaccination_Info vacinfo = new Vaccination_Info(username, password, fname, bday, gender3, status3, emailadd3, contactno3, barangay, doseCount, dateOfFirstDose, dateOfSecondDose, prioGroup, vaccineType, age1, img, "", "", address1);
                    vacinfo.Show();
                    this.Hide();
                }
                else {
                    MessageBox.Show("Same user exist in our database!");
                    return;
                }
                connection.Close();
           
        }

        private void txtContactno_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Login lg = new Login();
            lg.Show();
            this.Hide();
        }

        private void dbo_ValueChanged(object sender, EventArgs e)
        {
            dp = true;
            dbo.CustomFormat = "MM/dd/yyyy";
            TimeSpan age = DateTime.Now - dbo.Value;
            int years = DateTime.Now.Year - dbo.Value.Year;
            if (dbo.Value.AddYears(years) > DateTime.Now)
            {
                years--;              
            }
            bday = dbo.Value.Date.ToString("yyyy/MM/dd");
            age1 = years;
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "Choose Image(*.jpg; *.png; *.gif )|*.jpg; *.png *gif";
            if (opf.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.Image = Image.FromFile(opf.FileName);
                pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void addVaccine(object sender, EventArgs e)
        {
            if (vaccine.Text == string.Empty) {
                MessageBox.Show("Please input vaccine name!");
                vaccine.Focus();
                return;
            }
            if (DaysIntervalTxt.Text == string.Empty && Convert.ToInt32(DoseNeedTxt1.Text) != 1)
            {
                MessageBox.Show("Please input days interval!");
                DaysIntervalTxt.Focus();
                return;
            }
            if(DoseNeedTxt1.Text == string.Empty){
                MessageBox.Show("Please input dose need to be fully vaccinated!");
                DoseNeedTxt1.Focus();
                return;
            }

            if ((Convert.ToInt32(DoseNeedTxt1.Text) != 1) && (Convert.ToInt32(DoseNeedTxt1.Text) != 2))
            {
                MessageBox.Show("Please input only 1 or 2");
                DoseNeedTxt1.Text = string.Empty;
                return;
            }
                //check if there is already same vaccine
                
                string findPic = "select *  from vaccination_system.tblvaccine where vaccineType = '" + vaccine.Text + "';";
                MySqlCommand command = new MySqlCommand(findPic, connection);
                
                DataTable table = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(command);
                da.Fill(table);
                int i = Convert.ToInt32(table.Rows.Count.ToString());
              
                if (i == 0)
                {
                    connection.Open();
                    string insertQuery = "INSERT INTO vaccination_system.tblvaccine(vaccineType,vaccineDayInterval,doseneed) VALUES('" + vaccine.Text + "','" + DaysIntervalTxt.Text + "','" + DoseNeedTxt1.Text + "');";
                    command = new MySqlCommand(insertQuery, connection);

                    if (command.ExecuteNonQuery() == 1)
                    {
                      
                        MessageBox.Show("Query Executed");
                        vaccine.Text = null;
                        DaysIntervalTxt.Text = null;
                        DoseNeedTxt1.Text = null;
                        cbVaccine.Items.Clear();
                        connection.Close();
                        vaccineLoad();
                    }
                    else
                    {
                        MessageBox.Show("Query Not Executed");
                    }
                    

                }
                else {
                    MessageBox.Show("Vaccine already exist!");
                    return;
                }
                connection.Close();
        }

        private void deleteVaccineField(object sender, EventArgs e)
        {
           
            connection.Open();
            string deleteQuery = "delete from vaccination_system.tblvaccine where id = '" + id + "';";
            MySqlCommand command = new MySqlCommand(deleteQuery, connection);
            if (command.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Query Executed");
                vaccine.Text = null;
                DaysIntervalTxt.Text = null;
                DoseNeedTxt1.Text = null;
                cbVaccine.SelectedIndex = -1;
                cbVaccine.Items.Clear();
                connection.Close();
                vaccineLoad();
            }
            else
            {
                MessageBox.Show("Query Not Executed");
            }
            connection.Close();
        }

        private void DaysInterval_KeyPress(object sender, KeyPressEventArgs e)
        {
             e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
             
        }

        private void DoseNeed_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void updateQuery(object sender, EventArgs e)
        {
            connection.Open();
            String updateQuery;
            if (Convert.ToInt32(DoseNeedTxt1.Text) == 1)
            {
                
                updateQuery = "UPDATE vaccination_system.tblvaccine SET vaccineType = '" + vaccine.Text + "',vaccineDayInterval = '" + 0 + "', doseNeed = '" + DoseNeedTxt1.Text + "' WHERE id = '" + id + "'";
            }
            else {
                updateQuery = "UPDATE vaccination_system.tblvaccine SET vaccineType = '" + vaccine.Text + "',vaccineDayInterval = '" + DaysIntervalTxt.Text + "', doseNeed = '" + DoseNeedTxt1.Text + "' WHERE id = '" + id + "'";
            }
            
            MySqlCommand command = new MySqlCommand(updateQuery, connection);
            if (command.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Query Executed");
                cbVaccine.Items.Clear();
                vaccine.Text = null;
                DaysIntervalTxt.Text = null;
                DoseNeedTxt1.Text = null;
                cbVaccine.SelectedIndex = -1;
                connection.Close();
                vaccineLoad();

            }
            else
            {
                MessageBox.Show("Query Not Executed");
            }
            connection.Close();
        }

        private void DaysIntervalTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void DoseNeedTxt1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);          
        }



        private void barangayCb_Leave(object sender, EventArgs e)
        {
            barangayCb.Text = barangayCb.Value.ToString();
        }

       

      

    
    }
}

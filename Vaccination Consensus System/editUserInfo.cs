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
using QRCoder;

namespace Vaccination_Consensus_System
{
    public partial class editUserInfo : Form
    {
     

        public editUserInfo(int keyy)
        {      
            InitializeComponent();
            if (keyy == 0)
            {
                addAccount();          
            }
            else
            {
                key = keyy;
                loadData(false);
            }
            
        }
        //MySqlConnection connection = new MySqlConnection("datasource=localhost;port=3306; database=vaccination_system; username=root;password=");
        MySqlConnection connection = new MySqlConnection("datasource=localhost;port=3306; database=vaccination_system; username=root;password=;Convert Zero Datetime=True");
        int key,age1;
        string username, password, fname, bday, gender3, status3, emailadd3, contactno3;
        int barangay;
        string prioGroup, vaccineType, dateOfFirstDose, dateOfSecondDose;
        int doseCount, age;
        string vaccinator1, vaccinator2, address1;
        bool isFullyVaccinated,dp = false;
        int isFullyVaccinatedInt = 0;
        int qr_key;
        int boosterDoseCount;
        byte[] img, image1;

        private void addAccount() {
            //load vaccines
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
            button6.Visible = false;
            button4.Visible = false;
            button3.Visible = false;
            textBox16.Enabled = false;
            cbVaccine.DropDownStyle = ComboBoxStyle.DropDownList;
            //create qr    
            int qrKey,i;
            do
            {
                Random rd = new Random();
                qrKey = rd.Next(100000000, 999999999);
                //check if there is same qr key
                string checkIfSameQrKey = "select qr_key from vaccination_system.users where qr_key = '" + qrKey + "';";
                command = new MySqlCommand(checkIfSameQrKey, connection);
                DataTable dataSet = new DataTable();
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                dataAdapter.Fill(dataSet);
                i = Convert.ToInt32(dataSet.Rows.Count.ToString());
            } while (i != 0);
            textBox15.Text = qrKey.ToString();            
            dbo.CustomFormat = " ";
            dbo2.CustomFormat = " ";
            dbo3.CustomFormat = " ";
            vn.Enabled = false;
            td.Enabled = false;
            td.CustomFormat = " ";
            button5.Visible = false;
            
            //load qr         
            QRCodeGenerator qr = new QRCodeGenerator();
            QRCodeData data = qr.CreateQrCode(Convert.ToString(qr_key), QRCodeGenerator.ECCLevel.Q);
            QRCode code = new QRCode(data);
            pictureBox2.Image = code.GetGraphic(5);
            pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;

           
        }

        private void button8_Click(object sender, EventArgs e)
        {
            admin ad = new admin();
            ad.Show();
            this.Hide();
        }

        private void loadData(bool undo) {

            string LoginQuery = "select * from users where qr_key = '" + key + "' ";
            connection.Open();
            //connection and loginQuery
            MySqlCommand command = new MySqlCommand(LoginQuery, connection);
            DataTable dataSet = new DataTable();
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
            dataAdapter.Fill(dataSet);
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {                           
            //pass to local variable
            username = reader.GetString("username");
            textBox1.Text = username;
            password = reader.GetString("Password");
            textBox2.Text = password;
            fname = reader.GetString("fname");
            textBox3.Text = fname;
            bday = reader.GetString("bday");          
            dbo.Text = bday;

            gender3 = reader.GetString("gender");
            if (gender3 == "Male")
                radioButton1.Checked = true;
            else radioButton2.Checked = true;
            
            status3 = reader.GetString("civilStatus");
            cs.Text = status3;
            emailadd3 = reader.GetString("email");
            textBox7.Text = emailadd3;
            contactno3 = reader.GetString("contact");
            textBox8.Text = contactno3;
            barangay = Convert.ToInt32(reader.GetString("barangay"));
            numericUpDown1.Value = barangay;
            prioGroup = reader.GetString("prioGroup");
            comboBox2.Text = prioGroup;
            vaccineType = reader.GetString("vaccineType");
            cbVaccine.Text = vaccineType;
            dateOfFirstDose = reader.GetString("firstDoseDate");        
            dbo2.Text = dateOfFirstDose;
            //dateOfSecondDose = reader.GetString("SecondDoseDate");  
            DateTime? updateTime = reader.GetMySqlDateTime("SecondDoseDate").IsValidDateTime ? (DateTime?)reader["SecondDoseDate"] : null;             
            dateOfSecondDose = updateTime.ToString();
            if(dateOfSecondDose != string.Empty)
            dbo3.Text = dateOfSecondDose;
            doseCount = (Convert.ToInt32(reader.GetString("doseCount")));
            textBox14.Text = doseCount.ToString();
            age = (Convert.ToInt32(reader.GetString("age")));
            textBox16.Text = age.ToString();
            vaccinator1 = reader.GetString("firstDoseVaccinator");
            textBox17.Text = vaccinator1;
            vaccinator2 = reader.GetString("secondDoseVaccinator");
            textBox18.Text = vaccinator2;
            address1 = reader.GetString("address");
            textBox19.Text = address1;
            qr_key = reader.GetInt32("qr_key");
            textBox15.Text = qr_key.ToString();
            isFullyVaccinated = reader.GetBoolean("isFullyVaccinated");
            //load qr         
            QRCodeGenerator qr = new QRCodeGenerator();
            QRCodeData data = qr.CreateQrCode(Convert.ToString(qr_key), QRCodeGenerator.ECCLevel.Q);
            QRCode code = new QRCode(data);
            pictureBox2.Image = code.GetGraphic(5);
            pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;

            if (isFullyVaccinated)
                radioButton4.Checked = true;
            else radioButton3.Checked = true;

            boosterDoseCount = reader.GetInt32("boosterDoseCount");
            textBox21.Text = boosterDoseCount.ToString();         
                if (dataSet.Rows[0][17] is System.DBNull == false){
                  
                        img = (byte[])dataSet.Rows[0][17];
                        MemoryStream ms = new MemoryStream(img);
                        pictureBox1.Image = Image.FromStream(ms);
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }              
           
            //load vaccines
            reader.Close();
            String selectQuery = "select * from tblvaccine";
            command = new MySqlCommand(selectQuery, connection);
            reader = command.ExecuteReader();
            if (undo)
                cbVaccine.Items.Clear();
            while (reader.Read())
            {
                vt.Items.Add(reader.GetString("vaccineType"));
                cbVaccine.Items.Add(reader.GetString("vaccineType"));
            }
            reader.Close();

            //load boosternum
            String selectBoosterNum = "select * from boosterDose where linkedid = '"+qr_key+"'";
            command = new MySqlCommand(selectBoosterNum, connection);
            reader = command.ExecuteReader();
            while (reader.Read())
            {               
                comboBox1.Items.Add(reader.GetInt32("boosterNum"));          
            }
          

            if (dateOfSecondDose == string.Empty)
            {
                dbo3.CustomFormat = " ";
            
                
            }

           }

       
            connection.Close();
            if (comboBox1.Items != null)
            comboBox1.SelectedIndex = comboBox1.Items.Count-1;

            if (comboBox1.SelectedIndex == -1)
            {
                td.CustomFormat = " ";
                td.Enabled = false;
                vt.Enabled = false;
                vn.Enabled = false;
                button5.Enabled = false;
            }

            addAccountBt.Visible = false;
        }

        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar); 
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {                     
             connection.Open();
             string vaccineType = "select * from vaccination_system.boosterdose where linkedid = '" + key + "' && boosternum = '" + Convert.ToInt32(comboBox1.Text) + "'";
             MySqlCommand command = new MySqlCommand(vaccineType, connection);
             MySqlDataReader reader = command.ExecuteReader();
             if(reader.Read())
             {
             vt.Text =  reader.GetString("vaccineType");
             vn.Text = reader.GetString("vaccinatorName");
             td.Text = reader.GetString("thisDoseDate");
             } reader.Close();
             connection.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "Choose Image(*.jpg; *.png; *.gif )|*.jpg; *.png *gif";
            if (opf.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(opf.FileName);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            loadData(true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            connection.Open();
            if (radioButton1.Checked)
                gender3 = "Male";
            else gender3 = "Female";
            if (radioButton4.Checked == true)
            {
                isFullyVaccinatedInt = 1;
            }
            else if (radioButton3.Checked == true)
            {
                isFullyVaccinatedInt = 0;
            }

            string updateQuery1 = "UPDATE users SET username = '" + textBox1.Text + "', password = '" + textBox2.Text + "', fname = '" + textBox3.Text + "', bday = '" + dbo.Value.ToString("MM/dd/yyyy") + "',gender = '" + gender3 + "', civilStatus = '" + cs.Text + "', email = '" + textBox7.Text + "',contact = '" + textBox8.Text + "', barangay = '" + numericUpDown1.Value + "', prioGroup = '" + comboBox2.Text + "', vaccineType = '" + cbVaccine.Text + "', firstDoseDate = '" + dbo2.Value.ToString("yyyy/MM/dd") + "', secondDoseDate = '" + dateOfSecondDose + "', doseCount = '" + textBox14.Text + "',  age = '" + textBox16.Text + "', firstDoseVaccinator = '" + textBox17.Text + "',secondDoseVaccinator = '" + textBox18.Text + "',address = '" + textBox19.Text + "',isFullyVaccinated = '" + isFullyVaccinatedInt + "',vaccineProfilePic = @img, boosterDoseCount = '" + textBox21.Text + "' WHERE qr_key = '" + qr_key + "'";
            MemoryStream ms = new MemoryStream();
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                image1 = ms.ToArray();
            }
                MySqlCommand command = new MySqlCommand(updateQuery1, connection);
                command.Parameters.Add("@img", MySqlDbType.Blob);
                command.Parameters["@img"].Value = image1;
            
            if (command.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Query Executed");
            }
            else
            {
                MessageBox.Show("Query Not Executed");
            }
            connection.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            connection.Open();
            string updateBossterDose = "UPDATE boosterDose SET vaccinatorName ='" + vn.Text + "', vaccineType = '" + vt.Text + "', thisDoseDate = '" + td.Value.ToString("MM/dd/yyyy") + "' where linkedid = '" + qr_key + "' && boosterNum = '" + Convert.ToInt32(comboBox1.Text) + "' ";
            MySqlCommand command = new MySqlCommand(updateBossterDose, connection);
            if (command.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Query Executed");
            }
            else
            {
                MessageBox.Show("Query Not Executed");
            }           
            connection.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            connection.Open();
            string deleteQuery = "delete  from users where qr_key = '" + qr_key + "'";
            MySqlCommand command = new MySqlCommand(deleteQuery, connection);
            if (command.ExecuteNonQuery() == 1)
            {
                string deleteQuery2 = "delete  from boosterdose where linkedid = '" + qr_key + "'";
                command = new MySqlCommand(deleteQuery2, connection);
                command.ExecuteNonQuery();
                MessageBox.Show("Query Executed");
                admin ad = new admin();
                ad.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Query Not Executed");
            }
            connection.Close();
           
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
            bday = dbo.Value.Date.ToString("MM/dd/yyyy");
            age1 = years;
            textBox16.Text = age1.ToString();
        }

        private void dbo2_ValueChanged(object sender, EventArgs e)
        {
            dbo2.CustomFormat = "MM/dd/yyyy";
           
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dbo3.CustomFormat = "MM/dd/yyyy";
            dateOfSecondDose = dbo3.Value.ToString("yyyy/MM/dd");
        }

     

        private void numericUpDown1_Leave(object sender, EventArgs e)
        {
            numericUpDown1.Text = numericUpDown1.Value.ToString();
        }
    
        private void addAccountButtonClick(object sender, EventArgs e)
        {
            if (textBox3.Text == string.Empty)
            {
                MessageBox.Show("Please enter your full name!");
                textBox3.Focus();
                return;
            }
            if (textBox1.Text == string.Empty)
            {
                MessageBox.Show("Please enter your username");
                textBox1.Focus();
                return;
            }
            if (textBox2.Text == string.Empty)
            {
                MessageBox.Show("Please enter your password!");
                textBox2.Focus();
                return;
            }
            if (dp == false)
            {
                MessageBox.Show("Please enter your birthday!");
                panel1.Focus();
                return;
            }
            if (radioButton1.Checked == false && radioButton2.Checked == false)
            {
                MessageBox.Show("Please select your gender!");
                panel1.Focus();
                return;
            }
            if (cs.Text == string.Empty)
            {
                MessageBox.Show("Please select your civil status!");
                cs.Focus();
                return;
            }
            if (textBox7.Text == string.Empty)
            {
                MessageBox.Show("Please enter your email!");
                textBox7.Focus();
                return;
            }
            if (textBox8.Text == string.Empty)
            {
                MessageBox.Show("Please enter your contact number!");
                textBox8.Focus();
                return;
            }

            if (numericUpDown1.Text == null)
            {
                MessageBox.Show("Please select your barangay!");
                numericUpDown1.Focus();
                return;
            }
            if (comboBox2.Text == string.Empty)
            {
                MessageBox.Show("Please enter your priority group!");
                comboBox2.Focus();
                return;
            }
            if (textBox19.Text == string.Empty)
            {
                MessageBox.Show("Please enter your address!");
                textBox19.Focus();
                return;
            }
            if (cbVaccine.Text == string.Empty)
            {
                MessageBox.Show("Please enter your vaccine type!");
                cbVaccine.Focus();
                return;
            }
            if (dbo2.CustomFormat == " ")
            {
                MessageBox.Show("Please enter your firse dose date!");
                dbo2.Focus();
                return;
            }
            if (textBox14.Text == string.Empty)
            {
                MessageBox.Show("Please enter your dose count!");
                textBox14.Focus();
                return;
            }
            if (textBox17.Text == string.Empty)
            {
                MessageBox.Show("Please enter your first dose vaccinator!");
                textBox17.Focus();
                return;
            }
            if (radioButton4.Checked == false && radioButton3.Checked == false)
            {
                MessageBox.Show("Please select if your are fully vaccinated or  not!");
                panel2.Focus();
                return;
            }
            if (textBox21.Text == string.Empty)
            {
                MessageBox.Show("Please enter your booster dose count!");
                textBox21.Focus();
                return;
            }
            fname = textBox3.Text;
            username = textBox1.Text;
            password = textBox2.Text;
            bday = dbo.Value.ToString("MM/dd/yyyy");
            if (radioButton1.Checked)
                gender3 = "Male";
            else if (radioButton2.Checked)
                gender3 = "Female";
            status3 = cs.Text;
            emailadd3 = textBox7.Text;
            contactno3 = textBox8.Text;
            barangay = Convert.ToInt32(numericUpDown1.Text);
            prioGroup = comboBox2.Text;
            address1 = textBox19.Text;
            vaccineType = cbVaccine.Text;
            dateOfFirstDose = dbo2.Value.ToString("yyyy/MM/dd");
            if(dbo3.CustomFormat != " ")
                dateOfSecondDose = dbo3.Value.ToString("yyyy/MM/dd");
            doseCount = Convert.ToInt32(textBox14.Text);
            qr_key = Convert.ToInt32(textBox15.Text);
            vaccinator1 = textBox17.Text;
            vaccinator2 = textBox18.Text;
            if (radioButton4.Checked)
                isFullyVaccinatedInt = 1;
            else if (radioButton3.Checked)
                isFullyVaccinatedInt = 0;
            boosterDoseCount = Convert.ToInt32(textBox21.Text);

            //check if same user exist     
            MySqlConnection connection = new MySqlConnection("datasource=localhost;port=3306;username=root;password=");
            string select = "select * from vaccination_system.users where username = '" + username + "' or fname = '" + fname + "' ";
            connection.Open();
            MySqlCommand command = new MySqlCommand(select, connection);
            DataTable dataSet = new DataTable();
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
            dataAdapter.Fill(dataSet);
            int i = Convert.ToInt32(dataSet.Rows.Count.ToString());
            //acount dont exist
            if (i == 0)
            {
                string insertQuery = "INSERT INTO vaccination_system.users(username, password, fname, bday,	gender,	civilStatus, email,	contact, barangay, prioGroup, vaccineType, firstDoseDate, secondDoseDate, doseCount,qr_key,age,vaccineProfilePic,firstDoseVaccinator,secondDoseVaccinator,address,isFullyVaccinated,boosterDoseCount) VALUES('" + username + "','" + password + "', '" + fname + "','" + bday + "','" + gender3 + "','" + status3 + "','" + emailadd3 + "','" + contactno3 + "','" + barangay + "','" + prioGroup + "','" + vaccineType + "','" + dateOfFirstDose + "','" + dateOfSecondDose + "','" + doseCount + "','" + qr_key + "','" + age1 + "',@img ,'" + vaccinator1 + "','" + vaccinator2 + "','" + address1 + "','" + isFullyVaccinatedInt + "','" + boosterDoseCount + "');";
                if (pictureBox1.Image != null)
                {
                    MemoryStream ms = new MemoryStream();
                    pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                    image1 = ms.ToArray();
                }
                command = new MySqlCommand(insertQuery, connection);
                command.Parameters.Add("@img", MySqlDbType.Blob);
                command.Parameters["@img"].Value = image1;
                if (command.ExecuteNonQuery() == 1)
                {
                        MessageBox.Show("Query Executed");
                        //check if barangay population exist
                        string checkIfbarangayExist = "select barangaypopulationforthisbarangay from vaccination_system.barangayspopulation where barangay = '" + barangay + "'; ";
                        command = new MySqlCommand(checkIfbarangayExist, connection);
                        dataSet = new DataTable();
                        dataAdapter = new MySqlDataAdapter(command);
                        dataAdapter.Fill(dataSet);
                        i = Convert.ToInt32(dataSet.Rows.Count.ToString());
                        //if barangay population dont exist
                        if (i == 0)
                        {
                            string insertBarangayPopulation = "INSERT INTO vaccination_system.barangayspopulation(barangay,barangaypopulationforthisbarangay) values('" + barangay + "',1) ;";
                            command = new MySqlCommand(insertBarangayPopulation, connection);
                            command.ExecuteNonQuery();
                        }

                        else
                        {

                            string countQueryFullyVaccinated = "select count(isFullyVaccinated) from vaccination_system.users where (isFullyVaccinated = 1) and barangay = '" + barangay + "';";
                            string countQueryPartiallyVaccinated = "select count(isFullyVaccinated) from vaccination_system.users where (isFullyVaccinated = 0) and barangay = '" + barangay + "';";                         
                            string countBarangayPopulationForThisBarangay = "select BarangayPopulationForThisBarangay from vaccination_system.barangayspopulation where barangay = '" + barangay + "';";

                            command = new MySqlCommand(countQueryFullyVaccinated, connection);
                            int fullyVaccinated = Convert.ToInt32(command.ExecuteScalar());

                            command = new MySqlCommand(countQueryPartiallyVaccinated, connection);
                            int partiallyVaccinated = Convert.ToInt32(command.ExecuteScalar());

                            int sum = fullyVaccinated + partiallyVaccinated;

                            command = new MySqlCommand(countBarangayPopulationForThisBarangay, connection);
                            int barangayPopulationForThisBarangay = Convert.ToInt32(command.ExecuteScalar());

                            if (barangayPopulationForThisBarangay < sum)
                            {
                                string updatePopulation = "update vaccination_system.barangayspopulation set barangaypopulationforthisbarangay = barangaypopulationforthisbarangay+1 where barangay = '" + barangay + "';";
                                command = new MySqlCommand(updatePopulation, connection);
                                command.ExecuteNonQuery();
                               
                            }
                        }
                }
                else
                {
                    MessageBox.Show("Query Not Executed");
                }

                connection.Close();
                admin ad = new admin();
                ad.Show();
                this.Hide();
            }
            else
            {
                connection.Close();
                MessageBox.Show("Same user exist in our database!");
                return;
            }
            connection.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            dbo3.CustomFormat = " ";
            dateOfSecondDose = string.Empty;
        }

        

       
    }
}

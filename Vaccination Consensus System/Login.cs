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
    public partial class Login : Form
    {
       
        public Login()
        {
            
            InitializeComponent();
            connectionStatus();
           
        }
        string username, password, fname, bday, gender3, status3, emailadd3, contactno3;
        int barangay;
        string prioGroup, vaccineType, dateOfFirstDose, dateOfSecondDose;
        int doseCount = 2;
        bool dbs = false;
        int age;
        byte[] img;
        string vaccinator1, vaccinator2;
        string address1;


        //MySqlConnection connection = new MySqlConnection("datasource=localhost;port=3306; database=vaccination_system; username=root;password=;");
        MySqlConnection connection = new MySqlConnection("datasource=localhost;port=3306; database=vaccination_system; username=root;password=;Convert Zero Datetime=True");
        public void connectionStatus(){
            try
            {
               
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    databaseStatus.Text += " Connected";
                    databaseStatus.ForeColor = Color.Green;
                    dbs = true;
                }

            }
            catch (Exception)
            {
                databaseStatus.Text += " Not Connected";
                databaseStatus.ForeColor = Color.Red;
            }
            finally {
                connection.Close();
            }
        }

        private void login_Click(object sender, EventArgs e)
        {
            if(dbs == false){
                MessageBox.Show("Database not connected");
                return;
            }
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select account type");
                return;
            }
            if (comboBox1.SelectedItem.ToString() == "User")
            {

                try
                {

                    //query to string
                    string LoginQuery = "select * from vaccination_system.users where username = '" + txtUsername.Text + "' and password = '" + txtPassword.Text + "' ";
                    connection.Open();
                    //connection and loginQuery
                    MySqlCommand command = new MySqlCommand(LoginQuery, connection);
                    DataTable dataSet = new DataTable();
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                    dataAdapter.Fill(dataSet);
                    int i = Convert.ToInt32(dataSet.Rows.Count.ToString());

                    if (i == 0)
                    {
                        MessageBox.Show("You have entered invalid username or password");
                        return;
                    }
                    else
                    {

                        MySqlDataReader reader;
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            // check if fully vaccinated
                            if (reader.GetBoolean("isFullyVaccinated") == true)
                            {
                                //pass to local variable
                                username = reader.GetString("username");
                                password = reader.GetString("Password");
                                fname = reader.GetString("fname");
                                bday = reader.GetString("bday");
                                gender3 = reader.GetString("gender");
                                status3 = reader.GetString("civilStatus");
                                emailadd3 = reader.GetString("email");
                                contactno3 = reader.GetString("contact");
                                barangay = Convert.ToInt32(reader.GetString("barangay"));
                                prioGroup = reader.GetString("prioGroup");
                                vaccineType = reader.GetString("vaccineType");
                                //dateOfFirstDose = reader.GetString("firstDoseDate");
                                //dateOfSecondDose = reader.GetString("SecondDoseDate");                               
                                DateTime? updateTime = reader.GetMySqlDateTime("firstDoseDate").IsValidDateTime ? (DateTime?)reader["firstDoseDate"] : null;                               
                                dateOfFirstDose = updateTime.ToString();                           
                                DateTime? updateTime2 = reader.GetMySqlDateTime("SecondDoseDate").IsValidDateTime ? (DateTime?)reader["SecondDoseDate"] : null;
                                dateOfSecondDose = updateTime2.ToString();                             
                                doseCount = (Convert.ToInt32(reader.GetString("doseCount")));
                                age = (Convert.ToInt32(reader.GetString("age")));
                                vaccinator1 = reader.GetString("firstDoseVaccinator");
                                vaccinator2 = reader.GetString("secondDoseVaccinator");
                                address1 = reader.GetString("address");
                                bool isBooster = false;
                                bool ipof = false;
                                VaccineCard oc1 = new VaccineCard(username, password, fname, bday, gender3, status3, emailadd3, contactno3, barangay, prioGroup, vaccineType, dateOfFirstDose, dateOfSecondDose, "", doseCount, age, img, vaccinator1, vaccinator2, address1, false, "", "", "", "",false);
                                oc1.Show();
                                this.Hide();
                                return;
                            }


                            //dose count 2
                            username = reader.GetString("username");
                            password = reader.GetString("Password");
                            fname = reader.GetString("fname");
                            bday = reader.GetString("bday");
                            gender3 = reader.GetString("gender");
                            status3 = reader.GetString("civilStatus");
                            emailadd3 = reader.GetString("email");
                            contactno3 = reader.GetString("contact");
                            barangay = Convert.ToInt32(reader.GetString("barangay"));
                            prioGroup = reader.GetString("prioGroup");
                            vaccineType = reader.GetString("vaccineType");
                            //dateOfFirstDose = reader.GetString("firstDoseDate");
                            //dateOfSecondDose = reader.GetString("SecondDoseDate");
                            DateTime? updateTime3 = reader.GetMySqlDateTime("firstDoseDate").IsValidDateTime ? (DateTime?)reader["firstDoseDate"] : null;
                            dateOfFirstDose = updateTime3.ToString();                                
                            DateTime? updateTime4 = reader.GetMySqlDateTime("SecondDoseDate").IsValidDateTime ? (DateTime?)reader["SecondDoseDate"] : null;
                            dateOfSecondDose = updateTime4.ToString();        
                            doseCount = ((Convert.ToInt32(reader.GetString("doseCount"))) + 1);
                            vaccinator1 = reader.GetString("firstDoseVaccinator");
                            vaccinator2 = reader.GetString("secondDoseVaccinator");
                            age = reader.GetInt32("age");
                            address1 = reader.GetString("address");
                            //pass local variabl to final step to get vaccination
                            Vaccination_Info vi = new Vaccination_Info(username, password, fname, bday, gender3, status3, emailadd3, contactno3, barangay, doseCount, dateOfFirstDose, dateOfSecondDose, prioGroup, vaccineType, age, img, vaccinator1, vaccinator2, address1);
                            vi.Show();
                            this.Hide();
                        }
                        else { MessageBox.Show("Fail"); }
                    }
                    connection.Close();
                }
                catch (Exception ee) { MessageBox.Show("" + ee); return; }
                finally { connection.Close(); }
                
                    
                
            }
            else {
                //query admin account
                MySqlConnection connection = new MySqlConnection("datasource=localhost;port=3306;username=root;password=");
                string LoginQuery = "select * from vaccination_system.admin where adminName = '" + txtUsername.Text + "' and adminPassword = '" + txtPassword.Text + "' ";
                connection.Open();
                //connection and loginQuery
                MySqlCommand command = new MySqlCommand(LoginQuery, connection);
                DataTable dataSet = new DataTable();
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                dataAdapter.Fill(dataSet);
                int i = Convert.ToInt32(dataSet.Rows.Count.ToString());

                if (i == 0)
                {
                    MessageBox.Show("You have entered invalid username or password");
                    return;
                }
                else
                {
                    admin ad = new admin();
                    ad.Show();
                    this.Hide();
                }
                connection.Close();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (dbs == false)
            {
                MessageBox.Show("Database not connected");
                return;
            }
            Form1 rg = new Form1();
            rg.Show();
            this.Hide();
        }

       

        private void button2_Click(object sender, EventArgs e)
        {
            if (dbs == false)
            {
                MessageBox.Show("Database not connected");
                return;
            }
            ScannQR scQR = new ScannQR();
            scQR.Show();
            this.Hide();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dbs == false)
            {
                MessageBox.Show("Database not connected");
                return;
            }
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select account type");
                return;
            }
            if (comboBox1.SelectedItem.ToString() == "User")
            {
                try
                {
                    //query to string
                    string LoginQuery = "select * from vaccination_system.users where username = '" + txtUsername.Text + "' and password = '" + txtPassword.Text + "' ";
                    connection.Open();
                    //connection and loginQuery
                    MySqlCommand command = new MySqlCommand(LoginQuery, connection);
                    DataTable dataSet = new DataTable();
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                    dataAdapter.Fill(dataSet);
                    int i = Convert.ToInt32(dataSet.Rows.Count.ToString());

                    if (i == 0)
                    {
                        MessageBox.Show("You have entered invalid username or password");
                        connection.Close();
                        return;
                    }
                    else
                    {
                        MySqlDataReader reader;
                        reader = command.ExecuteReader();
                        // check if fully vaccinated
                        if (reader.Read())
                        {
                            // check if fully vaccinated
                            if (reader.GetBoolean("isFullyVaccinated") == true)
                            {
                                //pass to local variable
                                username = reader.GetString("username");
                                password = reader.GetString("Password");
                                fname = reader.GetString("fname");
                                bday = reader.GetString("bday");
                                gender3 = reader.GetString("gender");
                                status3 = reader.GetString("civilStatus");
                                emailadd3 = reader.GetString("email");
                                contactno3 = reader.GetString("contact");
                                barangay = Convert.ToInt32(reader.GetString("barangay"));
                                prioGroup = reader.GetString("prioGroup");
                                vaccineType = reader.GetString("vaccineType");
                                //dateOfFirstDose = reader.GetString("firstDoseDate");                              
                                //dateOfSecondDose = reader.GetString("SecondDoseDate");
                                DateTime? updateTime3 = reader.GetMySqlDateTime("firstDoseDate").IsValidDateTime ? (DateTime?)reader["firstDoseDate"] : null;
                                dateOfFirstDose = updateTime3.ToString();                                
                                DateTime? updateTime4 = reader.GetMySqlDateTime("SecondDoseDate").IsValidDateTime ? (DateTime?)reader["SecondDoseDate"] : null;
                                dateOfSecondDose = updateTime4.ToString();   
                                doseCount = (Convert.ToInt32(reader.GetString("doseCount")));
                                age = (Convert.ToInt32(reader.GetString("age")));
                                vaccinator1 = reader.GetString("firstDoseVaccinator");
                                vaccinator2 = reader.GetString("secondDoseVaccinator");
                                address1 = reader.GetString("address");
                                boosterShot bs = new boosterShot(username, password, fname, bday, gender3, status3, emailadd3, contactno3, barangay, doseCount, dateOfFirstDose, dateOfSecondDose, prioGroup, vaccineType, age, img, vaccinator1, vaccinator2, address1);
                                bs.Show();
                                this.Hide();
                                return;
                            }
                            else
                            {
                                MessageBox.Show("You are not yet fully vaccinated so you are not allowed to have booster dose!");
                                connection.Close();
                                return;
                            }
                        }
                        
                    }
                }
                catch (Exception ee)
                {
                    MessageBox.Show("Database Error: \n" + ee);
                    connection.Close();
                    return;
                }
               
            }
            else
            {
                MessageBox.Show("Please Select user as account type!");
                return;
            }
            connection.Close();
        }

        

       
    }
}

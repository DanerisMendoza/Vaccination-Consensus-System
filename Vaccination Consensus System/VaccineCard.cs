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
using QRCoder;
using System.IO;


namespace Vaccination_Consensus_System
{
    public partial class VaccineCard : Form
    {
        public VaccineCard(string usernameP, string passwordP, string fname, string bday, string gender3, string status3, string emailadd3, string contactno3, int barangay, string prioGroup, string typeOfVaccine, string dateOfFirstDose, string dateOfSecondDose, string sideEffect, int doseCount, int age, byte[] img, string vaccinator1, string vaccinator2, string address,bool isBooster,string vTBooster,string bD,string vn, string date,bool isPartiallyOrfully)      
        {
            InitializeComponent();
            fnamel = fname;
            string se = sideEffect;
            string username = usernameP;
            string password = passwordP;
            txtFullname.Text = fname;
            txtBirthday.Text = bday;
            txtGender3.Text = gender3;
            civilStatusTxt.Text = status3;
            txtEmailadd3.Text = emailadd3;
            label22.Text = address;
            address1 = address;
            txtContactno3.Text =  contactno3;
            barangayTxt.Text = Convert.ToString(barangay);
            txtPriogroup3.Text = prioGroup;
            txtVaccine3.Text = typeOfVaccine;
            DateTime firstDose = Convert.ToDateTime(dateOfFirstDose);
            txtFirstdose3.Text = firstDose.ToString("MM/dd/yyyy");
            if (dateOfSecondDose != string.Empty)
            {
                DateTime secondDose = Convert.ToDateTime(dateOfSecondDose);
                txtSeconddose3.Text = secondDose.ToString("MM/dd/yyyy");
            }
            doseCountTxt.Text = Convert.ToString(doseCount);
            ageLabel.Text = Convert.ToString(age);
            v1 = vaccinator1;
            v2 = vaccinator2;
            label12.Text = v1;
            label20.Text = v2;

            //read pic from arguments
            if(img != null){
            MemoryStream ms = new MemoryStream(img);
            pictureBox1.Image = Image.FromStream(ms);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            MemoryStream ms2 = new MemoryStream();
            pictureBox1.Image.Save(ms2, pictureBox1.Image.RawFormat);
            image1 = ms2.ToArray();
            cont = false;
            }

            //check if profilePicVaccineAvailable from db
            if (cont == true)
            {
                connection.Open();
                string findPic = "select *  from vaccination_system.users where fname = '" + fname + "';";
                command = new MySqlCommand(findPic, connection);
                DataTable table = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(command);
                da.Fill(table);
                MySqlDataReader reader;
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    if (table.Rows[0][17] is System.DBNull == false)
                    {
                        img = (byte[])table.Rows[0][17];
                        MemoryStream ms = new MemoryStream(img);
                        pictureBox1.Image = Image.FromStream(ms);
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }              
                reader.Close();
                connection.Close();
            }
                                                             
            //check if account already addded                    
                connection.Open();    
                string select = "select * from vaccination_system.users where username = '" + username + "' or fname = '"+fname+"' ";                       
                command = new MySqlCommand(select, connection);
                DataTable dataSet = new DataTable();
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                dataAdapter.Fill(dataSet);
                int i = Convert.ToInt32(dataSet.Rows.Count.ToString());
                //acount dont exist in db
                if (i == 0)
                {
                    //create account        
                    int qrKey;
                    do{
                    Random rd = new Random();
                    qrKey = rd.Next(100000000,999999999);
                    //check if there is same qr key
                    string checkIfSameQrKey = "select qr_key from vaccination_system.users where qr_key = '"+qrKey+"';";
                    command = new MySqlCommand(checkIfSameQrKey, connection);
                    dataSet = new DataTable();
                    dataAdapter = new MySqlDataAdapter(command);
                    dataAdapter.Fill(dataSet);
                    i = Convert.ToInt32(dataSet.Rows.Count.ToString());
                    }while(i != 0);
                    
                    
                    QRCodeGenerator qr = new QRCodeGenerator();
                    QRCodeData data = qr.CreateQrCode(Convert.ToString(qrKey),QRCodeGenerator.ECCLevel.Q);

                    QRCode code = new QRCode(data);
                    QrBox.Image = code.GetGraphic(6);
                 
                    //doseNeedQuery
                    string doseNeedQuery = "select doseNeed from tblvaccine where vaccineType = '" + typeOfVaccine + "'";
                    command = new MySqlCommand(doseNeedQuery, connection);
                    int doseNeed = Convert.ToInt32(command.ExecuteScalar());
                    if (doseCount == doseNeed)
                    {
                        isFullyVaccinated = 1;
                        checkBox1.Checked = true;
                        checkBox1.ForeColor = Color.Green;
                    }
                               
                    //insert new account
                    string insertQuery = "INSERT INTO vaccination_system.users(username, password, fname, bday,	gender,	civilStatus, email,	contact, barangay, prioGroup, vaccineType, firstDoseDate, secondDoseDate, doseCount,qr_key,age,vaccineProfilePic,firstDoseVaccinator,secondDoseVaccinator,address,isFullyVaccinated,boosterDoseCount) VALUES('" + usernameP + "', '" + passwordP + "', '" + fname + "', '" + bday + "', '" + gender3 + "', '" + status3 + "', '" + emailadd3 + "', '" + contactno3 + "', '" + Convert.ToInt32(barangay) + "', '" + prioGroup + "', '" + typeOfVaccine + "', '" + dateOfFirstDose + "', '" + dateOfSecondDose + "', '" + doseCount + "', '" + qrKey + "','" + age + "' ,@img,'" + v1 + "','" + v2 + "','" + address1 + "','" + isFullyVaccinated + "',0);";
                   
                    command = new MySqlCommand(insertQuery, connection);
                    command.Parameters.Add("@img", MySqlDbType.Blob);
                    command.Parameters["@img"].Value = image1;     
   
                    if (command.ExecuteNonQuery() == 1)
                    {
                        
                        //check if barangay population exist
                        string checkIfbarangayExist = "select barangaypopulationforthisbarangay from vaccination_system.barangayspopulation where barangay = '" + barangay + "'; ";
                        command = new MySqlCommand(checkIfbarangayExist, connection);

                        dataSet = new DataTable();
                        dataAdapter = new MySqlDataAdapter(command);
                        dataAdapter.Fill(dataSet);
                        i = Convert.ToInt32(dataSet.Rows.Count.ToString());
                        //if barangay population dont exist
                        if(i == 0){
                            string insertBarangayPopulation = "INSERT INTO vaccination_system.barangayspopulation(barangay,barangaypopulationforthisbarangay) values('"+barangay+"',1) ;";
                            command = new MySqlCommand(insertBarangayPopulation, connection);
                            command.ExecuteNonQuery();
                            
                        }
                        
                            else{

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
                            
                            if(barangayPopulationForThisBarangay < sum){
                                string updatePopulation = "update vaccination_system.barangayspopulation set barangaypopulationforthisbarangay = barangaypopulationforthisbarangay+1 where barangay = '" + barangay + "';";
                                command = new MySqlCommand(updatePopulation, connection);
                                command.ExecuteNonQuery();
                            }
                                                       
                        }
                        if (sideEffect != "")
                        {
                            string ConcernOrSideEffect = "INSERT INTO vaccination_system.concernorsideeffect(name,concern) VALUES('" + fname + "','" + sideEffect + "' );";
                            command = new MySqlCommand(ConcernOrSideEffect, connection);
                            command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        MessageBox.Show("data record unsuccessfully because it already exist!");
                       
                    }
                }
                else {
                    //Account Already Exist;      
                    if (isPartiallyOrfully)
                    {
                        string addVaccineCount = "update vaccination_system.users set doseCount = doseCount + 1 where username = '" + usernameP + "'and fname = '" + fname + "'";
                        command = new MySqlCommand(addVaccineCount, connection);
                        command.ExecuteNonQuery();

                        string addSecondVaccinatorName = "update vaccination_system.users set secondDoseVaccinator = '" + vaccinator2 + "' where username = '" + usernameP + "' and fname = '" + fname + "'";
                        command = new MySqlCommand(addSecondVaccinatorName, connection);
                        command.ExecuteNonQuery();

                     //doseNeedQuery
                    string doseNeedQuery = "select doseNeed from vaccination_system.tblvaccine where vaccineType = '" + typeOfVaccine + "'";
                    command = new MySqlCommand(doseNeedQuery, connection);
                    int doseNeed = Convert.ToInt32(command.ExecuteScalar());
                    if (doseCount == doseNeed)
                    {
                        isFullyVaccinated = 1;
                        string changeToFully = "update vaccination_system.users set  isFullyVaccinated = '" + 1 + "' where username = '" + usernameP + "' and fname = '" + fname + "'";
                        command = new MySqlCommand(changeToFully, connection);
                        command.ExecuteNonQuery();

                    }
                    }
                    string findKey1 = "select qr_key from vaccination_system.users where fname = '" + fname + "';";
                    command = new MySqlCommand(findKey1, connection);
                    key = Convert.ToInt32(command.ExecuteScalar());                                             
                    QRCodeGenerator qr = new QRCodeGenerator();
                    QRCodeData data = qr.CreateQrCode(Convert.ToString(key),QRCodeGenerator.ECCLevel.Q);
                    QRCode code = new QRCode(data);
                    QrBox.Image = code.GetGraphic(6);
                    qr_key = key;
                    if (sideEffect != "")
                    {
                        string ConcernOrSideEffect = "INSERT INTO vaccination_system.concernorsideeffect(name,concern) VALUES('" + fname + "','" + sideEffect + "' );";
                        command = new MySqlCommand(ConcernOrSideEffect, connection);
                        command.ExecuteNonQuery();
                    }
                    //query missing variable                  
                    if (isBooster == false)
                    {
                       
                        string findKey = "select qr_key from vaccination_system.users where username = '" + username + "';";
                        command = new MySqlCommand(findKey, connection);
                        key = Convert.ToInt32(command.ExecuteScalar());


                        string doseCountQuery = "select * from vaccination_system.users where qr_key = '" + key + "' ";
                        command = new MySqlCommand(doseCountQuery, connection);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            doseCountTxt.Text = Convert.ToString(reader.GetInt32("doseCount"));
                            label23.Text = Convert.ToString(reader.GetInt32("boosterDoseCount"));
                            boosterDose = Convert.ToInt32(label23.Text);
                            if (reader.GetBoolean("isfullyvaccinated"))
                            {
                               
                                isFullyVaccinated = 1;
                                checkBox1.Checked = true;
                                checkBox1.ForeColor = Color.Green;
                            }
                        } reader.Close();     

                      
                        string boosterDoseInfo = "select * from vaccination_system.boosterdose where linkedid = '" + key + "' and boosternum =  '"+boosterDose+"';";                       
                        command = new MySqlCommand(boosterDoseInfo, connection);                    
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            label30.Text = reader.GetString("thisdosedate");
                            label34.Text = reader.GetString("vaccineType");
                            label33.Text = reader.GetString("VaccinatorName");
                        }                   
                        reader.Close();

                       
                                                                                
                    }
                    //for booster
                    if (isBooster)
                    {
                        //key
                        string findKey = "select qr_key from vaccination_system.users where fname = '" + fname + "';";
                        command = new MySqlCommand(findKey, connection);
                        key = Convert.ToInt32(command.ExecuteScalar());

                        string boosterCount = "select boosterDoseCount from vaccination_system.users where qr_key = '" + key + "' ";
                        command = new MySqlCommand(boosterCount, connection);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            boosterDose = reader.GetInt32("boosterDoseCount");
                            reader.Close();
                        }     
                        
                        doseCountTxt.Text = Convert.ToString(doseCount + 1);
                        label23.Text = (boosterDose + 1).ToString();
                        label30.Text = bD;
                        label34.Text = vTBooster;
                        label33.Text = vn;
                                    
                        string addVaccineCount = "update vaccination_system.users set doseCount = doseCount + 1 where qr_key = '" + key + "'; ";
                        command = new MySqlCommand(addVaccineCount, connection);
                        command.ExecuteNonQuery();

                        string addbooster = "update vaccination_system.users set boosterDoseCount = boosterDoseCount + 1 where qr_key = '" + key + "'; ";
                        command = new MySqlCommand(addbooster, connection);
                        command.ExecuteNonQuery();


                        boosterDose += 1;
                        string addBoosterDoseInfo = "insert into vaccination_system.boosterdose(boosterNum,linkedId,VaccinatorName,vaccineType,thisDoseDate) values('"+boosterDose+"','" + key + "','" + vn + "','" + vTBooster + "','" + date + "');";
                        command = new MySqlCommand(addBoosterDoseInfo, connection);
                        command.ExecuteNonQuery();

                        string fullyVacc = "select qr_key from vaccination_system.users where fname = '" + fname + "';";
                        command = new MySqlCommand(fullyVacc, connection);
                        isFullyVaccinated = Convert.ToInt32(command.ExecuteScalar());
                        if (Convert.ToBoolean(isFullyVaccinated) == true)
                        {
                            checkBox1.Checked = true;
                            checkBox1.ForeColor = Color.Green;
                        }                                       
                    }                  
                }                             
            connection.Close();
        }

        byte[] image1;
        string v1 = "";
        string v2 = "";
        string address1;
        int isFullyVaccinated = 0;
        MySqlConnection connection = new MySqlConnection("datasource=localhost;port=3306; database=vaccination_system; username=root;password=;Convert Zero Datetime=True");
        MySqlDataReader reader;
        MySqlCommand command = new MySqlCommand();
        bool cont = true;
        int boosterDose;
        int key,qr_key;
        string fnamel;

        public void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void button1_Click(object sender, EventArgs e)
        {
          
        }

        private void btnExit_Click_1(object sender, EventArgs e)
        {
            Login lg = new Login();
            lg.Show();
            this.Hide();
            if ((Application.OpenForms["seeMore"]) != null)
            {
                if( (Application.OpenForms["seeMore"]).Visible == true){
                Application.OpenForms["seeMore"].Close();
                }
            }
            
        }

        private void VaccineCard_Load(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (isFullyVaccinated == 0) {
                MessageBox.Show("you are not yet fully vaccinated!");
                return;
            }
            seeMore sm = new seeMore(key);
            sm.Show();
            
          
        }

      

        

       
    }
}

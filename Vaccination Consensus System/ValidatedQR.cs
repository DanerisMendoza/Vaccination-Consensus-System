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
    
    public partial class ValidatedQR : Form
    {
        public ValidatedQR(string fname, string bday, string gender3, string status3, string emailadd3, string contactno3, string barangay, string prioGroup, string typeOfVaccine, string dateOfFirstDose, string dateOfSecondDose, string doseCount, int qrKey, int age, string vaccinator1, string vaccinator2, string address,int key,bool isFullyVaccinated)
        {
            InitializeComponent();
            
            txtFullname.Text = fname;
            txtBirthday.Text = bday;
            txtGender3.Text = gender3;
            civilStatusTxt.Text = status3;
            txtEmailadd3.Text = emailadd3;
            txtContactno3.Text = contactno3;
            barangayTxt.Text = Convert.ToString(barangay);
            
            txtVaccine3.Text = typeOfVaccine;
   
            DateTime firstDose = Convert.ToDateTime(dateOfFirstDose);
            txtFirstdose3.Text = firstDose.ToString("MM/dd/yyyy");
            if (dateOfSecondDose != string.Empty)
            {
                DateTime secondDose = Convert.ToDateTime(dateOfSecondDose);
                txtSeconddose3.Text = secondDose.ToString("MM/dd/yyyy");
            }
           
            
            label21.Text = age.ToString();
            label18.Text = address;
            label12.Text = vaccinator1;
            label11.Text = vaccinator2;
            label27.Text = prioGroup;
            label28.Text = doseCount;
            key2 = key;
           
            if (isFullyVaccinated)
            {
                checkBox1.Checked = true;
                checkBox1.ForeColor = Color.Green;
                isFullyVaccinated2 = 1;
            }

            //check if profile vaccine pic available
            connection.Open();
            string findPic = "select *  from vaccination_system.users where fname = '" + fname + "';";
            MySqlCommand command = new MySqlCommand(findPic, connection);
            DataTable table = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter(command);
            da.Fill(table);
            int i = Convert.ToInt32(table.Rows.Count.ToString());
            da.Fill(table);
            if (i != 0)
            {
                if (table.Rows[0][17] is System.DBNull == false)
                {
                byte[] img = (byte[])table.Rows[0][17];
                MemoryStream ms = new MemoryStream(img);
                pictureBox1.Image = Image.FromStream(ms);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
            da.Dispose();
   
            
            //query missing variables      
            string findKey = "select qr_key from vaccination_system.users where fname = '" + fname + "';";
            command = new MySqlCommand(findKey, connection);
            key = Convert.ToInt32(command.ExecuteScalar());


            string doseCountQuery = "select * from vaccination_system.users where qr_key = '" + key + "' ";
            command = new MySqlCommand(doseCountQuery, connection);
            reader = command.ExecuteReader();
            if (reader.Read())
            {
               
                label31.Text = Convert.ToString(reader.GetInt32("boosterDoseCount"));
                boosterDose = Convert.ToInt32(label31.Text);
                if (reader.GetBoolean("isfullyvaccinated"))
                {
                    isFullyVaccinated2 = 1;
                    checkBox1.Checked = true;
                    checkBox1.ForeColor = Color.Green;
                }
            } reader.Close();


            string boosterDoseInfo = "select * from vaccination_system.boosterdose where linkedid = '" + key + "' and boosternum =  '" + boosterDose + "';";
            command = new MySqlCommand(boosterDoseInfo, connection);
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                label39.Text = reader.GetString("thisdosedate");
                label34.Text = reader.GetString("vaccineType");
                label35.Text = reader.GetString("VaccinatorName");
            }
            reader.Close();
            connection.Close();

            //load qr         
            QRCodeGenerator qr = new QRCodeGenerator();
            QRCodeData data = qr.CreateQrCode(Convert.ToString(qrKey), QRCodeGenerator.ECCLevel.Q);
            QRCode code = new QRCode(data);
            QrBox.Image = code.GetGraphic(5);
        }

        //global variables
        MySqlDataReader reader;
        //MySqlConnection connection = new MySqlConnection("datasource=localhost;port=3306; database=vaccination_system; username=root;password=");
        MySqlConnection connection = new MySqlConnection("datasource=localhost;port=3306; database=vaccination_system; username=root;password=;Convert Zero Datetime=True");
        int isFullyVaccinated2 = 0;
        int key2,boosterDose;
       
        private void ValidatedQR_Load(object sender, EventArgs e)
        {

        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Login lg = new Login();
            lg.Show();
            this.Hide();
        }

 
        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void doseCountTxt_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void txtPriogroup3_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }    


        private void btnExit_Click(object sender, EventArgs e)
        {
            Login lg = new Login();
            lg.Show();
            this.Hide();
            if ((Application.OpenForms["seeMore"]) != null)
            {
                if ((Application.OpenForms["seeMore"]).Visible == true)
                {
                    Application.OpenForms["seeMore"].Close();
                }
            }
            
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (isFullyVaccinated2 == 0)
            {
                MessageBox.Show("you are not yet fully vaccinated!");
                return;
            }
            seeMore sm = new seeMore(Convert.ToInt32(key2));
            sm.Show();
            
        }
    }
}

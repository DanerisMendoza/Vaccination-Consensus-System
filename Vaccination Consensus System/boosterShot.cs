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

    public partial class boosterShot : Form
    {
        string fname, bday, gender3, civilStatus, emailadd3, contactno3;
        int barangay;
        string prioGroup, typeOfVaccine, dateOfFirstDose, dateOfSecondDose, sideEffect;
        int doseCountLocalVariable = 0;
        string username, password;
        int age;
        byte[] img1;     
        string address;
        string  v1,v2;
        bool isBooster = true;
        MySqlConnection connection = new MySqlConnection("datasource=localhost;port=3306;username=root;password=''");
        string vTBooster,bD,vn,date;


        public boosterShot(string usernameP, string passwordP, string fnameP, string bdayP, string gender3P, string status3P, string emailadd3P, string contactno3P, int barangayP, int doseCountP, string dateOfFirstDoseP, string dateOfSecondDoseP, string prioGroupP, string vaccineTypeP, int age1, byte[] img, string vnn1, string vnn2, string address1)
        {
            InitializeComponent();
            nameTxt.Text = fnameP;
            prioGroupTxt.Text = prioGroupP;          
            DateTime dt = DateTime.Now;
            firstDoseTxt.Text = dt.ToString("MM/dd/yyyy");
            bD = dt.ToString("MM/dd/yyyy");
            doseCountTxt.Text = (doseCountP+1).ToString();

            prioGroup = prioGroupP;
            typeOfVaccine = vaccineTypeP;
            fname = fnameP;
            bday = bdayP;
            gender3 = gender3P;
            civilStatus = status3P;
            emailadd3 = emailadd3P;
            address = address1;
            contactno3 = contactno3P;
            barangay = barangayP;
            doseCountLocalVariable = doseCountP;
            dateOfFirstDose = dateOfFirstDoseP;
            dateOfSecondDose = dateOfSecondDoseP;
            username = usernameP;
            password = passwordP;
            age = age1;
            img1 = img;
            v1 = vnn1;
            v2 = vnn2;
            vaccineLoad();
            date = dt.ToString("MM/dd/yyyy");
        }
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
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            sideEffect = txtSideEffect.Text;
            if (cbVaccine.SelectedIndex == -1)
            {
                MessageBox.Show("Please input vaccine type!");
                return;
            }
            if (vnTxt.Text == string.Empty)
            {
                MessageBox.Show("Please input vaccinator name!");
                return;
            }
            bool ipof = false;
            vn = vnTxt.Text;
            VaccineCard oc1 = new VaccineCard(username, password, fname, bday, gender3, civilStatus, emailadd3, contactno3, barangay, prioGroup, typeOfVaccine, dateOfFirstDose, dateOfSecondDose, sideEffect, doseCountLocalVariable, age, img1, v1, v2, address, isBooster, vTBooster, bD, vn, date, ipof);
            oc1.Show();
            this.Hide();
        }

        private void cbVaccine_SelectedIndexChanged(object sender, EventArgs e)
        {
            vTBooster = cbVaccine.Text;
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Login lg = new Login();
            lg.Show();
            this.Hide();
        }
    }
}

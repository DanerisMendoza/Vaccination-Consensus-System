using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vaccination_Consensus_System
{
    public partial class Vaccination_Info : Form
    {   
        public Vaccination_Info(string usernameP,string passwordP,string fnameP,string bdayP,string  gender3P,string status3P,string emailadd3P,string contactno3P,int barangayP,int doseCountP,string dateOfFirstDoseP,string dateOfSecondDoseP,string prioGroupP,string vaccineTypeP,int age1,byte[] img,string vnn1, string vnn2,string address1)
        {
            InitializeComponent();
            
            //text
            prioGroupTxt.Text = prioGroupP;
            vaccineTypeTxt.Text = vaccineTypeP;
            nameTxt.Text = fnameP;
            DateTime firstDose = Convert.ToDateTime(dateOfFirstDoseP);
            firstDoseTxt.Text = firstDose.ToString("MM/dd/yyyy");
            if (dateOfSecondDoseP != string.Empty)
            {
                DateTime secondDose = Convert.ToDateTime(dateOfSecondDoseP);
                secondDoseTxt.Text = secondDose.ToString("MM/dd/yyyy");
            }
           
            doseCountTxt.Text = Convert.ToString(doseCountP);

            //global variables
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
                    
            if (doseCountLocalVariable == 1)
            {
                vn2.Visible = false;
            }
            else
            {
                vn2.Visible = true;
            }
            if (doseCountLocalVariable == 2)
            {
                vn1.Text = vnn1;
                vn1.ReadOnly = true;
            }
        }

        string fname, bday, gender3, civilStatus, emailadd3, contactno3;
        int barangay;
        string prioGroup, typeOfVaccine, dateOfFirstDose, dateOfSecondDose, sideEffect;
        int doseCountLocalVariable = 0;
        string username, password;
        int age;
        byte[] img1;
        string vaccinator1 = "";
        string vaccinator2 = "";
        string address;

        private void btnSubmit_Click(object sender, EventArgs e)
        {

            vaccinator1 = vn1.Text;
            vaccinator2 = vn2.Text;
            if (doseCountLocalVariable == 1 && string.IsNullOrEmpty(vn1.Text))
            {
                MessageBox.Show("Please input the vaccinator name!");
                return;
            }
            if (doseCountLocalVariable == 2 && (string.IsNullOrEmpty(vn1.Text) || string.IsNullOrEmpty(vn2.Text)))
            {
                MessageBox.Show("Please input the vaccinator name!");
                return;
            }          
            sideEffect = txtSideEffect.Text;        
            //PASSING THE VARIABLES TO Vaccine card
            bool isBooster = false, isPartiallyOrfully = true;
            VaccineCard oc1 = new VaccineCard(username, password, fname, bday, gender3, civilStatus, emailadd3, contactno3, barangay, prioGroup, typeOfVaccine, dateOfFirstDose, dateOfSecondDose, sideEffect, doseCountLocalVariable, age, img1, vaccinator1, vaccinator2, address, isBooster, "", "", "", "", isPartiallyOrfully); 
            oc1.Show();
            this.Hide();
        }

        private void logout(object sender, EventArgs e)
        {
            Login lg = new Login();
            lg.Show();
            this.Hide();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
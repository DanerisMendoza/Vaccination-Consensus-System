using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Text.RegularExpressions;

namespace Vaccination_Consensus_System
{
    public partial class ScannQR : Form
    {
        public ScannQR()
        {
            InitializeComponent();
            camera();
            
        }

        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice captureDevice = new VideoCaptureDevice();
        MySqlConnection connection = new MySqlConnection("datasource=localhost;port=3306; database=vaccination_system; username=root;password=;Convert Zero Datetime=True");
        string vaccinator1,vaccinator2;
        
        private void camera()
        {
            try
            {
                filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                foreach (FilterInfo filterInfo in filterInfoCollection)
                {
                    comboBox1.Items.Add(filterInfo.Name);
                }
                comboBox1.SelectedIndex = 0;
            }
            catch (Exception) {
                MessageBox.Show("no camera detected");
                return;
            }
          

        }
     
     
        private void button1_Click(object sender, EventArgs e)
        {          
            captureDevice.Stop();           
            Login lg = new Login();
            lg.Show();
            this.Hide();
           
        }

        private void timer1_Tick(object sender, EventArgs e){
            if (pictureBox1.Image != null){
                BarcodeReader barcodeReader = new BarcodeReader();
                Result result = barcodeReader.Decode((Bitmap)pictureBox1.Image);
                if (result != null){               
                    try
                    {
                        string key = result.ToString();
                        int key2 = Convert.ToInt32(key);
                      
                        string findKey = "select * from vaccination_system.users where qr_key = '" + key2 + "';";
                        connection.Open();
                        MySqlCommand command = new MySqlCommand(findKey, connection);
                        DataTable dataSet = new DataTable();
                        MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                        dataAdapter.Fill(dataSet);
                        int i = Convert.ToInt32(dataSet.Rows.Count.ToString());

                        if (i == 0)
                        {
                            MessageBox.Show("This qr code is invalid and not in our database!");
                            return;
                        }
                        else
                        {


                            MySqlDataReader reader;
                            reader = command.ExecuteReader();
                            if (reader.Read())
                            {
                                //pass to local variable
                                string username = reader.GetString("username");
                                string password = reader.GetString("Password");
                                string fname = reader.GetString("fname");
                                string bday = reader.GetString("bday");
                                string gender3 = reader.GetString("gender");
                                string status3 = reader.GetString("civilStatus");
                                string emailadd3 = reader.GetString("email");
                                string contactno3 = reader.GetString("contact");
                                string barangay2 = reader.GetString("barangay");
                                string prioGroup = reader.GetString("prioGroup");
                                string vaccineType = reader.GetString("vaccineType");
                                //string dateOfFirstDose = reader.GetString("firstDoseDate");
                                //string dateOfSecondDose = reader.GetString("SecondDoseDate");
                                DateTime? updateTime = reader.GetMySqlDateTime("firstDoseDate").IsValidDateTime ? (DateTime?)reader["firstDoseDate"] : null;
                                string dateOfFirstDose = updateTime.ToString();
                                DateTime? updateTime2 = reader.GetMySqlDateTime("SecondDoseDate").IsValidDateTime ? (DateTime?)reader["SecondDoseDate"] : null;
                                string dateOfSecondDose = updateTime2.ToString();     
                                string doseCount2 = reader.GetString("doseCount");
                                int age = Convert.ToInt32(reader.GetString("age"));
                                vaccinator1 = reader.GetString("firstDoseVaccinator");
                                vaccinator2 = reader.GetString("secondDoseVaccinator");
                                string address1 = reader.GetString("address");
                                bool isFullyVaccinated = false;
                                if(reader.GetBoolean("isFullyVaccinated"))
                                isFullyVaccinated = true;
                                ValidatedQR vqr = new ValidatedQR(fname, bday, gender3, status3, emailadd3, contactno3, barangay2, prioGroup, vaccineType, dateOfFirstDose, dateOfSecondDose, doseCount2, key2, age, vaccinator1, vaccinator2, address1, key2, isFullyVaccinated);
                                vqr.Show();
                                this.Hide();
                                captureDevice.Stop();
                                                                                                                  
                            }
                        }

                    }
                    catch (Exception ee)
                    {
                        MessageBox.Show("db error: \n "+ee);
                        return;
                    }
                    finally {
                        connection.Close();
                    }


                    //------------------------//
                    if (captureDevice.IsRunning){
                    captureDevice.Stop();                
                    }
                    timer1.Stop();         
                }
            }
        }

      

        private void CaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void ScannQR_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (captureDevice.IsRunning)
            {
                captureDevice.Stop();
            }
        }

        private void New_Capture(object sender, EventArgs e)
        {
            captureDevice = new VideoCaptureDevice(filterInfoCollection[comboBox1.SelectedIndex].MonikerString);
            captureDevice.NewFrame += CaptureDevice_NewFrame;
            captureDevice.Start();
            timer1.Start();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (captureDevice.IsRunning)
            {
                captureDevice.Stop();
               
            }
        }
    }
}

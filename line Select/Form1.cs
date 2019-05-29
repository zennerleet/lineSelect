using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using MySql.Data.MySqlClient;
using System.Timers;

namespace line_Select
{
    public partial class Form1 : Form
    {
        private SerialPort _serial = new SerialPort();
        private Boolean isPortOpen = false, isMysql = false;
        private MySqlConnection connection;
        private int cars, trucks;
        private void getCOMs() {
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports) {
                    comboBox1.Items.Add(port);
            }
        }
        public Form1() {
            InitializeComponent();

            pictureBox1.Visible = true;
            pictureBox2.Visible = false;
        
            carPicture.Visible = false;
            truckPicture.Visible = false;
        }
        private void mysql_connect () {
            string server, database, user, password, connectionString;
            server = "localhost";
            database = "lineselect";
            user = "root";
            password = "";
            connectionString = "Server=" + server + ";Database=" + database + ";Uid=" + user + ";password=" + password;
            connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();
                label5.Text = "MySql connected!";
                isMysql = true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message + connectionString);
                label5.Text = "MySql not connected!";
                isMysql = false;
            }
        }
        private void button1_Click(object sender, EventArgs e) //connect
        {
            if (!isPortOpen)
            {
                if (comboBox1.SelectedItem != null)
                {
                    if (comboBox1.SelectedItem.ToString().Contains("COM"))
                    {
                        _serial.PortName = comboBox1.SelectedItem.ToString();
                        _serial.BaudRate = 9600;
                        try
                        {
                            _serial.Open();
                            isPortOpen = true;
                            setButton(false, true);
                            pictureBox1.Visible = false;
                            pictureBox2.Visible = true;
                            _serial.DataReceived += _serial_DataReceived;
                        } catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "Port is open");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No port selected");
                }
            }
            else
            {
                MessageBox.Show("Port is open");
                setButton(false, true);
            }
        }
       
        private void button2_Click(object sender, EventArgs e) //disconnect
        {
            if (isPortOpen)
            {
                _serial.Close();
                isPortOpen = false;
                setButton(true, false);

                pictureBox1.Visible = true;
                pictureBox2.Visible = false;
            }
            else
            {
                setButton(true, false);
            }
        }
        private void setButton(Boolean buttonConnect, Boolean buttonDisconnect)
        {
            button1.Enabled = buttonConnect;
            button2.Enabled = buttonDisconnect;

        }

        private void button3_Click(object sender, EventArgs e) // reset db
        {
            if (isMysql)
            {
                cars = 0;
                trucks = 0;
                uploadData();
            }
            else
            {
                MessageBox.Show("No connection to DB");
            }
        }

        private void _serial_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string line = _serial.ReadLine();
            this.BeginInvoke(new LineReceivedEvent(LineReceived), line);
        }

        private delegate void LineReceivedEvent(string line);
        private void LineReceived(string line)
        {
            //What to do with the received line here

            if (line.Contains("1")) // detected a car
            {
                cars += 1;
                carPicture.Visible = true;
                truckPicture.Visible = false;
            }
            else if (line.Contains("2")) // detected a truck
            {
                trucks += 1;
                carPicture.Visible = false;
                truckPicture.Visible = true;
            }
            else //detected neither
            {

                carPicture.Visible = false;
                truckPicture.Visible = false;
            }

            updateData();
            uploadData();
        }

        private void readData() // perf
        {
            try
            {
                string queryString = "select * from stats";
                MySqlCommand command = new MySqlCommand(queryString, connection);
                MySqlDataReader reader = command.ExecuteReader();
               
                    while (reader.Read())
                    {
                        cars = reader.GetInt32(0);
                        trucks = reader.GetInt32(1);
                    }
             
                reader.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "reader labels");
            }
        }
        private void readSerial() // not used
        {
            char input = '0';
            try
            {
                input = (char) _serial.ReadChar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
                if (input == '1') // detected a car
                {
                    cars += 1;
                carPicture.Visible = true;
                truckPicture.Visible = false;
                }
                else if (input == '2') // detected a truck
                {
                    trucks += 1;
                    carPicture.Visible = false;
                    truckPicture.Visible = true;
            }
                else
                {

                carPicture.Visible = false;
                truckPicture.Visible = false;
            }
        }
        private void updateData()
        {

            label7.Invoke((MethodInvoker)(() => label7.Text = cars + ""));
            label6.Invoke((MethodInvoker)(() => label6.Text = trucks + ""));
            label8.Invoke((MethodInvoker)(() => label8.Text = (cars + trucks) + ""));
        }
        private void uploadData() // perf
        {
            string queryString = "update `stats` set `trucks` = @newtrucks, `cars` = @newcars";

            MySqlCommand command = new MySqlCommand(queryString, connection);
            command.Parameters.AddWithValue("@newtrucks", trucks);
            command.Parameters.AddWithValue("@newcars", cars);

            try
            {
                MySqlDataReader reader = command.ExecuteReader();
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "reader picture 2");
            }
        }
       
      
        private void Form1_Load(object sender, EventArgs e)
        {
            getCOMs();
            mysql_connect();
            readData();
            updateData();
        }
    }
}

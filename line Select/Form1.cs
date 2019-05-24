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

namespace line_Select
{
    public partial class Form1 : Form
    {
        private SerialPort _serial = new SerialPort();
        private Boolean isPortOpen = false;
       
        private void getCOMs() {
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports) {
                    comboBox1.Items.Add(port);
            }
        }
        public Form1() {
            InitializeComponent();
        
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
                        //_serial.Open();
                        isPortOpen = true;
                        setButton(false, true);
                        pictureBox1.Visible = false;
                        pictureBox2.Visible = true;
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
                //_serial.Close();
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
        private void Form1_Load(object sender, EventArgs e)
        {
            getCOMs();
            pictureBox1.Visible = true;
            pictureBox2.Visible = false;

        }
    }
}

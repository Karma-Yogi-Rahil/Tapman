using Guna.UI2.WinForms;
using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tapman
{
    public partial class Form1 : Form
    {

        private Computer computer;

        private Timer timer;
        public Form1()
        {
            InitializeComponent();
            computer = new Computer();
            computer.CPUEnabled = true;
            //computer.CPUEnabled = true;
            computer.RAMEnabled = false;
            computer.MainboardEnabled = false;
            computer.GPUEnabled = true;
            computer.Open();

            timer = new Timer();
            timer.Interval = 1000; // 5 seconds
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            //This palette is made up of the 6 colors.Hex color codes:  #6340bc,  #794ee6,  #090612,  #20153c,  #362367 and  #4d3291.
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            // Read the temperature data and update the labels
            foreach (var hardwareItem in computer.Hardware)
            {
                hardwareItem.Update();
                if (hardwareItem.HardwareType == HardwareType.CPU)
                {
                    foreach (var sensor in hardwareItem.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            //CpuTempLabel.Text = $"{sensor.Value.Value:N1} C";
                           
                                try
                                {
                                    var temp = ((sensor.Value));
                                CpuLoadProgressBar.Maximum = (int)sensor.Max;
                                CpuLoadProgressBar.Minimum = (int)sensor.Min;
                                CpuTempProgressBar.Value = (int)temp;
                                
                                    guna2HtmlToolTip1.SetToolTip(CpuLoadProgressBar,temp.ToString());
                                CurrentTempLabel.Text = temp.ToString() + " °C";
                                    //guna2HtmlLabel1.Text = temp.ToString();
                                    //guna2HtmlLabel2.Text = sensor.Max.ToString();
                                    //guna2GroupBox1.Text = sensor.Name.ToString();
                                }
                                catch (Exception ex)
                                {
                                    //MessageBox.Show(ex.Message);
                                }


                            
                            
                        }
                        else if (sensor.SensorType == SensorType.Load)
                        {
                            var load = ((sensor.Value));
                            CpuLoadProgressBar.Value = (int)load;
                        }
                        else if (sensor.SensorType == SensorType.Voltage)
                        {
                            var fanspeed = ((sensor.Value));
                            //guna2CircleProgressBar1.Value = (int)fanspeed;
                        }
                    }
                }
                else if (hardwareItem.HardwareType == HardwareType.GpuNvidia || hardwareItem.HardwareType == HardwareType.GpuAti)
                {
                    foreach (var sensor in hardwareItem.Sensors)
                    {

                    }
                }
            }
        }


        private void guna2GroupBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void CpuLoadProgressBar_ValueChanged(object sender, EventArgs e)
        {

        }

        private void CpuTempProgressBar_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}

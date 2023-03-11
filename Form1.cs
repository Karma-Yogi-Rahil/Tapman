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
    public partial class TapmanUI : Form
    {

        private Computer computer;

        private Timer timer;
        public TapmanUI()
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

                switch (hardwareItem.HardwareType)
                {
                    case (HardwareType.CPU):
                        UpdateCpuDetails(hardwareItem); break;

                    case (HardwareType.RAM):
                        UpdateRamDetails(hardwareItem); break;

                    case (HardwareType.GpuNvidia):
                        UpdateGpuDetails(hardwareItem); break;

                    case(HardwareType.GpuAti): UpdateGpuDetails(hardwareItem); break;

                    case (HardwareType.HDD):
                       UpdateStorageDetails(hardwareItem); break;
                }

                //if (hardwareItem.HardwareType == HardwareType.CPU)
                //{
                //    CpuNameTitle.Text = $"CPU : {hardwareItem.Name}";
                //    foreach (var sensor in hardwareItem.Sensors)
                //    {

                //        if (sensor.SensorType == SensorType.Temperature)
                //        {
                //            //CpuTempLabel.Text = $"{sensor.Value.Value:N1} C";


                //            try
                //            {
                //                var temp = ((sensor.Value));
                //                //CpuLoadProgressBar.Maximum = (int)sensor.Max;
                //                //CpuLoadProgressBar.Minimum = (int)sensor.Min;
                //                //CpuTempProgressBar.Value = (int)temp;

                //                //    guna2HtmlToolTip1.SetToolTip(CpuLoadProgressBar,temp.ToString());
                //                //CurrentTempLabel.Text = temp.ToString() + " °C";
                //                //guna2HtmlLabel1.Text = temp.ToString();
                //                //guna2HtmlLabel2.Text = sensor.Max.ToString();
                //                //guna2GroupBox1.Text = sensor.Name.ToString();
                //            }
                //            catch (Exception ex)
                //            {
                //                //MessageBox.Show(ex.Message);
                //            }




                //        }
                //        else if (sensor.SensorType == SensorType.Load)
                //        {
                //            var load = ((sensor.Value));
                //            //CpuLoadProgressBar.Value = (int)load;
                //        }
                //        else if (sensor.SensorType == SensorType.Voltage)
                //        {
                //            var fanspeed = ((sensor.Value));
                //            //guna2CircleProgressBar1.Value = (int)fanspeed;
                //        }
                //    }
                //}
                //else if (hardwareItem.HardwareType == HardwareType.GpuNvidia || hardwareItem.HardwareType == HardwareType.GpuAti)
                //{
                //    foreach (var sensor in hardwareItem.Sensors)
                //    {

                //    }
                //}
            }
        }

        private void UpdateCpuDetails(IHardware cpu)
        {
            try
            {
                CpuNameTitle.Text = $"CPU : {cpu.Name}";
                foreach (var sensor in cpu.Sensors)
                {
                    if (sensor.SensorType == SensorType.Temperature)
                    {
                        try
                        {
                            var temp = ((sensor.Value));
                            CpuTempLabel.Text = string.Format("{0}°C", temp); ;
                            CpuAvgTempProgressBar.Value = (int)temp;
                            CpuMaxTempProgressBar.Value = (int)sensor.Max;
                            CpuMinTempProgressBar.Value = (int)sensor.Min;

                        }
                        catch(Exception ex) 
                        {
                            CpuTempLabel.Text = string.Format("{0}°C", 0); ;
                            CpuAvgTempProgressBar.Value = 0;
                            CpuMaxTempProgressBar.Value = 0;
                            CpuMinTempProgressBar.Value = 0;

                        }
                        
                        
                        //CpuTempLabel.Text = (temp+ ""+"C");

                        //CpuLoadProgressBar.Maximum = (int)sensor.Max;
                        //CpuLoadProgressBar.Minimum = (int)sensor.Min;
                        //CpuTempProgressBar.Value = (int)temp;
                    }
                    else if (sensor.SensorType == SensorType.Load)
                    {
                        CpuAvgLoadProgressBar.Value = (int)sensor.Value;
                        CpuMaxLoadProgressBar.Value = (int)sensor.Max;
                        CpuMinLoadProgressBar.Value = (int)sensor.Min;

                        CpuAvgLoadLabel.Text = string.Format(" {0:00} %", sensor.Value);
                        ;
                    }
                    else if (sensor.SensorType == SensorType.Power && sensor.Name == "CPU Package")
                    {
                        CpuAvgPowerProgressBar.Maximum = (int)sensor.Max;
                        
                       // CpuAvgPowerProgressBar.Value = (int)sensor.Value;
                       CpuMaxPowerProgressBar.Maximum= (int)sensor.Max;
                        CpuMaxPowerProgressBar.Value = (int)sensor.Max;
                        CpuMinPowerProgressBar.Value = (int)sensor.Min;
                        CpuAvgPowerProgressBar.Value = (int)sensor.Value;
                    }

                }
            }
            catch (Exception ex) { }
        }

        private void UpdateRamDetails(IHardware ram)
        {

        }

        private void UpdateStorageDetails(IHardware storage)
        {

        }

        private void UpdateGpuDetails(IHardware gpu)
        {
            try { }catch (Exception ex) { }
        }

        private void RamGroupBox_Click(object sender, EventArgs e)
        {

        }
    }
}

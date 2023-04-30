using CsvHelper;
using Guna.UI2.WinForms;
using Microsoft.VisualBasic.Devices;
using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Tapman.Structure;

namespace Tapman
{
    public partial class TapmanUI : Form
    {

        private OpenHardwareMonitor.Hardware.Computer computer;
        ComputerInfo info = new ComputerInfo();
        private ulong TotalMemory = 0;
        

        private System.Management.ManagementObjectCollection storageDrives;

        private Timer timer;

        private string StorageOne = null;
        private string StorageTwo = null;

        public TapmanUI()
        {
            InitializeComponent();
            var scope = new ManagementScope(@"\\.\root\cimv2");
            var query = new ObjectQuery("SELECT * FROM Win32_DiskDrive");

            using (var searcher = new ManagementObjectSearcher(scope, query))
            storageDrives = searcher.Get();

            computer = new OpenHardwareMonitor.Hardware.Computer();
            computer.CPUEnabled = true;
            computer.RAMEnabled = true;
            computer.MainboardEnabled = true;
            computer.RAMEnabled = true;
            computer.MainboardEnabled = true;
            computer.GPUEnabled = true;
            computer.HDDEnabled = true;
            computer.Open();

            
            TotalMemory = info.TotalPhysicalMemory/ (1024 * 1024 * 1024);




            timer = new Timer();
            timer.Interval = 1000; // 5 seconds
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            //This palette is made up of the 6 colors.Hex color codes:  #6340bc,  #794ee6,  #090612,  #20153c,  #362367 and  #4d3291.
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            RecordSystemData recordSystemData = new RecordSystemData();
            recordSystemData.RecordTimeStamo = DateTime.Now;
            // Read the temperature data and update the labels
            foreach (var hardwareItem in computer.Hardware)
            {
                hardwareItem.Update();

                switch (hardwareItem.HardwareType)
                {
                    case (HardwareType.CPU):
                        UpdateCpuDetails(hardwareItem, recordSystemData); break;

                    case (HardwareType.RAM):
                        UpdateRamDetails(hardwareItem, recordSystemData); break;

                    case (HardwareType.GpuNvidia):
                        UpdateGpuDetails(hardwareItem,recordSystemData); break;

                    case (HardwareType.GpuAti): 
                        UpdateGpuDetails(hardwareItem,recordSystemData); break;

                    case (HardwareType.HDD):
                        UpdateStorageDetails(hardwareItem,recordSystemData); break;

                    case(HardwareType.Mainboard):
                        UpdateMotherBoardDetails(hardwareItem,recordSystemData);
                        break;
                }
            }
            WriteRecordToCsv(recordSystemData, @"C:\Users\rahil\Desktop\testing\data.csv");
        }


        private static void WriteRecordToCsv(RecordSystemData record, string filePath)
        {
            bool isHeaderWritten = false;

            if (File.Exists(filePath))
            {
                // Check if the file already contains data
                using (var reader = new StreamReader(filePath))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        csv.Read();
                        isHeaderWritten = csv.ReadHeader();
                    }
                }
            }

            // Append the record to the file
            using (var writer = new StreamWriter(filePath, true))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    if (!isHeaderWritten)
                    {
                        // Write header row
                        csv.WriteHeader<RecordSystemData>();
                        csv.NextRecord();
                    }

                    // Write the record
                    csv.WriteRecord(record);
                    csv.NextRecord();
                }
            }
        }

        private void UpdateMotherBoardDetails(IHardware motherBoard ,RecordSystemData recordSystemData)
        {
            
                MotherBoardTitle.Text = string.Format("MainBoard : {0}", motherBoard.Name);
            


        }

        private void UpdateCpuDetails(IHardware cpu, RecordSystemData recordSystemData)
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
                        catch (Exception ex)
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
                    else if (sensor.SensorType == SensorType.Load && sensor.Name == "CPU Total")
                    {
                        CpuAvgLoadProgressBar.Value = (int)sensor.Value;
                        CpuMaxLoadProgressBar.Value = (int)sensor.Max;
                        CpuMinLoadProgressBar.Value = (int)sensor.Min;

                        CpuAvgLoadLabel.Text = string.Format(" {0:00} %", sensor.Value);
                        
                    }
                    else if (sensor.SensorType == SensorType.Power && sensor.Name == "CPU Package")
                    {
                        CpuAvgPowerProgressBar.Maximum = (int)sensor.Max;

                        // CpuAvgPowerProgressBar.Value = (int)sensor.Value;
                        CpuMaxPowerProgressBar.Maximum = (int)sensor.Max + 1;
                        CpuMaxPowerProgressBar.Value = (int)sensor.Max;
                        CpuMinPowerProgressBar.Value = (int)sensor.Min;
                        CpuAvgPowerProgressBar.Value = (int)sensor.Value;
                        CpuAvgPowerLabel.Text = string.Format("{0:00} W", sensor.Value);
                    }

                }

                recordSystemData.CpuAvgTemp = CpuAvgTempProgressBar.Value;
                recordSystemData.CpuMaxTemp = CpuMaxTempProgressBar.Value;
                recordSystemData.CpuMinTemp = CpuMinTempProgressBar.Value;
                
                recordSystemData.CpuAvgLoad = CpuAvgLoadProgressBar.Value;
                recordSystemData.CpuMaxLoad = CpuMaxLoadProgressBar.Value;
                recordSystemData.CpuMinLoad = CpuMinLoadProgressBar.Value;

                recordSystemData.CpuAvgPower = CpuAvgPowerProgressBar.Value;
                recordSystemData.CpuMaxPower = CpuMaxPowerProgressBar.Value;
                recordSystemData.CpuMinPower = CpuMinPowerProgressBar.Value;

            }
            catch (Exception ex) { }
        }

        private void UpdateRamDetails(IHardware ram,RecordSystemData recordSystemData)
        {
            RamNameTitle.Text = $"RAM : {ram.Name}";
            foreach (var sensor in ram.Sensors)
            {
                if (sensor.SensorType == SensorType.Load && sensor.Name == "Memory")
                {
                    RamAvgLoadProgressBar.Value = (int)sensor.Value;
                    RamMaxLoadProgressBar.Value = (int)sensor.Max;
                    RamMinLoadProgressBar.Value = (int)sensor.Min;
                    RamAvgTitle.Text = string.Format(" {0:00} %", sensor.Value);
                }
                else if (sensor.SensorType == SensorType.Data && sensor.Name == "Used Memory")
                {
                    RamAvgDataProgressBar.Maximum = (int)TotalMemory;
                    RamMinDataProgressBar.Maximum = (int)TotalMemory;
                    RamMaxDataProgressBar.Maximum = (int)TotalMemory;
                    
                    RamAvgDataProgressBar.Value = (int)sensor.Value;
                    RamMaxDataProgressBar.Value = (int)sensor.Max;
                    RamMinDataProgressBar.Value = (int)sensor.Min;
                    
                    RamAvgDataLabel.Text = string.Format(" {0:00} GB", sensor.Value); ;
                }
            }

            recordSystemData.RamAvgDataUsed = RamAvgDataProgressBar.Value;
            recordSystemData.RamMaxDataUsed = RamMaxDataProgressBar.Value;
            recordSystemData.RamMinDataUsed = RamMinDataProgressBar.Value;
            
            recordSystemData.RamAvgLoad = RamAvgLoadProgressBar.Value;
            recordSystemData.RamMaxLoad = RamMaxLoadProgressBar.Value;
            recordSystemData.RamMinLoad = RamMinLoadProgressBar.Value;

        }

        private void UpdateStorageDetails(IHardware storage,RecordSystemData recordSystemData)
        {
            if(storageDrives.Count > 1)
            {
                if(StorageOne == null)
                {
                    StorageOne = storage.Name;
                }
                
                if(StorageTwo == null)
                {
                    StorageTwo = storage.Name;
                }

                if(storage.Name ==  StorageOne ) 
                {
                    if (storage.Name.Length > 16) // Check if storage name is greater than 30 characters
                    {
                        StrorageName.Font = new Font(StrorageName.Font.FontFamily, 14, StrorageName.Font.Style);
                        StorageNameTitle.Text = $"STORAGE:";
                        StrorageName.Location = new Point(125, 13);
                    }

                        StrorageName.Text = $"{ storage.Name}";

                    //StorageNameTitle.Text = $"Storage : {storage.Name}";
                   
                    foreach(var sensor in storage.Sensors)
                    {
                        if(sensor.SensorType == SensorType.Temperature)
                        {
                            StorageAvgTemp.Text = $"{(int)sensor.Value}°C";
                            HddAvgTempProgressBar.Value = (int)sensor.Value;
                            HddMaxTempProgressBar.Value = (int)sensor.Max;
                            HddMinTempProgressBar.Value = (int)sensor.Min;
                        }
                        if (sensor.SensorType == SensorType.Load)
                        {
                            AvgStorageLoad.Text = $"{((int)sensor.Value)} %";
                            HddAvgLoadProgressBar.Value = (int)sensor.Value;
                            HddMaxLoadProgressBar.Value = (int)sensor.Max;
                            HddMinLoadProgressBar.Value = (int)sensor.Min;
                        }
                    }

                    recordSystemData.StorageAvgDataUsed = HddAvgLoadProgressBar.Value;
                    recordSystemData.StorageMaxDataUsed = HddMaxLoadProgressBar.Value;
                    recordSystemData.StorageMinDataUsed = HddMinLoadProgressBar.Value;

                    recordSystemData.StorageAvgTemp = HddAvgTempProgressBar.Value;
                    recordSystemData.StorageMaxTemp = HddMaxTempProgressBar.Value;
                    recordSystemData.StorageMinTemp = HddMinLoadProgressBar.Value;


                }
                

            }
            else
            {

            }

            //StorageNameTitle.Text = $"Storage : {storage.Name}";

        }

        private void UpdateGpuDetails(IHardware gpu,RecordSystemData recordSystemData)
        {
            try {

                GpuNameTitle.Text = $"GPU : {gpu.Name}";
                foreach(var sensor in gpu.Sensors)
                {
                    if (sensor.SensorType == SensorType.Temperature)
                    {
                        try
                        {
                            var temp = ((sensor.Value));
                            GpuAvgTempLabel.Text = string.Format("{0}°C", temp); ;
                            GpuAvgTempProgressBar.Value = (int)temp;
                            GpuMaxTempProgressBar.Value = (int)sensor.Max;
                            GpuMinTempProgressBar.Value = (int)sensor.Min;

                        }
                        catch (Exception ex)
                        {
                            CpuTempLabel.Text = string.Format("{0}°C", 0); ;
                            GpuAvgTempProgressBar.Value = 0;
                            GpuMaxTempProgressBar.Value = 0;
                            GpuMinTempProgressBar.Value = 0;

                        }


                        //CpuTempLabel.Text = (temp+ ""+"C");

                        //CpuLoadProgressBar.Maximum = (int)sensor.Max;
                        //CpuLoadProgressBar.Minimum = (int)sensor.Min;
                        //CpuTempProgressBar.Value = (int)temp;
                    }
                    else if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Core")
                    {
                        GpuAvgLoadProgressBar.Value = (int)sensor.Value;
                        GpuMaxLoadProgressBar.Value = (int)sensor.Max;
                        GpuMinLoadProgressBar.Value = (int)sensor.Min;

                        GpuAvgLoadLabel.Text = string.Format(" {0:00} %", sensor.Value);

                    }
                    else if (sensor.SensorType == SensorType.Power && sensor.Name == "GPU Power")
                    {
                        GpuAvgPowerProgressBar.Maximum = (int)sensor.Max;

                        // CpuAvgPowerProgressBar.Value = (int)sensor.Value;
                        GpuMaxPowerProgressBar.Maximum = (int)sensor.Max + 1;
                        GpuMaxPowerProgressBar.Value = (int)sensor.Max;
                        GpuMinPowerProgressBar.Value = (int)sensor.Min;
                        GpuAvgPowerProgressBar.Value = (int)sensor.Value;
                        GpuAvgPowerLabel.Text = string.Format("{0:00} W", sensor.Value);
                    }

                }

                recordSystemData.GpuAvgTemp = GpuAvgTempProgressBar.Value;
                recordSystemData.GpuMaxTemp = GpuMaxTempProgressBar.Value;
                recordSystemData.GpuMinTemp = GpuMinTempProgressBar.Value;

                recordSystemData.GpuAvgLoad = GpuAvgLoadProgressBar.Value;
                recordSystemData.GpuMaxLoad = GpuMaxLoadProgressBar.Value;
                recordSystemData.GpuMinLoad = GpuMinLoadProgressBar.Value;

                recordSystemData.GpuAvgPower = GpuAvgPowerProgressBar.Value;
                recordSystemData.GpuMinPower = GpuMinPowerProgressBar.Value;
                recordSystemData.GpuMaxPower = GpuMaxPowerProgressBar.Value;



            } catch (Exception ex) { }
        }

        private void RamGroupBox_Click(object sender, EventArgs e)
        {

        }

        private void TapmanUI_Load(object sender, EventArgs e)
        {

        }

        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(RecordData.Checked)
            {

            }

        }

        private void guna2ImageButton2_Click(object sender, EventArgs e)
        {
           DashboardUIPannel.Enabled = false;
            DashboardUIPannel.Visible = false;

            panel1.Enabled = true;
            panel1.Visible = true;
        }

        private void guna2ImageButton1_Click(object sender, EventArgs e)
        {
            panel1.Enabled = false;
            panel1.Visible = false;
            DashboardUIPannel.Enabled = true;
            DashboardUIPannel.Visible=true;
            timer.Stop();
            
            

        }
    }
}

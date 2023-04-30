using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tapman
{
    internal class Structure
    {
        internal class RecordSystemData
        {
            public DateTime RecordTimeStamo { get; set; } 
            
            public int CpuAvgTemp { get; set; } = default;
            public int CpuMaxTemp { get; set; }= default;
            public int CpuMinTemp { get; set;} = default;
            public int CpuAvgLoad { get; set; } = default;
            public int CpuMaxLoad { get; set;} = default;
            public int CpuMinLoad { get; set;} = default;
            public int CpuAvgPower { get; set; } = default;
            public int CpuMaxPower { get; set;} = default;
            public int CpuMinPower { get; set;} = default;

            public int GpuAvgTemp { get; set; } = default;
            public int GpuMaxTemp { get; set; } = default;
            public int GpuMinTemp { get; set; } = default;
            public int GpuAvgLoad { get; set; }=default;
            public int GpuMaxLoad { get; set; } = default;
            public int GpuMinLoad { get; set; } = default;
            public int GpuAvgPower { get;set; } = default;
            public int GpuMaxPower { get; set; } = default;
            public int GpuMinPower { get; set; } = default;


            public int RamAvgLoad { get; set; } = default;
            public int RamMaxLoad { get;set; } = default;
            public int RamMinLoad { get;set; } = default;

            public int RamAvgDataUsed { get; set; } = default;
            public int RamMaxDataUsed { get; set; } = default;
            public int RamMinDataUsed { get;set; } = default;

            public int StorageAvgTemp { get; set; } = default;
            public int StorageMaxTemp { get; set;} = default;
            public int StorageMinTemp { get;set; } = default;
            public int StorageAvgDataUsed { get;  set; } = default;
            public int StorageMaxDataUsed { get;set;} = default;
            public int StorageMinDataUsed { get;set ; } = default;
        }

    }
}

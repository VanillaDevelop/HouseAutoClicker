using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseAutoClicker
{
    class Utility
    {
        #region properties
        /// <summary>
        /// Describes the rough time it takes for 1 function call in milliseconds 
        /// </summary>
        public const int RTT = 1750;
        #endregion

        /// <summary>
        /// Returns all FFXIV Processes running on DX11 (denoted by the name ffxiv_dx11)
        /// </summary>
        /// <returns>A list of processes with names matching "ffxiv_dx11"</returns>
        public static Process[] GetAllXivProcesses()
        {
            return Process.GetProcessesByName("ffxiv_dx11");
        }
    }
}

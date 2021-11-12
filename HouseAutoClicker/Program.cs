using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HouseAutoClicker
{
    class Program
    {
        static void Main(string[] args)
        {
            ClickThreadManager manager = new ClickThreadManager();
            manager.ActiveLoop();
        }
    }
}

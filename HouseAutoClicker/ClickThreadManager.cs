using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HouseAutoClicker
{
    /// <summary>
    /// Manager class for the House Auto Clicker.
    /// </summary>
    class ClickThreadManager
    {
        #region fields
        private volatile SynchronizedStatus status;
        private Dictionary<int, Thread> activeThreads;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new Thread Manager
        /// </summary>
        public ClickThreadManager()
        {
            activeThreads = new Dictionary<int, Thread>();
            status = new SynchronizedStatus(0, -1);
        }
        #endregion

        #region methods
        public void ActiveLoop()
        {
            Console.WriteLine("#################################################");
            Console.WriteLine("############ FFXIV House AutoClicker ############");
            Console.WriteLine("####### Because we all have brain damage ########");
            Console.WriteLine("#################################################");
            Console.WriteLine("Tip: If you are new, type 'help' to get started.");
            Console.WriteLine("\n\n");
            while(true)
            {
                Console.Write("#: ");
                string input = Console.ReadLine();
                switch(input.Split(' ')[0])
                {
                    case "help":
                        PrintHelp();
                        break;

                    case "start":
                        ProcessStart(input);
                        break;

                    case "stop":
                        ProcessStop(input);
                        break;

                    case "list":
                        ListProcesses();
                        break;

                    case "sync":
                        ToggleSyncMode(input);
                        break;
                }
            }
        }

        public Thread StartThread(int id)
        {
            var processes = Utility.GetAllXivProcesses();
            if (processes.Length < id)
                return null;
            else
            {
                Thread t = new Thread(new ThreadStart(() => ClickThread.ThreadLoop(processes[id].MainWindowHandle, this.status, id)));
                t.Start();
                return t;
            }
        }

        public void StopThread(int id)
        {
            if(activeThreads.ContainsKey(id))
            {
                activeThreads[id].Abort();
            }
        }

        //=================== METHODS FOR PROCESSING CONSOLE COMMANDS ===================
        /// <summary>
        /// Prints help (help command in the console)
        /// </summary>
        public void PrintHelp()
        {
            Console.WriteLine("======================== COMMANDS ========================");
            Console.WriteLine("help - Shows this menu");
            Console.WriteLine("list - Lists all currently running FFXIV processes and clicker threads");
            Console.WriteLine("start [all/ID] - Starts a clicker thread on the corresponding FFXIV process ID (or all)");
            Console.WriteLine("stop [all/ID] - Stops the active clicker thread on the corresponding FFXIV process ID (or all)");
            Console.WriteLine("sync [on/off] - Turns clicker sync on or off - if turned on, threads will try to not overlap purchase attempts");
        }

        /// <summary>
        /// Attempts to start a clicker thread (start command in the console)
        /// </summary>
        /// <param name="input">The input provided in the console</param>
        public void ProcessStart(string input)
        {
            var processes  = Utility.GetAllXivProcesses();

            try
            {
                if (input.Split(' ')[1].ToLower().Equals("all"))
                {
                    for (int i = 0; i < processes.Length; i++)
                    {
                        if (!activeThreads.ContainsKey(i))
                        {
                            Thread t = StartThread(i);
                            if (t != null)
                                activeThreads.Add(i, t);
                        }
                    }
                    return;
                }

                int id = Convert.ToInt32(input.Split(' ')[1]);
                if (activeThreads.ContainsKey(id))
                    Console.WriteLine("This process is already running a clicker thread. Stop it first using 'stop [id]' if you wish to restart it.");
                else if (id >= processes.Length || id < 0)
                    Console.WriteLine("Process ID does not exist. See available processes with 'list'.");
                else
                {
                    Thread t = StartThread(id);
                    if (t != null)
                        activeThreads.Add(id, t);
                }
            }
            catch
            {
                Console.WriteLine("Could not launch thread. Syntax: 'start [all/ThreadID]'");
            }
        }

        /// <summary>
        /// Attempts to stop a clicker thread (stop command in the console)
        /// </summary>
        /// <param name="input">The input provided in the console</param>
        public void ProcessStop(string input)
        {
            try
            {
                if (input.Split(' ')[1].ToLower().Equals("all"))
                {
                    foreach (var kvp in activeThreads)
                    {
                        StopThread(kvp.Key);
                    }
                    activeThreads.Clear();
                    return;
                }

                int id = Convert.ToInt32(input.Split(' ')[1]);
                if (!activeThreads.ContainsKey(id))
                    Console.WriteLine("No thread with this id is running.");
                else
                {
                    StopThread(id);
                    activeThreads.Remove(id);
                }
            }
            catch
            {
                Console.WriteLine("Could not stop thread. Syntax: 'stop [all/ThreadID]'");
            }
        }

        /// <summary>
        /// Lists all currently running FFXIV processes and clicker threads (list command in the console)
        /// </summary>
        public void ListProcesses()
        {
            var processes = Utility.GetAllXivProcesses();
            Console.WriteLine("FFXIV Processes:");
            if (processes.Length > 0)
            {
                for (int i = 0; i < processes.Length; i++)
                {
                    var process = processes[i];
                    Console.WriteLine("ID " + i + " - Launched at " + process.StartTime);
                }
            }
            else
                Console.WriteLine("None");

            Console.WriteLine("Clicker Threads:");
            if (activeThreads.Count > 0)
            {
                foreach (KeyValuePair<int, Thread> kvp in activeThreads)
                {
                    Console.WriteLine("Thread running on FFXIV Process " + kvp.Key);
                }
            }
            else
                Console.WriteLine("None");
        }

        /// <summary>
        /// Toggles "sync mode" on or off (sync command in the console)
        /// </summary>
        /// <param name="input">The input provided in the console.</param>
        public void ToggleSyncMode(string input)
        {
            var onoff = input.Split(' ')[1];
            if (onoff.ToLower().Equals("on"))
            {
                status.SyncMode = true;
            }
            else if(onoff.ToLower().Equals("off"))
            {
                status.SyncMode = false;
            }
            else
            {
                Console.WriteLine("Not recognized. Use 'sync [on/off]'.");
            }
        }
        #endregion
    }
}

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
        private Dictionary<int, ThreadStatus> threadStatus;
        private DateTime startTime;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new Thread Manager
        /// </summary>
        public ClickThreadManager()
        {
            activeThreads = new Dictionary<int, Thread>();
            status = new SynchronizedStatus(0, -1);
            this.startTime = DateTime.UtcNow;
            threadStatus = new Dictionary<int, ThreadStatus>();
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

                    case "purchasefc":
                        TogglePurchaseFC(input);
                        break;

                    case "sanity":
                        PrintSanityCheck();
                        break;

                    default:
                        Console.WriteLine("Unrecognized command. Use 'help' for valid syntax.");
                        break;
                }
            }
        }

        public void StartThread(int id)
        {
            var processes = Utility.GetAllXivProcesses();
            if (processes.Length < id)
                return;

            if (threadStatus.ContainsKey(id)) threadStatus[id] = new ThreadStatus();
            else threadStatus.Add(id, new ThreadStatus());
            Thread t = new Thread(new ThreadStart(() => ClickThread.ThreadLoop(processes[id].MainWindowHandle, this.status, threadStatus[id], id)));
            t.Start();
            activeThreads.Add(id, t);
            status.AddThreadToQueue(id);
        }

        public void StopThread(int id, bool delete_from_list = true)
        {
            if(activeThreads.ContainsKey(id))
            { 
                activeThreads[id].Abort();
                status.RemoveThreadFromQueue(id);
                threadStatus[id].DeactivateThread();
                if (delete_from_list)
                    activeThreads.Remove(id);
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
            Console.WriteLine("sanity - Gives the user a sanity check. Not recommended.");
            Console.WriteLine("purchasefc [on/off] - If set to on, executes command to purchase an FC house. Otherwise house for own character (default).");
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
                            StartThread(i);
                    }
                    return;
                }

                int id = Convert.ToInt32(input.Split(' ')[1]);
                if (activeThreads.ContainsKey(id))
                    Console.WriteLine("This process is already running a clicker thread. Stop it first using 'stop [id]' if you wish to restart it.");
                else if (id >= processes.Length || id < 0)
                    Console.WriteLine("Process ID does not exist. See available processes with 'list'.");
                else
                    StartThread(id);
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
                        StopThread(kvp.Key, false);

                    activeThreads.Clear();
                    return;
                }

                int id = Convert.ToInt32(input.Split(' ')[1]);
                if (!activeThreads.ContainsKey(id))
                    Console.WriteLine("No thread with this id is running.");
                else
                    StopThread(id);
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

        /// <summary>
        /// Toggles "purchase FC house mode" on or off (purchasefc command in the console)
        /// </summary>
        /// <param name="input">The input provided in the console.</param>
        public void TogglePurchaseFC(string input)
        {
            var onoff = input.Split(' ')[1];
            if (onoff.ToLower().Equals("on"))
            {
                Settings.Instance.PurchaseSelfHouse = false;
            }
            else if (onoff.ToLower().Equals("off"))
            {
                Settings.Instance.PurchaseSelfHouse = true;
            }
            else
            {
                Console.WriteLine("Not recognized. Use 'purchasefc [on/off]'.");
            }
        }

        /// <summary>
        /// Prints a "sanity check" (usage statistic) to the console. (sanity command in the console)
        /// </summary>
        public void PrintSanityCheck()
        {
            Console.WriteLine("######################## SANITY CHECK ########################");
            Console.WriteLine("The House Auto Clicker has been running since " + startTime + " UTC");
            Console.WriteLine("The following thread statuses were found:");
            if (threadStatus.Count > 0)
            {
                foreach (var kvp in threadStatus)
                {
                    Console.WriteLine("ID: " + kvp.Key);
                    Console.WriteLine("Current Status: " + (kvp.Value.ThreadActive ? "Active" : "Inactive"));
                    Console.WriteLine("This Thread was started at " + kvp.Value.StartTime + " UTC and has made " + kvp.Value.Attempts +
                        " attempts to purchase a house.");
                }
            }
            else
                Console.WriteLine("No clicker threads have been run yet.");
        }
        #endregion
    }
}

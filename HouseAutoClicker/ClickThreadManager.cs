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
    /// <summary>
    /// Manager class for the House Auto Clicker.
    /// </summary>
    class ClickThreadManager
    {
        #region fields
        private Dictionary<int, Thread> activeThreads;
        private Dictionary<int, ThreadStatistics> threadStatus;
        private DateTime startTime;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new Thread Manager
        /// </summary>
        public ClickThreadManager()
        {
            activeThreads = new Dictionary<int, Thread>();
            this.startTime = DateTime.UtcNow;
            threadStatus = new Dictionary<int, ThreadStatistics>();
        }
        #endregion

        #region activeloop
        /// <summary>
        /// The active loop of the main thread which allows for user input.
        /// </summary>
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

                    case "info":
                        ListInfo();
                        break;

                    case "sync":
                        ToggleSyncMode(input);
                        break;

                    case "purchasefc":
                        TogglePurchaseFC(input);
                        break;

                    case "randomdelay":
                        ToggleRandomDelay(input);
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
        #endregion

        #region methods
        /// <summary>
        /// Starts a thread on the given index XIV client
        /// </summary>
        /// <param name="id">The index of the XIV client when listing the processes to which to attach the thread to.</param>
        public void StartThread(int id)
        {
            var processes = GetAllXivProcesses();
            if (processes.Length < id)
                return;

            if (!threadStatus.ContainsKey(id)) threadStatus.Add(id, new ThreadStatistics());
            Thread t = new Thread(new ThreadStart(() => ClickThread.ThreadLoop(processes[id].MainWindowHandle, threadStatus[id], id)));
            t.Start();
            activeThreads.Add(id, t);
            SynchronizedStatus.Instance.AddThreadToQueue(id);
        }

        /// <summary>
        /// Stops a thread on the given index XIV client
        /// </summary>
        /// <param name="id">The index of the XIV client when listing the processes to which the thread is attached to.</param>
        /// <param name="delete_from_list">Whether or not to remove the thread from the list of active threads</param>
        public void StopThread(int id, bool delete_from_list = true)
        {
            if(activeThreads.ContainsKey(id))
            { 
                activeThreads[id].Abort();
                SynchronizedStatus.Instance.RemoveThreadFromQueue(id);
                threadStatus[id].DeactivateThread();
                if (delete_from_list)
                    activeThreads.Remove(id);
            }
        }

        /// <summary>
        /// Returns all FFXIV Processes running on DX11 (denoted by the name ffxiv_dx11)
        /// Processes are sorted by start time
        /// </summary>
        /// <returns>A list of processes with names matching "ffxiv_dx11"</returns>
        public static Process[] GetAllXivProcesses()
        {
            return Process.GetProcessesByName("ffxiv_dx11").OrderBy(x => x.StartTime).ToArray();
        }
        #endregion

        #region console_handlers
        //=================== METHODS FOR PROCESSING CONSOLE COMMANDS ===================

        /// <summary>
        /// Prints help (help command in the console)
        /// </summary>
        public void PrintHelp()
        {
            Console.WriteLine("======================== COMMANDS ========================");
            Console.WriteLine("help - Shows this menu");
            Console.WriteLine("info - Lists all currently running FFXIV processes and clicker threads as well as the current settings.");
            Console.WriteLine("start [all/ID] - Starts a clicker thread on the corresponding FFXIV process ID (or all)");
            Console.WriteLine("stop [all/ID] - Stops the active clicker thread on the corresponding FFXIV process ID (or all)");
            Console.WriteLine("sync [on/off] - Turns clicker sync on or off - if turned on, threads will try to not overlap purchase attempts");
            Console.WriteLine("randomdelay [on/off] - Toggles a small random delay after each iteration. Might avoid bot detection but probably does nothing.");
            Console.WriteLine("purchasefc [on/off] - If set to on, executes command to purchase an FC house. Otherwise house for own character (default).");
            Console.WriteLine("sanity - Gives the user a sanity check. Not recommended.");
        }

        /// <summary>
        /// Attempts to start a clicker thread (start command in the console)
        /// </summary>
        /// <param name="input">The input provided in the console</param>
        public void ProcessStart(string input)
        {
            var processes  = GetAllXivProcesses();

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
        /// Lists all currently running FFXIV processes, settings, and clicker threads (info command in the console)
        /// </summary>
        public void ListInfo()
        {
            var processes = GetAllXivProcesses();
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

            Console.WriteLine("Current Settings:");
            Console.WriteLine("Sync Mode - " + (Settings.Instance.SyncMode ? "On" : "Off"));
            Console.WriteLine("Purchase Mode - " + (Settings.Instance.PurchaseSelfHouse ? "Personal" : "Free Company"));
            Console.WriteLine("Random Delay - " + (Settings.Instance.RandomDelay ? "On" : "Off"));
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
                Settings.Instance.SyncMode = true;
            }
            else if(onoff.ToLower().Equals("off"))
            {
                Settings.Instance.SyncMode = false;
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
        /// Toggles random delay on or off (randomdelay command in the console)
        /// </summary>
        /// <param name="input">The input provided in the console.</param>
        public void ToggleRandomDelay(string input)
        {
            var onoff = input.Split(' ')[1];
            if (onoff.ToLower().Equals("on"))
            {
                Settings.Instance.RandomDelay = true;
            }
            else if (onoff.ToLower().Equals("off"))
            {
                Settings.Instance.RandomDelay = false;
            }
            else
            {
                Console.WriteLine("Not recognized. Use 'randomdelay [on/off]'.");
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
                    if (kvp.Value.ThreadActive)
                    {
                        Console.WriteLine("The current thread was started at " + kvp.Value.StartTime + " UTC and has made " + kvp.Value.CurrentAttempts +
                        " attempts to purchase a house.");
                    }
                    else
                    {
                        Console.WriteLine("The last thread on this client stopped at " + kvp.Value.EndTime + " UTC and made " + kvp.Value.CurrentAttempts + 
                            " attempts to purchase a house.");
                    }
                    Console.WriteLine("In total, there was " + kvp.Value.TotalAttempts + " attempts to purchase a house on this client.\n");
                }
            }
            else
                Console.WriteLine("No clicker threads have been run yet.");
        }
        #endregion
    }
}

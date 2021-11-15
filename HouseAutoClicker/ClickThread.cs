using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace HouseAutoClicker
{
    class ClickThread
    {
        #region  DLLImports
        //Low level call which posts a message to a specific window handle
        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        #endregion

        #region constants
        //Defines constants for sending key-up and key-down messages
        const uint WM_KEYDOWN = 0x100;
        const uint WM_KEYUP = 0x0101;
        static readonly Random r = new Random();
        #endregion

        #region methods
        public static void ThreadLoop(IntPtr handle, SynchronizedStatus status, ThreadStatus tStatus, int threadId)
        {
            SendHousePurchase(handle, true);
            while (true)
            {
                //If sync mode is on, we set the following threshold:
                //For N threads, There has to be at least RTT/2N ms between each call, where RTT is the time one request takes
                //e.g., if it takes 2000 ms to do one request, the ideal distance between each request is 2000/N
                //we set the minimum distance to half that, i.e. RTT/2N
                //Additionally, threads can only operate in a fixed order set by the queue of the synchronized status.
                if (status.SyncMode && status.GetNThreads() > 1)
                {

                    while(!status.IsNextThread(threadId))
                        Thread.Sleep(50);

                    //When it's this threads turn to operate, make sure that it's been at least the threshold time since the last call
                    long time_since_last_call = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - status.LastThreadClick;
                    if(time_since_last_call < Utility.RTT / (2 * status.GetNThreads()))
                    {
                        //set an artificial delay such that delay + time_since_last_call = RTT/N, the ideal distance
                        Thread.Sleep((int)((long)Math.Round((double)Utility.RTT / status.GetNThreads()) - time_since_last_call));
                    }

                    //Update the status
                    lock (status)
                    {
                        status.LastThreadClick = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                        status.UpdateQueue();
                    }
                }
                SendHousePurchase(handle);
                tStatus.Attempts++;
                Thread.Sleep(r.Next(400, 550));
            }
        }

        /// <summary>
        /// Sends the specific combination of key presses required to purchase a house (FC purchase)
        /// </summary>
        /// <param name="handle">The window handle of the window to send the key presses to</param>
        /// <param name="first_run">Adds an additional "confirm" click after opening the placard. This is necessary if KB/M is 
        /// usually used to operate the game, as the "gamepad selection cursor" is not enabled by default under these circumstances.</param>
        private static void SendHousePurchase(IntPtr handle, bool first_run = false)
        {

            SendKeyPress(handle, Settings.Instance.ConfirmKey, 50);
            SendKeyPress(handle, Settings.Instance.ConfirmKey, 900);
            if (first_run)
                SendKeyPress(handle, Settings.Instance.ConfirmKey, 50);
            SendKeyPress(handle, Settings.Instance.ConfirmKey, 100);
            if(!Settings.Instance.PurchaseSelfHouse) 
                SendKeyPress(handle, Settings.Instance.DownKey, 50);
            SendKeyPress(handle, Settings.Instance.ConfirmKey, 200);
            SendKeyPress(handle, Settings.Instance.RightKey, 50);
            SendKeyPress(handle, Settings.Instance.ConfirmKey, 0);
        }

        /// <summary>
        /// Sends a keypress to a window handle via low level api calls and delays for given amount of ms
        /// </summary>
        /// <param name="handle">The window handle of the window to send the key press</param>
        /// <param name="key">The key to send</param>
        /// <param name="delay">The delay in ms after the key press</param>
        private static void SendKeyPress(IntPtr handle, Keys key, int delay)
        {
            PostMessage(handle, WM_KEYDOWN, (IntPtr)key, IntPtr.Zero);
            PostMessage(handle, WM_KEYUP, (IntPtr)key, IntPtr.Zero);
            Thread.Sleep(delay);
        }
        #endregion
    }
}

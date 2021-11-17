using HouseAutoClicker.models;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace HouseAutoClicker
{
    /// <summary>
    /// Class which manages behavior for one clicker thread.
    /// </summary>
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
        public static void ThreadLoop(IntPtr handle, ThreadStatistics tStatus, int threadId)
        {
            //signify to the statistics object that the thread has started
            tStatus.ActivateThread();

            //Handle the initial purchase attempt (with the extra click)
            ExecuteSequence(handle, ConstantData.OpenInitialPurchaseWindowSequence);
            if (Settings.Instance.PurchaseSelfHouse) ExecuteSequence(handle, ConstantData.PurchaseSelfHouseSequence);
            else ExecuteSequence(handle, ConstantData.PurchaseFCHouseSequence);

            while (true)
            {
                //pick FC or personal house sequence depending on the flag
                KeyPressSequence nextSequence = Settings.Instance.PurchaseSelfHouse ? ConstantData.PurchaseSelfHouseSequence : ConstantData.PurchaseFCHouseSequence;

                //If sync mode is on, we set the following threshold:
                //For N threads, There has to be at least RTT/2N ms between each call, where RTT is the time one request takes
                //e.g., if it takes 2000 ms to do one request, the ideal distance between each request is 2000/N
                //we set the minimum distance to half that, i.e. RTT/2N
                //Additionally, threads can only operate in a fixed order set by the queue of the synchronized status.
                if (Settings.Instance.SyncMode && SynchronizedStatus.Instance.GetNThreads() > 1)
                {
                    //Estimate RTT based on the chosen sequence
                    int RTT = ConstantData.OpenPurchaseWindowSequence.RTT + nextSequence.RTT;

                    while (!SynchronizedStatus.Instance.IsNextThread(threadId))
                        Thread.Sleep(50);

                    //When it's this threads turn to operate, make sure that it's been at least the threshold time since the last call
                    long time_since_last_call = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - SynchronizedStatus.Instance.LastThreadClick;
                    if(time_since_last_call < RTT / (2 * SynchronizedStatus.Instance.GetNThreads()))
                    {
                        //set an artificial delay such that delay + time_since_last_call = RTT/N, the ideal distance
                        Thread.Sleep((int)((long)Math.Round((double)RTT / SynchronizedStatus.Instance.GetNThreads()) - time_since_last_call));
                    }

                    //Update the status
                    SynchronizedStatus.Instance.UpdateQueue();
                }
                
                ExecuteSequence(handle, nextSequence);
                tStatus.AddAttempt();
                if(Settings.Instance.RandomDelay) Thread.Sleep(r.Next(0, ConstantData.RandomDelayMax));
            }
        }

        /// <summary>
        /// Executes a KeyPressSequence, including delays
        /// </summary>
        /// <param name="handle">The window handle of the window to send the key sequence</param>
        /// <param name="sequence">The sequence to execute</param>
        private static void ExecuteSequence(IntPtr handle, KeyPressSequence sequence)
        {
            foreach (KeyPress keyPress in sequence.Sequence)
            {
                SendKeyPress(handle, keyPress.Key, keyPress.DelayAfterKeyPress);
            }
            Thread.Sleep(sequence.FinalDelay);
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

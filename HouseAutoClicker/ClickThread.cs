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
        public static void ThreadLoop(IntPtr handle, SynchronizedStatus status, int threadId)
        {
            SendHousePurchase(handle, true);
            while (true)
            {
                if(status.SyncMode)
                {
                    var timeSinceLastCall = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - status.LastThreadClick;
                    //if it has been less than 500ms since another thread last started a purchase
                    if(status.LastThreadId != threadId && timeSinceLastCall < 500)
                    {
                        //wait until we are offset by 1000ms from that threads call
                        var delay = 1000 - timeSinceLastCall;
                        //write this delay 
                        lock (status)
                        {
                            status.LastThreadClick = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + delay;
                        }
                        //do the delay
                        Thread.Sleep((int)delay);
                    }
                    status.LastThreadId = threadId;
                    status.LastThreadClick = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                }

                SendHousePurchase(handle);
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
            SendKeyPress(handle, Keys.Up, 50);
            SendKeyPress(handle, Keys.Up, 900);
            if (first_run)
                SendKeyPress(handle, Keys.Up, 50);
            SendKeyPress(handle, Keys.Up, 100);
            SendKeyPress(handle, Keys.Left, 50);
            SendKeyPress(handle, Keys.Up, 200);
            SendKeyPress(handle, Keys.Right, 50);
            SendKeyPress(handle, Keys.Up, 0);
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

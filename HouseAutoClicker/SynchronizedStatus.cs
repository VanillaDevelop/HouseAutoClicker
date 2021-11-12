using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseAutoClicker
{
    /// <summary>
    /// Model class to hold current status for thread synchronization
    /// </summary>
    class SynchronizedStatus
    {
        #region properties

        /// <summary>
        /// The timestamp of the start of the last purchase event
        /// </summary>
        public long LastThreadClick { get; set; }
        /// <summary>
        /// The ID of the thread which last started a purchase event
        /// </summary>
        public int LastThreadId { get; set; }
        /// <summary>
        /// The number of currently running threads
        /// </summary>
        public int NThreads { get; set; }
        /// <summary>
        /// The current status (on/off) of the sync mode
        /// </summary>
        public bool SyncMode { get; set; }
        #endregion

        #region ctor
        /// <summary>
        /// Initialize the ThreadStatus
        /// </summary>
        public SynchronizedStatus(long lastThreadClick, int lastThreadId)
        {
            this.LastThreadId = -1;
            this.LastThreadClick = 0;
            this.SyncMode = false;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseAutoClicker
{
    /// <summary>
    /// Stores the thread internal status of a single thread.
    /// </summary>
    class ThreadStatus
    {
        #region properties
        /// <summary>
        /// Stores the amounts of purchase attempts made on this thread
        /// </summary>
        public int Attempts { get; set; }
        /// <summary>
        /// Stores the UTC Start time of this thread
        /// </summary>
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        /// <summary>
        /// Stores whether or not the Thread is still running
        /// </summary>
        public bool ThreadActive { get; private set; }
        #endregion

        #region ctor
        public ThreadStatus()
        {
            this.StartTime = DateTime.UtcNow;
            this.Attempts = 0;
            this.ThreadActive = true;
        }
        #endregion

        #region methods
        /// <summary>
        /// Sets this thread to deactivated
        /// </summary>
        public void DeactivateThread()
        {
            ThreadActive = false;
            this.EndTime = DateTime.UtcNow;
        }
        #endregion
    }
}

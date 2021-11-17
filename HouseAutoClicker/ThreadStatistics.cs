using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseAutoClicker
{
    /// <summary>
    /// Stores the internal statistics of a single thread.
    /// </summary>
    class ThreadStatistics
    {
        #region properties
        /// <summary>
        /// The total amounts of purchase attempts made on the XIV client belonging to the corresponding Thread ID
        /// </summary>
        public int TotalAttempts { get; private set; }
        /// <summary>
        /// The amount of purchase attempts made on a currently running thread of the corresponding Thread ID
        /// </summary>
        public int CurrentAttempts { get; private set; }
        /// <summary>
        /// Stores the UTC Start Time of the current thread
        /// </summary>
        public DateTime StartTime { get; private set; }
        /// <summary>
        /// Stores the UTC End Time of the current thread
        /// </summary>
        public DateTime EndTime { get; private set; }
        /// <summary>
        /// Stores whether or not a thread is currently running on this client.
        /// </summary>
        public bool ThreadActive { get; private set; }
        #endregion

        #region ctor
        public ThreadStatistics()
        {
            this.StartTime = DateTime.UtcNow;
            this.EndTime = DateTime.UtcNow;
            this.TotalAttempts = 0;
            this.CurrentAttempts = 0;
            this.ThreadActive = true;
        }
        #endregion

        #region methods
        /// <summary>
        /// Signifies that a new thread has started running on this client ID.
        /// </summary>
        public void ActivateThread()
        {
            this.ThreadActive = true;
            this.StartTime = DateTime.UtcNow;
            this.CurrentAttempts = 0;
        }
        /// <summary>
        /// Signifies that the current thread has stopped running on this client ID.
        /// </summary>
        public void DeactivateThread()
        {
            this.ThreadActive = false;
            this.EndTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Adds a purchase attempt to the statistics
        /// </summary>
        public void AddAttempt()
        {
            this.TotalAttempts++;
            this.CurrentAttempts++;
        }
        #endregion
    }
}

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
        public int Attempts { get; set; }
        public DateTime StartTime { get; private set; }
        #endregion

        #region ctor
        public ThreadStatus()
        {
            this.StartTime = DateTime.UtcNow;
            this.Attempts = 0;
        }
        #endregion
    }
}

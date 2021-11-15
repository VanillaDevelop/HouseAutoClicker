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
        #region fields
        /// <summary>
        /// Queue for determining which thread may send a purchase request next in Sync Mode
        /// </summary>
        private Queue<int> syncModeQueue;
        #endregion

        #region properties
        /// <summary>
        /// The timestamp of the start of the last purchase event
        /// </summary>
        public long LastThreadClick { get; set; }
        /// <summary>
        /// The ID of the thread which last started a purchase event
        /// </summary>
        public int LastThreadId { get; private set; }
        /// <summary>
        /// The number of currently running threads
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
            this.syncModeQueue = new Queue<int>();
        }
        #endregion

        #region methods
        /// <summary>
        /// This should be called when a new clicker thread is added, to give it an initial spot in the queue.
        /// </summary>
        /// <param name="threadId">The ID of the newly created thread</param>
        public void AddThreadToQueue(int threadId)
        {
            syncModeQueue.Enqueue(threadId);
        }

        /// <summary>
        /// This removes a specific thread from the queue. This is a relatively costly operation due to queue design,
        /// but it is only performed when a thread is permanently closed, which should not occur often.
        /// </summary>
        /// <param name="threadId">The ID of the thread to remove</param>
        public void RemoveThreadFromQueue(int threadId)
        {
            //create a new queue of the same order, but where the specific thread is expressely removed using LINQ expressions.
            syncModeQueue = new Queue<int>(syncModeQueue.Where(x => x != threadId));
        }

        /// <summary>
        /// Returns whether or not the given threadId is next in line to run a purchase request
        /// </summary>
        /// <param name="threadId">The ID of the thread</param>
        /// <returns>True if the thread is at the front of the queue, false otherwise</returns>
        public bool IsNextThread(int threadId)
        {
            return syncModeQueue.Peek() == threadId;
        }

        /// <summary>
        /// Returns the time since the last purchase request was started
        /// </summary>
        /// <returns>The time in milliseconds since the last purchase request</returns>
        public long TimeSinceLastRequest()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - this.LastThreadClick;
        }

        /// <summary>
        /// Places the current front ID of the queue in the back
        /// </summary>
        public void UpdateQueue()
        {
            var lastThreadId = syncModeQueue.Dequeue();
            this.LastThreadId = LastThreadId;
            syncModeQueue.Enqueue(lastThreadId);
        }

        /// <summary>
        /// Returns the number of threads currently in the queue
        /// </summary>
        /// <returns>The number of active threads</returns>
        public int GetNThreads()
        {
            return syncModeQueue.Count;
        }
    #endregion
}
}

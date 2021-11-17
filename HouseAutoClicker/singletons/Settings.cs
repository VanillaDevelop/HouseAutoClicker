using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HouseAutoClicker
{
    /// <summary>
    /// Singleton class which stores application wide settings that may change during runtime.
    /// </summary>
    public sealed class Settings
    {
        #region fields
        /// <summary>
        /// The current singleton isntance
        /// </summary>
        private static Settings instance = null;
        #endregion

        #region properties
        /// <summary>
        /// The current singleton instance
        /// </summary>
        public static Settings Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new Settings();
                }
                return instance;
            }
        }

        /// <summary>
        /// Whether or not the current purchase attempt is made for a personal house or an FC house.
        /// </summary>
        public bool PurchaseSelfHouse { get; set; }
        /// <summary>
        /// Whether or not to add a random delay after 
        /// </summary>
        public bool RandomDelay { get; set; }
        /// <summary>
        /// Whether or not threads will alternate and space out making requests, or just go at will.
        /// </summary>
        public bool SyncMode { get; set; }
        #endregion

        #region ctor
        private Settings()
        {
            //initialize default settings
            PurchaseSelfHouse = true;
            RandomDelay = true;
            SyncMode = false;
        }
        #endregion
    }
}

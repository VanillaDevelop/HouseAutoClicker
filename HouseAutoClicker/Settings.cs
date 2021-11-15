using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HouseAutoClicker
{
    /// <summary>
    /// Singleton class which stores settings
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
        /// Key for confirming or selecting a prompt option in FFXIV
        /// </summary>
        public Keys ConfirmKey { get; set; }
        /// <summary>
        /// Key for moving an option to the right in FFXIV
        /// </summary>
        public Keys RightKey { get; set; }
        /// <summary>
        /// Key for moving an option down in FFXIV
        /// </summary>
        public Keys DownKey { get; set; }
        /// <summary>
        /// Boolean whether or not the house is a self purchase or an FC purchase.
        /// </summary>
        public bool PurchaseSelfHouse { get; set; }
        #endregion

        #region ctor
        private Settings()
        {
            //initialize default settings
            ConfirmKey = Keys.NumPad0;
            RightKey = Keys.NumPad6;
            DownKey = Keys.NumPad2;
            PurchaseSelfHouse = true;
        }
        #endregion
    }
}

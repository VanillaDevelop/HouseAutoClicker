using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HouseAutoClicker.models
{
    /// <summary>
    /// Class which models one key press and the waiting duration after it.
    /// </summary>
    class KeyPress
    {
        #region properties
        /// <summary>
        /// The key being pressed
        /// </summary>
        public Keys Key { get; private set; }
        /// <summary>
        /// The delay in milliseconds after the key press is finished, before another key press can take place.
        /// </summary>
        public int DelayAfterKeyPress { get; private set; }
        #endregion

        #region ctor
        /// <summary>
        /// Instantiates a KeyPress object.
        /// </summary>
        /// <param name="key">The key that should be pressed.</param>
        /// <param name="delayMs">The delay in milliseconds after the key press.</param>
        public KeyPress(Keys key, int delayMs)
        {
            this.Key = key;
            this.DelayAfterKeyPress = delayMs;
        }
        #endregion
    }
}

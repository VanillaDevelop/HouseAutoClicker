using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseAutoClicker.models
{    
    /// <summary>
     /// Class which models a sequence of key presses. (e.g. an attempt to purchase a house)
     /// </summary>
    class KeyPressSequence
    {
        #region properties
        /// <summary>
        /// The sequence of key presses in the key press sequence
        /// </summary>
        public KeyPress[] Sequence { get; private set; }
        /// <summary>
        /// The time it takes for the whole sequence to execute.
        /// </summary>
        public int RTT { get; private set; }
        /// <summary>
        /// A final delay in ms to add after finishing the sequence. Does not count against the RTT.
        /// </summary>
        public int FinalDelay { get; private set; }
        #endregion

        #region ctor
        public KeyPressSequence(KeyPress[] KeyPresses, int FinalDelay = 0)
        {
            this.Sequence = KeyPresses;
            this.RTT = 0;
            foreach (KeyPress key in KeyPresses)
            {
                this.RTT += key.DelayAfterKeyPress;
            }
            this.FinalDelay = FinalDelay;
        }
        #endregion
    }
}

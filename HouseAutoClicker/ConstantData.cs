using HouseAutoClicker.models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HouseAutoClicker
{
    /// <summary>
    /// Utility class for holding constant settings.
    /// </summary>
    class ConstantData
    {
        #region properties
        /// <summary>
        /// Key mapping for "confirm dialogue"
        /// </summary>
        public const Keys ConfirmKey = Keys.NumPad0;
        /// <summary>
        /// Key mapping for "move dialogue option right"
        /// </summary>
        public const Keys RightKey = Keys.NumPad6;
        /// <summary>
        /// Key mapping for "move dialogue option down"
        /// </summary>
        public const Keys DownKey = Keys.NumPad2;
        /// <summary>
        /// The maximum amount of ms to add as a random delay after execution of one loop.
        /// </summary>
        public const int RandomDelayMax = 150;

        /// <summary>
        /// The key press sequence required to open a purchase window.
        /// If another control sequence has been used before (i.e. a KB/M), <see cref="ConstantData.OpenInitialPurchaseWindowSequence"/> should be used instead.
        /// </summary>
        public static readonly KeyPressSequence OpenPurchaseWindowSequence = new KeyPressSequence(new KeyPress[]
            {
                new KeyPress(ConstantData.ConfirmKey, 50),
                new KeyPress(ConstantData.ConfirmKey, 900)
            });

        /// <summary>
        /// The key press sequence required to open a purchase window, provided other control methods were used previously.
        /// If this is not the case, use <see cref="ConstantData.OpenPurchaseWindowSequence"/> instead.
        /// </summary>
        public static readonly KeyPressSequence OpenInitialPurchaseWindowSequence = new KeyPressSequence(new KeyPress[]
        {
                new KeyPress(ConstantData.ConfirmKey, 50),
                new KeyPress(ConstantData.ConfirmKey, 900),
                new KeyPress(ConstantData.ConfirmKey, 50)
        });

        /// <summary>
        /// The key press sequence required to purchase a personal house when the placard is opened and the initial option selected.
        /// This should be called after <see cref="ConstantData.OpenInitialPurchaseWindowSequence"/> or <see cref="ConstantData.OpenPurchaseWindowSequence"/>
        /// </summary>
        public static readonly KeyPressSequence PurchaseSelfHouseSequence = new KeyPressSequence(new KeyPress[]
        {
            new KeyPress(ConstantData.ConfirmKey, 100),
            new KeyPress(ConstantData.ConfirmKey, 200),
            new KeyPress(ConstantData.RightKey, 50),
            new KeyPress(ConstantData.ConfirmKey, 0)
        }, 400);

        /// <summary>
        /// The key press sequence required to purchase a free company house when the placard is opened and the initial option selected.
        /// This should be called after <see cref="ConstantData.OpenInitialPurchaseWindowSequence"/> or <see cref="ConstantData.OpenPurchaseWindowSequence"/>
        /// </summary>
        public static readonly KeyPressSequence PurchaseFCHouseSequence = new KeyPressSequence(new KeyPress[]
        {
            new KeyPress(ConstantData.ConfirmKey, 100),
            new KeyPress(ConstantData.DownKey, 50),
            new KeyPress(ConstantData.ConfirmKey, 200),
            new KeyPress(ConstantData.RightKey, 50),
            new KeyPress(ConstantData.ConfirmKey, 0)
        }, 400);
        #endregion
    }
}

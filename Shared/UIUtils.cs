//-----------------------------------------------------------------------
// <copyright file="UIUtils.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------
namespace Mcaddy
{
    using System;
    using System.IO;
    using System.Windows.Controls;
    
    /// <summary>
    /// Shared utilities for User Interfaces
    /// </summary>
    public class UIUtils
    {
        /// <summary>
        /// Size Suffixes Array
        /// </summary>
        private static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        /// <summary>
        /// Gets the optimum Size suffix for a storage size
        /// </summary>
        /// <param name="value">value in bytes</param>
        /// <param name="decimalPlaces">number of decimal places to return</param>
        /// <returns>a display friendly size</returns>
        public static string SizeSuffix(long value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0)
            {
                throw new ArgumentOutOfRangeException("decimalPlaces");
            }

            if (value < 0)
            {
                return "-" + SizeSuffix(-value);
            }

            if (value == 0)
            {
                return string.Format("{0:n" + decimalPlaces + "} bytes", 0);
            }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format(
                "{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }

        /// <summary>
        /// Selects an item from a ComboBox based on it's tag value
        /// </summary>
        /// <param name="comboBox">Combo Box to enumerate</param>
        /// <param name="tag">Tag to select</param>
        /// <returns>true if item found, false otherwise</returns>
        public static bool SelectItemByTag(ComboBox comboBox, string tag)
        {
            comboBox.SelectedIndex = -1;
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.Tag.ToString().ToUpper().Equals(tag.ToUpper()))
                {
                    item.IsSelected = true;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Bind the Drive List
        /// </summary>
        /// <param name="comboBox">Combo Box to populate</param>
        /// <returns>Number of items in the Drive list</returns>
        public static bool BindDriveList(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            comboBox.Text = string.Empty;

            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                if (d.DriveType.Equals(DriveType.Removable) && d.IsReady)
                {
                    ComboBoxItem item = new ComboBoxItem()
                    {
                        Content = $"{d.Name} - {d.VolumeLabel} ({SizeSuffix(d.TotalSize, 1)})",
                        Tag = d.Name
                    };
                    comboBox.Items.Add(item);
                }
            }

            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }

            return comboBox.Items.Count > 0;
        }
    }
}

//-----------------------------------------------------------------------
// <copyright file="CameraSettings.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------
namespace PocketGpsWorld
{
    /// <summary>
    /// Camera settings to be used for the export
    /// </summary>
    public class CameraSettings
    {
        /// <summary>
        /// Gets or sets the PocketGPSWorld Password
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the PocketGPSWorld Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the Target drive
        /// </summary>
        public string TargertDrive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include Unverified cameras
        /// </summary>
        public bool IncludeUnverified { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include Static cameras in the export
        /// </summary>
        public bool IncludeStatic { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include Mobile cameras in the export
        /// </summary>
        public bool IncludeMobile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include Specs cameras in the export
        /// </summary>
        public bool IncludeSpecs { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include Red light cameras in the export
        /// </summary>
        public bool IncludeRedLight { get; set; }
    }
}

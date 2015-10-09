//-----------------------------------------------------------------------
// <copyright file="CameraType.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------
namespace PocketGpsWorld
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Camera type enumeration
    /// </summary>
    [Flags]
    public enum CameraType
    {
        /// <summary>
        /// No Camera Type
        /// </summary>
        None = 0,

        /// <summary>
        /// Gatso Camera type 
        /// </summary>
        [Description("Gatso")]
        GATSO = 1,

        /// <summary>
        /// Truvelo Camera type
        /// </summary>
        [Description("Truvelo")]
        TRUVELO = 2,

        /// <summary>
        /// Monitron Camera type
        /// </summary>
        [Description("Monitron")]
        MONITRON = 4,

        /// <summary>
        /// Red light Camera type
        /// </summary>
        [Description("Redlight")]
        REDLIGHT = 8,

        /// <summary>
        /// Red speed Camera type
        /// </summary>
        [Description("Redspeed")]
        REDSPEED = 16,

        /// <summary>
        /// Specs Camera Type
        /// </summary>
        [Description("Specs")]
        SPECS = 32,

        /// <summary>
        /// Mobile Camera Type
        /// </summary>
        [Description("Mobile")]
        MOBILE = 64
    }
}

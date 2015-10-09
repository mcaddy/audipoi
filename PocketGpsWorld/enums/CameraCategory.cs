//-----------------------------------------------------------------------
// <copyright file="CameraCategory.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------
namespace PocketGpsWorld
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Camera Category enumeration
    /// </summary>
    [Flags]
    public enum CameraCategory
    {
        /// <summary>
        /// No Category
        /// </summary>
        None = 0,

        /// <summary>
        /// Fixed Camera Category
        /// </summary>
        [Description("Cameras - Fixed")]
        Fixed = 1,

        /// <summary>
        /// Mobile Camera Category
        /// </summary>
        [Description("Cameras - Mobile")]
        Mobile = 2,

        /// <summary>
        /// Specs Camera Category
        /// </summary>
        [Description("Cameras - Specs")]
        Specs = 4,

        /// <summary>
        /// Red light Camera Category
        /// </summary>
        [Description("Cameras - Redlight")]
        RedLight = 8,

        /// <summary>
        /// Provisional Mobile Category
        /// </summary>
        [Description("Cameras - pMobile")]
        PMobile = 16
    }
}

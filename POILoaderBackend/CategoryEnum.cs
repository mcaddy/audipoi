//-----------------------------------------------------------------------
// <copyright file="CategoryEnum.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------
namespace POILoaderBackend
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Category Enumeration
    /// </summary>
    [Flags]
    public enum CategoryEnum
    {
        /// <summary>
        /// None Enum
        /// </summary>
        [Description("None")]
        None = 0,

        /// <summary>
        /// National Trust
        /// </summary>
        [Description("National Trust")]
        NationalTrust = 1,
        
        /// <summary>
        /// English Heritage
        /// </summary>
        [Description("English Heritage")]
        EnglishHeritage = 2,
        
        /// <summary>
        /// RSPB Reserves
        /// </summary>
        [Description("RSPB Reserves")]
        RSPBReserves = 4,
        
        /// <summary>
        /// Historic Houses
        /// </summary>
        [Description("Historic Houses")]
        HistoricHouses = 8,
        
        /// <summary>
        /// Historic Scotland
        /// </summary>
        [Description("Historic Scotland")]
        HistoricScotland = 16,
        
        /// <summary>
        /// National Trust for Scotland
        /// </summary>
        [Description("National Trust for Scotland")]
        NationalTrustScotland = 32
    }
}

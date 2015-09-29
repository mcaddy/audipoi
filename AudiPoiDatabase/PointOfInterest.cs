//-----------------------------------------------------------------------
// <copyright file="PointOfInterest.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------
namespace Mcaddy.AudiPoiDatabase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Point of Interest Class
    /// </summary>
    public class PointOfInterest
    {
        /// <summary>
        /// Gets or sets the Latitude of the Poi
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the Longitude of the Poi
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the Name of the Poi
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the House Number of the Poi
        /// </summary>
        public string HouseNumber { get; set; }

        /// <summary>
        /// Gets or sets the Street of the Poi
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Gets or sets the City of the Poi
        /// </summary>
        public string City { get; set; }
    }
}

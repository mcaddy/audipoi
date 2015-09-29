//-----------------------------------------------------------------------
// <copyright file="PointOfInterestCategory.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------
namespace Mcaddy.AudiPoiDatabase
{ 
    using System.Collections.ObjectModel;
    using System.Drawing;

    /// <summary>
    /// Poi Category
    /// </summary>
    public class PointOfInterestCategory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PointOfInterestCategory"/> class.
        /// </summary>
        /// <param name="id">If for new POI Category</param>
        /// <param name="name">name for new POI Category</param>
        /// <param name="icon">Icon for new POI Category</param>
        public PointOfInterestCategory(int id, string name, Bitmap icon)
        {
            this.Id = id;
            this.Name = name;
            this.Icon = icon;
            this.Items = new Collection<PointOfInterest>();
        }

        /// <summary>
        /// Gets or sets the Category Id
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the Category Name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the Category Icon
        /// </summary>
        public Bitmap Icon { get; set; }

        /// <summary>
        /// Gets the Items for the Point of Interest category
        /// </summary>
        public Collection<PointOfInterest> Items { get; private set; }
    }
}

//-----------------------------------------------------------------------
// <copyright file="GPX.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------
namespace Geocaching
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using Geocaching.Properties;
    using Mcaddy.AudiPoiDatabase;

    /// <summary>
    /// Handle GPX files
    /// </summary>
    public static class GPX
    {
        /// <summary>
        /// Process a given GPX file extracting the Points of Interest
        /// </summary>
        /// <param name="sourceFile">Source File</param>
        /// <param name="skipOwner">skip caches owned by</param>
        /// <param name="skipFound">skip found caches</param>
        /// <returns>A Point of Interest Database</returns>
        /// <exception cref="InvalidDataException">Thrown if unable to select the relevant elements from the GPX file</exception>
        public static Collection<PointOfInterestCategory> ProcessGpxFile(string sourceFile, string skipOwner, bool skipFound)
        {
            Collection<PointOfInterestCategory> pois = new Collection<PointOfInterestCategory>();

            //// other types to consider [Event Cache,Cache In Trash Out Event,Mega-Event Cache]

            // Category lookup
            Dictionary<string, PointOfInterestCategory> categories = new Dictionary<string, PointOfInterestCategory>()
                {
                    { "Earthcache", new PointOfInterestCategory(0, "Caches - Earthcaches", Resources.earthcacheIcon) },
                    { "Letterbox Hybrid", new PointOfInterestCategory(1, "Caches - Letterbox", Resources.letterboxIcon) },
                    { "Multi-cache", new PointOfInterestCategory(2, "Caches - Multi", Resources.multiIcon) },
                    { "Traditional Cache", new PointOfInterestCategory(3, "Caches - Traditional", Resources.traditionalIcon) },
                    { "Unknown Cache", new PointOfInterestCategory(4, "Caches - Mystery", Resources.mysteryIcon) },
                    { "Virtual Cache", new PointOfInterestCategory(5, "Caches - Virtual", Resources.virtualIcon) },
                    { "Wherigo Cache", new PointOfInterestCategory(6, "Caches - Wherigo", Resources.wherigoIcon) },
                    { "Webcam Cache", new PointOfInterestCategory(7, "Caches - Webcam", Resources.webcamIcon) }
                };

            // Load GPX file
            XmlDocument gpxFile = new XmlDocument();
            gpxFile.Load(sourceFile);
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(gpxFile.NameTable);
            namespaceManager.AddNamespace("gpx", "http://www.topografix.com/GPX/1/0");
            namespaceManager.AddNamespace("groundspeak", "http://www.groundspeak.com/cache/1/0/1");
            XmlNodeList waypoints = gpxFile.SelectNodes(@"/gpx:gpx/gpx:wpt", namespaceManager);

            foreach (XmlNode waypoint in waypoints)
            {
                string type = waypoint.SelectSingleNode("gpx:type", namespaceManager).InnerText;
                List<string> attributes = new List<string>(type.Split(new char[] { '|' }));

                // We don't want include child waypoints
                if (attributes.Remove("Geocache"))
                {
                    // Skip caches that have been found 
                    if (skipFound && attributes.Contains("Found"))
                    {
                        continue;
                    }
                    else
                    {
                        attributes.Remove("Found");
                    }

                    PointOfInterestCategory waypointCategory = null;
                    if (attributes.Count.Equals(1) && categories.TryGetValue(attributes[0], out waypointCategory))
                    {
                        string code = waypoint.SelectSingleNode("gpx:name", namespaceManager).InnerText;
                        string name = waypoint.SelectSingleNode("groundspeak:cache/groundspeak:name", namespaceManager).InnerText;
                        string owner = waypoint.SelectSingleNode("groundspeak:cache/groundspeak:owner", namespaceManager).InnerText;

                        // Skip caches with the named owner
                        if (owner.Equals(skipOwner))
                        {
                            continue;
                        }

                        string size = waypoint.SelectSingleNode("groundspeak:cache/groundspeak:container", namespaceManager).InnerText;

                        double lat, lon = 0;
                        double difficulty, terrain = 0;

                        if (!double.TryParse(waypoint.SelectSingleNode("@lat", namespaceManager).InnerText, out lat))
                        {
                            throw new InvalidDataException(waypoint.SelectSingleNode("@lat", namespaceManager).InnerText);
                        }

                        if (!double.TryParse(waypoint.SelectSingleNode("@lon", namespaceManager).InnerText, out lon))
                        {
                            throw new InvalidDataException(waypoint.SelectSingleNode("@lon", namespaceManager).InnerText);
                        }

                        if (!double.TryParse(waypoint.SelectSingleNode("groundspeak:cache/groundspeak:difficulty", namespaceManager).InnerText, out difficulty))
                        {
                            throw new InvalidDataException(waypoint.SelectSingleNode("groundspeak:cache/groundspeak:difficulty", namespaceManager).InnerText);
                        }

                        if (!double.TryParse(waypoint.SelectSingleNode("groundspeak:cache/groundspeak:terrain", namespaceManager).InnerText, out terrain))
                        {
                            throw new InvalidDataException(waypoint.SelectSingleNode("groundspeak:cache/groundspeak:terrain", namespaceManager).InnerText);
                        }

                        PointOfInterestCategory currentCategory = pois.Where(x => x.Id == waypointCategory.Id).SingleOrDefault();

                        if (currentCategory == null)
                        {
                            currentCategory = waypointCategory;
                            pois.Add(currentCategory);
                        }

                        currentCategory.Items.Add(new PointOfInterest()
                        {
                            Name = name,
                            Latitude = lat,
                            Longitude = lon,
                            HouseNumber = string.Format("By {0}", owner),
                            Street = string.Format("{0} [{1}/{2}]", code, difficulty, terrain),
                            City = size,
                        });
                    }
                }
            }

            return pois;
        }
    }
}

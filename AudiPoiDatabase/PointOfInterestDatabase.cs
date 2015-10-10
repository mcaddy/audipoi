//-----------------------------------------------------------------------
// <copyright file="PointOfInterestDatabase.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------
namespace Mcaddy.AudiPoiDatabase
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SQLite;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Xml.Linq;
    using Mcaddy.AudiPoiDatabase.Properties;

    /// <summary>
    /// Database Class
    /// </summary>
    public sealed class PointOfInterestDatabase
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="PointOfInterestDatabase"/> class from being created
        /// </summary>
        private PointOfInterestDatabase()
        {
        }

        /// <summary>
        /// Load POIs from the referenced drive
        /// </summary>
        /// <param name="sourceDrive">The source to load from</param>
        /// <returns>A collection of POI categories</returns>
        public static Collection<PointOfInterestCategory> LoadPois(string sourceDrive)
        {
            Collection<PointOfInterestCategory> currentPois = new Collection<PointOfInterestCategory>();
            if (PointOfInterestDatabase.Exists(sourceDrive))
            {
                currentPois = PointOfInterestDatabase.GetCategories(sourceDrive);

                foreach (PointOfInterestCategory currentCategory in currentPois)
                {
                    currentCategory.Items.AddRange(
                    PointOfInterestDatabase.GetPointsOfInterest(sourceDrive, currentCategory));
                }
            }

            GC.Collect();

            return currentPois;
        }

        /// <summary>
        /// Save the referenced POIs to the target drive
        /// </summary>
        /// <param name="pointsOfInterest">The POIs to save</param>
        /// <param name="targetDrive">The Drive to Save the POIs to</param>
        /// <param name="backgroundWorker">instance of a background worker to update, null if not required</param>
        /// <returns>The number of POIs saved to the Drive</returns>
        public static int SavePois(Collection<PointOfInterestCategory> pointsOfInterest, string targetDrive, BackgroundWorker backgroundWorker)
        {
            int loadedWaypoints = 0;
            PointOfInterestDatabase.BuildStaticContent(targetDrive, pointsOfInterest);

            string databaseLocation = PointOfInterestDatabase.BuildEmptyDatabase(targetDrive);

            loadedWaypoints = PointOfInterestDatabase.Populate(databaseLocation, pointsOfInterest, backgroundWorker);

            PointOfInterestDatabase.CompleteDatabase(targetDrive);

            return loadedWaypoints;
        }

        /// <summary>
        /// Merge Two lists of POI Categories
        /// </summary>
        /// <param name="currentPois">Current POI Categories</param>
        /// <param name="newPois">New Categories</param>
        /// <returns>A Merged list of Categories</returns>
        public static Collection<PointOfInterestCategory> MergePointsOfInterest(Collection<PointOfInterestCategory> currentPois, Collection<PointOfInterestCategory> newPois)
        {
            Collection<PointOfInterestCategory> output = new Collection<PointOfInterestCategory>();

            // Add the existing POIs that arn't in the New file
            foreach (PointOfInterestCategory currentPoiCategory in currentPois)
            {
                // Search for an existing category matching the GPX category, 
                if (newPois.Where(x => x.Name.Equals(currentPoiCategory.Name)).SingleOrDefault() == null)
                {
                    output.Add(currentPoiCategory);
                }
            }

            // Add the New categories
            foreach (PointOfInterestCategory newPoiCategory in newPois)
            {
                output.Add(newPoiCategory);
            }

            // Renumber
            int counter = 1;
            foreach (PointOfInterestCategory outputCategory in output)
            {
                outputCategory.Id = counter;
                counter++;
            }

            return output;
        }

        /// <summary>
        /// Populate the database from a Geocache GPX file
        /// </summary>
        /// <param name="databaseLocation">The database to update</param>
        /// <param name="pointsOfInterest">Points of interest to load</param>
        /// <param name="backgroundWorker">instance of a background worker to update, null if not required</param>
        /// <returns>then number of caches loaded</returns>
        public static int Populate(string databaseLocation,  Collection<PointOfInterestCategory> pointsOfInterest, BackgroundWorker backgroundWorker)
        {
            if (pointsOfInterest == null)
            {
                throw new ArgumentNullException("pointsOfInterest");
            }

            int waypointCount = 0;
            using (SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", databaseLocation)))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    int waypointTotal = 0;
                    foreach (PointOfInterestCategory category in pointsOfInterest)
                    {
                        waypointTotal += category.Items.Count;
                    }

                    foreach (PointOfInterestCategory category in pointsOfInterest)
                    {
                        foreach (PointOfInterest poi in category.Items)
                        {
                            AddPoiToDatabase(connection, poi, category);

                            waypointCount++;

                            if (backgroundWorker != null)
                            {
                                backgroundWorker.ReportProgress((int)Math.Floor(((double)waypointCount / (double)waypointTotal) * 100), string.Format("Building Database - Loaded {0} waypoints", waypointCount));
                            }
                        }
                    }

                    transaction.Commit();
                }
            }

            return waypointCount;
        }

        /// <summary>
        /// Build the Static content for the POIs
        /// </summary>
        /// <param name="rootPath">Root path to which the data should be written</param>
        /// <param name="categories">Categories to include</param>
        public static void BuildStaticContent(string rootPath, IEnumerable<PointOfInterestCategory> categories)
        {
            if (categories == null)
            {
                throw new ArgumentNullException("categories");
            }

            // Build the Update.txt file with todays date for the version
            string updateFileLocation = string.Format(Resources.UpdateTxtFilePath, rootPath);
            Directory.CreateDirectory(Path.GetDirectoryName(updateFileLocation));
            File.WriteAllText(updateFileLocation, string.Format(Resources.UpdateTxtTemplate, DateTime.Now));

            // Ensure the Data file directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(string.Format(Resources.DataFilePath, rootPath, string.Empty)));

            // Build PPOIversion.txt
            File.WriteAllText(string.Format(Resources.DataFilePath, rootPath, "PPOIversion.txt"), string.Format("version.ota={0:yyyyMMdd}-{0:HHmmss}", DateTime.Now));

            BuildVersionsXml(rootPath);
            BuildLangMapXml(rootPath);
            BuildStringsFile(rootPath, categories, "en-GB");
            BuildStringsFile(rootPath, categories, "de-DE");
            BuildBitmapsXml(rootPath, categories);
            BuildCategoriesPc(rootPath, categories);
        }

        /// <summary>
        /// Complete the POI export, setting the relevant hashes and info
        /// </summary>
        /// <param name="rootPath">Target Path</param>
        public static void CompleteDatabase(string rootPath)
        {
            BuildHashesTxt(rootPath);
            BuildMetainfo2Txt(rootPath);
        }

        /// <summary>
        /// Gets the categories from an existing database
        /// </summary>
        /// <param name="rootPath">Target Path</param>
        /// <returns>A collection of POI Categories</returns>
        public static Collection<PointOfInterestCategory> GetCategories(string rootPath)
        {
            Collection<PointOfInterestCategory> categories = new Collection<PointOfInterestCategory>();

            XDocument categoriesPcDoc = XDocument.Load(string.Format(Resources.DataFilePath, rootPath, "categories.pc"));
            
            // TODO get the default lanaguage file
            string defaultLanguageFilename = "strings_en-GB.xml";
            XDocument defaultLanguageDoc = XDocument.Load(string.Format(Resources.DataFilePath, rootPath, defaultLanguageFilename));

            Dictionary<int, string> categoryTitles = new Dictionary<int, string>();
            IEnumerable<XElement> strings = defaultLanguageDoc.Root.Elements("string");
            foreach (XElement stringElement in strings)
            {
                categoryTitles.Add(int.Parse(stringElement.Attribute("id").Value), stringElement.Element("text").Value);
            }

            XElement categoriesNode = categoriesPcDoc.Root.Element("categories");
            if (categoriesNode != null)
            {
                foreach (XElement category in categoriesNode.Descendants("category"))
                {
                    int id = 0;
                    if (!int.TryParse(category.Attribute("id").Value, out id))
                    {
                        throw new InvalidCastException("id");
                    }

                    categories.Add(new PointOfInterestCategory(id, categoryTitles[id], GetImage(category.Element("bitmap"), rootPath)));
                }
            }

            return categories;
        }

        /// <summary>
        /// Gets the Points of Interest for a given Category
        /// </summary>
        /// <param name="rootPath">Target Path</param>
        /// <param name="category">Category to search</param>
        /// <returns>A collection of Points of interest</returns>
        public static Collection<PointOfInterest> GetPointsOfInterest(string rootPath, PointOfInterestCategory category)
        {
            if (category == null)
            {
                throw new ArgumentNullException("category");
            }

            Collection<PointOfInterest> pointsOfInterest = new Collection<PointOfInterest>();

            string databaseLocation = string.Format(Resources.DataFilePath, rootPath, "poidata.db");
            using (SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", databaseLocation)))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(
                    "SELECT latmin + (latmax - latmin / 2) as lat, lonmin + (lonmax - lonmin / 2) as lon, city, street, housenr, name FROM poicoord JOIN poidata ON poicoord.poiid = poidata.poiid JOIN poiname on poiname.docid = poicoord.poiid WHERE type = @category",
                    connection))
                {
                    command.Parameters.Add(new SQLiteParameter("category", DbType.Int32) { Value = category.Id });

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        int latOrdinal = reader.GetOrdinal("lat");
                        int lonOrdinal = reader.GetOrdinal("lon");
                        int nameOrdinal = reader.GetOrdinal("name");
                        int houseNrOrdinal = reader.GetOrdinal("housenr");
                        int streetOrdinal = reader.GetOrdinal("street");
                        int cityOrdinal = reader.GetOrdinal("city");

                        while (reader.Read())
                        {
                            pointsOfInterest.Add(new PointOfInterest()
                            {
                                Latitude = reader.GetDouble(latOrdinal),
                                Longitude = reader.GetDouble(lonOrdinal),
                                Name = reader.GetString(nameOrdinal),
                                HouseNumber = reader.IsDBNull(houseNrOrdinal) ? string.Empty : reader.GetString(houseNrOrdinal),
                                Street = reader.IsDBNull(streetOrdinal) ? string.Empty : reader.GetString(streetOrdinal),
                                City = reader.IsDBNull(cityOrdinal) ? string.Empty : reader.GetString(cityOrdinal),
                            });
                        }
                    }
                }
            }

            return pointsOfInterest;
        }

        /// <summary>
        /// Build a blank POI database
        /// </summary>
        /// <param name="rootPath">Target path</param>
        /// <returns>the open connection</returns>
        public static string BuildEmptyDatabase(string rootPath)
        {
            string databaseLocation = string.Format(Resources.DataFilePath, rootPath, "poidata.db");

            Directory.CreateDirectory(Path.GetDirectoryName(databaseLocation));

            if (File.Exists(databaseLocation))
            {
                File.Delete(databaseLocation);
            }
            
            SQLiteConnection.CreateFile(databaseLocation);
            using (SQLiteConnection databaseConnection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", databaseLocation)))
            {
                databaseConnection.Open();
                using (SQLiteCommand command = new SQLiteCommand("CREATE VIRTUAL TABLE poicoord USING rtree (poiid INTEGER,latmin REAL,latmax REAL,lonmin REAL,lonmax REAL)", databaseConnection))
                {
                    command.ExecuteNonQuery();
                    command.CommandText = "CREATE TABLE poidata (poiid INTEGER,type INTEGER,namephon TEXT,ccode INTEGER,zipcode TEXT,city TEXT,street TEXT,housenr TEXT,phone TEXT,ntlimportance INTEGER,exttype TEXT,extcont TEXT,warning TEXT,warnphon TEXT,CONSTRAINT PK_poidata PRIMARY KEY (poiid))";
                    command.ExecuteNonQuery();
                    command.CommandText = "CREATE VIRTUAL TABLE poiname USING fts3 (name TEXT)";
                    command.ExecuteNonQuery();
                }
            }

            return databaseLocation;
        }

        /// <summary>
        /// Check to see if expected files exist in path
        /// </summary>
        /// <param name="rootPath">target path</param>
        /// <returns>true if files exist</returns>
        public static bool Exists(string rootPath)
        {
            return File.Exists(string.Format(@"{0}\metainfo2.txt", rootPath)) &&
                File.Exists(string.Format(Resources.DataFilePath, rootPath, "categories.pc")) &&
                File.Exists(string.Format(Resources.DataFilePath, rootPath, "poidata.db"));
        }

        /// <summary>
        /// Add a POI to the database
        /// </summary>
        /// <param name="connection">The Database to update</param>
        /// <param name="pointOfInterest">The Point of Interest to add</param>
        /// <param name="category">Cache Category</param>
        private static void AddPoiToDatabase(SQLiteConnection connection, PointOfInterest pointOfInterest, PointOfInterestCategory category)
        {
            using (SQLiteCommand command = new SQLiteCommand("INSERT INTO poiname (name) VALUES (@name)", connection))
            {
                command.Parameters.Add(new SQLiteParameter("name", DbType.String) { Value = pointOfInterest.Name.Replace("'", "''") });
                command.ExecuteNonQuery();
                command.CommandText = "SELECT last_insert_rowid()";
                long poiId = (long)command.ExecuteScalar();

                // The other implimentations seem to adjust the cords from the suppled to a value a little bigger and a little smaller, not sure why but lets do the same to avoid issues
                double offset = 0.000005;
                command.CommandText = "INSERT INTO poicoord (poiid, latmin, latmax, lonmin, lonmax) VALUES (@poiid, @latmin, @latmax, @lonmin, @lonmax)";
                command.Parameters.Add(new SQLiteParameter("poiid", DbType.Int32) { Value = poiId });
                command.Parameters.Add(new SQLiteParameter("latmin", DbType.Double) { Value = pointOfInterest.Latitude - offset });
                command.Parameters.Add(new SQLiteParameter("latmax", DbType.Double) { Value = pointOfInterest.Latitude + offset });
                command.Parameters.Add(new SQLiteParameter("lonmin", DbType.Double) { Value = pointOfInterest.Longitude - offset });
                command.Parameters.Add(new SQLiteParameter("lonmax", DbType.Double) { Value = pointOfInterest.Longitude + offset });
                command.ExecuteNonQuery();

                // Unused fields in the poidata table - ccode, ntlimportance, exttype, extcont, warning, warnphon, namephon, zipcode, phone
                command.CommandText = "INSERT INTO poidata (poiid, type, namephon, zipcode, city, street, housenr, phone) VALUES (@poiid, @category, 'namephon', 'zipcode', @city, @street, @housenr, 'phone')";
                command.Parameters.Add(new SQLiteParameter("poiid", DbType.Int32) { Value = poiId });
                command.Parameters.Add(new SQLiteParameter("category", DbType.Int32) { Value = category.Id });
                command.Parameters.Add(new SQLiteParameter("city", DbType.String) { Value = pointOfInterest.City });
                command.Parameters.Add(new SQLiteParameter("street", DbType.String) { Value = pointOfInterest.Street });
                command.Parameters.Add(new SQLiteParameter("housenr", DbType.String) { Value = pointOfInterest.HouseNumber });
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Extract an Image from the given Element and load from the target system
        /// </summary>
        /// <param name="element">Element to parse</param>
        /// <param name="rootPath">Target Path</param>
        /// <returns>The Image referenced by the Element</returns>
        private static Bitmap GetImage(XElement element, string rootPath)
        {
            string[] imageElements = element.Value.Split(new char[] { ',' });
            using (Bitmap bitmap = new Bitmap(string.Format(Resources.DataFilePath, rootPath, imageElements[0].Replace(@"/", @"\"))))
            {
                return new Bitmap(bitmap);
            }
        }

        /// <summary>
        /// Builds a string element for the languages file
        /// </summary>
        /// <param name="category">POI Category</param>
        /// <param name="language">POI Language</param>
        /// <returns>an XElement with the relevant children</returns>
        private static XElement BuildStringElement(PointOfInterestCategory category, string language)
        {
            return new XElement(
                "string",
                new XAttribute("id", category.Id),
                new XAttribute("type", 0),
                new XElement(
                    "lang",
                    new XAttribute("lang", language)),
                new XElement("text", category.Name));
        }

        /// <summary>
        /// Build the VersionsXml File
        /// </summary>
        /// <param name="rootPath">Target Path</param>
        private static void BuildVersionsXml(string rootPath)
        {
            XDocument versionXml = new XDocument(
                new XDeclaration("1.0", "utf-8", "no"),
                new XElement(
                    "versioninfos",
                    new XElement(
                        "versioninfo",
                        new XElement("component", "sqlite"),
                        new XElement(
                            "version",
                            new XElement("major", "3.6.23"),
                            new XElement("minor"))),
                    new XElement(
                        "versioninfo",
                        new XElement("component", "poidb"),
                        new XElement(
                            "version",
                            new XElement("major", "2"),
                            new XElement("minor", "0"))),
                    new XElement(
                        "versioninfo",
                        new XElement("component", "poiconfig"),
                        new XElement(
                            "version",
                            new XElement("major", "1"),
                            new XElement("minor", "1")))));
            versionXml.Save(string.Format(Resources.DataFilePath, rootPath, "versions.xml"), SaveOptions.DisableFormatting);
        }

        /// <summary>
        /// Build a strings file, accepts language
        /// </summary>
        /// <param name="rootPath">Target Path</param>
        /// <param name="categories">Categories to render</param>
        /// <param name="language">language for the file</param>
        private static void BuildStringsFile(string rootPath, IEnumerable<PointOfInterestCategory> categories, string language)
        {
            XDocument strings_xml = new XDocument(
                new XDeclaration("1.0", "utf-8", "no"),
                new XElement("strings"));

            foreach (PointOfInterestCategory category in categories)
            {
                strings_xml.Root.Add(BuildStringElement(category, language));
            }

            strings_xml.Save(string.Format(Resources.DataFilePath, rootPath, string.Format("strings_{0}.xml", language)), SaveOptions.DisableFormatting);
        }

        /// <summary>
        /// Builds the Lang_Map.Xml
        /// </summary>
        /// <param name="rootPath">Target Path</param>
        private static void BuildLangMapXml(string rootPath)
        {
            XDocument lang_mapXml = new XDocument(
                new XDeclaration("1.0", "utf-8", "no"),
                new XElement(
                    "LanguageCodeToFileMappings",
                    new XElement(
                        "Mapping",
                        new XAttribute("file", "strings_en-GB.xml"),
                        new XAttribute("langCode", "default")),
                    new XElement(
                        "Mapping",
                        new XAttribute("file", "strings_en-GB.xml"),
                        new XAttribute("langCode", "en-GB")),
                    new XElement(
                        "Mapping",
                        new XAttribute("file", "strings_de-DE.xml"),
                        new XAttribute("langCode", "de-DE"))));
            lang_mapXml.Save(string.Format(Resources.DataFilePath, rootPath, "lang_map.xml"), SaveOptions.DisableFormatting);
        }

        /// <summary>
        /// Build the bitmaps.xml file and transfer the icons to the 
        /// </summary>
        /// <param name="rootPath">Target Path</param>
        /// <param name="categories">Categories to render</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "category", Justification = "I have not found a more efficent way to count the number of items in an enum")]
        private static void BuildBitmapsXml(string rootPath, IEnumerable<PointOfInterestCategory> categories)
        {
            string bitmapPath = string.Format(Resources.DataFilePath, rootPath, @"bitmaps\");
            Directory.CreateDirectory(bitmapPath);

            int categoryCount = 0;

            foreach (PointOfInterestCategory category in categories)
            {
                categoryCount++;
            }

            XDocument bitmapsxml = new XDocument(
                new XDeclaration("1.0", "utf-8", "no"),
                new XElement(
                    "bitmaps",
                new XAttribute("count", categoryCount)));

            foreach (PointOfInterestCategory category in categories)
            {
                bitmapsxml.Root.Add(
                    new XElement(
                        "resource",
                        new XAttribute("id", category.Id + 2),
                        new XAttribute("name", string.Format(Resources.BitmapFormat, category.Id + 2))));
                category.Icon.Save(string.Format("{0}image_{1}.png", bitmapPath, category.Id + 2));
            }

            bitmapsxml.Save(string.Format(Resources.DataFilePath, rootPath, "bitmaps.xml"), SaveOptions.DisableFormatting);

            Resources.stacking_2.Save(string.Format("{0}{1}.png", bitmapPath, "stacking_2"));
            Resources.stacking_3.Save(string.Format("{0}{1}.png", bitmapPath, "stacking_3"));
        }

        /// <summary>
        /// Build the Categories.pc file
        /// </summary>
        /// <param name="rootPath">Target Path</param>
        /// <param name="categories">Categories to render</param>
        private static void BuildCategoriesPc(string rootPath, IEnumerable<PointOfInterestCategory> categories)
        {
            XDocument categoriespc = new XDocument(
                new XDeclaration("1.0", "utf-8", "no"),
                new XElement(
                    "poicategories",
                    new XAttribute("version", "02010011")));
            XElement categoriesNode = new XElement("categories");
            categoriespc.Root.Add(categoriesNode);
            XElement typesNode = new XElement("types");
            categoriespc.Root.Add(typesNode);
            XElement searchNode = new XElement(
                "search",
                new XAttribute("type", "Generic"));
            categoriespc.Root.Add(searchNode);

            foreach (PointOfInterestCategory category in categories)
            {
                categoriesNode.Add(
                    new XComment(category.Name),
                    new XElement(
                        "category",
                    new XAttribute("bitmapIndex", category.Id + 2),
                    new XAttribute("id", category.Id),
                    new XAttribute("name", category.Id),
                    new XAttribute("type", 0),
                    new XElement(
                        "bitmap",
                        new XAttribute("res_id", category.Id + 2),
                        string.Format(Resources.BitmapFormat, category.Id + 2))));

                typesNode.Add(new XElement(
                    "type",
                    new XAttribute("id", category.Id),
                    new XElement(
                        "bitmap",
                        new XAttribute("module", 0),
                        new XAttribute("res_id", category.Id + 2),
                        new XAttribute("size", 10),
                        string.Format(Resources.BitmapFormat, category.Id + 2)),
                    new XElement(
                        "bitmap",
                        new XAttribute("module", 1),
                        new XAttribute("res_id", category.Id + 2),
                        new XAttribute("size", 10),
                        string.Format(Resources.BitmapFormat, category.Id + 2)),
                    new XElement(
                        "bitmap",
                        new XAttribute("module", 1),
                        new XAttribute("res_id", category.Id + 2),
                        new XAttribute("size", 20),
                        string.Format(Resources.BitmapFormat, category.Id + 2)),
                    new XElement(
                        "zoomlevel",
                        new XAttribute("max", 48),
                        new XAttribute("min", 0)),
                        new XElement("priority", 1),
                        new XElement("code", category.Id),
                        new XElement("description")));

                searchNode.Add(
                    new XComment(category.Name),
                    new XElement(
                        "category",
                    new XAttribute("id", category.Id),
                    new XAttribute("index", 10),
                    new XElement(
                        "type",
                        new XAttribute("id", category.Id))));
            }

            categoriespc.Save(string.Format(Resources.DataFilePath, rootPath, "categories.pc"), SaveOptions.DisableFormatting);
        }

        /// <summary>
        /// Build the Metainfo2.txt file
        /// </summary>
        /// <param name="rootPath">Path to use as target</param>
        private static void BuildMetainfo2Txt(string rootPath)
        {
            // Build metainfo2.txt
            string updateFileLocation = string.Format(Resources.UpdateTxtFilePath, rootPath);
            using (var cryptoProvider = new SHA1CryptoServiceProvider())
            {
                string hashesFileHash = BitConverter.ToString(cryptoProvider.ComputeHash(File.ReadAllBytes(string.Format(Resources.DataFilePath, rootPath, "hashes.txt")))).ToLower(CultureInfo.CurrentCulture).Replace("-", string.Empty);
                long datafoldersize = DirectoryUtilities.GetDirectorySize(string.Format(Resources.DataFilePath, rootPath, string.Empty));
                string updateTxtFileHash = BitConverter.ToString(cryptoProvider.ComputeHash(File.ReadAllBytes(updateFileLocation))).ToLower(CultureInfo.CurrentCulture).Replace("-", string.Empty);
                long updateTxtFileSize = new FileInfo(updateFileLocation).Length;

                string metainfo2txtFile = string.Format(
                    Resources.metainfo2TxtTemplate,
                    hashesFileHash,
                    datafoldersize,
                    updateTxtFileHash,
                    updateTxtFileSize);

                string metafileHash = BitConverter.ToString(cryptoProvider.ComputeHash(Encoding.ASCII.GetBytes(metainfo2txtFile))).ToLower(CultureInfo.CurrentCulture).Replace("-", string.Empty);

                File.WriteAllText(
                    string.Format(@"{0}\metainfo2.txt", rootPath),
                    metainfo2txtFile.Replace("[common]", string.Format("[common]\r\nMetafileChecksum = \"{0}\"", metafileHash)));
            }
        }

        /// <summary>
        /// Build Hashes.Txt file
        /// </summary>
        /// <param name="rootPath">Target Path</param>
        private static void BuildHashesTxt(string rootPath)
        {
            // Build hashes.txt
            StringBuilder hashesStringBuilder = new StringBuilder();
            string rootfolder = string.Format(Resources.DataFilePath, rootPath, string.Empty);
            string[] datafiles = Directory.GetFiles(rootfolder, "*.*", SearchOption.AllDirectories);
            using (var cryptoProvider = new SHA1CryptoServiceProvider())
            {
                foreach (string datafile in datafiles)
                {
                    long filesize = new FileInfo(datafile).Length;
                    int checksumsize = 524288;

                    hashesStringBuilder.AppendFormat(
                        "FileName = \"{0}\"\nFileSize = \"{1}\"\nCheckSumSize=\"{2}\"\n",
                        datafile.Replace(rootfolder, string.Empty).Replace(@"\", @"/"),
                        filesize,
                        checksumsize);

                    IEnumerable<byte[]> chunks = File.ReadAllBytes(datafile).Split(checksumsize);

                    int checksumcount = 0;
                    foreach (byte[] chunk in chunks)
                    {
                        hashesStringBuilder.AppendFormat("CheckSum{0} = \"{1}\"\n", checksumcount == 0 ? string.Empty : checksumcount.ToString(), BitConverter.ToString(cryptoProvider.ComputeHash(chunk)).ToLower(CultureInfo.CurrentCulture).Replace("-", string.Empty));
                        checksumcount++;
                    }

                    hashesStringBuilder.Append("\n");
                }
            }

            File.WriteAllText(string.Format(Resources.DataFilePath, rootPath, "hashes.txt"), hashesStringBuilder.ToString());
        }
    }
}

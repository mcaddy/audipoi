//-----------------------------------------------------------------------
// <copyright file="Form1.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------

namespace Mcaddy.Audi
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using System.Xml;
    using Mcaddy.GeocacheToPoi.Properties;
    using AudiPoiDatabase;

    /// <summary>
    /// Main Form
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Process a given GPX file extracting the Points of Interest
        /// </summary>
        /// <param name="sourceFile">Source File</param>
        /// <param name="skipOwner">skip caches owned by</param>
        /// <param name="skipFound">skip found caches</param>
        /// <returns>A Point of Interest Database</returns>
        private static Collection<PointOfInterestCategory> ProcessGpxFile(string sourceFile, string skipOwner, bool skipFound)
        {
            Collection<PointOfInterestCategory> pois = new Collection<PointOfInterestCategory>();

            //// other types to consider [Event Cache,Cache In Trash Out Event,Mega-Event Cache]

            // Category lookup
            Dictionary<string, PointOfInterestCategory> categories = new Dictionary<string, PointOfInterestCategory>()
                {
                    { "Earthcache", new PointOfInterestCategory(0, "Earthcaches", Resources.earthcacheIcon) },
                    { "Letterbox Hybrid", new PointOfInterestCategory(1, "Letterbox Caches", Resources.letterboxIcon) },
                    { "Multi-cache", new PointOfInterestCategory(2, "Multi Caches", Resources.multiIcon) },
                    { "Traditional Cache", new PointOfInterestCategory(3, "Traditional Caches", Resources.traditionalIcon) },
                    { "Unknown Cache", new PointOfInterestCategory(4, "Mystery Caches", Resources.mysteryIcon) },
                    { "Virtual Cache", new PointOfInterestCategory(5, "Virtual Caches", Resources.virtualIcon) },
                    { "Wherigo Cache", new PointOfInterestCategory(6, "Wherigo Caches", Resources.wherigoIcon) },
                    { "Webcam Cache", new PointOfInterestCategory(7, "Webcam Caches", Resources.webcamIcon) }
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
                List<string> bits = new List<string>(type.Split(new char[] { '|' }));

                // We don't want include child waypoints
                if (bits.Remove("Geocache"))
                {
                    // Skip caches that have been found 
                    if (skipFound && bits.Contains("Found"))
                    {
                        continue;
                    }
                    else
                    {
                        bits.Remove("Found");
                    }

                    PointOfInterestCategory waypointCategory = null;
                    if (bits.Count.Equals(1) && categories.TryGetValue(bits[0], out waypointCategory))
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

        /// <summary>
        /// Merge GPX Categories with the existing POI Categories
        /// </summary>
        /// <param name="currentPois">Current POI Categories</param>
        /// <param name="gpxPois">GPX Categories</param>
        /// <returns>A Merged list of Categories</returns>
        private static Collection<PointOfInterestCategory> MergePointsOfInterest(Collection<PointOfInterestCategory> currentPois, Collection<PointOfInterestCategory> gpxPois)
        {
            Collection<PointOfInterestCategory> output = new Collection<PointOfInterestCategory>();

            // Add the existing POIs that arn't in the GPX file
            foreach (PointOfInterestCategory currentPoiCategory in currentPois)
            {
                // Search for an existing category matching the GPX category, 
                if (gpxPois.Where(x => x.Name.Equals(currentPoiCategory.Name)).SingleOrDefault() == null)
                {
                    output.Add(currentPoiCategory);
                }
            }

            // Add the GPX categories
            foreach (PointOfInterestCategory gpxPoiCategory in gpxPois)
            {
                output.Add(gpxPoiCategory);
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
        /// Handle the Select Source Click event
        /// </summary>
        /// <param name="sender">Sender Argument</param>
        /// <param name="e">Event Argument</param>
        private void SelectSourceGpxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.openSourceFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.gpxFilenameTextBox.Text = this.openSourceFileDialog.FileName;
            }
        }

        /// <summary>
        /// Select the Target Folder click event handler
        /// </summary>
        /// <param name="sender">Sender Argument</param>
        /// <param name="e">Event Argument</param>
        private void SelectTargetFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.targetFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.targetTextBox.Text = this.targetFolderBrowserDialog.SelectedPath;
            }
        }

        /// <summary>
        /// Exit Click event handler
        /// </summary>
        /// <param name="sender">Sender Argument</param>
        /// <param name="e">Event Argument</param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Process Button Click Event handler
        /// </summary>
        /// <param name="sender">Sender Argument</param>
        /// <param name="e">Event Argument</param>
        private void Button1_Click(object sender, EventArgs e)
        {
            this.buildDatabaseBackgroundWorker.RunWorkerAsync();
            this.processButton.Enabled = false;
        }

        /// <summary>
        /// Do Work event Handler
        /// </summary>
        /// <param name="sender">Sender Argument</param>
        /// <param name="e">Event Argument</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Justification = "Required to ensure the SQLite DB is released")]
        private void BackgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (Directory.Exists(this.targetTextBox.Text) && File.Exists(this.gpxFilenameTextBox.Text))
            {
                // Load existing POIs
                buildDatabaseBackgroundWorker.ReportProgress(1, "Loading existing POIs");
                Collection<PointOfInterestCategory> currentPois = new Collection<PointOfInterestCategory>();
                if (PointOfInterestDatabase.Exists(this.targetTextBox.Text))
                {
                    currentPois = PointOfInterestDatabase.GetCategories(this.targetTextBox.Text);

                    foreach (PointOfInterestCategory currentCategory in currentPois)
                    {
                        currentCategory.Items.AddRange(
                        PointOfInterestDatabase.GetPointsOfInterest(this.targetTextBox.Text, currentCategory));
                    }
                }

                buildDatabaseBackgroundWorker.ReportProgress(2, "Load GPX POIs");
                Collection<PointOfInterestCategory> gpxPois = ProcessGpxFile(this.gpxFilenameTextBox.Text, "mcaddy", true);

                Collection<PointOfInterestCategory> pointsOfInterest = MergePointsOfInterest(currentPois, gpxPois);

                buildDatabaseBackgroundWorker.ReportProgress(3, "Building Database");
                PointOfInterestDatabase.BuildStaticContent(this.targetTextBox.Text, pointsOfInterest);
                int loadedWaypoints = 0;

                string databaseLocation = PointOfInterestDatabase.Build(this.targetTextBox.Text);

                loadedWaypoints = PointOfInterestDatabase.Populate(databaseLocation, pointsOfInterest, this.buildDatabaseBackgroundWorker);

                GC.Collect();

                PointOfInterestDatabase.CompleteDatabase(this.targetTextBox.Text);

                e.Result = loadedWaypoints;
            }
            else
            {
                e.Result = -1;
                MessageBox.Show(
                Resources.FileNotFoundError,
                Resources.ErrorTitle, 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1, 
                MessageBoxOptions.RightAlign);
            }
        }

        /// <summary>
        /// Handle the Progress Changed Event
        /// </summary>
        /// <param name="sender">Sender Argument</param>
        /// <param name="e">Event Argument</param>
        private void BackgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
            this.processButton.Text = e.UserState.ToString();
        }

        /// <summary>
        /// Handle the worker complete event
        /// </summary>
        /// <param name="sender">Sender Argument</param>
        /// <param name="e">Event Argument</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Our messagebox won't be right reading")]
        private void BackgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            this.progressBar.Value = 100;
            if ((int)e.Result >= 0)
            {
                MessageBox.Show(
                    string.Format(Resources.CompletionFormatString, e.Result),
                    Resources.CompletionTitle);
            }

            this.processButton.Text = "Process";
            this.processButton.Enabled = true;
        }

        private void TargetFolderLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
//-----------------------------------------------------------------------
// <copyright file="MainForm.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------

namespace Mcaddy.Audi
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Windows.Forms;
    using AudiPoiDatabase;
    using Geocaching;
    using Mcaddy.GeocacheToPoi.Properties;

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
        /// Bind the Drive list
        /// </summary>
        /// <returns>true if the list contains entries</returns>
        private bool BindDriveList()
        {
            targetDriveComboBox.Text = string.Empty;

            DriveInfo[] allDrives = DriveInfo.GetDrives();

            Dictionary<string, string> filterItems = new Dictionary<string, string>();

            foreach (DriveInfo d in allDrives)
            {
                if (d.DriveType.Equals(DriveType.Removable) && d.IsReady)
                {
                    filterItems.Add(d.Name, $"{d.Name} - {d.VolumeLabel} ({UIUtils.SizeSuffix(d.TotalSize, 1)})");
                }
            }

            this.targetDriveComboBox.DisplayMember = "Value";
            this.targetDriveComboBox.ValueMember = "Key";
            this.targetDriveComboBox.DataSource = new BindingSource(filterItems, null);

            if (targetDriveComboBox.Items.Count > 0)
            {
                targetDriveComboBox.SelectedIndex = 0;
            }

            return targetDriveComboBox.Items.Count > 0;
        }
        
        /// <summary>
        /// Process Button Click Event handler
        /// </summary>
        /// <param name="sender">Sender Argument</param>
        /// <param name="e">Event Argument</param>
        private void ProcessButton_Click(object sender, EventArgs e)
        {
            Tuple<string, string, bool> settings = new Tuple<string, string, bool>(
            this.targetDriveComboBox.SelectedValue.ToString(),
            this.excludeOwnedCachesCheckBox.Checked ? this.geocachingUsernameTextBox.Text : string.Empty,
            this.excludeFoundCachesCheckBox.Checked);

            this.buildDatabaseBackgroundWorker.RunWorkerAsync(settings);
            this.processButton.Enabled = false;
        }

        /// <summary>
        /// Do Work event Handler
        /// </summary>
        /// <param name="sender">Sender Argument</param>
        /// <param name="e">Event Argument</param>
        private void BuildDatabaseBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Tuple<string, string, bool> settings = (Tuple<string, string, bool>)e.Argument;

            if (Directory.Exists(settings.Item1) && File.Exists(this.gpxFilenameTextBox.Text))
            {
                // Load existing POIs
                buildDatabaseBackgroundWorker.ReportProgress(1, "Loading existing POIs");
                Collection<PointOfInterestCategory> currentPois = PointOfInterestDatabase.LoadPois(settings.Item1);

                buildDatabaseBackgroundWorker.ReportProgress(2, "Loading GPX POIs");
                Collection<PointOfInterestCategory> gpxPois = GPX.ProcessGpxFile(this.gpxFilenameTextBox.Text, settings.Item2, settings.Item3);

                buildDatabaseBackgroundWorker.ReportProgress(3, "Merging new POIs");
                Collection<PointOfInterestCategory> pointsOfInterest = PointOfInterestDatabase.MergePointsOfInterest(currentPois, gpxPois);

                buildDatabaseBackgroundWorker.ReportProgress(4, "Building Database");
                int loadedWaypoints = PointOfInterestDatabase.SavePois(pointsOfInterest, settings.Item1, this.buildDatabaseBackgroundWorker);

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
        private void BuildDatabaseBackgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
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
        private void BuildDatabaseBackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            this.progressBar.Value = 100;
            if ((int)e.Result >= 0)
            {
                if (Program.Auto)
                {
                    this.Close();
                }
                else
                {
                    MessageBox.Show(
                    string.Format(Resources.CompletionFormatString, e.Result),
                    Resources.CompletionTitle);
                }
            }

            this.processButton.Text = "Process";
            this.processButton.Enabled = true;
        }

        /// <summary>
        /// Handle the form load event
        /// </summary>
        /// <param name="sender">the Sender argument</param>
        /// <param name="e">the Event Arguments argument</param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!this.BindDriveList())
            {
                this.processButton.Enabled = false;
            }

            this.Text += string.Format(" (v{0})", Application.ProductVersion);
            this.LoadSettings();

            if (Program.Auto)
            {
                if (!string.IsNullOrEmpty(Program.GpxPath))
                {
                    gpxFilenameTextBox.Text = Program.GpxPath;
                }

                targetDriveComboBox.SelectedValue = Program.TargetDrive;

                if (targetDriveComboBox.SelectedValue == null)
                {
                    MessageBox.Show(Resources.InvalidTargetDriveOnCommandLine, Resources.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                this.ProcessButton_Click(this.processButton, new EventArgs());
            }
        }

        /// <summary>
        /// Load the settings
        /// </summary>
        private void LoadSettings()
        {
            geocachingUsernameTextBox.Text = Settings.Default.GeocachingUsername;
            excludeFoundCachesCheckBox.Checked = Settings.Default.ExcludeFound;
            excludeOwnedCachesCheckBox.Checked = Settings.Default.ExcludeOwned;
            gpxFilenameTextBox.Text = Settings.Default.GpxPath;
        }

        /// <summary>
        /// Save the settings
        /// </summary>
        private void SaveSettings()
        {
            Settings.Default.GeocachingUsername = geocachingUsernameTextBox.Text;
            Settings.Default.ExcludeFound = excludeFoundCachesCheckBox.Checked;
            Settings.Default.ExcludeOwned = excludeOwnedCachesCheckBox.Checked;
            Settings.Default.GpxPath = gpxFilenameTextBox.Text;
            Settings.Default.Save();
        }

        /// <summary>
        /// Handle the Select Source Click event
        /// </summary>
        /// <param name="sender">Sender Argument</param>
        /// <param name="e">Event Argument</param>
        private void SelectSourceGPX_Click(object sender, EventArgs e)
        {
            if (this.openSourceFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.gpxFilenameTextBox.Text = this.openSourceFileDialog.FileName;
            }
        }

        /// <summary>
        /// Handle the Form Closing event
        /// </summary>
        /// <param name="sender">Sender Argument</param>
        /// <param name="e">Event Argument</param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.SaveSettings();
        }

        /// <summary>
        /// Handle the Refresh Button Click event
        /// </summary>
        /// <param name="sender">Sender argument</param>
        /// <param name="e">Event Argument</param>
        private void RefreshButton_Click(object sender, EventArgs e)
        {
            if (this.BindDriveList())
            {
                this.processButton.Enabled = true;
            }
            else
            {
                this.processButton.Enabled = false;
            }
        }
    }
}
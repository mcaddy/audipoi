//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------

namespace SpeedCameraToPoi
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Net;
    using System.Windows;
    using System.Windows.Input;
    using Mcaddy.AudiPoiDatabase;
    using PocketGpsWorld;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        /// <summary>
        /// Background worker
        /// </summary>
        private BackgroundWorker buildDatabaseBackgroundWorker;

        /// <summary>
        /// In support of IDisposable Pattern
        /// </summary>
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.buildDatabaseBackgroundWorker = new BackgroundWorker();
            this.buildDatabaseBackgroundWorker.WorkerReportsProgress = true;
            this.buildDatabaseBackgroundWorker.DoWork +=
                new DoWorkEventHandler(this.BackgroundWorker1_DoWork);
            this.buildDatabaseBackgroundWorker.ProgressChanged +=
                new ProgressChangedEventHandler(this.BackgroundWorker1_ProgressChanged);
            this.buildDatabaseBackgroundWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(this.BackgroundWorker1_RunWorkerCompleted);
        }

        /// <summary>
        /// This code added to correctly implement the disposable pattern. 
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) below.
            this.Dispose(true);
        }

        /// <summary>
        /// This code added to correctly implement the disposable pattern. 
        /// </summary>
        /// <param name="disposing">We are disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.buildDatabaseBackgroundWorker.Dispose();
                }

                this.disposedValue = true;
            }
        }

        /// <summary>
        /// Bind the Drive List
        /// </summary>
        private void BindDriveList()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                if (d.DriveType.Equals(DriveType.Removable))
                {
                    targetDriveComboBox.Items.Add(d.Name);
                }
            }

            if (targetDriveComboBox.Items.Count > 0)
            {
                targetDriveComboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Handle the button click event
        /// </summary>
        /// <param name="sender">the Sender object</param>
        /// <param name="e">The Event Args</param>
        private void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.buildDatabaseBackgroundWorker.IsBusy)
            {
                if (string.IsNullOrEmpty(this.usernameTextBox.Text))
                {
                    MessageBox.Show(Properties.Resources.UsernameError, Properties.Resources.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    return;
                }

                if (string.IsNullOrEmpty(this.passwordBox.Password))
                {
                    MessageBox.Show(Properties.Resources.PasswordError, Properties.Resources.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    return;
                }

                if (this.targetDriveComboBox.SelectedValue == null)
                {
                    MessageBox.Show(Properties.Resources.TargetDriveError, Properties.Resources.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    return;
                }

                this.Cursor = Cursors.Wait;
                this.processButton.IsEnabled = false;

                CameraSettings settings = new CameraSettings()
                {
                    Username = this.usernameTextBox.Text,
                    Password = this.passwordBox.Password,
                    TargertDrive = this.targetDriveComboBox.SelectedValue.ToString(),
                    IncludeUnverified = this.yesRadioButton.IsChecked.HasValue && (bool)this.yesRadioButton.IsChecked,
                    IncludeStatic = this.fixedCheckBox.IsChecked.HasValue && (bool)this.fixedCheckBox.IsChecked,
                    IncludeMobile = this.mobileCheckBox.IsChecked.HasValue && (bool)this.mobileCheckBox.IsChecked,
                    IncludeSpecs = this.specsCheckBox.IsChecked.HasValue && (bool)this.specsCheckBox.IsChecked,
                    IncludeRedLight = this.redlightCheckBox.IsChecked.HasValue && (bool)this.redlightCheckBox.IsChecked
                };

                this.buildDatabaseBackgroundWorker.RunWorkerAsync(settings);
            }
        }

        /// <summary>
        /// Do Work event Handler
        /// </summary>
        /// <param name="sender">Sender Argument</param>
        /// <param name="e">Event Argument</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Justification = "Required to ensure the SQLite DB is released")]
        private void BackgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            CameraSettings settings = (CameraSettings)e.Argument;

            if (Directory.Exists(settings.TargertDrive))
            {
                // Load POIs from PocketGPSWorld
                this.buildDatabaseBackgroundWorker.ReportProgress(1, "Loading Cameras from PocketGPSWorld.com");

                byte[] camerasZip = null;

                try
                {
                    camerasZip = SpeedCameras.Load(settings.Username, settings.Password);
                }
                catch (WebException webException)
                {
                    // Offer the user to build from manual download
                    if (MessageBox.Show(webException.Message + "\r\r" + Properties.Resources.ManualDownloadPrompt, Properties.Resources.ErrorTitle, MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    {
                        MessageBox.Show(Properties.Resources.ManualDownloadGuidance, Properties.Resources.ManualDownloadGuidanceTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                        Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
                        fileDialog.DefaultExt = ".zip";
                        fileDialog.Filter = "Zip Files|*.zip";
                        bool? dialogResult = fileDialog.ShowDialog();
                        if (dialogResult == true)
                        {
                            string filename = fileDialog.FileName;
                            camerasZip = File.ReadAllBytes(filename);
                        }
                    }
                }

                if (camerasZip == null)
                {
                    return;
                }

                Collection<PointOfInterestCategory> cameras = null;

                try
                {
                    cameras = SpeedCameras.Filter(
                        SpeedCameras.SortCameras(
                        SpeedCameras.UnpackCameras(camerasZip)), 
                        settings);
                }
                catch (FileFormatException fileFormatException)
                {
                    MessageBox.Show(fileFormatException.Message, Properties.Resources.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }

                // Check we got some Cameras from the PocketGPSWorld
                if (cameras != null && cameras.Count > 0)
                {
                    // Load existing POIs from card
                    this.buildDatabaseBackgroundWorker.ReportProgress(2, "Load existing POIs from Card");
                    Collection<PointOfInterestCategory> existingCategories = PointOfInterestDatabase.LoadPois(settings.TargertDrive);

                    // We need to do a merge
                    Collection<PointOfInterestCategory> mergedPois = PointOfInterestDatabase.MergePointsOfInterest(existingCategories, cameras);

                    // Build the SD card
                    this.buildDatabaseBackgroundWorker.ReportProgress(3, "Building Database");
                    int loadedWaypoints = PointOfInterestDatabase.SavePois(mergedPois, settings.TargertDrive, this.buildDatabaseBackgroundWorker);

                    e.Result = loadedWaypoints;
                }
            }
            else
            {
                e.Result = -1;
                MessageBox.Show(
                Properties.Resources.FileNotFoundError,
                Properties.Resources.ErrorTitle,
                MessageBoxButton.OK,
                MessageBoxImage.Error,
                MessageBoxResult.OK,
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
            this.processButton.Content = e.UserState.ToString();
        }

        /// <summary>
        /// Handle the worker complete event
        /// </summary>
        /// <param name="sender">Sender Argument</param>
        /// <param name="e">Event Argument</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Our messagebox won't be right reading")]
        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null && (int)e.Result >= 0)
            {
                MessageBox.Show(
                    string.Format(Properties.Resources.CompletionFormatString, e.Result),
                    Properties.Resources.CompletionTitle);
                this.progressBar.Value = 100;
            }
            else
            {
                this.progressBar.Value = 0;
            }

            this.processButton.IsEnabled = true;
            this.Cursor = Cursors.Arrow;
            this.processButton.Content = "Get Cameras";
        }

        /// <summary>
        /// Handle the window load event
        /// </summary>
        /// <param name="sender">The Sender object</param>
        /// <param name="e">The event args</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.BindDriveList();
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.Title += string.Format(" (v{0}.{1}.{2}.{3})", version.Major, version.Minor, version.Build, version.MinorRevision);
            this.LoadSettings();
        }

        /// <summary>
        /// Load the Application settings
        /// </summary>
        private void LoadSettings()
        {
            usernameTextBox.Text = Properties.Settings.Default.PocketGpsWorldUsername;
            if (!string.IsNullOrEmpty(Properties.Settings.Default.PocketGpsWorldPassword))
            {
                // passwordBox.Password = SecureStrings.ToInsecureString(SecureStrings.DecryptString(Properties.Settings.Default.PocketGpsWorldPassword));
            }

            yesRadioButton.IsChecked = Properties.Settings.Default.IncludeUnverifiedCameras;
            fixedCheckBox.IsChecked = Properties.Settings.Default.IncludeFixedCameras;
            mobileCheckBox.IsChecked = Properties.Settings.Default.IncludeMobileCameras;
            specsCheckBox.IsChecked = Properties.Settings.Default.IncludeSpecsCameras;
            redlightCheckBox.IsChecked = Properties.Settings.Default.IncludeRedlightCameras;
            databaseFormatComboBox.SelectedItem = Properties.Settings.Default.DatabaseFormat;
        }

        /// <summary>
        /// Save the Application settings
        /// </summary>
        private void SaveSettings()
        {
            Properties.Settings.Default.PocketGpsWorldUsername = usernameTextBox.Text;

            // Properties.Settings.Default.PocketGpsWorldPassword = SecureStrings.EncryptString(SecureStrings.ToSecureString(passwordBox.Password));
            Properties.Settings.Default.IncludeUnverifiedCameras = yesRadioButton.IsChecked.HasValue ? (bool)yesRadioButton.IsChecked : true;
            Properties.Settings.Default.IncludeFixedCameras = fixedCheckBox.IsChecked.HasValue ? (bool)fixedCheckBox.IsChecked : true;
            Properties.Settings.Default.IncludeMobileCameras = mobileCheckBox.IsChecked.HasValue ? (bool)mobileCheckBox.IsChecked : true;
            Properties.Settings.Default.IncludeSpecsCameras = specsCheckBox.IsChecked.HasValue ? (bool)specsCheckBox.IsChecked : true;
            Properties.Settings.Default.IncludeRedlightCameras = redlightCheckBox.IsChecked.HasValue ? (bool)redlightCheckBox.IsChecked : true;
            Properties.Settings.Default.DatabaseFormat = databaseFormatComboBox.SelectedItem.ToString();
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// On Closing event
        /// </summary>
        /// <param name="sender">default sender</param>
        /// <param name="e">default event arguments</param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.SaveSettings();
        }
    }
}

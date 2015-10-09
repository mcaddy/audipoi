//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------

namespace SpeedCameraToPoi
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Windows;
    using System.Windows.Input;
    using Mcaddy.AudiPoiDatabase;
    using PocketGpsWorld;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Background worker
        /// </summary>
        private BackgroundWorker buildDatabaseBackgroundWorker;

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
                processButton.Content = "Cancel";
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
                byte[] camerasZip = SpeedCameras.Load(settings.Username, settings.Password);
                Collection<PointOfInterestCategory> cameras = SpeedCameras.Filter(SpeedCameras.SortCameras(SpeedCameras.UnpackCameras(camerasZip)), settings);

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
        private void BackgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            this.progressBar.Value = 100;
            if ((int)e.Result >= 0)
            {
                MessageBox.Show(
                    string.Format(Properties.Resources.CompletionFormatString, e.Result),
                    Properties.Resources.CompletionTitle);
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
        }
    }
}

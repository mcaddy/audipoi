//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------
namespace POIBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Security.Principal;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Mcaddy;
    using Mcaddy.AudiPoiDatabase;
    using Newtonsoft.Json;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        /// <summary>
        /// Background worker
        /// </summary>
        private readonly BackgroundWorker buildDatabaseBackgroundWorker;

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

            this.buildDatabaseBackgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };

            this.buildDatabaseBackgroundWorker.DoWork +=
                new DoWorkEventHandler(this.BackgroundWorker_DoWork);
            this.buildDatabaseBackgroundWorker.ProgressChanged +=
                new ProgressChangedEventHandler(this.BackgroundWorker_ProgressChanged);
            this.buildDatabaseBackgroundWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(this.BackgroundWorker_RunWorkerCompleted);
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
        /// Bind the checkboxes from Azure Data
        /// </summary>
        /// <returns>false if unable to bind the checkboxes</returns>
        private bool BindCheckBoxes()
        {
            try
            {
                string response = Azure.InvokeFunction(
                    App.Configuration.Get("CategoriesFunctionPath"),
                    string.Empty);

                Dictionary<int, string> categories = JsonConvert.DeserializeObject<Dictionary<int, string>>(response);
                foreach (int categoryId in categories.Keys)
                {
                    CheckBox dynamicCheckbox = new CheckBox
                    {
                        Content = categories[categoryId],
                        CommandParameter = categoryId
                    };

                    poiCategories.Children.Add(dynamicCheckbox);
                }

                return true;
            }
            catch (WebException webEx)
            {
                MessageBox.Show($"{Properties.Resources.PoiCategoryDownloadError}\r\n\r\nDetail:\r\n{webEx.Message}", Properties.Resources.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return false;
            }
        }

        /// <summary>
        /// Handle the Process Button event
        /// </summary>
        /// <param name="sender">Default Sender Args</param>
        /// <param name="e">Default Event Args</param>
        private void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.buildDatabaseBackgroundWorker.IsBusy)
            {
                int categoriesFlag = this.CaculateCategoryFlag();
                if (categoriesFlag.Equals(0))
                {
                    MessageBox.Show(Properties.Resources.SelectAtLeastOnePOICategory, Properties.Resources.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    return;
                }

                if (this.targetDriveComboBox.SelectedItem == null)
                {
                    MessageBox.Show(Properties.Resources.TargetDriveError, Properties.Resources.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    return;
                }

                this.Cursor = Cursors.Wait;
                this.processButton.IsEnabled = false;

                Tuple<int, string, string> settings;
                using (WindowsIdentity currentUser = WindowsIdentity.GetCurrent())
                {
                    settings = new Tuple<int, string, string>(
                        categoriesFlag,
                        ((ComboBoxItem)this.targetDriveComboBox.SelectedItem).Tag.ToString(),
                        currentUser.Name);
                }

                this.buildDatabaseBackgroundWorker.RunWorkerAsync(settings);
            }
        }

        /// <summary>
        /// Do Work event Handler
        /// </summary>
        /// <param name="sender">Sender Argument</param>
        /// <param name="e">Event Argument</param>
        private void BackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Tuple<int, string, string> settings = (Tuple<int, string, string>)e.Argument;

            int categoriesFlag = settings.Item1;
            string targetDrive = settings.Item2;
            string username = settings.Item3;

            LoggingClient logger = new LoggingClient(App.Configuration);
            logger.Log(username, $"POI Loader - Requested {categoriesFlag}");

            if (Directory.Exists(targetDrive))
            {
                this.buildDatabaseBackgroundWorker.ReportProgress(1, "Loading POIs");

                // Get the POIs
                string response = Azure.InvokeFunction(string.Format(App.Configuration.Get("POIsFunctionPath"), categoriesFlag), string.Empty);
                Collection<PointOfInterestCategory> poiCategories = JsonConvert.DeserializeObject<Collection<PointOfInterestCategory>>(response);

                // Get the Category icons
                response = Azure.InvokeFunction(string.Format(App.Configuration.Get("CategoryImagesFunctionPath"), categoriesFlag), string.Empty);
                Dictionary<int, string> categoryIcons = JsonConvert.DeserializeObject<Dictionary<int, string>>(response);
                
                // Update the category to include the correct icons
                foreach (PointOfInterestCategory poiCategory in poiCategories)
                {
                    poiCategory.Icon = this.BitmapFromBase64(categoryIcons[poiCategory.Id]);
                }

                // Check we got some Pois
                if (poiCategories != null && poiCategories.Count > 0)
                {
                    // Load existing POIs from card
                    this.buildDatabaseBackgroundWorker.ReportProgress(2, "Load existing POIs from Target Drive");
                    Collection<PointOfInterestCategory> existingCategories = PointOfInterestDatabase.LoadPois(targetDrive);

                    if (existingCategories.Count > 0)
                    {
                        List<string> categories = new List<string>();
                        foreach (PointOfInterestCategory category in existingCategories)
                        {
                            categories.Add(category.Name);
                        }

                        logger.Log(username, $"POI Loader - Already had {JsonConvert.SerializeObject(categories)}");
                    }

                    // We need to do a merge
                    Collection<PointOfInterestCategory> mergedPois = PointOfInterestDatabase.MergePointsOfInterest(existingCategories, poiCategories);

                    // Build the SD card
                    this.buildDatabaseBackgroundWorker.ReportProgress(3, "Building Database");
                    int loadedWaypoints = PointOfInterestDatabase.SavePois(mergedPois, targetDrive, this.buildDatabaseBackgroundWorker);

                    e.Result = loadedWaypoints;

                    logger.Log(username, $"POI Loader - Complete");
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
        private void BackgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
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
        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null && (int)e.Result >= 0)
            {
                if ((bool)App.Current.Properties["auto"])
                {
                    this.Close();
                }
                else
                {
                    MessageBox.Show(
                        string.Format(Properties.Resources.CompletionFormatString, e.Result),
                        Properties.Resources.CompletionTitle);
                    this.progressBar.Value = 100;
                }
            }
            else
            {
                this.progressBar.Value = 0;
            }

            this.processButton.IsEnabled = true;
            this.Cursor = Cursors.Arrow;
            this.processButton.Content = "Get POIs";
        }

        /// <summary>
        /// Takes a Base64 Encoded image and returns a Bitmap
        /// </summary>
        /// <param name="base64CategoryIcon">The Base64 encoded file</param>
        /// <returns>A Bitmap</returns>
        private Bitmap BitmapFromBase64(string base64CategoryIcon)
        {
            Regex regex = new Regex(@"data:image\/(?<imageType>[a-zA-Z]*);base64,(?<base64>[a-zA-Z0-9/=+]*)");
            Match match = regex.Match(base64CategoryIcon);
            if (match.Success && match.Groups["imageType"].Value.Equals("png"))
            {
                byte[] imageData = Convert.FromBase64String(match.Groups["base64"].Value);

                using (MemoryStream ms = new MemoryStream(imageData))
                {
                    return new Bitmap(ms);
                }
            }

            return null;
        }

        /// <summary>
        /// Calculate the Categories flag for download
        /// </summary>
        /// <returns>the value indicating the combination of POI categories to load</returns>
        private int CaculateCategoryFlag()
        {
            int flag = 0;

            foreach (Control control in poiCategories.Children)
            {
                if (control is CheckBox && (bool)((CheckBox)control).IsChecked)
                {
                    flag += (int)((CheckBox)control).CommandParameter;
                }
            }

            return flag;
        }

        /// <summary>
        /// Handle the window Loaded event
        /// </summary>
        /// <param name="sender">Sender Argument</param>
        /// <param name="e">Event Argument</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.Title += string.Format(" (v{0}.{1}.{2}.{3})", version.Major, version.Minor, version.Build, version.MinorRevision);

            this.processButton.IsEnabled = false;

            if (this.BindCheckBoxes())
            {
                this.LoadSettings();
                if (UIUtils.BindDriveList(this.targetDriveComboBox))
                {
                    this.processButton.IsEnabled = true;
                }
            }
            else
            {
                Environment.Exit(-10);
            }
        }

        /// <summary>
        /// Load the Application settings
        /// </summary>
        private void LoadSettings()
        {
            int categoriesFlag = Properties.Settings.Default.CategoriesFlag;
            if (categoriesFlag >= 0)
            {
                int maxFlag = 0;

                // Get all the check boxes
                Dictionary<int, CheckBox> checkBoxes = new Dictionary<int, CheckBox>();
                foreach (CheckBox checkBox in poiCategories.Children)
                {
                    checkBoxes.Add((int)checkBox.CommandParameter, checkBox);
                    maxFlag = Math.Max((int)checkBox.CommandParameter, maxFlag);
                }

                // Work through the categories flag setting the check boxes as appropriate
                while (categoriesFlag > 0)
                {
                    if (categoriesFlag - maxFlag >= 0)
                    {
                        checkBoxes[maxFlag].IsChecked = true;
                        categoriesFlag -= maxFlag;
                    }

                    maxFlag /= 2;
                }
            }
        }

        /// <summary>
        /// Save the Application settings
        /// </summary>
        private void SaveSettings()
        {
            Properties.Settings.Default.CategoriesFlag = this.CaculateCategoryFlag();
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Handle the window closing Event
        /// </summary>
        /// <param name="sender">Default sender arguments</param>
        /// <param name="e">Default event argument</param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.SaveSettings();
        }

        /// <summary>
        /// Handle the Content Rendered Event
        /// </summary>
        /// <param name="sender">Sender Argument</param>
        /// <param name="e">Event Argument</param>
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(App.Current.Properties["targetDrive"].ToString()))
            {
                // Default the drive
                if (!UIUtils.SelectItemByTag(this.targetDriveComboBox, App.Current.Properties["targetDrive"].ToString()))
                {
                    MessageBox.Show(Properties.Resources.InvalidTargetDriveOnCommandLine, Properties.Resources.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);

                    return;
                }
            }

            if ((bool)App.Current.Properties["auto"])
            {
                this.ProcessButton_Click(this.processButton, new RoutedEventArgs());
            }
        }

        /// <summary>
        /// Event handler for the Refresh button
        /// </summary>
        /// <param name="sender">Sender Argument</param>
        /// <param name="e">Event Argument</param>
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (UIUtils.BindDriveList(this.targetDriveComboBox))
            {
                this.processButton.IsEnabled = true;
            }
            else
            {
                this.processButton.IsEnabled = false;
            }
        }
    }
}

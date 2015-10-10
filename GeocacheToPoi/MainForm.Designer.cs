namespace Mcaddy.Audi
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.openSourceFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.gpxFileLabel = new System.Windows.Forms.Label();
            this.gpxFilenameTextBox = new System.Windows.Forms.TextBox();
            this.targetFolderLabel = new System.Windows.Forms.Label();
            this.processButton = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.buildDatabaseBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.targetDriveComboBox = new System.Windows.Forms.ComboBox();
            this.selectSourceGPX = new System.Windows.Forms.Button();
            this.geocachingUsernameTextBox = new System.Windows.Forms.TextBox();
            this.excludeOwnedCachesCheckBox = new System.Windows.Forms.CheckBox();
            this.excludeFoundCachesCheckBox = new System.Windows.Forms.CheckBox();
            this.optionsGroupBox = new System.Windows.Forms.GroupBox();
            this.geocachingUsernameLabel = new System.Windows.Forms.Label();
            this.sourceGroupBox = new System.Windows.Forms.GroupBox();
            this.targetGroupBox = new System.Windows.Forms.GroupBox();
            this.optionsGroupBox.SuspendLayout();
            this.sourceGroupBox.SuspendLayout();
            this.targetGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpxFileLabel
            // 
            this.gpxFileLabel.AutoSize = true;
            this.gpxFileLabel.Location = new System.Drawing.Point(8, 23);
            this.gpxFileLabel.Name = "gpxFileLabel";
            this.gpxFileLabel.Size = new System.Drawing.Size(51, 13);
            this.gpxFileLabel.TabIndex = 1;
            this.gpxFileLabel.Text = "GPX File:";
            // 
            // gpxFilenameTextBox
            // 
            this.gpxFilenameTextBox.Location = new System.Drawing.Point(63, 20);
            this.gpxFilenameTextBox.Name = "gpxFilenameTextBox";
            this.gpxFilenameTextBox.ReadOnly = true;
            this.gpxFilenameTextBox.Size = new System.Drawing.Size(228, 20);
            this.gpxFilenameTextBox.TabIndex = 2;
            this.gpxFilenameTextBox.Text = "C:\\Users\\mike\\Documents\\waypoints.gpx";
            // 
            // targetFolderLabel
            // 
            this.targetFolderLabel.AutoSize = true;
            this.targetFolderLabel.Location = new System.Drawing.Point(9, 27);
            this.targetFolderLabel.Name = "targetFolderLabel";
            this.targetFolderLabel.Size = new System.Drawing.Size(65, 13);
            this.targetFolderLabel.TabIndex = 3;
            this.targetFolderLabel.Text = "Drive Letter:";
            // 
            // processButton
            // 
            this.processButton.Location = new System.Drawing.Point(15, 215);
            this.processButton.Name = "processButton";
            this.processButton.Size = new System.Drawing.Size(394, 23);
            this.processButton.TabIndex = 5;
            this.processButton.Text = "Process";
            this.processButton.UseVisualStyleBackColor = true;
            this.processButton.Click += new System.EventHandler(this.ProcessButton_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(15, 248);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(394, 23);
            this.progressBar.TabIndex = 6;
            // 
            // buildDatabaseBackgroundWorker
            // 
            this.buildDatabaseBackgroundWorker.WorkerReportsProgress = true;
            this.buildDatabaseBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BuildDatabaseBackgroundWorker_DoWork);
            this.buildDatabaseBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BuildDatabaseBackgroundWorker_ProgressChanged);
            this.buildDatabaseBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BuildDatabaseBackgroundWorker_RunWorkerCompleted);
            // 
            // targetDriveComboBox
            // 
            this.targetDriveComboBox.FormattingEnabled = true;
            this.targetDriveComboBox.Location = new System.Drawing.Point(81, 23);
            this.targetDriveComboBox.Name = "targetDriveComboBox";
            this.targetDriveComboBox.Size = new System.Drawing.Size(121, 21);
            this.targetDriveComboBox.TabIndex = 7;
            // 
            // selectSourceGPX
            // 
            this.selectSourceGPX.Location = new System.Drawing.Point(302, 19);
            this.selectSourceGPX.Name = "selectSourceGPX";
            this.selectSourceGPX.Size = new System.Drawing.Size(85, 23);
            this.selectSourceGPX.TabIndex = 8;
            this.selectSourceGPX.Text = "Select Source";
            this.selectSourceGPX.UseVisualStyleBackColor = true;
            this.selectSourceGPX.Click += new System.EventHandler(this.SelectSourceGPX_Click);
            // 
            // geocachingUsernameTextBox
            // 
            this.geocachingUsernameTextBox.Location = new System.Drawing.Point(156, 16);
            this.geocachingUsernameTextBox.Name = "geocachingUsernameTextBox";
            this.geocachingUsernameTextBox.Size = new System.Drawing.Size(100, 20);
            this.geocachingUsernameTextBox.TabIndex = 10;
            // 
            // excludeOwnedCachesCheckBox
            // 
            this.excludeOwnedCachesCheckBox.AutoSize = true;
            this.excludeOwnedCachesCheckBox.Location = new System.Drawing.Point(14, 45);
            this.excludeOwnedCachesCheckBox.Name = "excludeOwnedCachesCheckBox";
            this.excludeOwnedCachesCheckBox.Size = new System.Drawing.Size(137, 17);
            this.excludeOwnedCachesCheckBox.TabIndex = 11;
            this.excludeOwnedCachesCheckBox.Text = "Exclude owned caches";
            this.excludeOwnedCachesCheckBox.UseVisualStyleBackColor = true;
            // 
            // excludeFoundCachesCheckBox
            // 
            this.excludeFoundCachesCheckBox.AutoSize = true;
            this.excludeFoundCachesCheckBox.Location = new System.Drawing.Point(158, 45);
            this.excludeFoundCachesCheckBox.Name = "excludeFoundCachesCheckBox";
            this.excludeFoundCachesCheckBox.Size = new System.Drawing.Size(132, 17);
            this.excludeFoundCachesCheckBox.TabIndex = 12;
            this.excludeFoundCachesCheckBox.Text = "Exclude found caches";
            this.excludeFoundCachesCheckBox.UseVisualStyleBackColor = true;
            // 
            // optionsGroupBox
            // 
            this.optionsGroupBox.Controls.Add(this.geocachingUsernameLabel);
            this.optionsGroupBox.Controls.Add(this.excludeFoundCachesCheckBox);
            this.optionsGroupBox.Controls.Add(this.geocachingUsernameTextBox);
            this.optionsGroupBox.Controls.Add(this.excludeOwnedCachesCheckBox);
            this.optionsGroupBox.Location = new System.Drawing.Point(15, 71);
            this.optionsGroupBox.Name = "optionsGroupBox";
            this.optionsGroupBox.Size = new System.Drawing.Size(394, 73);
            this.optionsGroupBox.TabIndex = 13;
            this.optionsGroupBox.TabStop = false;
            this.optionsGroupBox.Text = "Options";
            // 
            // geocachingUsernameLabel
            // 
            this.geocachingUsernameLabel.AutoSize = true;
            this.geocachingUsernameLabel.Location = new System.Drawing.Point(10, 19);
            this.geocachingUsernameLabel.Name = "geocachingUsernameLabel";
            this.geocachingUsernameLabel.Size = new System.Drawing.Size(140, 13);
            this.geocachingUsernameLabel.TabIndex = 11;
            this.geocachingUsernameLabel.Text = "Geocaching.com username:";
            // 
            // sourceGroupBox
            // 
            this.sourceGroupBox.Controls.Add(this.gpxFileLabel);
            this.sourceGroupBox.Controls.Add(this.gpxFilenameTextBox);
            this.sourceGroupBox.Controls.Add(this.selectSourceGPX);
            this.sourceGroupBox.Location = new System.Drawing.Point(12, 11);
            this.sourceGroupBox.Name = "sourceGroupBox";
            this.sourceGroupBox.Size = new System.Drawing.Size(397, 54);
            this.sourceGroupBox.TabIndex = 14;
            this.sourceGroupBox.TabStop = false;
            this.sourceGroupBox.Text = "Source";
            // 
            // targetGroupBox
            // 
            this.targetGroupBox.Controls.Add(this.targetDriveComboBox);
            this.targetGroupBox.Controls.Add(this.targetFolderLabel);
            this.targetGroupBox.Location = new System.Drawing.Point(15, 151);
            this.targetGroupBox.Name = "targetGroupBox";
            this.targetGroupBox.Size = new System.Drawing.Size(394, 54);
            this.targetGroupBox.TabIndex = 15;
            this.targetGroupBox.TabStop = false;
            this.targetGroupBox.Text = "Target";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 283);
            this.Controls.Add(this.targetGroupBox);
            this.Controls.Add(this.sourceGroupBox);
            this.Controls.Add(this.optionsGroupBox);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.processButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Geocaching POI Builder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.optionsGroupBox.ResumeLayout(false);
            this.optionsGroupBox.PerformLayout();
            this.sourceGroupBox.ResumeLayout(false);
            this.sourceGroupBox.PerformLayout();
            this.targetGroupBox.ResumeLayout(false);
            this.targetGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openSourceFileDialog;
        private System.Windows.Forms.Label gpxFileLabel;
        private System.Windows.Forms.TextBox gpxFilenameTextBox;
        private System.Windows.Forms.Label targetFolderLabel;
        private System.Windows.Forms.Button processButton;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.ComponentModel.BackgroundWorker buildDatabaseBackgroundWorker;
        private System.Windows.Forms.ComboBox targetDriveComboBox;
        private System.Windows.Forms.Button selectSourceGPX;
        private System.Windows.Forms.TextBox geocachingUsernameTextBox;
        private System.Windows.Forms.CheckBox excludeOwnedCachesCheckBox;
        private System.Windows.Forms.CheckBox excludeFoundCachesCheckBox;
        private System.Windows.Forms.GroupBox optionsGroupBox;
        private System.Windows.Forms.Label geocachingUsernameLabel;
        private System.Windows.Forms.GroupBox sourceGroupBox;
        private System.Windows.Forms.GroupBox targetGroupBox;
    }
}


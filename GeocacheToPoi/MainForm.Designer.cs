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
            this.refreshButton = new System.Windows.Forms.Button();
            this.optionsGroupBox.SuspendLayout();
            this.sourceGroupBox.SuspendLayout();
            this.targetGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpxFileLabel
            // 
            this.gpxFileLabel.AutoSize = true;
            this.gpxFileLabel.Location = new System.Drawing.Point(16, 44);
            this.gpxFileLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.gpxFileLabel.Name = "gpxFileLabel";
            this.gpxFileLabel.Size = new System.Drawing.Size(103, 25);
            this.gpxFileLabel.TabIndex = 1;
            this.gpxFileLabel.Text = "GPX File:";
            // 
            // gpxFilenameTextBox
            // 
            this.gpxFilenameTextBox.Location = new System.Drawing.Point(126, 38);
            this.gpxFilenameTextBox.Margin = new System.Windows.Forms.Padding(6);
            this.gpxFilenameTextBox.Name = "gpxFilenameTextBox";
            this.gpxFilenameTextBox.ReadOnly = true;
            this.gpxFilenameTextBox.Size = new System.Drawing.Size(452, 31);
            this.gpxFilenameTextBox.TabIndex = 2;
            // 
            // targetFolderLabel
            // 
            this.targetFolderLabel.AutoSize = true;
            this.targetFolderLabel.Location = new System.Drawing.Point(18, 52);
            this.targetFolderLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.targetFolderLabel.Name = "targetFolderLabel";
            this.targetFolderLabel.Size = new System.Drawing.Size(129, 25);
            this.targetFolderLabel.TabIndex = 3;
            this.targetFolderLabel.Text = "Drive Letter:";
            // 
            // processButton
            // 
            this.processButton.Location = new System.Drawing.Point(30, 413);
            this.processButton.Margin = new System.Windows.Forms.Padding(6);
            this.processButton.Name = "processButton";
            this.processButton.Size = new System.Drawing.Size(788, 44);
            this.processButton.TabIndex = 5;
            this.processButton.Text = "Process";
            this.processButton.UseVisualStyleBackColor = true;
            this.processButton.Click += new System.EventHandler(this.ProcessButton_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(30, 477);
            this.progressBar.Margin = new System.Windows.Forms.Padding(6);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(788, 44);
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
            this.targetDriveComboBox.Location = new System.Drawing.Point(162, 44);
            this.targetDriveComboBox.Margin = new System.Windows.Forms.Padding(6);
            this.targetDriveComboBox.Name = "targetDriveComboBox";
            this.targetDriveComboBox.Size = new System.Drawing.Size(238, 33);
            this.targetDriveComboBox.TabIndex = 7;
            // 
            // selectSourceGPX
            // 
            this.selectSourceGPX.Location = new System.Drawing.Point(604, 37);
            this.selectSourceGPX.Margin = new System.Windows.Forms.Padding(6);
            this.selectSourceGPX.Name = "selectSourceGPX";
            this.selectSourceGPX.Size = new System.Drawing.Size(170, 44);
            this.selectSourceGPX.TabIndex = 8;
            this.selectSourceGPX.Text = "Select Source";
            this.selectSourceGPX.UseVisualStyleBackColor = true;
            this.selectSourceGPX.Click += new System.EventHandler(this.SelectSourceGPX_Click);
            // 
            // geocachingUsernameTextBox
            // 
            this.geocachingUsernameTextBox.Location = new System.Drawing.Point(312, 31);
            this.geocachingUsernameTextBox.Margin = new System.Windows.Forms.Padding(6);
            this.geocachingUsernameTextBox.Name = "geocachingUsernameTextBox";
            this.geocachingUsernameTextBox.Size = new System.Drawing.Size(196, 31);
            this.geocachingUsernameTextBox.TabIndex = 10;
            // 
            // excludeOwnedCachesCheckBox
            // 
            this.excludeOwnedCachesCheckBox.AutoSize = true;
            this.excludeOwnedCachesCheckBox.Location = new System.Drawing.Point(28, 87);
            this.excludeOwnedCachesCheckBox.Margin = new System.Windows.Forms.Padding(6);
            this.excludeOwnedCachesCheckBox.Name = "excludeOwnedCachesCheckBox";
            this.excludeOwnedCachesCheckBox.Size = new System.Drawing.Size(265, 29);
            this.excludeOwnedCachesCheckBox.TabIndex = 11;
            this.excludeOwnedCachesCheckBox.Text = "Exclude owned caches";
            this.excludeOwnedCachesCheckBox.UseVisualStyleBackColor = true;
            // 
            // excludeFoundCachesCheckBox
            // 
            this.excludeFoundCachesCheckBox.AutoSize = true;
            this.excludeFoundCachesCheckBox.Location = new System.Drawing.Point(316, 87);
            this.excludeFoundCachesCheckBox.Margin = new System.Windows.Forms.Padding(6);
            this.excludeFoundCachesCheckBox.Name = "excludeFoundCachesCheckBox";
            this.excludeFoundCachesCheckBox.Size = new System.Drawing.Size(256, 29);
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
            this.optionsGroupBox.Location = new System.Drawing.Point(30, 137);
            this.optionsGroupBox.Margin = new System.Windows.Forms.Padding(6);
            this.optionsGroupBox.Name = "optionsGroupBox";
            this.optionsGroupBox.Padding = new System.Windows.Forms.Padding(6);
            this.optionsGroupBox.Size = new System.Drawing.Size(788, 140);
            this.optionsGroupBox.TabIndex = 13;
            this.optionsGroupBox.TabStop = false;
            this.optionsGroupBox.Text = "Options";
            // 
            // geocachingUsernameLabel
            // 
            this.geocachingUsernameLabel.AutoSize = true;
            this.geocachingUsernameLabel.Location = new System.Drawing.Point(20, 37);
            this.geocachingUsernameLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.geocachingUsernameLabel.Name = "geocachingUsernameLabel";
            this.geocachingUsernameLabel.Size = new System.Drawing.Size(280, 25);
            this.geocachingUsernameLabel.TabIndex = 11;
            this.geocachingUsernameLabel.Text = "Geocaching.com username:";
            // 
            // sourceGroupBox
            // 
            this.sourceGroupBox.Controls.Add(this.gpxFileLabel);
            this.sourceGroupBox.Controls.Add(this.gpxFilenameTextBox);
            this.sourceGroupBox.Controls.Add(this.selectSourceGPX);
            this.sourceGroupBox.Location = new System.Drawing.Point(24, 21);
            this.sourceGroupBox.Margin = new System.Windows.Forms.Padding(6);
            this.sourceGroupBox.Name = "sourceGroupBox";
            this.sourceGroupBox.Padding = new System.Windows.Forms.Padding(6);
            this.sourceGroupBox.Size = new System.Drawing.Size(794, 104);
            this.sourceGroupBox.TabIndex = 14;
            this.sourceGroupBox.TabStop = false;
            this.sourceGroupBox.Text = "Source";
            // 
            // targetGroupBox
            // 
            this.targetGroupBox.Controls.Add(this.refreshButton);
            this.targetGroupBox.Controls.Add(this.targetDriveComboBox);
            this.targetGroupBox.Controls.Add(this.targetFolderLabel);
            this.targetGroupBox.Location = new System.Drawing.Point(30, 290);
            this.targetGroupBox.Margin = new System.Windows.Forms.Padding(6);
            this.targetGroupBox.Name = "targetGroupBox";
            this.targetGroupBox.Padding = new System.Windows.Forms.Padding(6);
            this.targetGroupBox.Size = new System.Drawing.Size(788, 104);
            this.targetGroupBox.TabIndex = 15;
            this.targetGroupBox.TabStop = false;
            this.targetGroupBox.Text = "Target";
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(418, 42);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(107, 44);
            this.refreshButton.TabIndex = 8;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(836, 544);
            this.Controls.Add(this.targetGroupBox);
            this.Controls.Add(this.sourceGroupBox);
            this.Controls.Add(this.optionsGroupBox);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.processButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
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
        private System.Windows.Forms.Button refreshButton;
    }
}


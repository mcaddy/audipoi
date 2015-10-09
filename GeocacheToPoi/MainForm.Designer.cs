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
            this.openSourceFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectSourceGpxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectTargetFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gpxFileLabel = new System.Windows.Forms.Label();
            this.gpxFilenameTextBox = new System.Windows.Forms.TextBox();
            this.targetFolderLabel = new System.Windows.Forms.Label();
            this.processButton = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.buildDatabaseBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.targetDriveComboBox = new System.Windows.Forms.ComboBox();
            this.mainMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // openSourceFileDialog
            // 
            this.openSourceFileDialog.FileName = "openFileDialog";
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(570, 24);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectSourceGpxToolStripMenuItem,
            this.selectTargetFolderToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // selectSourceGpxToolStripMenuItem
            // 
            this.selectSourceGpxToolStripMenuItem.Name = "selectSourceGpxToolStripMenuItem";
            this.selectSourceGpxToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.selectSourceGpxToolStripMenuItem.Text = "Select Source Gpx";
            this.selectSourceGpxToolStripMenuItem.Click += new System.EventHandler(this.SelectSourceGpxToolStripMenuItem_Click);
            // 
            // selectTargetFolderToolStripMenuItem
            // 
            this.selectTargetFolderToolStripMenuItem.Name = "selectTargetFolderToolStripMenuItem";
            this.selectTargetFolderToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.selectTargetFolderToolStripMenuItem.Text = "Select Target Folder";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(174, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // gpxFileLabel
            // 
            this.gpxFileLabel.AutoSize = true;
            this.gpxFileLabel.Location = new System.Drawing.Point(12, 39);
            this.gpxFileLabel.Name = "gpxFileLabel";
            this.gpxFileLabel.Size = new System.Drawing.Size(51, 13);
            this.gpxFileLabel.TabIndex = 1;
            this.gpxFileLabel.Text = "GPX File:";
            // 
            // gpxFilenameTextBox
            // 
            this.gpxFilenameTextBox.Location = new System.Drawing.Point(70, 36);
            this.gpxFilenameTextBox.Name = "gpxFilenameTextBox";
            this.gpxFilenameTextBox.ReadOnly = true;
            this.gpxFilenameTextBox.Size = new System.Drawing.Size(264, 20);
            this.gpxFilenameTextBox.TabIndex = 2;
            this.gpxFilenameTextBox.Text = "C:\\Users\\mike\\Documents\\waypoints.gpx";
            // 
            // targetFolderLabel
            // 
            this.targetFolderLabel.AutoSize = true;
            this.targetFolderLabel.Location = new System.Drawing.Point(22, 78);
            this.targetFolderLabel.Name = "targetFolderLabel";
            this.targetFolderLabel.Size = new System.Drawing.Size(41, 13);
            this.targetFolderLabel.TabIndex = 3;
            this.targetFolderLabel.Text = "Target:";
            // 
            // processButton
            // 
            this.processButton.Location = new System.Drawing.Point(70, 113);
            this.processButton.Name = "processButton";
            this.processButton.Size = new System.Drawing.Size(476, 23);
            this.processButton.TabIndex = 5;
            this.processButton.Text = "Process";
            this.processButton.UseVisualStyleBackColor = true;
            this.processButton.Click += new System.EventHandler(this.Button1_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(70, 151);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(476, 23);
            this.progressBar.TabIndex = 6;
            // 
            // buildDatabaseBackgroundWorker
            // 
            this.buildDatabaseBackgroundWorker.WorkerReportsProgress = true;
            this.buildDatabaseBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker1_DoWork);
            this.buildDatabaseBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorker1_ProgressChanged);
            this.buildDatabaseBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker1_RunWorkerCompleted);
            // 
            // targetDriveComboBox
            // 
            this.targetDriveComboBox.FormattingEnabled = true;
            this.targetDriveComboBox.Location = new System.Drawing.Point(70, 75);
            this.targetDriveComboBox.Name = "targetDriveComboBox";
            this.targetDriveComboBox.Size = new System.Drawing.Size(121, 21);
            this.targetDriveComboBox.TabIndex = 7;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(570, 195);
            this.Controls.Add(this.targetDriveComboBox);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.processButton);
            this.Controls.Add(this.targetFolderLabel);
            this.Controls.Add(this.gpxFilenameTextBox);
            this.Controls.Add(this.gpxFileLabel);
            this.Controls.Add(this.mainMenuStrip);
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "MainForm";
            this.Text = "Audi POI Builder";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openSourceFileDialog;
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectSourceGpxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Label gpxFileLabel;
        private System.Windows.Forms.TextBox gpxFilenameTextBox;
        private System.Windows.Forms.ToolStripMenuItem selectTargetFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.Label targetFolderLabel;
        private System.Windows.Forms.Button processButton;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.ComponentModel.BackgroundWorker buildDatabaseBackgroundWorker;
        private System.Windows.Forms.ComboBox targetDriveComboBox;
    }
}


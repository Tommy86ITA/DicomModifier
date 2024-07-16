//using DicomModifier.Models;
//using System.Text.Json;

using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;
using System.Xml.Linq;

namespace DicomModifier
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            groupSelectFiles = new GroupBox();
            buttonFolder = new Button();
            buttonDicomDir = new Button();
            buttonDicomFile = new Button();
            dataGridView1 = new DataGridView();
            PatientNameColumn = new DataGridViewTextBoxColumn();
            PatientDOBColumn = new DataGridViewTextBoxColumn();
            PatientIDColumn = new DataGridViewTextBoxColumn();
            StudyDescriptionColumn = new DataGridViewTextBoxColumn();
            StudyDateColumn = new DataGridViewTextBoxColumn();
            ModalityColumn = new DataGridViewTextBoxColumn();
            SeriesCountColumn = new DataGridViewTextBoxColumn();
            ImageCountColumn = new DataGridViewTextBoxColumn();
            StudyInstanceUIDColumn = new DataGridViewTextBoxColumn();
            FilePathColumn = new DataGridViewTextBoxColumn();
            groupBoxPatientID = new GroupBox();
            buttonUpdateID = new Button();
            textBoxNewID = new TextBox();
            buttonSend = new Button();
            groupBox1 = new GroupBox();
            statusStrip = new StatusStrip();
            toolStripStatusLabel = new ToolStripStatusLabel();
            toolStripStatusLabelFileCount = new ToolStripStatusLabel();
            toolStripProgressBar = new ToolStripProgressBar();
            toolStripStatusLabelDev = new ToolStripStatusLabel();
            toolStripDropDownButton = new ToolStripDropDownButton();
            esciToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            buttonResetQueue = new Button();
            groupSelectFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            groupBoxPatientID.SuspendLayout();
            groupBox1.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // groupSelectFiles
            // 
            groupSelectFiles.Controls.Add(buttonFolder);
            groupSelectFiles.Controls.Add(buttonDicomDir);
            groupSelectFiles.Controls.Add(buttonDicomFile);
            groupSelectFiles.Location = new Point(440, 9);
            groupSelectFiles.Margin = new Padding(2, 1, 2, 1);
            groupSelectFiles.Name = "groupSelectFiles";
            groupSelectFiles.Padding = new Padding(2, 1, 2, 1);
            groupSelectFiles.Size = new Size(500, 71);
            groupSelectFiles.TabIndex = 0;
            groupSelectFiles.TabStop = false;
            groupSelectFiles.Text = "Selezione file";
            // 
            // buttonFolder
            // 
            buttonFolder.BackColor = SystemColors.Highlight;
            buttonFolder.FlatStyle = FlatStyle.Flat;
            buttonFolder.Font = new System.Drawing.Font("Segoe UI", 10F);
            buttonFolder.ForeColor = Color.White;
            buttonFolder.Location = new Point(333, 26);
            buttonFolder.Margin = new Padding(2, 1, 2, 1);
            buttonFolder.Name = "buttonFolder";
            buttonFolder.Size = new Size(129, 31);
            buttonFolder.TabIndex = 2;
            buttonFolder.Text = "Apri cartella";
            buttonFolder.UseVisualStyleBackColor = false;
            // 
            // buttonDicomDir
            // 
            buttonDicomDir.BackColor = SystemColors.Highlight;
            buttonDicomDir.FlatStyle = FlatStyle.Flat;
            buttonDicomDir.Font = new System.Drawing.Font("Segoe UI", 10F);
            buttonDicomDir.ForeColor = Color.White;
            buttonDicomDir.Location = new Point(180, 26);
            buttonDicomDir.Margin = new Padding(2, 1, 2, 1);
            buttonDicomDir.Name = "buttonDicomDir";
            buttonDicomDir.Size = new Size(129, 31);
            buttonDicomDir.TabIndex = 1;
            buttonDicomDir.Text = "Apri DICOMDIR";
            buttonDicomDir.UseVisualStyleBackColor = false;
            // 
            // buttonDicomFile
            // 
            buttonDicomFile.BackColor = SystemColors.Highlight;
            buttonDicomFile.FlatStyle = FlatStyle.Flat;
            buttonDicomFile.Font = new System.Drawing.Font("Segoe UI", 10F);
            buttonDicomFile.ForeColor = Color.White;
            buttonDicomFile.Location = new Point(28, 26);
            buttonDicomFile.Margin = new Padding(2, 1, 2, 1);
            buttonDicomFile.Name = "buttonDicomFile";
            buttonDicomFile.Size = new Size(129, 31);
            buttonDicomFile.TabIndex = 0;
            buttonDicomFile.Text = "Apri file DICOM";
            buttonDicomFile.UseVisualStyleBackColor = false;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.BorderStyle = BorderStyle.Fixed3D;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { PatientNameColumn, PatientDOBColumn, PatientIDColumn, StudyDescriptionColumn, StudyDateColumn, ModalityColumn, SeriesCountColumn, ImageCountColumn, StudyInstanceUIDColumn, FilePathColumn });
            dataGridView1.Location = new Point(18, 108);
            dataGridView1.Margin = new Padding(2, 1, 2, 1);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ShowEditingIcon = false;
            dataGridView1.Size = new Size(1344, 146);
            dataGridView1.TabIndex = 2;
            // 
            // PatientNameColumn
            // 
            PatientNameColumn.HeaderText = "Cognome e nome";
            PatientNameColumn.MinimumWidth = 350;
            PatientNameColumn.Name = "PatientNameColumn";
            PatientNameColumn.ReadOnly = true;
            PatientNameColumn.Width = 450;
            // 
            // PatientDOBColumn
            // 
            PatientDOBColumn.HeaderText = "Data nascita";
            PatientDOBColumn.MinimumWidth = 100;
            PatientDOBColumn.Name = "PatientDOBColumn";
            PatientDOBColumn.ReadOnly = true;
            // 
            // PatientIDColumn
            // 
            PatientIDColumn.HeaderText = "ID Paziente";
            PatientIDColumn.MinimumWidth = 100;
            PatientIDColumn.Name = "PatientIDColumn";
            PatientIDColumn.ReadOnly = true;
            PatientIDColumn.Width = 150;
            // 
            // StudyDescriptionColumn
            // 
            StudyDescriptionColumn.HeaderText = "Descrizione studio";
            StudyDescriptionColumn.MinimumWidth = 150;
            StudyDescriptionColumn.Name = "StudyDescriptionColumn";
            StudyDescriptionColumn.ReadOnly = true;
            StudyDescriptionColumn.Width = 275;
            // 
            // StudyDateColumn
            // 
            StudyDateColumn.HeaderText = "Data esame";
            StudyDateColumn.MinimumWidth = 100;
            StudyDateColumn.Name = "StudyDateColumn";
            StudyDateColumn.ReadOnly = true;
            // 
            // ModalityColumn
            // 
            ModalityColumn.HeaderText = "Modalità";
            ModalityColumn.MinimumWidth = 75;
            ModalityColumn.Name = "ModalityColumn";
            ModalityColumn.ReadOnly = true;
            ModalityColumn.Width = 75;
            // 
            // SeriesCountColumn
            // 
            SeriesCountColumn.HeaderText = "Serie";
            SeriesCountColumn.MinimumWidth = 75;
            SeriesCountColumn.Name = "SeriesCountColumn";
            SeriesCountColumn.ReadOnly = true;
            SeriesCountColumn.Width = 75;
            // 
            // ImageCountColumn
            // 
            ImageCountColumn.HeaderText = "Immagini";
            ImageCountColumn.MinimumWidth = 75;
            ImageCountColumn.Name = "ImageCountColumn";
            ImageCountColumn.ReadOnly = true;
            ImageCountColumn.Width = 75;
            // 
            // StudyInstanceUIDColumn
            // 
            StudyInstanceUIDColumn.HeaderText = "StudyInstanceUID ";
            StudyInstanceUIDColumn.MinimumWidth = 2;
            StudyInstanceUIDColumn.Name = "StudyInstanceUIDColumn";
            StudyInstanceUIDColumn.ReadOnly = true;
            StudyInstanceUIDColumn.Visible = false;
            StudyInstanceUIDColumn.Width = 2;
            // 
            // FilePathColumn
            // 
            FilePathColumn.HeaderText = "FilePath";
            FilePathColumn.MinimumWidth = 2;
            FilePathColumn.Name = "FilePathColumn";
            FilePathColumn.ReadOnly = true;
            FilePathColumn.Visible = false;
            FilePathColumn.Width = 2;
            // 
            // groupBoxPatientID
            // 
            groupBoxPatientID.Controls.Add(buttonUpdateID);
            groupBoxPatientID.Controls.Add(textBoxNewID);
            groupBoxPatientID.Location = new Point(300, 307);
            groupBoxPatientID.Margin = new Padding(2, 1, 2, 1);
            groupBoxPatientID.Name = "groupBoxPatientID";
            groupBoxPatientID.Padding = new Padding(2, 1, 2, 1);
            groupBoxPatientID.Size = new Size(310, 71);
            groupBoxPatientID.TabIndex = 4;
            groupBoxPatientID.TabStop = false;
            groupBoxPatientID.Text = "Nuovo ID Paziente (opzionale)";
            // 
            // buttonUpdateID
            // 
            buttonUpdateID.BackColor = SystemColors.Highlight;
            buttonUpdateID.FlatStyle = FlatStyle.Flat;
            buttonUpdateID.Font = new System.Drawing.Font("Segoe UI", 10F);
            buttonUpdateID.ForeColor = Color.White;
            buttonUpdateID.Location = new Point(172, 30);
            buttonUpdateID.Margin = new Padding(2, 1, 2, 1);
            buttonUpdateID.Name = "buttonUpdateID";
            buttonUpdateID.Size = new Size(129, 31);
            buttonUpdateID.TabIndex = 1;
            buttonUpdateID.Text = "Modifica ID";
            buttonUpdateID.UseVisualStyleBackColor = false;
            // 
            // textBoxNewID
            // 
            textBoxNewID.Location = new Point(10, 35);
            textBoxNewID.Margin = new Padding(2, 1, 2, 1);
            textBoxNewID.Name = "textBoxNewID";
            textBoxNewID.PlaceholderText = "Inserire qui il nuovo ID";
            textBoxNewID.ShortcutsEnabled = false;
            textBoxNewID.Size = new Size(135, 23);
            textBoxNewID.TabIndex = 0;
            textBoxNewID.WordWrap = false;
            // 
            // buttonSend
            // 
            buttonSend.BackColor = SystemColors.Highlight;
            buttonSend.FlatStyle = FlatStyle.Flat;
            buttonSend.Font = new System.Drawing.Font("Segoe UI", 10F);
            buttonSend.ForeColor = Color.White;
            buttonSend.Location = new Point(90, 29);
            buttonSend.Margin = new Padding(2, 1, 2, 1);
            buttonSend.Name = "buttonSend";
            buttonSend.Size = new Size(129, 31);
            buttonSend.TabIndex = 0;
            buttonSend.Text = "Invia al PACS";
            buttonSend.UseVisualStyleBackColor = false;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(buttonSend);
            groupBox1.Location = new Point(771, 307);
            groupBox1.Margin = new Padding(2, 1, 2, 1);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(2, 1, 2, 1);
            groupBox1.Size = new Size(310, 71);
            groupBox1.TabIndex = 5;
            groupBox1.TabStop = false;
            groupBox1.Text = "Invio";
            // 
            // statusStrip
            // 
            statusStrip.ImageScalingSize = new Size(32, 32);
            statusStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel, toolStripStatusLabelFileCount, toolStripProgressBar, toolStripStatusLabelDev, toolStripDropDownButton });
            statusStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            statusStrip.Location = new Point(0, 419);
            statusStrip.Name = "statusStrip";
            statusStrip.Padding = new Padding(1, 0, 8, 0);
            statusStrip.Size = new Size(1376, 38);
            statusStrip.SizingGrip = false;
            statusStrip.TabIndex = 6;
            statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.AutoSize = false;
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new Size(300, 32);
            toolStripStatusLabel.Text = "Stato:";
            toolStripStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabelFileCount
            // 
            toolStripStatusLabelFileCount.AutoSize = false;
            toolStripStatusLabelFileCount.Name = "toolStripStatusLabelFileCount";
            toolStripStatusLabelFileCount.Size = new Size(150, 32);
            toolStripStatusLabelFileCount.Text = "Attesa file";
            toolStripStatusLabelFileCount.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar
            // 
            toolStripProgressBar.AutoSize = false;
            toolStripProgressBar.Name = "toolStripProgressBar";
            toolStripProgressBar.Padding = new Padding(0, 2, 0, 0);
            toolStripProgressBar.Size = new Size(500, 32);
            toolStripProgressBar.Style = ProgressBarStyle.Continuous;
            // 
            // toolStripStatusLabelDev
            // 
            toolStripStatusLabelDev.AutoSize = false;
            toolStripStatusLabelDev.Name = "toolStripStatusLabelDev";
            toolStripStatusLabelDev.Size = new Size(350, 32);
            // 
            // toolStripDropDownButton
            // 
            toolStripDropDownButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripDropDownButton.DropDownItems.AddRange(new ToolStripItem[] { esciToolStripMenuItem, helpToolStripMenuItem, aboutToolStripMenuItem, settingsToolStripMenuItem });
            toolStripDropDownButton.Image = Properties.Resources.settings_icon;
            toolStripDropDownButton.ImageTransparentColor = Color.Magenta;
            toolStripDropDownButton.Name = "toolStripDropDownButton";
            toolStripDropDownButton.Size = new Size(45, 36);
            toolStripDropDownButton.Text = "toolStripDropDownButton1";
            // 
            // esciToolStripMenuItem
            // 
            esciToolStripMenuItem.Name = "esciToolStripMenuItem";
            esciToolStripMenuItem.Size = new Size(151, 22);
            esciToolStripMenuItem.Text = "Esci";
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(151, 22);
            helpToolStripMenuItem.Text = "Help";
            helpToolStripMenuItem.Click += HelpToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(151, 22);
            aboutToolStripMenuItem.Text = "About...";
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(151, 22);
            settingsToolStripMenuItem.Text = "Impostazioni...";
            settingsToolStripMenuItem.ToolTipText = "Apri la finestra delle impostazioni.";
            // 
            // buttonResetQueue
            // 
            buttonResetQueue.BackColor = Color.DarkOrange;
            buttonResetQueue.FlatStyle = FlatStyle.Flat;
            buttonResetQueue.Font = new System.Drawing.Font("Segoe UI", 10F);
            buttonResetQueue.ForeColor = Color.White;
            buttonResetQueue.Location = new Point(1233, 268);
            buttonResetQueue.Margin = new Padding(2, 1, 2, 1);
            buttonResetQueue.Name = "buttonResetQueue";
            buttonResetQueue.Size = new Size(129, 31);
            buttonResetQueue.TabIndex = 7;
            buttonResetQueue.Text = "Pulisci";
            buttonResetQueue.UseVisualStyleBackColor = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(1376, 457);
            Controls.Add(buttonResetQueue);
            Controls.Add(statusStrip);
            Controls.Add(groupBox1);
            Controls.Add(groupBoxPatientID);
            Controls.Add(dataGridView1);
            Controls.Add(groupSelectFiles);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2, 1, 2, 1);
            MaximizeBox = false;
            Name = "MainForm";
            Text = "DICOM Import & Edit";
            groupSelectFiles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            groupBoxPatientID.ResumeLayout(false);
            groupBoxPatientID.PerformLayout();
            groupBox1.ResumeLayout(false);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public GroupBox groupSelectFiles;
        public Button buttonFolder;
        public Button buttonDicomDir;
        public Button buttonDicomFile;
        public DataGridView dataGridView1;
        public GroupBox groupBoxPatientID;
        public Button buttonSend;
        public GroupBox groupBox1;
        public TextBox textBoxNewID;
        public StatusStrip statusStrip;
        public ToolStripStatusLabel toolStripStatusLabel;
        public ToolStripStatusLabel toolStripStatusLabelFileCount;
        public ToolStripProgressBar toolStripProgressBar;
        public ToolStripDropDownButton toolStripDropDownButton;
        public ToolStripMenuItem esciToolStripMenuItem;
        public ToolStripMenuItem aboutToolStripMenuItem;
        public ToolStripMenuItem settingsToolStripMenuItem;
        public ToolStripStatusLabel toolStripStatusLabelDev;
        public Button buttonResetQueue;
        public Button buttonUpdateID;
        private DataGridViewTextBoxColumn PatientNameColumn;
        private DataGridViewTextBoxColumn PatientDOBColumn;
        private DataGridViewTextBoxColumn PatientIDColumn;
        private DataGridViewTextBoxColumn StudyDescriptionColumn;
        private DataGridViewTextBoxColumn StudyDateColumn;
        private DataGridViewTextBoxColumn ModalityColumn;
        private DataGridViewTextBoxColumn SeriesCountColumn;
        private DataGridViewTextBoxColumn ImageCountColumn;
        private DataGridViewTextBoxColumn StudyInstanceUIDColumn;
        private DataGridViewTextBoxColumn FilePathColumn;
        private ToolStripMenuItem helpToolStripMenuItem;
    }
}
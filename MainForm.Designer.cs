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
            groupSelectFiles = new GroupBox();
            buttonFolder = new Button();
            buttonDicomDir = new Button();
            buttonDicomFile = new Button();
            dataGridView1 = new DataGridView();
            groupBoxPatientID = new GroupBox();
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
            aboutToolStripMenuItem = new ToolStripMenuItem();
            impostazioniToolStripMenuItem = new ToolStripMenuItem();
            NameColumn = new DataGridViewTextBoxColumn();
            DOBColumn = new DataGridViewTextBoxColumn();
            IDColumn = new DataGridViewTextBoxColumn();
            StudyDescriptionColumn = new DataGridViewTextBoxColumn();
            StudyDateColumn = new DataGridViewTextBoxColumn();
            ModalityColumn = new DataGridViewTextBoxColumn();
            SeriesCountColumn = new DataGridViewTextBoxColumn();
            ImageCountColumn = new DataGridViewTextBoxColumn();
            StudyInstanceUIDColumn = new DataGridViewTextBoxColumn();
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
            buttonFolder.Location = new Point(333, 26);
            buttonFolder.Margin = new Padding(2, 1, 2, 1);
            buttonFolder.Name = "buttonFolder";
            buttonFolder.Size = new Size(129, 31);
            buttonFolder.TabIndex = 2;
            buttonFolder.Text = "Apri cartella";
            buttonFolder.UseVisualStyleBackColor = true;
            // 
            // buttonDicomDir
            // 
            buttonDicomDir.Location = new Point(180, 26);
            buttonDicomDir.Margin = new Padding(2, 1, 2, 1);
            buttonDicomDir.Name = "buttonDicomDir";
            buttonDicomDir.Size = new Size(129, 31);
            buttonDicomDir.TabIndex = 1;
            buttonDicomDir.Text = "Apri file DICOMDIR";
            buttonDicomDir.UseVisualStyleBackColor = true;
            // 
            // buttonDicomFile
            // 
            buttonDicomFile.Location = new Point(28, 26);
            buttonDicomFile.Margin = new Padding(2, 1, 2, 1);
            buttonDicomFile.Name = "buttonDicomFile";
            buttonDicomFile.Size = new Size(129, 31);
            buttonDicomFile.TabIndex = 0;
            buttonDicomFile.Text = "Apri file DICOM";
            buttonDicomFile.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { NameColumn, DOBColumn, IDColumn, StudyDescriptionColumn, StudyDateColumn, ModalityColumn, SeriesCountColumn, ImageCountColumn, StudyInstanceUIDColumn });
            dataGridView1.Location = new Point(18, 108);
            dataGridView1.Margin = new Padding(2, 1, 2, 1);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(1344, 146);
            dataGridView1.TabIndex = 2;
            // 
            // groupBoxPatientID
            // 
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
            // textBoxNewID
            // 
            textBoxNewID.Location = new Point(53, 35);
            textBoxNewID.Margin = new Padding(2, 1, 2, 1);
            textBoxNewID.Name = "textBoxNewID";
            textBoxNewID.Size = new Size(205, 23);
            textBoxNewID.TabIndex = 0;
            // 
            // buttonSend
            // 
            buttonSend.Location = new Point(90, 29);
            buttonSend.Margin = new Padding(2, 1, 2, 1);
            buttonSend.Name = "buttonSend";
            buttonSend.Size = new Size(129, 31);
            buttonSend.TabIndex = 0;
            buttonSend.Text = "Invia al PACS";
            buttonSend.UseVisualStyleBackColor = true;
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
            statusStrip.Location = new Point(0, 415);
            statusStrip.Name = "statusStrip";
            statusStrip.Padding = new Padding(1, 0, 8, 0);
            statusStrip.Size = new Size(1380, 42);
            statusStrip.SizingGrip = false;
            statusStrip.TabIndex = 6;
            statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.AutoSize = false;
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new Size(300, 37);
            toolStripStatusLabel.Text = "Stato:";
            toolStripStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabelFileCount
            // 
            toolStripStatusLabelFileCount.AutoSize = false;
            toolStripStatusLabelFileCount.Name = "toolStripStatusLabelFileCount";
            toolStripStatusLabelFileCount.Size = new Size(150, 37);
            toolStripStatusLabelFileCount.Text = "File inviati:";
            toolStripStatusLabelFileCount.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar
            // 
            toolStripProgressBar.Name = "toolStripProgressBar";
            toolStripProgressBar.Size = new Size(500, 36);
            // 
            // toolStripStatusLabelDev
            // 
            toolStripStatusLabelDev.AutoSize = false;
            toolStripStatusLabelDev.Name = "toolStripStatusLabelDev";
            toolStripStatusLabelDev.Size = new Size(350, 37);
            toolStripStatusLabelDev.Text = "     Developed by Thomas Amaranto - 2024 || SW vers. 1.0    ";
            // 
            // toolStripDropDownButton
            // 
            toolStripDropDownButton.AutoSize = false;
            toolStripDropDownButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripDropDownButton.DropDownItems.AddRange(new ToolStripItem[] { esciToolStripMenuItem, aboutToolStripMenuItem, impostazioniToolStripMenuItem });
            toolStripDropDownButton.Image = Properties.Resources.settings_icon;
            toolStripDropDownButton.ImageTransparentColor = Color.Magenta;
            toolStripDropDownButton.Name = "toolStripDropDownButton";
            toolStripDropDownButton.Size = new Size(54, 40);
            toolStripDropDownButton.Text = "toolStripDropDownButton1";
            // 
            // esciToolStripMenuItem
            // 
            esciToolStripMenuItem.Name = "esciToolStripMenuItem";
            esciToolStripMenuItem.Size = new Size(151, 22);
            esciToolStripMenuItem.Text = "Esci";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(151, 22);
            aboutToolStripMenuItem.Text = "About...";
            // 
            // impostazioniToolStripMenuItem
            // 
            impostazioniToolStripMenuItem.Name = "impostazioniToolStripMenuItem";
            impostazioniToolStripMenuItem.Size = new Size(151, 22);
            impostazioniToolStripMenuItem.Text = "Impostazioni...";
            // 
            // NameColumn
            // 
            NameColumn.HeaderText = "Cognome e nome";
            NameColumn.MinimumWidth = 350;
            NameColumn.Name = "PatientNameColumn";
            NameColumn.ReadOnly = true;
            NameColumn.Width = 450;
            // 
            // DOBColumn
            // 
            DOBColumn.HeaderText = "Data nascita";
            DOBColumn.MinimumWidth = 100;
            DOBColumn.Name = "PatientDOBColumn";
            DOBColumn.ReadOnly = true;
            // 
            // IDColumn
            // 
            IDColumn.HeaderText = "ID Paziente";
            IDColumn.MinimumWidth = 100;
            IDColumn.Name = "PatientIDColumn";
            IDColumn.ReadOnly = true;
            // 
            // StudyDescriptionColumn
            // 
            StudyDescriptionColumn.HeaderText = "Descrizione studio";
            StudyDescriptionColumn.MinimumWidth = 150;
            StudyDescriptionColumn.Name = "StudyDescriptionColumn";
            StudyDescriptionColumn.ReadOnly = true;
            StudyDescriptionColumn.Width = 300;
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
            ModalityColumn.MinimumWidth = 100;
            ModalityColumn.Name = "ModalityColumn";
            ModalityColumn.ReadOnly = true;
            // 
            // SeriesNumberColumn
            // 
            SeriesCountColumn.HeaderText = "Serie";
            SeriesCountColumn.MinimumWidth = 75;
            SeriesCountColumn.Name = "SeriesCountColumn";
            SeriesCountColumn.ReadOnly = true;
            SeriesCountColumn.Width = 75;
            // 
            // ImageNumberColumn
            // 
            ImageCountColumn.HeaderText = "Immagini";
            ImageCountColumn.MinimumWidth = 75;
            ImageCountColumn.Name = "ImageCountColumn";
            ImageCountColumn.ReadOnly = true;
            ImageCountColumn.Width = 75;
            // 
            // StudyInstanceUID
            // 
            StudyInstanceUIDColumn.HeaderText = "StudyInstanceUID ";
            StudyInstanceUIDColumn.Name = "StudyInstanceUIDColumn";
            StudyInstanceUIDColumn.ReadOnly = true;
            StudyInstanceUIDColumn.Visible = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(1380, 457);
            Controls.Add(statusStrip);
            Controls.Add(groupBox1);
            Controls.Add(groupBoxPatientID);
            Controls.Add(dataGridView1);
            Controls.Add(groupSelectFiles);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Margin = new Padding(2, 1, 2, 1);
            MaximizeBox = false;
            Name = "MainForm";
            Text = "DICOM Import";
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

        private GroupBox groupSelectFiles;
        private Button buttonDicomFile;
        private Button buttonFolder;
        private Button buttonDicomDir;
        public  DataGridView dataGridView1;
        private GroupBox groupBoxPatientID;
        private Button buttonSend;
        private GroupBox groupBox1;
        private TextBox textBoxNewID;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel toolStripStatusLabel;
        private ToolStripStatusLabel toolStripStatusLabelFileCount;
        private ToolStripProgressBar toolStripProgressBar;
        private ToolStripDropDownButton toolStripDropDownButton;
        private ToolStripMenuItem esciToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem impostazioniToolStripMenuItem;
        private ToolStripStatusLabel toolStripStatusLabelDev;
        private DataGridViewTextBoxColumn NameColumn;
        private DataGridViewTextBoxColumn DOBColumn;
        private DataGridViewTextBoxColumn IDColumn;
        private DataGridViewTextBoxColumn StudyDescriptionColumn;
        private DataGridViewTextBoxColumn StudyDateColumn;
        private DataGridViewTextBoxColumn ModalityColumn;
        private DataGridViewTextBoxColumn SeriesCountColumn;
        private DataGridViewTextBoxColumn ImageCountColumn;
        private DataGridViewTextBoxColumn StudyInstanceUIDColumn;
    }
}
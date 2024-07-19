//using DicomModifier.Models;
//using System.Text.Json;

namespace DicomModifier
{
    partial class SettingsForm
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            groupBox1 = new GroupBox();
            labelLocalAE = new Label();
            textBoxLocalAETitle = new TextBox();
            textBoxTimeout = new TextBox();
            labelServerTimeout = new Label();
            textBoxServerPort = new TextBox();
            labelServerPort = new Label();
            textBoxServerIP = new TextBox();
            labelServerIP = new Label();
            textBoxAETitle = new TextBox();
            labelServerAE = new Label();
            buttonEchoTest = new Button();
            buttonCancel = new Button();
            buttonSave = new Button();
            groupBoxLocalAE = new GroupBox();
            panelEchoStatus = new Panel();
            groupBox1.SuspendLayout();
            groupBoxLocalAE.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(labelLocalAE);
            groupBox1.Controls.Add(textBoxLocalAETitle);
            groupBox1.Controls.Add(textBoxTimeout);
            groupBox1.Controls.Add(labelServerTimeout);
            groupBox1.Controls.Add(textBoxServerPort);
            groupBox1.Controls.Add(labelServerPort);
            groupBox1.Controls.Add(textBoxServerIP);
            groupBox1.Controls.Add(labelServerIP);
            groupBox1.Controls.Add(textBoxAETitle);
            groupBox1.Controls.Add(labelServerAE);
            groupBox1.Location = new Point(12, 9);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(359, 240);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Parametri Server PACS";
            // 
            // labelLocalAE
            // 
            labelLocalAE.AutoSize = true;
            labelLocalAE.Location = new Point(39, 41);
            labelLocalAE.Name = "labelLocalAE";
            labelLocalAE.Size = new Size(83, 15);
            labelLocalAE.TabIndex = 8;
            labelLocalAE.Text = "AE Title Locale";
            // 
            // textBoxLocalAETitle
            // 
            textBoxLocalAETitle.Location = new Point(157, 38);
            textBoxLocalAETitle.Name = "textBoxLocalAETitle";
            textBoxLocalAETitle.PlaceholderText = "Inserire AE Title Locale";
            textBoxLocalAETitle.Size = new Size(163, 23);
            textBoxLocalAETitle.TabIndex = 0;
            textBoxLocalAETitle.WordWrap = false;
            // 
            // textBoxTimeout
            // 
            textBoxTimeout.Location = new Point(191, 190);
            textBoxTimeout.Name = "textBoxTimeout";
            textBoxTimeout.Size = new Size(129, 23);
            textBoxTimeout.TabIndex = 7;
            textBoxTimeout.KeyPress += TextBoxTimeout_KeyPress;
            // 
            // labelServerTimeout
            // 
            labelServerTimeout.AutoSize = true;
            labelServerTimeout.Location = new Point(39, 193);
            labelServerTimeout.Name = "labelServerTimeout";
            labelServerTimeout.Size = new Size(78, 15);
            labelServerTimeout.TabIndex = 6;
            labelServerTimeout.Text = "Timeout [ms]";
            // 
            // textBoxServerPort
            // 
            textBoxServerPort.Location = new Point(191, 152);
            textBoxServerPort.Name = "textBoxServerPort";
            textBoxServerPort.Size = new Size(129, 23);
            textBoxServerPort.TabIndex = 5;
            textBoxServerPort.TextChanged += TextBoxServerPort_TextChanged;
            textBoxServerPort.KeyPress += TextBoxServerPort_KeyPress;
            // 
            // labelServerPort
            // 
            labelServerPort.AutoSize = true;
            labelServerPort.Location = new Point(39, 155);
            labelServerPort.Name = "labelServerPort";
            labelServerPort.Size = new Size(35, 15);
            labelServerPort.TabIndex = 4;
            labelServerPort.Text = "Porta";
            // 
            // textBoxServerIP
            // 
            textBoxServerIP.Location = new Point(191, 114);
            textBoxServerIP.Name = "textBoxServerIP";
            textBoxServerIP.Size = new Size(129, 23);
            textBoxServerIP.TabIndex = 3;
            textBoxServerIP.KeyPress += TextBoxServerIP_KeyPress;
            // 
            // labelServerIP
            // 
            labelServerIP.AutoSize = true;
            labelServerIP.Location = new Point(39, 117);
            labelServerIP.Name = "labelServerIP";
            labelServerIP.Size = new Size(99, 15);
            labelServerIP.TabIndex = 2;
            labelServerIP.Text = "Indirizzo IP Server";
            // 
            // textBoxAETitle
            // 
            textBoxAETitle.Location = new Point(157, 76);
            textBoxAETitle.Name = "textBoxAETitle";
            textBoxAETitle.PlaceholderText = "Inserire AE Title Server PACS";
            textBoxAETitle.Size = new Size(163, 23);
            textBoxAETitle.TabIndex = 1;
            // 
            // labelServerAE
            // 
            labelServerAE.AutoSize = true;
            labelServerAE.Location = new Point(39, 79);
            labelServerAE.Name = "labelServerAE";
            labelServerAE.Size = new Size(81, 15);
            labelServerAE.TabIndex = 0;
            labelServerAE.Text = "AE Title Server";
            // 
            // buttonEchoTest
            // 
            buttonEchoTest.Location = new Point(39, 30);
            buttonEchoTest.Margin = new Padding(2, 1, 2, 1);
            buttonEchoTest.Name = "buttonEchoTest";
            buttonEchoTest.Size = new Size(129, 31);
            buttonEchoTest.TabIndex = 8;
            buttonEchoTest.Text = "Esegui C-ECHO";
            buttonEchoTest.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(203, 362);
            buttonCancel.Margin = new Padding(2, 1, 2, 1);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(129, 31);
            buttonCancel.TabIndex = 3;
            buttonCancel.Text = "Annulla";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            buttonSave.Location = new Point(51, 362);
            buttonSave.Margin = new Padding(2, 1, 2, 1);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(129, 31);
            buttonSave.TabIndex = 2;
            buttonSave.Text = "Salva";
            buttonSave.UseVisualStyleBackColor = true;
            // 
            // groupBoxLocalAE
            // 
            groupBoxLocalAE.Controls.Add(panelEchoStatus);
            groupBoxLocalAE.Controls.Add(buttonEchoTest);
            groupBoxLocalAE.Location = new Point(12, 255);
            groupBoxLocalAE.Name = "groupBoxLocalAE";
            groupBoxLocalAE.Size = new Size(359, 82);
            groupBoxLocalAE.TabIndex = 1;
            groupBoxLocalAE.TabStop = false;
            groupBoxLocalAE.Text = "Test C-ECHO";
            // 
            // panelEchoStatus
            // 
            panelEchoStatus.BackColor = Color.White;
            panelEchoStatus.BorderStyle = BorderStyle.FixedSingle;
            panelEchoStatus.Location = new Point(228, 37);
            panelEchoStatus.Margin = new Padding(2);
            panelEchoStatus.Name = "panelEchoStatus";
            panelEchoStatus.Size = new Size(56, 20);
            panelEchoStatus.TabIndex = 9;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(383, 413);
            Controls.Add(buttonCancel);
            Controls.Add(buttonSave);
            Controls.Add(groupBoxLocalAE);
            Controls.Add(groupBox1);
            HelpButton = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsForm";
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Impostazioni";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBoxLocalAE.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Button buttonCancel;
        private Button buttonSave;
        private Button buttonEchoTest;
        private TextBox textBoxTimeout;
        private Label labelServerTimeout;
        private TextBox textBoxServerPort;
        private Label labelServerPort;
        private TextBox textBoxServerIP;
        private Label labelServerIP;
        private TextBox textBoxAETitle;
        private Label labelServerAE;
        private TextBox textBoxLocalAETitle;
        private Label labelLocalAE;
        private GroupBox groupBoxLocalAE;
        private Panel panelEchoStatus;
    }
}
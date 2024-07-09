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
            groupBox1.Location = new Point(17, 15);
            groupBox1.Margin = new Padding(4, 5, 4, 5);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 5, 4, 5);
            groupBox1.Size = new Size(513, 400);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Parametri Server PACS";
            // 
            // labelLocalAE
            // 
            labelLocalAE.AutoSize = true;
            labelLocalAE.Location = new Point(56, 68);
            labelLocalAE.Margin = new Padding(4, 0, 4, 0);
            labelLocalAE.Name = "labelLocalAE";
            labelLocalAE.Size = new Size(124, 25);
            labelLocalAE.TabIndex = 8;
            labelLocalAE.Text = "AE Title Locale";
            // 
            // textBoxLocalAETitle
            // 
            textBoxLocalAETitle.Location = new Point(224, 63);
            textBoxLocalAETitle.Margin = new Padding(4, 5, 4, 5);
            textBoxLocalAETitle.Name = "textBoxLocalAETitle";
            textBoxLocalAETitle.Size = new Size(231, 31);
            textBoxLocalAETitle.TabIndex = 0;
            // 
            // textBoxTimeout
            // 
            textBoxTimeout.Location = new Point(273, 317);
            textBoxTimeout.Margin = new Padding(4, 5, 4, 5);
            textBoxTimeout.Name = "textBoxTimeout";
            textBoxTimeout.Size = new Size(183, 31);
            textBoxTimeout.TabIndex = 7;
            textBoxTimeout.KeyPress += TextBoxTimeout_KeyPress;
            // 
            // labelServerTimeout
            // 
            labelServerTimeout.AutoSize = true;
            labelServerTimeout.Location = new Point(56, 322);
            labelServerTimeout.Margin = new Padding(4, 0, 4, 0);
            labelServerTimeout.Name = "labelServerTimeout";
            labelServerTimeout.Size = new Size(116, 25);
            labelServerTimeout.TabIndex = 6;
            labelServerTimeout.Text = "Timeout [ms]";
            // 
            // textBoxServerPort
            // 
            textBoxServerPort.Location = new Point(273, 253);
            textBoxServerPort.Margin = new Padding(4, 5, 4, 5);
            textBoxServerPort.Name = "textBoxServerPort";
            textBoxServerPort.Size = new Size(183, 31);
            textBoxServerPort.TabIndex = 5;
            textBoxServerPort.TextChanged += TextBoxServerPort_TextChanged;
            textBoxServerPort.KeyPress += TextBoxServerPort_KeyPress;
            // 
            // labelServerPort
            // 
            labelServerPort.AutoSize = true;
            labelServerPort.Location = new Point(56, 258);
            labelServerPort.Margin = new Padding(4, 0, 4, 0);
            labelServerPort.Name = "labelServerPort";
            labelServerPort.Size = new Size(53, 25);
            labelServerPort.TabIndex = 4;
            labelServerPort.Text = "Porta";
            // 
            // textBoxServerIP
            // 
            textBoxServerIP.Location = new Point(273, 190);
            textBoxServerIP.Margin = new Padding(4, 5, 4, 5);
            textBoxServerIP.Name = "textBoxServerIP";
            textBoxServerIP.Size = new Size(183, 31);
            textBoxServerIP.TabIndex = 3;
            textBoxServerIP.KeyPress += TextBoxServerIP_KeyPress;
            // 
            // labelServerIP
            // 
            labelServerIP.AutoSize = true;
            labelServerIP.Location = new Point(56, 195);
            labelServerIP.Margin = new Padding(4, 0, 4, 0);
            labelServerIP.Name = "labelServerIP";
            labelServerIP.Size = new Size(153, 25);
            labelServerIP.TabIndex = 2;
            labelServerIP.Text = "Indirizzo IP Server";
            // 
            // textBoxAETitle
            // 
            textBoxAETitle.Location = new Point(224, 127);
            textBoxAETitle.Margin = new Padding(4, 5, 4, 5);
            textBoxAETitle.Name = "textBoxAETitle";
            textBoxAETitle.Size = new Size(231, 31);
            textBoxAETitle.TabIndex = 1;
            // 
            // labelServerAE
            // 
            labelServerAE.AutoSize = true;
            labelServerAE.Location = new Point(56, 132);
            labelServerAE.Margin = new Padding(4, 0, 4, 0);
            labelServerAE.Name = "labelServerAE";
            labelServerAE.Size = new Size(124, 25);
            labelServerAE.TabIndex = 0;
            labelServerAE.Text = "AE Title Server";
            // 
            // buttonEchoTest
            // 
            buttonEchoTest.Location = new Point(56, 50);
            buttonEchoTest.Margin = new Padding(3, 2, 3, 2);
            buttonEchoTest.Name = "buttonEchoTest";
            buttonEchoTest.Size = new Size(184, 52);
            buttonEchoTest.TabIndex = 8;
            buttonEchoTest.Text = "Esegui C-ECHO";
            buttonEchoTest.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(290, 603);
            buttonCancel.Margin = new Padding(3, 2, 3, 2);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(184, 52);
            buttonCancel.TabIndex = 3;
            buttonCancel.Text = "Annulla";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            buttonSave.Location = new Point(73, 603);
            buttonSave.Margin = new Padding(3, 2, 3, 2);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(184, 52);
            buttonSave.TabIndex = 2;
            buttonSave.Text = "Salva";
            buttonSave.UseVisualStyleBackColor = true;
            // 
            // groupBoxLocalAE
            // 
            groupBoxLocalAE.Controls.Add(panelEchoStatus);
            groupBoxLocalAE.Controls.Add(buttonEchoTest);
            groupBoxLocalAE.Location = new Point(17, 425);
            groupBoxLocalAE.Margin = new Padding(4, 5, 4, 5);
            groupBoxLocalAE.Name = "groupBoxLocalAE";
            groupBoxLocalAE.Padding = new Padding(4, 5, 4, 5);
            groupBoxLocalAE.Size = new Size(513, 137);
            groupBoxLocalAE.TabIndex = 1;
            groupBoxLocalAE.TabStop = false;
            groupBoxLocalAE.Text = "AE Title locale";
            // 
            // panelEchoStatus
            // 
            panelEchoStatus.BackColor = Color.White;
            panelEchoStatus.BorderStyle = BorderStyle.FixedSingle;
            panelEchoStatus.Location = new Point(325, 61);
            panelEchoStatus.Name = "panelEchoStatus";
            panelEchoStatus.Size = new Size(79, 32);
            panelEchoStatus.TabIndex = 9;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(547, 688);
            Controls.Add(buttonCancel);
            Controls.Add(buttonSave);
            Controls.Add(groupBoxLocalAE);
            Controls.Add(groupBox1);
            Margin = new Padding(4, 5, 4, 5);
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
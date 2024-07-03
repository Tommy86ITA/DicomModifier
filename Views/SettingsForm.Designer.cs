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
            buttonEchoTest = new Button();
            textBoxTimeout = new TextBox();
            labelServerTimeout = new Label();
            textBoxServerPort = new TextBox();
            labelServerPort = new Label();
            textBoxServerIP = new TextBox();
            labelServerIP = new Label();
            textBoxAETitle = new TextBox();
            labelServerAE = new Label();
            groupBoxLocalAE = new GroupBox();
            textBoxLocalAETitle = new TextBox();
            buttonCancel = new Button();
            buttonSave = new Button();
            groupBox1.SuspendLayout();
            groupBoxLocalAE.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(buttonEchoTest);
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
            groupBox1.Size = new Size(359, 281);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Parametri Server PACS";
            // 
            // buttonEchoTest
            // 
            buttonEchoTest.Location = new Point(115, 233);
            buttonEchoTest.Margin = new Padding(2, 1, 2, 1);
            buttonEchoTest.Name = "buttonEchoTest";
            buttonEchoTest.Size = new Size(129, 31);
            buttonEchoTest.TabIndex = 8;
            buttonEchoTest.Text = "Esegui C-ECHO";
            buttonEchoTest.UseVisualStyleBackColor = true;
            // 
            // textBoxTimeout
            // 
            textBoxTimeout.Location = new Point(157, 179);
            textBoxTimeout.Name = "textBoxTimeout";
            textBoxTimeout.Size = new Size(163, 23);
            textBoxTimeout.TabIndex = 7;
            // 
            // labelServerTimeout
            // 
            labelServerTimeout.AutoSize = true;
            labelServerTimeout.Location = new Point(31, 183);
            labelServerTimeout.Name = "labelServerTimeout";
            labelServerTimeout.Size = new Size(51, 15);
            labelServerTimeout.TabIndex = 6;
            labelServerTimeout.Text = "Timeout";
            // 
            // textBoxServerPort
            // 
            textBoxServerPort.Location = new Point(157, 137);
            textBoxServerPort.Name = "textBoxServerPort";
            textBoxServerPort.Size = new Size(163, 23);
            textBoxServerPort.TabIndex = 5;
            // 
            // labelServerPort
            // 
            labelServerPort.AutoSize = true;
            labelServerPort.Location = new Point(31, 141);
            labelServerPort.Name = "labelServerPort";
            labelServerPort.Size = new Size(35, 15);
            labelServerPort.TabIndex = 4;
            labelServerPort.Text = "Porta";
            // 
            // textBoxServerIP
            // 
            textBoxServerIP.Location = new Point(157, 95);
            textBoxServerIP.Name = "textBoxServerIP";
            textBoxServerIP.Size = new Size(163, 23);
            textBoxServerIP.TabIndex = 3;
            // 
            // labelServerIP
            // 
            labelServerIP.AutoSize = true;
            labelServerIP.Location = new Point(31, 99);
            labelServerIP.Name = "labelServerIP";
            labelServerIP.Size = new Size(99, 15);
            labelServerIP.TabIndex = 2;
            labelServerIP.Text = "Indirizzo IP Server";
            // 
            // textBoxAETitle
            // 
            textBoxAETitle.Location = new Point(157, 49);
            textBoxAETitle.Name = "textBoxAETitle";
            textBoxAETitle.Size = new Size(163, 23);
            textBoxAETitle.TabIndex = 1;
            // 
            // labelServerAE
            // 
            labelServerAE.AutoSize = true;
            labelServerAE.Location = new Point(31, 53);
            labelServerAE.Name = "labelServerAE";
            labelServerAE.Size = new Size(46, 15);
            labelServerAE.TabIndex = 0;
            labelServerAE.Text = "AE Title";
            // 
            // groupBoxLocalAE
            // 
            groupBoxLocalAE.Controls.Add(textBoxLocalAETitle);
            groupBoxLocalAE.Location = new Point(12, 305);
            groupBoxLocalAE.Name = "groupBoxLocalAE";
            groupBoxLocalAE.Size = new Size(359, 82);
            groupBoxLocalAE.TabIndex = 1;
            groupBoxLocalAE.TabStop = false;
            groupBoxLocalAE.Text = "AE Title locale";
            // 
            // textBoxLocalAETitle
            // 
            textBoxLocalAETitle.Location = new Point(39, 37);
            textBoxLocalAETitle.Name = "textBoxLocalAETitle";
            textBoxLocalAETitle.Size = new Size(281, 23);
            textBoxLocalAETitle.TabIndex = 0;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(203, 409);
            buttonCancel.Margin = new Padding(2, 1, 2, 1);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(129, 31);
            buttonCancel.TabIndex = 3;
            buttonCancel.Text = "Annulla";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            buttonSave.Location = new Point(51, 409);
            buttonSave.Margin = new Padding(2, 1, 2, 1);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(129, 31);
            buttonSave.TabIndex = 2;
            buttonSave.Text = "Salva";
            buttonSave.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(383, 450);
            Controls.Add(buttonCancel);
            Controls.Add(buttonSave);
            Controls.Add(groupBoxLocalAE);
            Controls.Add(groupBox1);
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
            groupBoxLocalAE.PerformLayout();
            ResumeLayout(false);

            this.textBoxServerPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxServerPort_KeyPress);
            this.textBoxTimeout.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxTimeout_KeyPress);
            this.textBoxServerIP.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxServerIP_KeyPress);
            this.textBoxServerPort.TextChanged += new System.EventHandler(this.TextBoxServerPort_TextChanged);

        }

        #endregion

        private GroupBox groupBox1;
        private GroupBox groupBoxLocalAE;
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

    }
}
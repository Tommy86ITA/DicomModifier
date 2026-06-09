// Interfaces/LoginForm.Designer.cs

namespace DicomModifier.Views
{
    partial class LoginForm
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
            pictureBox1 = new PictureBox();
            labelAppName = new Label();
            labelUsername = new Label();
            groupBox1 = new GroupBox();
            textBoxPassword = new TextBox();
            textBoxUsername = new TextBox();
            labelPassword = new Label();
            buttonQuit = new Button();
            buttonLogin = new Button();
            labelVersion = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Enabled = false;
            pictureBox1.Image = Properties.Resources.DICOMImp_Logo;
            pictureBox1.Location = new Point(22, 14);
            pictureBox1.Margin = new Padding(2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(245, 245);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // labelAppName
            // 
            labelAppName.AutoSize = true;
            labelAppName.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            labelAppName.Location = new Point(383, 14);
            labelAppName.Margin = new Padding(2, 0, 2, 0);
            labelAppName.Name = "labelAppName";
            labelAppName.Size = new Size(234, 32);
            labelAppName.TabIndex = 1;
            labelAppName.Text = "DICOM Import Edit";
            labelAppName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelUsername
            // 
            labelUsername.AutoSize = true;
            labelUsername.Location = new Point(17, 28);
            labelUsername.Margin = new Padding(2, 0, 2, 0);
            labelUsername.Name = "labelUsername";
            labelUsername.Size = new Size(78, 15);
            labelUsername.TabIndex = 2;
            labelUsername.Text = "Nome Utente";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(textBoxPassword);
            groupBox1.Controls.Add(textBoxUsername);
            groupBox1.Controls.Add(labelPassword);
            groupBox1.Controls.Add(labelUsername);
            groupBox1.Location = new Point(303, 88);
            groupBox1.Margin = new Padding(2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(2);
            groupBox1.Size = new Size(394, 100);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "Login";
            // 
            // textBoxPassword
            // 
            textBoxPassword.Location = new Point(118, 61);
            textBoxPassword.Margin = new Padding(2);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.PlaceholderText = "Inserire password";
            textBoxPassword.Size = new Size(226, 23);
            textBoxPassword.TabIndex = 5;
            textBoxPassword.UseSystemPasswordChar = true;
            textBoxPassword.WordWrap = false;
            // 
            // textBoxUsername
            // 
            textBoxUsername.Location = new Point(118, 28);
            textBoxUsername.Margin = new Padding(2);
            textBoxUsername.Name = "textBoxUsername";
            textBoxUsername.PlaceholderText = "Inserire nome utente";
            textBoxUsername.Size = new Size(226, 23);
            textBoxUsername.TabIndex = 4;
            // 
            // labelPassword
            // 
            labelPassword.AutoSize = true;
            labelPassword.Location = new Point(17, 62);
            labelPassword.Margin = new Padding(2, 0, 2, 0);
            labelPassword.Name = "labelPassword";
            labelPassword.Size = new Size(57, 15);
            labelPassword.TabIndex = 3;
            labelPassword.Text = "Password";
            // 
            // buttonQuit
            // 
            buttonQuit.BackColor = SystemColors.Highlight;
            buttonQuit.FlatStyle = FlatStyle.Flat;
            buttonQuit.Font = new Font("Segoe UI", 10F);
            buttonQuit.ForeColor = Color.White;
            buttonQuit.Location = new Point(569, 227);
            buttonQuit.Margin = new Padding(2, 1, 2, 1);
            buttonQuit.Name = "buttonQuit";
            buttonQuit.Size = new Size(129, 31);
            buttonQuit.TabIndex = 5;
            buttonQuit.Text = "Chiudi";
            buttonQuit.UseVisualStyleBackColor = false;
            // 
            // buttonLogin
            // 
            buttonLogin.BackColor = SystemColors.Highlight;
            buttonLogin.FlatStyle = FlatStyle.Flat;
            buttonLogin.Font = new Font("Segoe UI", 10F);
            buttonLogin.ForeColor = Color.White;
            buttonLogin.Location = new Point(304, 227);
            buttonLogin.Margin = new Padding(2, 1, 2, 1);
            buttonLogin.Name = "buttonLogin";
            buttonLogin.Size = new Size(129, 31);
            buttonLogin.TabIndex = 4;
            buttonLogin.Text = "Login";
            buttonLogin.UseVisualStyleBackColor = false;
            // 
            // labelVersion
            // 
            labelVersion.AutoSize = true;
            labelVersion.Location = new Point(347, 46);
            labelVersion.Margin = new Padding(2, 0, 2, 0);
            labelVersion.Name = "labelVersion";
            labelVersion.Size = new Size(51, 15);
            labelVersion.TabIndex = 6;
            labelVersion.Text = "versione";
            labelVersion.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(726, 268);
            Controls.Add(labelVersion);
            Controls.Add(buttonQuit);
            Controls.Add(buttonLogin);
            Controls.Add(groupBox1);
            Controls.Add(labelAppName);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LoginForm";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Label labelAppName;
        private Label labelUsername;
        private GroupBox groupBox1;
        private Label labelPassword;
        private TextBox textBoxPassword;
        private TextBox textBoxUsername;
        public Button buttonQuit;
        public Button buttonLogin;
        private Label labelVersion;
    }
}
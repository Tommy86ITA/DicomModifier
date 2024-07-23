namespace DicomModifier.Views
{
    partial class ChangePasswordForm
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
            labelConfirmNewPassword = new Label();
            textBoxConfirmNewPassword = new TextBox();
            labelNewPassword = new Label();
            textBoxNewPassword = new TextBox();
            labelCurrentPassword = new Label();
            textBoxCurrentPassword = new TextBox();
            buttonCancel = new Button();
            buttonChangePassword = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(labelConfirmNewPassword);
            groupBox1.Controls.Add(textBoxConfirmNewPassword);
            groupBox1.Controls.Add(labelNewPassword);
            groupBox1.Controls.Add(textBoxNewPassword);
            groupBox1.Controls.Add(labelCurrentPassword);
            groupBox1.Controls.Add(textBoxCurrentPassword);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(351, 178);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Modifica Password";
            // 
            // labelConfirmNewPassword
            // 
            labelConfirmNewPassword.AutoSize = true;
            labelConfirmNewPassword.Location = new Point(18, 127);
            labelConfirmNewPassword.Name = "labelConfirmNewPassword";
            labelConfirmNewPassword.Size = new Size(113, 15);
            labelConfirmNewPassword.TabIndex = 5;
            labelConfirmNewPassword.Text = "Conferma Password";
            // 
            // textBoxConfirmNewPassword
            // 
            textBoxConfirmNewPassword.Location = new Point(140, 119);
            textBoxConfirmNewPassword.Name = "textBoxConfirmNewPassword";
            textBoxConfirmNewPassword.Size = new Size(188, 23);
            textBoxConfirmNewPassword.TabIndex = 4;
            textBoxConfirmNewPassword.UseSystemPasswordChar = true;
            // 
            // labelNewPassword
            // 
            labelNewPassword.AutoSize = true;
            labelNewPassword.Location = new Point(18, 87);
            labelNewPassword.Name = "labelNewPassword";
            labelNewPassword.Size = new Size(95, 15);
            labelNewPassword.TabIndex = 3;
            labelNewPassword.Text = "Nuova Password";
            // 
            // textBoxNewPassword
            // 
            textBoxNewPassword.Location = new Point(140, 79);
            textBoxNewPassword.Name = "textBoxNewPassword";
            textBoxNewPassword.Size = new Size(188, 23);
            textBoxNewPassword.TabIndex = 2;
            textBoxNewPassword.UseSystemPasswordChar = true;
            // 
            // labelCurrentPassword
            // 
            labelCurrentPassword.AutoSize = true;
            labelCurrentPassword.Location = new Point(18, 47);
            labelCurrentPassword.Name = "labelCurrentPassword";
            labelCurrentPassword.Size = new Size(96, 15);
            labelCurrentPassword.TabIndex = 1;
            labelCurrentPassword.Text = "Password attuale";
            // 
            // textBoxCurrentPassword
            // 
            textBoxCurrentPassword.Location = new Point(140, 39);
            textBoxCurrentPassword.Name = "textBoxCurrentPassword";
            textBoxCurrentPassword.Size = new Size(188, 23);
            textBoxCurrentPassword.TabIndex = 0;
            textBoxCurrentPassword.UseSystemPasswordChar = true;
            // 
            // buttonCancel
            // 
            buttonCancel.BackColor = SystemColors.Highlight;
            buttonCancel.FlatStyle = FlatStyle.Flat;
            buttonCancel.Font = new Font("Segoe UI", 10F);
            buttonCancel.ForeColor = Color.White;
            buttonCancel.Location = new Point(211, 210);
            buttonCancel.Margin = new Padding(2, 1, 2, 1);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(129, 31);
            buttonCancel.TabIndex = 2;
            buttonCancel.Text = "Annulla";
            buttonCancel.UseVisualStyleBackColor = false;
            // 
            // buttonChangePassword
            // 
            buttonChangePassword.BackColor = Color.LightCoral;
            buttonChangePassword.FlatStyle = FlatStyle.Flat;
            buttonChangePassword.Font = new Font("Segoe UI", 10F);
            buttonChangePassword.ForeColor = Color.White;
            buttonChangePassword.Location = new Point(30, 210);
            buttonChangePassword.Margin = new Padding(2, 1, 2, 1);
            buttonChangePassword.Name = "buttonChangePassword";
            buttonChangePassword.Size = new Size(129, 31);
            buttonChangePassword.TabIndex = 3;
            buttonChangePassword.Text = "Salva Password";
            buttonChangePassword.UseVisualStyleBackColor = false;
            // 
            // ChangePasswordForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(374, 260);
            Controls.Add(buttonCancel);
            Controls.Add(buttonChangePassword);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "ChangePasswordForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Opzioni Account";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Label labelNewPassword;
        private TextBox textBoxNewPassword;
        private Label labelCurrentPassword;
        private TextBox textBoxCurrentPassword;
        public Button buttonCancel;
        public Button buttonChangePassword;
        private Label labelConfirmNewPassword;
        private TextBox textBoxConfirmNewPassword;
    }
}
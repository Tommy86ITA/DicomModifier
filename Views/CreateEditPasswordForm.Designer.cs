namespace DicomModifier.Views
{
    partial class CreateEditPasswordForm
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
            buttonCancel = new Button();
            buttonSavePassword = new Button();
            groupBox1 = new GroupBox();
            labelConfirmNewPassword = new Label();
            textBoxConfirmPassword = new TextBox();
            labelNewPassword = new Label();
            textBoxPassword = new TextBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // buttonCancel
            // 
            buttonCancel.BackColor = SystemColors.Highlight;
            buttonCancel.FlatStyle = FlatStyle.Flat;
            buttonCancel.Font = new Font("Segoe UI", 10F);
            buttonCancel.ForeColor = Color.White;
            buttonCancel.Location = new Point(211, 174);
            buttonCancel.Margin = new Padding(2, 1, 2, 1);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(129, 31);
            buttonCancel.TabIndex = 5;
            buttonCancel.Text = "Annulla";
            buttonCancel.UseVisualStyleBackColor = false;
            // 
            // buttonSavePassword
            // 
            buttonSavePassword.BackColor = Color.LightCoral;
            buttonSavePassword.FlatStyle = FlatStyle.Flat;
            buttonSavePassword.Font = new Font("Segoe UI", 10F);
            buttonSavePassword.ForeColor = Color.White;
            buttonSavePassword.Location = new Point(30, 174);
            buttonSavePassword.Margin = new Padding(2, 1, 2, 1);
            buttonSavePassword.Name = "buttonSavePassword";
            buttonSavePassword.Size = new Size(129, 31);
            buttonSavePassword.TabIndex = 6;
            buttonSavePassword.Text = "Salva Password";
            buttonSavePassword.UseVisualStyleBackColor = false;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(labelConfirmNewPassword);
            groupBox1.Controls.Add(textBoxConfirmPassword);
            groupBox1.Controls.Add(labelNewPassword);
            groupBox1.Controls.Add(textBoxPassword);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(351, 145);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Modifica Password";
            // 
            // labelConfirmNewPassword
            // 
            labelConfirmNewPassword.AutoSize = true;
            labelConfirmNewPassword.Location = new Point(18, 89);
            labelConfirmNewPassword.Name = "labelConfirmNewPassword";
            labelConfirmNewPassword.Size = new Size(113, 15);
            labelConfirmNewPassword.TabIndex = 5;
            labelConfirmNewPassword.Text = "Conferma Password";
            // 
            // textBoxConfirmPassword
            // 
            textBoxConfirmPassword.Location = new Point(140, 81);
            textBoxConfirmPassword.Name = "textBoxConfirmPassword";
            textBoxConfirmPassword.Size = new Size(188, 23);
            textBoxConfirmPassword.TabIndex = 4;
            textBoxConfirmPassword.UseSystemPasswordChar = true;
            // 
            // labelNewPassword
            // 
            labelNewPassword.AutoSize = true;
            labelNewPassword.Location = new Point(18, 49);
            labelNewPassword.Name = "labelNewPassword";
            labelNewPassword.Size = new Size(57, 15);
            labelNewPassword.TabIndex = 3;
            labelNewPassword.Text = "Password";
            // 
            // textBoxPassword
            // 
            textBoxPassword.Location = new Point(140, 41);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.Size = new Size(188, 23);
            textBoxPassword.TabIndex = 2;
            textBoxPassword.UseSystemPasswordChar = true;
            // 
            // CreateEditPasswordForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(375, 226);
            Controls.Add(buttonCancel);
            Controls.Add(buttonSavePassword);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CreateEditPasswordForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "CreateEditPasswordForm";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        public Button buttonCancel;
        public Button buttonSavePassword;
        private GroupBox groupBox1;
        private Label labelConfirmNewPassword;
        private TextBox textBoxConfirmPassword;
        private Label labelNewPassword;
        private TextBox textBoxPassword;
    }
}
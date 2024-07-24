// Interfaces/CreateEditUserForm.Designer.cs

namespace DicomModifier.Views
{
    partial class CreateEditUserForm
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
            groupBoxUserName = new GroupBox();
            label1 = new Label();
            textBoxUsername = new TextBox();
            groupBoxRole = new GroupBox();
            comboBoxRole = new ComboBox();
            groupBox1 = new GroupBox();
            labelConfirmNewPassword = new Label();
            textBoxConfirmPassword = new TextBox();
            labelNewPassword = new Label();
            textBoxPassword = new TextBox();
            buttonCancel = new Button();
            buttonSave = new Button();
            groupBoxUserName.SuspendLayout();
            groupBoxRole.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxUserName
            // 
            groupBoxUserName.Controls.Add(label1);
            groupBoxUserName.Controls.Add(textBoxUsername);
            groupBoxUserName.Location = new Point(11, 12);
            groupBoxUserName.Name = "groupBoxUserName";
            groupBoxUserName.Size = new Size(214, 90);
            groupBoxUserName.TabIndex = 0;
            groupBoxUserName.TabStop = false;
            groupBoxUserName.Text = "Utente";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(1, 19);
            label1.Name = "label1";
            label1.Size = new Size(78, 15);
            label1.TabIndex = 8;
            label1.Text = "Nome Utente";
            // 
            // textBoxUsername
            // 
            textBoxUsername.Location = new Point(5, 53);
            textBoxUsername.Margin = new Padding(2);
            textBoxUsername.Name = "textBoxUsername";
            textBoxUsername.PlaceholderText = "Inserire nome utente";
            textBoxUsername.Size = new Size(188, 23);
            textBoxUsername.TabIndex = 5;
            // 
            // groupBoxRole
            // 
            groupBoxRole.Controls.Add(comboBoxRole);
            groupBoxRole.Location = new Point(11, 108);
            groupBoxRole.Name = "groupBoxRole";
            groupBoxRole.Size = new Size(214, 90);
            groupBoxRole.TabIndex = 1;
            groupBoxRole.TabStop = false;
            groupBoxRole.Text = "Ruolo";
            // 
            // comboBoxRole
            // 
            comboBoxRole.FormattingEnabled = true;
            comboBoxRole.Items.AddRange(new object[] { "Technician", "Administrator" });
            comboBoxRole.Location = new Point(6, 33);
            comboBoxRole.Name = "comboBoxRole";
            comboBoxRole.Size = new Size(188, 23);
            comboBoxRole.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(labelConfirmNewPassword);
            groupBox1.Controls.Add(textBoxConfirmPassword);
            groupBox1.Controls.Add(labelNewPassword);
            groupBox1.Controls.Add(textBoxPassword);
            groupBox1.Location = new Point(261, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(214, 186);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Password";
            // 
            // labelConfirmNewPassword
            // 
            labelConfirmNewPassword.AutoSize = true;
            labelConfirmNewPassword.Location = new Point(10, 95);
            labelConfirmNewPassword.Name = "labelConfirmNewPassword";
            labelConfirmNewPassword.Size = new Size(113, 15);
            labelConfirmNewPassword.TabIndex = 9;
            labelConfirmNewPassword.Text = "Conferma Password";
            // 
            // textBoxConfirmPassword
            // 
            textBoxConfirmPassword.Location = new Point(10, 129);
            textBoxConfirmPassword.Name = "textBoxConfirmPassword";
            textBoxConfirmPassword.Size = new Size(188, 23);
            textBoxConfirmPassword.TabIndex = 8;
            textBoxConfirmPassword.UseSystemPasswordChar = true;
            // 
            // labelNewPassword
            // 
            labelNewPassword.AutoSize = true;
            labelNewPassword.Location = new Point(10, 19);
            labelNewPassword.Name = "labelNewPassword";
            labelNewPassword.Size = new Size(95, 15);
            labelNewPassword.TabIndex = 7;
            labelNewPassword.Text = "Nuova Password";
            // 
            // textBoxPassword
            // 
            textBoxPassword.Location = new Point(10, 53);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.Size = new Size(188, 23);
            textBoxPassword.TabIndex = 6;
            textBoxPassword.UseSystemPasswordChar = true;
            // 
            // buttonCancel
            // 
            buttonCancel.BackColor = Color.DodgerBlue;
            buttonCancel.FlatStyle = FlatStyle.Flat;
            buttonCancel.Font = new Font("Segoe UI", 10F);
            buttonCancel.ForeColor = Color.White;
            buttonCancel.Location = new Point(269, 217);
            buttonCancel.Margin = new Padding(2, 1, 2, 1);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(129, 31);
            buttonCancel.TabIndex = 4;
            buttonCancel.Text = "Annulla";
            buttonCancel.UseVisualStyleBackColor = false;
            // 
            // buttonSave
            // 
            buttonSave.BackColor = Color.DodgerBlue;
            buttonSave.FlatStyle = FlatStyle.Flat;
            buttonSave.Font = new Font("Segoe UI", 10F);
            buttonSave.ForeColor = Color.White;
            buttonSave.Location = new Point(88, 217);
            buttonSave.Margin = new Padding(2, 1, 2, 1);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(129, 31);
            buttonSave.TabIndex = 5;
            buttonSave.Text = "Salva";
            buttonSave.UseVisualStyleBackColor = false;
            // 
            // CreateEditUserForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(487, 267);
            Controls.Add(buttonCancel);
            Controls.Add(buttonSave);
            Controls.Add(groupBox1);
            Controls.Add(groupBoxRole);
            Controls.Add(groupBoxUserName);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CreateEditUserForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "CreateEditUserForm";
            groupBoxUserName.ResumeLayout(false);
            groupBoxUserName.PerformLayout();
            groupBoxRole.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBoxUserName;
        private TextBox textBoxUsername;
        private GroupBox groupBoxRole;
        private ComboBox comboBoxRole;
        private GroupBox groupBox1;
        private Label labelConfirmNewPassword;
        private TextBox textBoxConfirmPassword;
        private Label labelNewPassword;
        private TextBox textBoxPassword;
        public Button buttonCancel;
        public Button buttonSave;
        private Label label1;
    }
}
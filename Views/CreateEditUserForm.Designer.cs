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
            textBoxUsername = new TextBox();
            groupBoxRole = new GroupBox();
            comboBoxRole = new ComboBox();
            buttonCancel = new Button();
            buttonSave = new Button();
            groupBoxEnableUser = new GroupBox();
            checkBoxEnableUser = new CheckBox();
            groupBoxUserName.SuspendLayout();
            groupBoxRole.SuspendLayout();
            groupBoxEnableUser.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxUserName
            // 
            groupBoxUserName.Controls.Add(textBoxUsername);
            groupBoxUserName.Location = new Point(11, 12);
            groupBoxUserName.Name = "groupBoxUserName";
            groupBoxUserName.Size = new Size(276, 90);
            groupBoxUserName.TabIndex = 0;
            groupBoxUserName.TabStop = false;
            groupBoxUserName.Text = "Utente";
            // 
            // textBoxUsername
            // 
            textBoxUsername.Location = new Point(44, 32);
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
            groupBoxRole.Size = new Size(276, 90);
            groupBoxRole.TabIndex = 1;
            groupBoxRole.TabStop = false;
            groupBoxRole.Text = "Ruolo";
            // 
            // comboBoxRole
            // 
            comboBoxRole.AutoCompleteCustomSource.AddRange(new string[] { "Administrator", "Technician" });
            comboBoxRole.AutoCompleteSource = AutoCompleteSource.CustomSource;
            comboBoxRole.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxRole.FormattingEnabled = true;
            comboBoxRole.Items.AddRange(new object[] { "Technician", "Administrator" });
            comboBoxRole.Location = new Point(44, 39);
            comboBoxRole.Name = "comboBoxRole";
            comboBoxRole.Size = new Size(188, 23);
            comboBoxRole.TabIndex = 0;
            // 
            // buttonCancel
            // 
            buttonCancel.BackColor = Color.DodgerBlue;
            buttonCancel.FlatStyle = FlatStyle.Flat;
            buttonCancel.Font = new Font("Segoe UI", 10F);
            buttonCancel.ForeColor = Color.White;
            buttonCancel.Location = new Point(157, 323);
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
            buttonSave.Location = new Point(10, 323);
            buttonSave.Margin = new Padding(2, 1, 2, 1);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(129, 31);
            buttonSave.TabIndex = 5;
            buttonSave.Text = "Salva";
            buttonSave.UseVisualStyleBackColor = false;
            // 
            // groupBoxEnableUser
            // 
            groupBoxEnableUser.Controls.Add(checkBoxEnableUser);
            groupBoxEnableUser.Location = new Point(10, 204);
            groupBoxEnableUser.Name = "groupBoxEnableUser";
            groupBoxEnableUser.Size = new Size(276, 90);
            groupBoxEnableUser.TabIndex = 6;
            groupBoxEnableUser.TabStop = false;
            groupBoxEnableUser.Text = "Abilitazione";
            // 
            // checkBoxEnableUser
            // 
            checkBoxEnableUser.AutoSize = true;
            checkBoxEnableUser.Checked = true;
            checkBoxEnableUser.CheckState = CheckState.Checked;
            checkBoxEnableUser.Location = new Point(90, 39);
            checkBoxEnableUser.Name = "checkBoxEnableUser";
            checkBoxEnableUser.Size = new Size(97, 19);
            checkBoxEnableUser.TabIndex = 0;
            checkBoxEnableUser.Text = "Abilita utente";
            checkBoxEnableUser.UseVisualStyleBackColor = true;
            // 
            // CreateEditUserForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(298, 364);
            Controls.Add(groupBoxEnableUser);
            Controls.Add(buttonCancel);
            Controls.Add(buttonSave);
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
            groupBoxEnableUser.ResumeLayout(false);
            groupBoxEnableUser.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBoxUserName;
        private TextBox textBoxUsername;
        private GroupBox groupBoxRole;
        private ComboBox comboBoxRole;
        public Button buttonCancel;
        public Button buttonSave;
        private GroupBox groupBoxEnableUser;
        private CheckBox checkBoxEnableUser;
    }
}
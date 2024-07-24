// Interfaces/ManageUsersForm.Designer.cs

namespace DicomModifier.Views
{
    partial class ManageUsersForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            dataGridViewUsers = new DataGridView();
            columnUserName = new DataGridViewTextBoxColumn();
            roleColumn = new DataGridViewTextBoxColumn();
            enabledColumn = new DataGridViewCheckBoxColumn();
            buttonClose = new Button();
            buttonEditUser = new Button();
            buttonDeleteUser = new Button();
            buttonAddUser = new Button();
            buttonChangePassword = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridViewUsers).BeginInit();
            SuspendLayout();
            // 
            // dataGridViewUsers
            // 
            dataGridViewUsers.AllowUserToResizeColumns = false;
            dataGridViewUsers.AllowUserToResizeRows = false;
            dataGridViewUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewUsers.Columns.AddRange(new DataGridViewColumn[] { columnUserName, roleColumn, enabledColumn });
            dataGridViewUsers.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridViewUsers.Location = new Point(12, 12);
            dataGridViewUsers.MultiSelect = false;
            dataGridViewUsers.Name = "dataGridViewUsers";
            dataGridViewUsers.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewUsers.ShowEditingIcon = false;
            dataGridViewUsers.Size = new Size(626, 200);
            dataGridViewUsers.TabIndex = 0;
            // 
            // columnUserName
            // 
            columnUserName.HeaderText = "Nome Utente";
            columnUserName.MinimumWidth = 350;
            columnUserName.Name = "columnUserName";
            columnUserName.Width = 350;
            // 
            // roleColumn
            // 
            roleColumn.HeaderText = "Ruolo";
            roleColumn.MinimumWidth = 150;
            roleColumn.Name = "roleColumn";
            roleColumn.Resizable = DataGridViewTriState.True;
            roleColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            roleColumn.Width = 150;
            // 
            // enabledColumn
            // 
            enabledColumn.HeaderText = "Abilitazione";
            enabledColumn.MinimumWidth = 80;
            enabledColumn.Name = "enabledColumn";
            enabledColumn.Width = 80;
            // 
            // buttonClose
            // 
            buttonClose.BackColor = Color.DodgerBlue;
            buttonClose.FlatStyle = FlatStyle.Flat;
            buttonClose.Font = new Font("Segoe UI", 10F);
            buttonClose.ForeColor = Color.White;
            buttonClose.Location = new Point(325, 234);
            buttonClose.Margin = new Padding(2, 1, 2, 1);
            buttonClose.Name = "buttonClose";
            buttonClose.Size = new Size(129, 31);
            buttonClose.TabIndex = 12;
            buttonClose.Text = "Chiudi";
            buttonClose.UseVisualStyleBackColor = false;
            // 
            // buttonEditUser
            // 
            buttonEditUser.BackColor = Color.DodgerBlue;
            buttonEditUser.FlatStyle = FlatStyle.Flat;
            buttonEditUser.Font = new Font("Segoe UI", 10F);
            buttonEditUser.ForeColor = Color.White;
            buttonEditUser.Location = new Point(643, 58);
            buttonEditUser.Margin = new Padding(2, 1, 2, 1);
            buttonEditUser.Name = "buttonEditUser";
            buttonEditUser.Size = new Size(129, 31);
            buttonEditUser.TabIndex = 13;
            buttonEditUser.Text = "Modifica utente";
            buttonEditUser.UseVisualStyleBackColor = false;
            // 
            // buttonDeleteUser
            // 
            buttonDeleteUser.BackColor = Color.LightCoral;
            buttonDeleteUser.FlatStyle = FlatStyle.Flat;
            buttonDeleteUser.Font = new Font("Segoe UI", 10F);
            buttonDeleteUser.ForeColor = Color.White;
            buttonDeleteUser.Location = new Point(643, 181);
            buttonDeleteUser.Margin = new Padding(2, 1, 2, 1);
            buttonDeleteUser.Name = "buttonDeleteUser";
            buttonDeleteUser.Size = new Size(129, 31);
            buttonDeleteUser.TabIndex = 14;
            buttonDeleteUser.Text = "Elimina";
            buttonDeleteUser.UseVisualStyleBackColor = false;
            // 
            // buttonAddUser
            // 
            buttonAddUser.BackColor = Color.DodgerBlue;
            buttonAddUser.FlatStyle = FlatStyle.Flat;
            buttonAddUser.Font = new Font("Segoe UI", 10F);
            buttonAddUser.ForeColor = Color.White;
            buttonAddUser.Location = new Point(643, 12);
            buttonAddUser.Margin = new Padding(2, 1, 2, 1);
            buttonAddUser.Name = "buttonAddUser";
            buttonAddUser.Size = new Size(129, 31);
            buttonAddUser.TabIndex = 15;
            buttonAddUser.Text = "Nuovo utente";
            buttonAddUser.UseVisualStyleBackColor = false;
            // 
            // buttonChangePassword
            // 
            buttonChangePassword.BackColor = Color.DodgerBlue;
            buttonChangePassword.FlatStyle = FlatStyle.Flat;
            buttonChangePassword.Font = new Font("Segoe UI", 9.7F);
            buttonChangePassword.ForeColor = Color.White;
            buttonChangePassword.Location = new Point(643, 104);
            buttonChangePassword.Margin = new Padding(2, 1, 2, 1);
            buttonChangePassword.Name = "buttonChangePassword";
            buttonChangePassword.Size = new Size(129, 31);
            buttonChangePassword.TabIndex = 16;
            buttonChangePassword.Text = "Modifica Password";
            buttonChangePassword.UseVisualStyleBackColor = false;
            // 
            // ManageUsersForm
            // 
            ClientSize = new Size(791, 284);
            Controls.Add(buttonChangePassword);
            Controls.Add(buttonAddUser);
            Controls.Add(buttonDeleteUser);
            Controls.Add(buttonEditUser);
            Controls.Add(buttonClose);
            Controls.Add(dataGridViewUsers);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ManageUsersForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Gestione Account";
            ((System.ComponentModel.ISupportInitialize)dataGridViewUsers).EndInit();
            ResumeLayout(false);
        }

        private System.Windows.Forms.DataGridView dataGridViewUsers;
        public Button buttonClose;
        public Button buttonEditUser;
        public Button buttonDeleteUser;
        public Button buttonAddUser;
        private DataGridViewTextBoxColumn columnUserName;
        private DataGridViewTextBoxColumn roleColumn;
        private DataGridViewCheckBoxColumn enabledColumn;
        public Button buttonChangePassword;
    }
}

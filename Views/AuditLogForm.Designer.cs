// Interfaces/AuditLogForm.Designer.cs

namespace DicomModifier.Views
{
    partial class AuditLogForm
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
            dataGridViewAuditLog = new DataGridView();
            buttonClose = new Button();
            buttonExport = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridViewAuditLog).BeginInit();
            SuspendLayout();
            // 
            // dataGridViewAuditLog
            // 
            dataGridViewAuditLog.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewAuditLog.Location = new Point(12, 12);
            dataGridViewAuditLog.Name = "dataGridViewAuditLog";
            dataGridViewAuditLog.Size = new Size(1142, 426);
            dataGridViewAuditLog.TabIndex = 0;
            // 
            // buttonClose
            // 
            buttonClose.BackColor = SystemColors.Highlight;
            buttonClose.FlatStyle = FlatStyle.Flat;
            buttonClose.Font = new Font("Segoe UI", 10F);
            buttonClose.ForeColor = Color.White;
            buttonClose.Location = new Point(651, 449);
            buttonClose.Margin = new Padding(2, 1, 2, 1);
            buttonClose.Name = "buttonClose";
            buttonClose.Size = new Size(129, 31);
            buttonClose.TabIndex = 7;
            buttonClose.Text = "Chiudi";
            buttonClose.UseVisualStyleBackColor = false;
            // 
            // buttonExport
            // 
            buttonExport.BackColor = SystemColors.Highlight;
            buttonExport.FlatStyle = FlatStyle.Flat;
            buttonExport.Font = new Font("Segoe UI", 10F);
            buttonExport.ForeColor = Color.White;
            buttonExport.Location = new Point(386, 449);
            buttonExport.Margin = new Padding(2, 1, 2, 1);
            buttonExport.Name = "buttonExport";
            buttonExport.Size = new Size(129, 31);
            buttonExport.TabIndex = 6;
            buttonExport.Text = "Esporta in *.xslx";
            buttonExport.UseVisualStyleBackColor = false;
            // 
            // AuditLogForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1166, 490);
            Controls.Add(buttonClose);
            Controls.Add(buttonExport);
            Controls.Add(dataGridViewAuditLog);
            Name = "AuditLogForm";
            Text = "AuditLogForm";
            ((System.ComponentModel.ISupportInitialize)dataGridViewAuditLog).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridViewAuditLog;
        public Button buttonClose;
        public Button buttonExport;
    }
}
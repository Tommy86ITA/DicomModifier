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
            ((System.ComponentModel.ISupportInitialize)dataGridViewAuditLog).BeginInit();
            SuspendLayout();
            // 
            // dataGridViewAuditLog
            // 
            dataGridViewAuditLog.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewAuditLog.Location = new Point(12, 12);
            dataGridViewAuditLog.Name = "dataGridViewAuditLog";
            dataGridViewAuditLog.Size = new Size(776, 426);
            dataGridViewAuditLog.TabIndex = 0;
            // 
            // AuditLogForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(dataGridViewAuditLog);
            Name = "AuditLogForm";
            Text = "AuditLogForm";
            ((System.ComponentModel.ISupportInitialize)dataGridViewAuditLog).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridViewAuditLog;
    }
}
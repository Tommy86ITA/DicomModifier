//// Interfaces/AuditLogForm.cs

using DicomModifier.Models;
using DicomModifier.Services;
using Microsoft.Data.Sqlite;
using System.Data;

namespace DicomModifier.Views
{
    public partial class AuditLogForm : Form
    {
        // Constructor
        public AuditLogForm()
        {
            InitializeComponent();
            InitializeEvents();
            LoadAuditLogs();

            // Add CellFormatting event handler
            dataGridViewAuditLog.CellFormatting += DataGridViewAuditLog_CellFormatting;
        }

        private void InitializeEvents()
        {
            buttonClose.Click += ButtonClose_Click;
            buttonExport.Click += ButtonExport_Click;
        }

        private void ButtonExport_Click(object? sender, EventArgs e)
        {
            // Recupera i log attuali
            var logs = LoadAuditLogs();
            using SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "Excel Files|*.xlsx";
            saveFileDialog.Title = "Save Audit Log as Excel File";
            saveFileDialog.FileName = "AuditLog.xlsx";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Call your method to save the file here
                // For example, if you have a method called ExportToExcel that takes the file path as a parameter:
                LogExporter.ExportLogsToExcel(saveFileDialog.FileName, logs);
            }

            MessageBox.Show("File di log esportato correttamente", "Esportazione log", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ButtonClose_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        // Method to load audit logs into the DataGridView and format the columns
        private List<LogEntry> LoadAuditLogs()
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            const string query = "SELECT Timestamp, Username, EventType, Message, EventSeverity FROM AuditLog ORDER BY Timestamp DESC";
            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();

            var logs = new List<LogEntry>();
            while (reader.Read())
            {
                var log = new LogEntry
                {
                    Timestamp = reader.GetDateTime(0),
                    Username = reader.GetString(1),
                    EventType = (EventMapping.EventType)Enum.Parse(typeof(EventMapping.EventType), reader.GetString(2)),
                    Message = reader.GetString(3),
                    EventSeverity = (EventMapping.EventSeverity)Enum.Parse(typeof(EventMapping.EventSeverity), reader.GetString(4))
                };
                logs.Add(log);
            }

            dataGridViewAuditLog.DataSource = logs;

            dataGridViewAuditLog.Columns["Timestamp"].HeaderText = "Timestamp";
            dataGridViewAuditLog.Columns["Username"].HeaderText = "Username";
            dataGridViewAuditLog.Columns["EventType"].HeaderText = "Evento";
            dataGridViewAuditLog.Columns["Message"].HeaderText = "Messaggio";
            dataGridViewAuditLog.Columns["EventSeverity"].HeaderText = "Livello evento";

            dataGridViewAuditLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            foreach (DataGridViewColumn column in dataGridViewAuditLog.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            return logs;
        }

        // Aggiorna l'event handler per il cell formatting
        private void DataGridViewAuditLog_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridViewAuditLog.Columns[e.ColumnIndex].Name == "Timestamp" && e.Value != null)
            {
                if (DateTime.TryParse(e.Value.ToString(), out DateTime dateValue))
                {
                    e.Value = dateValue.ToString("dd/MM/yyyy HH:mm:ss");
                    e.FormattingApplied = true;
                }
            }

            if (dataGridViewAuditLog.Columns[e.ColumnIndex].Name == "EventSeverity" && e.Value != null)
            {
                string eventType = e.Value.ToString()!;
                if (eventType == nameof(EventMapping.EventSeverity.Error))
                {
                    e.CellStyle!.BackColor = Color.LightCoral;
                }
                else if (eventType == nameof(EventMapping.EventSeverity.Warning))
                {
                    e.CellStyle!.BackColor = Color.Khaki;
                }
                else if (eventType == nameof(EventMapping.EventSeverity.Informational))
                {
                    e.CellStyle!.BackColor = Color.LightGreen;
                }
            }
        }
    }
}
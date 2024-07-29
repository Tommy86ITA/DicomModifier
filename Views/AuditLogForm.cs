//// Interfaces/AuditLogForm.cs

//using DicomModifier.Services;
//using Microsoft.Data.Sqlite;
//using System.Data;

//namespace DicomModifier.Views
//{
//    public partial class AuditLogForm : Form
//    {
//        // Constructor
//        public AuditLogForm()
//        {
//            InitializeComponent();
//            LoadAuditLogs();

//            // Add CellFormatting event handler
//            dataGridViewAuditLog.CellFormatting += DataGridViewAuditLog_CellFormatting;
//        }

//        // Method to load audit logs into the DataGridView and format the columns
//        private void LoadAuditLogs()
//        {
//            using var connection = DatabaseHelper.GetConnection();
//            connection.Open();

//            string query = "SELECT Timestamp, Username, Action, Severity FROM AuditLog";
//            using var command = new SqliteCommand(query, connection);
//            using var reader = command.ExecuteReader();

//            var dataTable = new DataTable();
//            dataTable.Load(reader);

//            dataGridViewAuditLog.DataSource = dataTable;

//            dataGridViewAuditLog.Columns["Timestamp"].HeaderText = "Timestamp";
//            dataGridViewAuditLog.Columns["Username"].HeaderText = "Username";
//            dataGridViewAuditLog.Columns["Action"].HeaderText = "Evento";
//            dataGridViewAuditLog.Columns["Severity"].HeaderText = "Livello evento";

//            dataGridViewAuditLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

//            foreach (DataGridViewColumn column in dataGridViewAuditLog.Columns)
//            {
//                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
//            }

//        }

//        // Aggiorna l'event handler per il cell formatting
//        private void DataGridViewAuditLog_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
//        {
//            if (dataGridViewAuditLog.Columns[e.ColumnIndex].Name == "Timestamp" && e.Value != null)
//            {
//                if (DateTime.TryParse(e.Value.ToString(), out DateTime dateValue))
//                {
//                    e.Value = dateValue.ToString("dd/MM/yyyy HH:mm:ss");
//                    e.FormattingApplied = true;
//                }
//            }

//            if (dataGridViewAuditLog.Columns[e.ColumnIndex].Name == "Livello evento" && e.Value != null)
//            {
//                string eventType = e.Value.ToString()!;
//                if (eventType == LogManager.EventSeverity.Error.ToString())
//                {
//                    e.CellStyle!.BackColor = Color.LightCoral;
//                }
//                else if (eventType == LogManager.EventSeverity.Warning.ToString())
//                {
//                    e.CellStyle!.BackColor = Color.Khaki;
//                }
//                else if (eventType == LogManager.EventSeverity.Informational.ToString())
//                {
//                    e.CellStyle!.BackColor = Color.LightGreen;
//                }
//            }
//        }
//    }
//}

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
            LoadAuditLogs();

            // Add CellFormatting event handler
            dataGridViewAuditLog.CellFormatting += DataGridViewAuditLog_CellFormatting;
        }

        // Method to load audit logs into the DataGridView and format the columns
        private void LoadAuditLogs()
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            string query = "SELECT Timestamp, Username, Action, Severity FROM AuditLog ORDER BY Timestamp DESC";
            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();

            var dataTable = new DataTable();
            dataTable.Load(reader);

            dataGridViewAuditLog.DataSource = dataTable;

            dataGridViewAuditLog.Columns["Timestamp"].HeaderText = "Timestamp";
            dataGridViewAuditLog.Columns["Username"].HeaderText = "Username";
            dataGridViewAuditLog.Columns["Action"].HeaderText = "Evento";
            dataGridViewAuditLog.Columns["Severity"].HeaderText = "Livello evento";

            dataGridViewAuditLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            foreach (DataGridViewColumn column in dataGridViewAuditLog.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

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

            if (dataGridViewAuditLog.Columns[e.ColumnIndex].Name == "Severity" && e.Value != null)
            {
                string eventType = e.Value.ToString()!;
                if (eventType == LogManager.EventSeverity.Error.ToString())
                {
                    e.CellStyle!.BackColor = Color.LightCoral;
                }
                else if (eventType == LogManager.EventSeverity.Warning.ToString())
                {
                    e.CellStyle!.BackColor = Color.Khaki;
                }
                else if (eventType == LogManager.EventSeverity.Informational.ToString())
                {
                    e.CellStyle!.BackColor = Color.LightGreen;
                }
            }
        }
    }
}


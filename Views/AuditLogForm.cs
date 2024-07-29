// Interfaces/AuditLogForm.cs

using DicomModifier.Services;
using Microsoft.Data.Sqlite;
using System.Data;

namespace DicomModifier.Views
{
    public partial class AuditLogForm : Form
    {
        // Constructor
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

            // SQL query to select audit log entries
            string query = "SELECT Timestamp, Username, Action FROM AuditLog";
            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();

            // Load the data into a DataTable
            var dataTable = new DataTable();
            dataTable.Load(reader);

            // Set the DataTable as the DataSource for the DataGridView
            dataGridViewAuditLog.DataSource = dataTable;

            // Set column headers
            dataGridViewAuditLog.Columns["Timestamp"].HeaderText = "Timestamp";
            dataGridViewAuditLog.Columns["Username"].HeaderText = "Username";
            dataGridViewAuditLog.Columns["Action"].HeaderText = "Action";

            // Adjust column widths to fill the DataGridView
            dataGridViewAuditLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            foreach (DataGridViewColumn column in dataGridViewAuditLog.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        // Event handler for cell formatting to format the Timestamp column
        private void DataGridViewAuditLog_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridViewAuditLog.Columns[e.ColumnIndex].Name == "Timestamp" && e.Value != null)
            {
                if (DateTime.TryParse(e.Value.ToString(), out DateTime dateValue))
                {
                    e.Value = dateValue.ToString("dd/MM/yyyy HH:mm:ss");
                    e.FormattingApplied = true;
                }
            }
        }
    }
}

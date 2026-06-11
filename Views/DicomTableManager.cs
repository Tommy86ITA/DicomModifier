using FellowOakDicom;

namespace DicomModifier.Views;

/// <summary>
/// Manages the DataGridView used to display imported DICOM studies.
/// Belongs to the View layer and has no dependency on services.
/// </summary>
public class DicomTableManager
{
    private readonly DataGridView _dataGridView;
    private readonly Action _onDataChanged;

    /// <summary>
    /// Initializes a new <see cref="DicomTableManager"/> and subscribes to grid events.
    /// </summary>
    /// <param name="dataGridView">The grid to manage.</param>
    /// <param name="onDataChanged">Callback invoked whenever rows are added, so callers can refresh control states.</param>
    public DicomTableManager(DataGridView dataGridView, Action onDataChanged)
    {
        _dataGridView = dataGridView;
        _onDataChanged = onDataChanged;
        _dataGridView.CellFormatting += DataGridView_CellFormatting;
        _dataGridView.SelectionChanged += DataGridView_SelectionChanged;
    }

    /// <summary>Adds a DICOM dataset to the grid, merging series/image counts for existing studies.</summary>
    /// <param name="dataset">The DICOM dataset to add.</param>
    public void AddDicomToGrid(DicomDataset dataset)
    {
        string patientID = dataset.GetSingleValueOrDefault(DicomTag.PatientID, "Unknown");
        string studyInstanceUID = dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, "Unknown");
        string modality = dataset.GetSingleValueOrDefault(DicomTag.Modality, "Unknown");

        int patientIdColumnIndex = GetColumnIndex("PatientIDColumn");
        int studyInstanceUidColumnIndex = GetColumnIndex("StudyInstanceUIDColumn");
        int patientNameColumnIndex = GetColumnIndex("PatientNameColumn");
        int patientDobColumnIndex = GetColumnIndex("PatientDOBColumn");
        int studyDescriptionColumnIndex = GetColumnIndex("StudyDescriptionColumn");
        int studyDateColumnIndex = GetColumnIndex("StudyDateColumn");
        int modalityColumnIndex = GetColumnIndex("ModalityColumn");
        int seriesCountColumnIndex = GetColumnIndex("SeriesCountColumn");
        int imageCountColumnIndex = GetColumnIndex("ImageCountColumn");

        DataGridViewRow? existingRow = _dataGridView.Rows
            .Cast<DataGridViewRow>()
            .FirstOrDefault(r =>
                r.Cells[patientIdColumnIndex].Value?.ToString() == patientID &&
                r.Cells[studyInstanceUidColumnIndex].Value?.ToString() == studyInstanceUID);

        if (existingRow != null)
        {
            if (existingRow.Cells[seriesCountColumnIndex].Value is int seriesCount)
            {
                existingRow.Cells[seriesCountColumnIndex].Value = seriesCount + 1;
            }

            if (existingRow.Cells[imageCountColumnIndex].Value is int imageCount)
            {
                existingRow.Cells[imageCountColumnIndex].Value = imageCount + 1;
            }

            string existingModality = existingRow.Cells[modalityColumnIndex].Value?.ToString() ?? string.Empty;
            if (!existingModality.Contains(modality))
            {
                existingRow.Cells[modalityColumnIndex].Value = string.IsNullOrWhiteSpace(existingModality)
                    ? modality
                    : $"{existingModality}+{modality}";
            }
        }
        else
        {
            DataGridViewRow newRow = new();
            newRow.CreateCells(_dataGridView);

            newRow.Cells[patientIdColumnIndex].Value = patientID;
            newRow.Cells[studyInstanceUidColumnIndex].Value = studyInstanceUID;
            newRow.Cells[patientNameColumnIndex].Value =
                dataset.GetSingleValueOrDefault(DicomTag.PatientName, "Unknown");
            newRow.Cells[patientDobColumnIndex].Value =
                dataset.GetSingleValueOrDefault(DicomTag.PatientBirthDate, DateTime.MinValue).ToString("dd/MM/yyyy");
            newRow.Cells[studyDescriptionColumnIndex].Value =
                dataset.GetSingleValueOrDefault(DicomTag.StudyDescription, "Unknown");
            newRow.Cells[studyDateColumnIndex].Value =
                dataset.GetSingleValueOrDefault(DicomTag.StudyDate, DateTime.MinValue).ToString("dd/MM/yyyy");
            newRow.Cells[modalityColumnIndex].Value = modality;
            newRow.Cells[seriesCountColumnIndex].Value = 1;
            newRow.Cells[imageCountColumnIndex].Value = 1;

            _dataGridView.Rows.Add(newRow);
        }

        _onDataChanged();
    }

    /// <summary>
    /// Applies alternating row colors for better readability.
    /// </summary>
    /// <param name="sender">The DataGridView that is being formatted.</param>
    /// <param name="e">The event data for the cell formatting event.</param>
    private void DataGridView_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (sender is not DataGridView || e.RowIndex < 0 || e.CellStyle is null)
        {
            return;
        }

        e.CellStyle.BackColor = e.RowIndex % 2 == 0
            ? ColorTranslator.FromHtml("#F5F5F5")
            : ColorTranslator.FromHtml("#FFFFFF");
    }

    /// <summary>
    /// Highlights all rows belonging to the same patient when a row is selected, improving visual grouping.
    /// </summary>
    /// <param name="sender">The DataGridView where the selection changed.</param>
    /// <param name="e">The event data for the selection changed event.</param>
    private void DataGridView_SelectionChanged(object? sender, EventArgs e)
    {
        if (sender is not DataGridView)
        {
            return;
        }

        HighlightSamePatientRows();
    }

    /// <summary>
    /// Highlights all rows in the DataGridView that have the same PatientID as the currently selected row.
    /// </summary>
    private void HighlightSamePatientRows()
    {
        if (_dataGridView.SelectedRows.Count == 0)
        {
            return;
        }

        int patientIdColumnIndex = GetColumnIndex("PatientIDColumn");

        string? selectedPatientID = _dataGridView.SelectedRows[0].Cells[patientIdColumnIndex].Value?.ToString();
        if (selectedPatientID is null)
        {
            return;
        }

        foreach (DataGridViewRow row in _dataGridView.Rows)
        {
            row.DefaultCellStyle.BackColor = row.Cells[patientIdColumnIndex].Value?.ToString() == selectedPatientID
                ? ColorTranslator.FromHtml("#FFFACD")
                : row.Index % 2 == 0
                    ? ColorTranslator.FromHtml("#F5F5F5")
                    : ColorTranslator.FromHtml("#FFFFFF");
        }
    }

    /// <summary>
    /// Gets the index of a column by its name, throwing an exception if the column does not exist.
    /// </summary>
    /// <param name="columnName">The name of the column to find.</param>
    /// <returns>The index of the column.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the column does not exist.</exception>
    private int GetColumnIndex(string columnName)
    {
        DataGridViewColumn? column = _dataGridView.Columns[columnName];

        return column?.Index
            ?? throw new InvalidOperationException($"La colonna '{columnName}' non esiste nella tabella DICOM.");
    }
}
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
    public void AddDicomToGrid(DicomDataset dataset)
    {
        string patientID = dataset.GetSingleValueOrDefault(DicomTag.PatientID, "Unknown");
        string studyInstanceUID = dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, "Unknown");
        string modality = dataset.GetSingleValueOrDefault(DicomTag.Modality, "Unknown");

        DataGridViewRow? existingRow = _dataGridView.Rows
            .Cast<DataGridViewRow>()
            .FirstOrDefault(r =>
                r.Cells["PatientIDColumn"].Value?.ToString() == patientID &&
                r.Cells["StudyInstanceUIDColumn"].Value?.ToString() == studyInstanceUID);

        if (existingRow != null)
        {
            if (existingRow.Cells[_dataGridView.Columns["SeriesCountColumn"].Index].Value is int seriesCount)
                existingRow.Cells[_dataGridView.Columns["SeriesCountColumn"].Index].Value = seriesCount + 1;

            if (existingRow.Cells[_dataGridView.Columns["ImageCountColumn"].Index].Value is int imageCount)
                existingRow.Cells[_dataGridView.Columns["ImageCountColumn"].Index].Value = imageCount + 1;

            string existingModality = existingRow.Cells[_dataGridView.Columns["ModalityColumn"].Index].Value?.ToString() ?? string.Empty;
            if (!existingModality.Contains(modality))
                existingRow.Cells[_dataGridView.Columns["ModalityColumn"].Index].Value = $"{existingModality}+{modality}";
        }
        else
        {
            DataGridViewRow newRow = new();
            newRow.CreateCells(_dataGridView);

            newRow.Cells[_dataGridView.Columns["PatientIDColumn"].Index].Value = patientID;
            newRow.Cells[_dataGridView.Columns["StudyInstanceUIDColumn"].Index].Value = studyInstanceUID;
            newRow.Cells[_dataGridView.Columns["PatientNameColumn"].Index].Value =
                dataset.GetSingleValueOrDefault(DicomTag.PatientName, "Unknown");
            newRow.Cells[_dataGridView.Columns["PatientDOBColumn"].Index].Value =
                dataset.GetSingleValueOrDefault(DicomTag.PatientBirthDate, DateTime.MinValue).ToString("dd/MM/yyyy");
            newRow.Cells[_dataGridView.Columns["StudyDescriptionColumn"].Index].Value =
                dataset.GetSingleValueOrDefault(DicomTag.StudyDescription, "Unknown");
            newRow.Cells[_dataGridView.Columns["StudyDateColumn"].Index].Value =
                dataset.GetSingleValueOrDefault(DicomTag.StudyDate, DateTime.MinValue).ToString("dd/MM/yyyy");
            newRow.Cells[_dataGridView.Columns["ModalityColumn"].Index].Value = modality;
            newRow.Cells[_dataGridView.Columns["SeriesCountColumn"].Index].Value = 1;
            newRow.Cells[_dataGridView.Columns["ImageCountColumn"].Index].Value = 1;

            _dataGridView.Rows.Add(newRow);
        }

        _onDataChanged();
    }

    private void DataGridView_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (sender is not DataGridView) return;
        e.CellStyle!.BackColor = e.RowIndex % 2 == 0
            ? ColorTranslator.FromHtml("#F5F5F5")
            : ColorTranslator.FromHtml("#FFFFFF");
    }

    private void DataGridView_SelectionChanged(object? sender, EventArgs e)
    {
        if (sender is not DataGridView) return;
        HighlightSamePatientRows();
    }

    private void HighlightSamePatientRows()
    {
        if (_dataGridView.SelectedRows.Count == 0) return;

        string? selectedPatientID = _dataGridView.SelectedRows[0].Cells["PatientIDColumn"].Value?.ToString();
        if (selectedPatientID is null) return;

        foreach (DataGridViewRow row in _dataGridView.Rows)
        {
            row.DefaultCellStyle.BackColor = row.Cells["PatientIDColumn"].Value?.ToString() == selectedPatientID
                ? ColorTranslator.FromHtml("#FFFACD")
                : row.Index % 2 == 0
                    ? ColorTranslator.FromHtml("#F5F5F5")
                    : ColorTranslator.FromHtml("#FFFFFF");
        }
    }
}

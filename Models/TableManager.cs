// Interfaces/TableManager.cs

//using DicomImport.Controllers;
//using DicomModifier;
//using FellowOakDicom;

//namespace DicomImport.Models
//{
//    /// <summary>
//    /// Initializes a new instance of the <see cref="TableManager"/> class.
//    /// </summary>
//    /// <param name="dataGridView">The data grid view.</param>
//    /// <param name="uiController">The UI controller.</param>
//    public class TableManager(DataGridView dataGridView, UIController uiController)
//    {
//        private readonly DataGridView _dataGridView = dataGridView;
//        private readonly UIController _uiController = uiController;

//        /// <summary>
//        /// Adds the DICOM files to grid.
//        /// </summary>
//        /// <param name="dataset">The dataset.</param>
//        public void AddDicomToGrid(DicomDataset dataset)
//        {
//            string patientID = dataset.GetSingleValueOrDefault(DicomTag.PatientID, "Unknown");
//            string studyInstanceUID = dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, "Unknown");
//            string seriesInstanceUID = dataset.GetSingleValueOrDefault(DicomTag.SeriesInstanceUID, "Unknown");
//            string modality = dataset.GetSingleValueOrDefault(DicomTag.Modality, "Unknown");

//            DataGridViewRow? existingRow = _dataGridView.Rows
//                .Cast<DataGridViewRow>()
//                .FirstOrDefault(row => row.Cells["PatientIDColumn"].Value?.ToString() == patientID &&
//                                       row.Cells["StudyInstanceUIDColumn"].Value?.ToString() == studyInstanceUID);

//            if (existingRow != null)
//            {
//                existingRow.Cells[_dataGridView.Columns["SeriesCountColumn"].Index].Value =
//                    (int)existingRow.Cells[_dataGridView.Columns["SeriesCountColumn"].Index].Value + 1;
//                existingRow.Cells[_dataGridView.Columns["ImageCountColumn"].Index].Value =
//                    (int)existingRow.Cells[_dataGridView.Columns["ImageCountColumn"].Index].Value + 1;

//                string existingModality = existingRow.Cells[_dataGridView.Columns["ModalityColumn"].Index].Value?.ToString() ?? string.Empty;
//                if (!existingModality.Contains(modality))
//                {
//                    existingRow.Cells[_dataGridView.Columns["ModalityColumn"].Index].Value = $"{existingModality}+{modality}";
//                }
//            }
//            else
//            {
//                DataGridViewRow newRow = new();
//                newRow.CreateCells(_dataGridView);

//                newRow.Cells[_dataGridView.Columns["PatientIDColumn"].Index].Value = patientID;
//                newRow.Cells[_dataGridView.Columns["StudyInstanceUIDColumn"].Index].Value = studyInstanceUID;
//                newRow.Cells[_dataGridView.Columns["PatientNameColumn"].Index].Value = dataset.GetSingleValueOrDefault(DicomTag.PatientName, "Unknown");
//                newRow.Cells[_dataGridView.Columns["PatientDOBColumn"].Index].Value = dataset.GetSingleValueOrDefault(DicomTag.PatientBirthDate, DateTime.MinValue).ToString("dd/MM/yyyy");
//                newRow.Cells[_dataGridView.Columns["StudyDescriptionColumn"].Index].Value = dataset.GetSingleValueOrDefault(DicomTag.StudyDescription, "Unknown");
//                newRow.Cells[_dataGridView.Columns["StudyDateColumn"].Index].Value = dataset.GetSingleValueOrDefault(DicomTag.StudyDate, DateTime.MinValue).ToString("dd/MM/yyyy");
//                newRow.Cells[_dataGridView.Columns["ModalityColumn"].Index].Value = modality;
//                newRow.Cells[_dataGridView.Columns["SeriesCountColumn"].Index].Value = 1;
//                newRow.Cells[_dataGridView.Columns["ImageCountColumn"].Index].Value = 1;

//                _dataGridView.Rows.Add(newRow);
//            }

//            // Update control states after adding a row
//            var mainForm = _dataGridView.FindForm() as MainForm;
//            _uiController.UpdateControlStates();
//        }
//    }
//}

using DicomImport.Controllers;
using DicomModifier;
using FellowOakDicom;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DicomImport.Models
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TableManager"/> class.
    /// </summary>
    /// <param name="dataGridView">The data grid view.</param>
    /// <param name="uiController">The UI controller.</param>
    public class TableManager
    {
        private readonly DataGridView _dataGridView;
        private readonly UIController _uiController;

        public TableManager(DataGridView dataGridView, UIController uiController)
        {
            _dataGridView = dataGridView;
            _uiController = uiController;
            _dataGridView.CellFormatting += DataGridView_CellFormatting;
            _dataGridView.SelectionChanged += DataGridView_SelectionChanged;
        }

        /// <summary>
        /// Adds the DICOM files to grid.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        public void AddDicomToGrid(DicomDataset dataset)
        {
            string patientID = dataset.GetSingleValueOrDefault(DicomTag.PatientID, "Unknown");
            string studyInstanceUID = dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, "Unknown");
            string seriesInstanceUID = dataset.GetSingleValueOrDefault(DicomTag.SeriesInstanceUID, "Unknown");
            string modality = dataset.GetSingleValueOrDefault(DicomTag.Modality, "Unknown");

            DataGridViewRow? existingRow = _dataGridView.Rows
                .Cast<DataGridViewRow>()
                .FirstOrDefault(row => row.Cells["PatientIDColumn"].Value?.ToString() == patientID &&
                                       row.Cells["StudyInstanceUIDColumn"].Value?.ToString() == studyInstanceUID);

            if (existingRow != null)
            {
                if (existingRow.Cells[_dataGridView.Columns["SeriesCountColumn"].Index].Value is int seriesCount)
                {
                    existingRow.Cells[_dataGridView.Columns["SeriesCountColumn"].Index].Value = seriesCount + 1;
                }

                if (existingRow.Cells[_dataGridView.Columns["ImageCountColumn"].Index].Value is int imageCount)
                {
                    existingRow.Cells[_dataGridView.Columns["ImageCountColumn"].Index].Value = imageCount + 1;
                }

                string existingModality = existingRow.Cells[_dataGridView.Columns["ModalityColumn"].Index].Value?.ToString() ?? string.Empty;
                if (!existingModality.Contains(modality))
                {
                    existingRow.Cells[_dataGridView.Columns["ModalityColumn"].Index].Value = $"{existingModality}+{modality}";
                }
            }
            else
            {
                DataGridViewRow newRow = new();
                newRow.CreateCells(_dataGridView);

                newRow.Cells[_dataGridView.Columns["PatientIDColumn"].Index].Value = patientID;
                newRow.Cells[_dataGridView.Columns["StudyInstanceUIDColumn"].Index].Value = studyInstanceUID;
                newRow.Cells[_dataGridView.Columns["PatientNameColumn"].Index].Value = dataset.GetSingleValueOrDefault(DicomTag.PatientName, "Unknown");
                newRow.Cells[_dataGridView.Columns["PatientDOBColumn"].Index].Value = dataset.GetSingleValueOrDefault(DicomTag.PatientBirthDate, DateTime.MinValue).ToString("dd/MM/yyyy");
                newRow.Cells[_dataGridView.Columns["StudyDescriptionColumn"].Index].Value = dataset.GetSingleValueOrDefault(DicomTag.StudyDescription, "Unknown");
                newRow.Cells[_dataGridView.Columns["StudyDateColumn"].Index].Value = dataset.GetSingleValueOrDefault(DicomTag.StudyDate, DateTime.MinValue).ToString("dd/MM/yyyy");
                newRow.Cells[_dataGridView.Columns["ModalityColumn"].Index].Value = modality;
                newRow.Cells[_dataGridView.Columns["SeriesCountColumn"].Index].Value = 1;
                newRow.Cells[_dataGridView.Columns["ImageCountColumn"].Index].Value = 1;

                _dataGridView.Rows.Add(newRow);
            }

            // Update control states after adding a row
            var mainForm = _dataGridView.FindForm() as MainForm;
            _uiController.UpdateControlStates();
        }

        private void DataGridView_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (sender is not DataGridView) return;

            if (e.RowIndex % 2 == 0)
            {
                e.CellStyle!.BackColor = ColorTranslator.FromHtml("#F5F5F5");
            }
            else
            {
                e.CellStyle!.BackColor = ColorTranslator.FromHtml("#FFFFFF");
            }
        }

        private void DataGridView_SelectionChanged(object? sender, EventArgs e)
        {
            if (sender is not DataGridView) return;

            HighlightSamePatientRows();
        }

        private void HighlightSamePatientRows()
        {
            if (_dataGridView.SelectedRows.Count > 0)
            {
                string? selectedPatientID = _dataGridView.SelectedRows[0].Cells["PatientIDColumn"].Value?.ToString();
                if (selectedPatientID != null)
                {
                    foreach (DataGridViewRow row in _dataGridView.Rows)
                    {
                        if (row.Cells["PatientIDColumn"].Value?.ToString() == selectedPatientID)
                        {
                            row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFFACD");
                        }
                        else
                        {
                            row.DefaultCellStyle.BackColor = row.Index % 2 == 0 ? ColorTranslator.FromHtml("#F5F5F5") : ColorTranslator.FromHtml("#FFFFFF");
                        }
                    }
                }
            }
        }

    }
}

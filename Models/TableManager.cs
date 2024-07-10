﻿using FellowOakDicom;

namespace DicomModifier.Models
{
    public class TableManager
    {
        private readonly DataGridView _dataGridView;
        private readonly UIController _uiController;

        public TableManager(DataGridView dataGridView, UIController uiController)
        {
            _dataGridView = dataGridView;
            _uiController = uiController;
        }

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
                existingRow.Cells[_dataGridView.Columns["SeriesCountColumn"].Index].Value =
                    (int)existingRow.Cells[_dataGridView.Columns["SeriesCountColumn"].Index].Value + 1;
                existingRow.Cells[_dataGridView.Columns["ImageCountColumn"].Index].Value =
                    (int)existingRow.Cells[_dataGridView.Columns["ImageCountColumn"].Index].Value + 1;

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

        public void ClearTable()
        {
            _dataGridView.Rows.Clear();

            // Update control states after clearing the table
            var mainForm = _dataGridView.FindForm() as MainForm;
            _uiController.UpdateControlStates();

        }
    }
}

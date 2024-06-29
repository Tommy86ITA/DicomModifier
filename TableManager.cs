using FellowOakDicom;
using System.Globalization;

namespace DicomModifier
{
    public class TableManager
    {
        private DataGridView _dataGridView;

        public TableManager(DataGridView dataGridView)
        {
            _dataGridView = dataGridView;
        }

        public void AddDicomToGrid(DicomDataset dicomData)
        {
            var name = dicomData.GetString(DicomTag.PatientName);
            var dob = FormatDate(dicomData.GetString(DicomTag.PatientBirthDate));
            var id = dicomData.GetString(DicomTag.PatientID);
            var studyDescription = dicomData.GetString(DicomTag.StudyDescription);
            var studyDate = FormatDate(dicomData.GetString(DicomTag.StudyDate));
            var modality = dicomData.GetString(DicomTag.Modality);
            var seriesNumber = dicomData.GetString(DicomTag.SeriesNumber);
            var imageNumber = dicomData.GetString(DicomTag.InstanceNumber);

            _dataGridView.Rows.Add(name, dob, id, studyDescription, studyDate, modality, seriesNumber, imageNumber);
        }

        private string FormatDate(string dicomDate)
        {
            if (string.IsNullOrEmpty(dicomDate))
            {
                return string.Empty;
            }

            if (DateTime.TryParseExact(dicomDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date.ToString("dd/MM/yyyy");
            }
            return dicomDate;
        }
    }
}

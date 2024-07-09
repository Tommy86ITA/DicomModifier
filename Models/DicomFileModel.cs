namespace DicomModifier.Models
{
    public class DicomFileModel
    {
        public string FilePath { get; set; }
        public string? PatientID { get; set; }
        public string StudyInstanceUID { get; set; }
        public string SeriesInstanceUID { get; set; }
        public string? Modality { get; set; }
    }
}

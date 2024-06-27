using System;
using System.Collections.Generic;
using FellowOakDicom;

namespace DicomModifier
{
    public class DicomManager
    {
        private Queue<string> dicomQueue;

        public DicomManager()
        {
            dicomQueue = new Queue<string>();
        }

        public void AddDicomFile(string filePath)
        {
            dicomQueue.Enqueue(filePath);
        }

        public DicomFile GetNextDicomFile()
        {
            if (dicomQueue.Count > 0)
            {
                string filePath = dicomQueue.Dequeue();
                return DicomFile.Open(filePath);
            }
            return null;
        }
    }
}

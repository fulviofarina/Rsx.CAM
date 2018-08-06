using System;

namespace Rsx.CAM
{
    public interface IDetectorX
    {
        bool Acquiring { get; set; }
        double Area { get; set; }
        double AreaUnc { get; set; }

        bool Controlled { get; set; }

        double CountTime { get; set; }

        string Description1 { get; set; }

        string Description2 { get; set; }

        string Description3 { get; set; }

        string Description4 { get; set; }

        string DetectorCodeName { get; set; }

        decimal DT { get; set; }

        SystemException Exception { get; set; }

        double Integral { get; set; }

        System.IO.FileInfo LastFileInfo { get; set; }

        string LastFileTag { get; set; }

        DateTime LastStart { get; set; }

        double LiveTime { get; set; }

        bool Opened { get; set; }

        double PresetTime { get; set; }

        string SampleTitle { get; set; }

        string Server { get; set; }

        DateTime StartDate { get; set; }

        string User { get; set; }

        double VDMDelay { get; set; }

        void Clear();
        void Dispose();
        bool GetROI(double energyStart, double energyEnd, double backCh, double limitUnc);

        void GetSampleData();

        void GetTimes();
        void IsAcquiring();
        bool Save(string measurementPath);
        void SetSampleData(string user, string project, string sample, string measlabel);

        void Start();
        void Stop();
    }
}
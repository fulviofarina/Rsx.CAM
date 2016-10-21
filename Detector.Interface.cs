using System;

namespace Rsx.CAM
{
  public interface IDetectorX
  {
    bool Acquiring { get; set; }
    double Area { get; set; }
    double AreaUnc { get; set; }

    void Clear();

    bool Controlled { get; set; }
    double CountTime { get; set; }
    string DetectorCodeName { get; set; }

    void Dispose();

    decimal DT { get; set; }
    SystemException Exception { get; set; }

    bool GetROI(double energyStart, double energyEnd, double backCh, double limitUnc);

    void GetTimes();

    double Integral { get; set; }

    void IsAcquiring();

    DateTime LastStart { get; set; }
    double LiveTime { get; set; }
    bool Opened { get; set; }
    double PresetTime { get; set; }

    bool Save(string measurementPath);

    string Server { get; set; }

    void Start();

    DateTime StartDate { get; set; }

    void Stop();

    double VDMDelay { get; set; }
    System.IO.FileInfo LastFileInfo { get; set; }
    string LastFileTag { get; set; }
    string SampleTitle { get; set; }
    string User { get; set; }
    string Description1 { get; set; }
    string Description2 { get; set; }
    string Description3 { get; set; }
    string Description4 { get; set; }

    void GetSampleData();

    void SetSampleData(string user, string project, string sample, string measlabel);
  }
}
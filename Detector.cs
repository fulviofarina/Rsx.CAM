﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Rsx.CAM
{
    public class DetectorX : IDetectorX
    {
        private bool acquiring = false;
        private double area;
        private double areaUnc;
        private CAMSRCLib.OpenOptions camControl = CAMSRCLib.OpenOptions.camReadWrite;
        private CAMSRCLib.OpenOptions camReadonly = CAMSRCLib.OpenOptions.camReadOnly;
        private double countTime;
        private string description1;
        private string description2;
        private string description3;
        private string description4;
        private string detectorCodeName = string.Empty;
        private double dT;
        private SystemException exception;
        private double integral;
        private System.IO.FileInfo lastFileInfo;
        private string lastFileTag;

        private DateTime lastStart;

        private double liveTime;

        private bool opened = false;

        private double presetTime;

        private CAMSRCLib.CamDatasourceClass Reader;

        private string sampleTitle;

        private string server;

        private DateTime startDate;

        private CAMSRCLib.SourceTypes type = CAMSRCLib.SourceTypes.camSpectroscopyDetector;

        private string user;

        private double vDMDelay = 0;

        private bool writable = false;

        private CAMSRCLib.CamDatasourceClass Writer;

        public bool Acquiring
        {
            get { return acquiring; }
            set { acquiring = value; }
        }

        public double Area
        {
            get { return area; }
            set { area = value; }
        }

        public double AreaUnc
        {
            get { return areaUnc; }
            set { areaUnc = value; }
        }

        public bool Controlled
        {
            get { return writable; }
            set { writable = value; }
        }

        public double CountTime
        {
            get { return countTime; }
            set { countTime = value; }
        }

        public string Description1
        {
            get { return description1; }
            set { description1 = value; }
        }

        public string Description2
        {
            get { return description2; }
            set { description2 = value; }
        }

        public string Description3
        {
            get { return description3; }
            set { description3 = value; }
        }

        public string Description4
        {
            get { return description4; }
            set { description4 = value; }
        }

        public string DetectorCodeName
        {
            get { return detectorCodeName; }
            set { detectorCodeName = value; }
        }

        public decimal DT
        {
            get
            {
                this.dT = 0;
                if (countTime != 0)
                {
                    this.dT = (1 - (liveTime / countTime));
                    this.dT = dT * 100;
                }
                return Decimal.Round(Convert.ToDecimal(dT), 1);
            }
            set { dT = (double)value; }
        }

        public SystemException Exception
        {
            get { return exception; }
            set { exception = value; }
        }

        public double Integral
        {
            get { return integral; }
            set { integral = value; }
        }

        public System.IO.FileInfo LastFileInfo
        {
            get { return lastFileInfo; }
            set { lastFileInfo = value; }
        }

        public string LastFileTag
        {
            get
            {
                return lastFileTag;
            }
            set { lastFileTag = value; }
        }
        public DateTime LastStart
        {
            get { return lastStart; }
            set { lastStart = value; }
        }

        public double LiveTime
        {
            get { return liveTime; }
            set { liveTime = value; }
        }

        public bool Opened
        {
            get { return opened; }
            set { opened = value; }
        }

        public double PresetTime
        {
            get { return presetTime; }
            set { presetTime = value; }
        }

        public string SampleTitle
        {
            get { return sampleTitle; }
            set { sampleTitle = value; }
        }
        public string Server
        {
            get { return server; }
            set { server = value; }
        }

        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        public string User
        {
            get { return user; }
            set { user = value; }
        }

        public double VDMDelay
        {
            get
            {
                this.vDMDelay = (lastStart - startDate).TotalSeconds;

                return vDMDelay;
            }
            set { vDMDelay = value; }
        }

        public static void KillVDM(string AServer)
        {
            if (System.Environment.MachineName.ToUpper().CompareTo(AServer) == 0) return;
            IEnumerable<System.Diagnostics.Process> processes = System.Diagnostics.Process.GetProcesses();
            System.Diagnostics.Process VDM = processes.Where(p => p.ProcessName.Contains("winvdm")).FirstOrDefault();
            if (VDM != null) VDM.Kill();
        }

        public static void NewLink(ref IDetectorX link)
        {
            if (link != null) link.Dispose();
            link = null;
            link = new DetectorX();
        }

        public void Clear()
        {
            OpenWrite();

            if (writable)
            {
                try
                {
                    Writer.Device.ExecuteCommand(CAMSRCLib.DeviceCommands.camClear);
                    if (acquiring) lastStart = DateTime.Now;
                }
                catch (SystemException ex)
                {
                    exception = ex;
                }
            }
            CloseWrite();
        }

        public void Dispose()
        {
            CloseRead();
            CloseWrite();
            Reader = null;
            Writer = null;
        }

        public bool GetROI(double energyStart, double energyEnd, double backCh, double limitUnc)
        {
            bool success = false;

            CAMSRCLib.CamDatasourceClass ReaderAux = new CAMSRCLib.CamDatasourceClass();

            exception = null;
            try
            {
                ReaderAux.OpenEx(detectorCodeName, this.camReadonly, type, this.server);

                /*
             object o = device.get_Information( CAMSRCLib.InformationType.camName);
             object time =  device.get_Parameter(CanberraDataAccessLib.ParamCodes.CAM_X_EREAL, 0, 0);
             object roi = device.get_Parameter(CanberraDeviceAccessLib.ParamCodes.CAM_L_SHOWROIS, 0, 0);
             object f = o;
                 o = device.get_Parameter(CanberraDeviceAccessLib.ParamCodes.CAM_L_EXPROIS, 0, 0);

                 */
            }
            catch (SystemException ex)
            {
                exception = ex;
            }
            try
            {
                object[] argsIn = new object[] { energyStart, energyEnd, backCh, 1, true };
                object result = ReaderAux.Evaluate.Evaluate(CAMSRCLib.EvaluateOptions.camRoiInformation, argsIn);
                double[] args = result as double[];
                if (args[2] < limitUnc) success = true;
                this.area = args[1];
                this.integral = args[0];
                this.areaUnc = args[2];
                this.countTime = (double)ReaderAux.get_Parameter(CanberraDataAccessLib.ParamCodes.CAM_X_EREAL, 0, 0);
            }
            catch (SystemException ex)
            {
                exception = ex;
            }
            try
            {
                ReaderAux.Close();
            }
            catch (SystemException ex)
            {
                exception = ex;
            }

            ReaderAux = null;

            return success;
        }

        /// <summary>
        /// Gets the info inside the Sample Info of the detector
        /// </summary>
        public void GetSampleData()
        {
            OpenRead();

            if (opened)
            {
                try
                {
                    sampleTitle = (string)Reader.get_Parameter(CanberraDataAccessLib.ParamCodes.CAM_T_STITLE);
                    user = (string)Reader.get_Parameter(CanberraDataAccessLib.ParamCodes.CAM_T_SCOLLNAME);
                    description1 = (string)Reader.get_Parameter(CanberraDataAccessLib.ParamCodes.CAM_T_SDESC1);
                    description2 = (string)Reader.get_Parameter(CanberraDataAccessLib.ParamCodes.CAM_T_SDESC2);
                    description3 = (string)Reader.get_Parameter(CanberraDataAccessLib.ParamCodes.CAM_T_SDESC3);
                    description4 = (string)Reader.get_Parameter(CanberraDataAccessLib.ParamCodes.CAM_T_SDESC4);
                }
                catch (SystemException ex)
                {
                    exception = ex;
                }
            }
            CloseRead();
        }

        public void GetTimes()
        {
            OpenRead();
            this.startDate = DateTime.MinValue;
            this.liveTime = 0;
            this.countTime = 0;
            this.presetTime = 0;

            if (opened)
            {
                try
                {
                    this.countTime = Convert.ToDouble(Reader.get_Parameter(CanberraDataAccessLib.ParamCodes.CAM_X_EREAL));
                    lastStart = DateTime.Now.Subtract(new TimeSpan(0, 0, Convert.ToInt32(this.countTime)));
                    this.liveTime = Convert.ToDouble(Reader.get_Parameter(CanberraDataAccessLib.ParamCodes.CAM_X_ELIVE));
                    this.presetTime = Convert.ToDouble(Reader.get_Parameter(CanberraDataAccessLib.ParamCodes.CAM_X_PREAL));
                    this.startDate = Convert.ToDateTime(Reader.get_Parameter(CanberraDeviceAccessLib.ParamCodes.CAM_X_ASTIME));
                }
                catch (SystemException ex)
                {
                    exception = ex;
                }
            }
            CloseRead();
        }

        public void IsAcquiring()
        {
            OpenRead();

            this.acquiring = false;
            if (opened)
            {
                try
                {
                    int status = Convert.ToInt16(Reader.Status.ToString());
                    //stoped and cleared
                    // if (status == 2080)
                    //started (acquiring)	//2052 when acquiring?
                    //status == 2084 when I take control?
                    if (status == 2052 || status == 2084) this.acquiring = true;
                }
                catch (SystemException x)
                {
                    exception = x;
                }
            }

            CloseRead();
        }

        public bool Save(string measurementPath)
        {
            OpenWrite();
            bool exists = false;

            if (writable)
            {
                try
                {
                    Writer.Save(measurementPath, true);
                    exists = System.IO.File.Exists(measurementPath);
                    lastFileInfo = null;
                    lastFileTag = string.Empty;

                    if (exists)
                    {
                        lastFileInfo = new System.IO.FileInfo(measurementPath);
                        System.IO.FileInfo f = this.lastFileInfo;
                        lastFileTag = "Saved on: " + f.LastWriteTime + "\nSize: " + f.Length + "\nPath: " + f.FullName;

                        long size = lastFileInfo.Length;
                        if (size < 50000)
                        {
                            throw new SystemException("The size of the spectra file " + measurementPath + " is less than 50Kb (" + size + " bytes), which does not look good.\nPlease verify the VDM is not blocked, by closing and reopening (Genie and then the VDM) on the server computer.\nPlease do it while this program is turned off.");
                        }
                    }
                }
                catch (SystemException ex)
                {
                    exception = ex;
                }
            }
            CloseWrite();

            return exists;
        }

        public void SetSampleData(string user, string project, string sample, string measlabel)
        {
            OpenWrite();
            // bool exists = false;

            if (writable)
            {
                try
                {
                    this.Writer.set_Parameter(CanberraDataAccessLib.ParamCodes.CAM_T_STITLE, 0, 0, detectorCodeName);
                    this.Writer.set_Parameter(CanberraDataAccessLib.ParamCodes.CAM_T_SCOLLNAME, 0, 0, user);
                    this.Writer.set_Parameter(CanberraDataAccessLib.ParamCodes.CAM_T_STYPE, 0, 0, string.Empty);
                    this.Writer.set_Parameter(CanberraDataAccessLib.ParamCodes.CAM_T_SIDENT, 0, 0, string.Empty);
                    this.Writer.set_Parameter(CanberraDataAccessLib.ParamCodes.CAM_T_SDESC1, 0, 0, detectorCodeName);
                    this.Writer.set_Parameter(CanberraDataAccessLib.ParamCodes.CAM_T_SDESC2, 0, 0, project);
                    this.Writer.set_Parameter(CanberraDataAccessLib.ParamCodes.CAM_T_SDESC3, 0, 0, sample);
                    this.Writer.set_Parameter(CanberraDataAccessLib.ParamCodes.CAM_T_SDESC4, 0, 0, measlabel);
                }
                catch (SystemException ex)
                {
                    exception = ex;
                }
            }
            CloseWrite();
        }
        public void Start()
        {
            OpenWrite();

            if (writable)
            {
                try
                {
                    Writer.Device.ExecuteCommand(CAMSRCLib.DeviceCommands.camStart);
                    acquiring = true;
                    lastStart = DateTime.Now;
                }
                catch (SystemException ex)
                {
                    exception = ex;
                }
            }
            CloseWrite();
        }

        public void Stop()
        {
            OpenWrite();
            if (writable)
            {
                try
                {
                    Writer.Device.ExecuteCommand(CAMSRCLib.DeviceCommands.camStop);
                    acquiring = false;
                }
                catch (SystemException ex)
                {
                    exception = ex;
                }
            }
            CloseWrite();
        }

        private void CloseRead()
        {
            // if (!opened) return;

            try
            {
                Reader.Close();
                opened = false;
            }
            catch (SystemException x)
            {
                exception = x;
            }
        }

        private void CloseWrite()
        {
            // if (!writable) return;
            try
            {
                Writer.Close();
                writable = false;
            }
            catch (SystemException x)
            {
                exception = x;
            }
        }

        private void OpenRead()
        {
            exception = null;
            opened = false;
            try
            {
                Reader.OpenEx(detectorCodeName, this.camReadonly, type, this.server);
                opened = true;
            }
            catch (SystemException x)
            {
                exception = x;
            }
        }

        private void OpenWrite()
        {
            exception = null;
            writable = false;
            try
            {
                Writer.OpenEx(detectorCodeName, this.camControl, type, this.server);
                writable = true;
            }
            catch (SystemException ex)
            {
                exception = ex;
            }
        }

        public DetectorX()
        {
            try
            {
                Reader = new CAMSRCLib.CamDatasourceClass();
                Writer = new CAMSRCLib.CamDatasourceClass();
            }
            catch (SystemException EX)
            {
                exception = EX;
            }
        }
    }
}
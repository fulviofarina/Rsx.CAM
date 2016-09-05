using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Rsx.CAM
{
    public class CamFileReader
    {
        private CanberraDataAccessLib.DataAccess dataAccess;
        private string fileName = string.Empty;

        // Aquisition parameters
        private string _aSTIME; // Acquisition start time

        private double _pLIVE; // Preset live time
        private double _pREAL; // Preset real time
        private double _eLIVE; // Elapsed live time
        private double _eREAL; // Elapsed real time

        private string _detectorName; // detector name

        public CamFileReader()
        {
            this.dataAccess = new CanberraDataAccessLib.DataAccess();
        }

        public bool GetData(string fileName)
        {
            bool succes = false;
            this._exception = null;

            this.fileName = fileName;
            try
            {
                dataAccess.Open(this.fileName, CanberraDataAccessLib.OpenMode.dReadOnly, 8192);
            }
            catch (Exception ex)
            {
                //_exception = ex; //nothing should be here because always generates exception
            }
            try
            {
                this._aSTIME = dataAccess.get_Param(CanberraDataAccessLib.ParamCodes.CAM_X_ASTIME, 0, 0).ToString();
                this._pLIVE = Convert.ToDouble(dataAccess.get_Param(CanberraDataAccessLib.ParamCodes.CAM_X_PLIVE));
                this._pREAL = Convert.ToDouble(dataAccess.get_Param(CanberraDataAccessLib.ParamCodes.CAM_X_PREAL));
                this._eLIVE = Convert.ToDouble(dataAccess.get_Param(CanberraDataAccessLib.ParamCodes.CAM_X_ELIVE));
                this._eREAL = Convert.ToDouble(dataAccess.get_Param(CanberraDataAccessLib.ParamCodes.CAM_X_EREAL));
                this._detectorName = dataAccess.get_Param(CanberraDataAccessLib.ParamCodes.CAM_T_DETNAME).ToString();

                succes = true;
            }
            catch (Exception ex)
            {
                _exception = ex;
            }

            try
            {
                dataAccess.Close(CanberraDataAccessLib.CloseMode.dNoUpdate);
            }
            catch (Exception ex)
            {
                _exception = ex;
            }

            return succes;
        }

        private Exception _exception;

        public Exception Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }

        public string ASTIME
        {
            get { return _aSTIME; }
        }

        public double PLIVE
        {
            get { return _pLIVE; }
        }

        public double PREAL
        {
            get { return _pREAL; }
        }

        public double EREAL
        {
            get { return _eREAL; }
        }

        public double ELIVE
        {
            get { return _eLIVE; }
        }

        public string DetectorName
        {
            get { return _detectorName; }
        }
    }

  
}
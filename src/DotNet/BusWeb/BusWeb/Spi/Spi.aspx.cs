using Bus.Model;
using BusWeb.DAO;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Threading;
using System.Web.UI;

namespace BusWeb.Spi
{
    public partial class Spi : Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Spi));
        private static Thread _mergeDataThread = null;
        private static readonly HashSet<string> _preventDupSet = new HashSet<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            object rtn = null;
            Response.Clear();
            string spiName = Request["spi"];
            switch (spiName)
            {
                case "gnv":  // get new version
                    rtn = getNewVersion();
                    break;
                case "inl": // insert new line
                    rtn = insertNewLine(Request["linename"],
                        string.Format("{0}-{1}", Request["lang"], Request["country"])
                        );
                    break;
                case "ins": // insert new stop
                    insertNewStop(int.Parse(Request["lineid"]),
                        Request["stopname"],
                        double.Parse(Request["longitude"]),
                        double.Parse(Request["latitude"]),
                        double.Parse(Request["curLongitude"]),
                        double.Parse(Request["curLatitude"]),
                        Request["owner"],
                        string.Format("{0}-{1}", Request["lang"], Request["country"]));
                    break;
                case "gll": // get line list
                    rtn = getLineList(double.Parse(Request["longitude"]),
                        double.Parse(Request["latitude"]),
                        double.Parse(Request["radius"]),
                        Request["owner"],
                        1);
                    break;
                case "gsbl":    // get stop by line
                    rtn = getStopsByLineID(int.Parse(Request["lineid"]), Request["owner"]);
                    break;
                case "rs":  // rate stop
                    rateStop(int.Parse(Request["stopid"]),
                        Request["gb"]);
                    break;
                case "ds":  // delete stop by owner
                    rtn = deleteStop(int.Parse(Request["stopid"]), Request["owner"]);
                    break;
            }
            if (null != rtn) Response.Write(JsonConvert.SerializeObject(rtn));
            Response.End();
        }

        private string getNewVersion()
        {
            return ConfigurationManager.AppSettings["LatestVersion"];
        }

#if(DEBUG)
        public int insertNewLine(string lineName, string culture)
#else
        private int insertNewLine(string lineName, string culture)
#endif
        {
            _log.Info(MethodInfo.GetCurrentMethod().Name);
            var db = BusWebDataService.GetServiceInstance();
            {
                return db.InsertNewLine(lineName, culture);
            }
        }
#if(DEBUG)
        public void insertNewStop(int lineID, string stopName, double longitude, double latitude, double curLongitude, double curLatitude, string owner, string culture)
#else
        private void insertNewStop(int lineID, string stopName, double longitude, double latitude, double curLongitude, double curLatitude, string owner, string culture)
#endif
        {
            _log.Info(MethodInfo.GetCurrentMethod().Name);
            #region issue #38 http://dowill-svr/btnet/edit_bug.aspx?id=38
            string hashData = string.Format("{0}{1}{2}{3}{4}", lineID, stopName, longitude, latitude, owner);
            lock (_preventDupSet)
            {
                if (_preventDupSet.Contains(hashData))
                {
                    _preventDupSet.Clear(); 
                    return;
                }
                _preventDupSet.Add(hashData);
            }
            #endregion

            var db = BusWebDataService.GetServiceInstance();
            int StopID = db.InsertNewStop(stopName, longitude, latitude, curLongitude, curLatitude, ((string.IsNullOrEmpty(owner)) ? "SYSTEM" : owner), culture);
            db.InsertNewLine2StopRelation(lineID, StopID);



            #region Do remove bad rating stops and data merging
            lock (typeof(Spi))
            {
                if (null == _mergeDataThread || 
                    _mergeDataThread.ThreadState != ThreadState.Running)
                {
                    _mergeDataThread = new Thread(new ParameterizedThreadStart(removeBadRatingAndMergeLines));
                    _mergeDataThread.Start(db);
                }
            }
            #endregion
        }

        private void removeBadRatingAndMergeLines(object obj)
        {
            _log.Info(MethodInfo.GetCurrentMethod().Name);
            BusWebDataService db = (BusWebDataService)obj;
            //db.RemoveBadRatingStops();
            //db.MergeLines();
        }
#if(DEBUG)
        public LineInfo[] getLineList(double Longitude, double Latitude, double radius, string owner, byte device)
#else
        private LineInfo[] getLineList(double Longitude, double Latitude, double radius, string owner, byte device)
#endif
        {
            _log.Info(MethodInfo.GetCurrentMethod().Name);
            List<LineInfo> list = new List<LineInfo>();
            var db = BusWebDataService.GetServiceInstance();
            {
                db.LogAUsageCase(owner, Longitude, Latitude, radius, device);
                var dt = db.GetLinesList(Longitude, Latitude, radius, owner);
                foreach (var dr in dt) list.Add(new LineInfo() { LineID = dr.LineID, LineName = dr.LineName });
            }
            return list.ToArray();
        }

        private StopInfo[] getStopsByLineID(int lineID, string owner)
        {
            _log.Info(MethodInfo.GetCurrentMethod().Name);
            List<StopInfo> list = new List<StopInfo>();
            var db = BusWebDataService.GetServiceInstance();
            {
                var dt = db.GetStopsByLineID(lineID, owner);
                foreach (var dr in dt) list.Add(new StopInfo()
                {
                    StopID = dr.StopID,
                    StopName = dr.StopName,
                    Latitude = dr.Latitude,
                    Longtitude = dr.Longitude,
                    Bad = dr.RatingBad,
                    Good = dr.RatingGood
                });
            }
            return list.ToArray();
        }

        private void rateStop(int stopID, string goodOrBad)
        {
            _log.Info(MethodInfo.GetCurrentMethod().Name);
            var db = BusWebDataService.GetServiceInstance();
                switch (goodOrBad)
                {
                    case "g":
                        db.RateStopGood(stopID);
                        break;
                    case "b":
                        db.RateStopBad(stopID);
                        break;
                }
        }
#if(DEBUG)
        public bool deleteStop(int stopID, string owner)
#else
        private bool deleteStop(int stopID, string owner)
#endif
        {
            _log.Info(MethodInfo.GetCurrentMethod().Name);
            bool success = false;
            var db = BusWebDataService.GetServiceInstance();
                if (0 != db.GetStopIDByOwnerAndStopID(stopID, owner))
                {
                    db.DeleteStopByID(stopID);
                    success = true;
                }
                else
                {
                    success = false;
                }
            return success;
        }
    }
}
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusWeb.DAO
{
    public class BusWebDataService
    {
        protected static readonly ILog _log = LogManager.GetLogger(typeof(BusWebDataService));
        protected static readonly double DELTA_RADIUS = GetDeltaRadius(10); // Get coordinator delta of 10 KM

        /// <summary>
        /// Retrieve a new service instance. It's thread-safe.
        /// </summary>
        /// <returns>Database service instance</returns>
        public static BusWebDataService GetServiceInstance()
        {
            // TODO: Enable copyright validation.
            //CopyrightChecker.CheckCopyright(Assembly.GetCallingAssembly());
            return new BusWebDataService();
        }

        public void LogAUsageCase(string userCode, double longitude, double latitude, double radius, byte device)
        {
            try
            {
                using (var db = new BusStopDataContext())
                    db.LogAUsageCase(userCode, longitude, latitude, radius, device);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public int InsertNewLine(string lineName, string culture)
        {
            int? lineId = 0;
            using (var db = new BusStopDataContext())
                db.InsertNewLine(lineName, culture, ref lineId);
            return lineId ?? 0;
        }

        public void UpdateLine(string lineName, int lineID)
        {
            using (var db = new BusStopDataContext())
                db.UpdateLine(lineID, lineName);
        }

        public int InsertNewStop(string stopName, double longitude, double latitude, double creatorLongitude, double creatorLatitude, string owner, string culture)
        {
            int? stopID = 0;

            using (var db = new BusStopDataContext())
                db.InsertNewStop(stopName, longitude, latitude, creatorLongitude, creatorLatitude, owner, culture, ref stopID);

            return stopID ?? 0;
        }

        public void UpdateStop(string stopName, int stopID)
        {
            using (var db = new BusStopDataContext())
                db.UpdateStop(stopID, stopName);
        }

        public void InsertNewLine2StopRelation(int lineID, int stopID)
        {
            using (var db = new BusStopDataContext())
                db.InsertNewLine2StopRelation(lineID, stopID);
        }

        public IList<GetLinesListResult> GetLinesList(double longitude, double latitude, double radius, string owner)
        {
            using (var db = new BusStopDataContext())
                return db.GetLinesList(owner, latitude, longitude, radius).ToList();
        }

        public IList<GetLinesListAllResult> GetLinesList()
        {
            using (var db = new BusStopDataContext())
                return db.GetLinesListAll().ToList();
        }

        public IList<GetLinesListByDateResult> GetLinesList(DateTime stpBgnDT, DateTime stpEndDT)
        {
            using (var db = new BusStopDataContext())
                return db.GetLinesListByDate(stpBgnDT, stpEndDT).ToList();
        }

        public IList<GetStopsByLineIDResult> GetStopsByLineID(int lineID)
        {
            using (var db = new BusStopDataContext())
                return db.GetStopsByLineID(lineID).ToList();
        }

        public IList<GetStopsByLineIDResult> GetStopsByLineID(int lineID, string owner)
        {
            using (var db = new BusStopDataContext())
            {
                var dt = db.GetStopsByLineID(lineID).ToList();
                if (!string.IsNullOrEmpty(owner))
                    foreach (var dr in dt)
                        if (dr.Owner == owner) dr.StopName = string.Format("{0}(*)", dr.StopName);
                return dt;
            }
        }

        public void RateStopGood(int stopID)
        {
            using (var db = new BusStopDataContext())
                db.RateStopGood(stopID);
        }

        public void RateStopBad(int stopID)
        {
            using (var db = new BusStopDataContext())
                db.RateStopBad(stopID);
        }

        public void DeleteStopByID(int stopID)
        {
            using (var db = new BusStopDataContext())
                db.DeleteStopByID(stopID);
        }

        public int GetStopIDByOwnerAndStopID(int stopID, string owner)
        {
            using (var db = new BusStopDataContext())
            {
                var d = db.GetStopIDByOwnerAndStopID(stopID, owner).FirstOrDefault();
                if (d != null)
                    return d.StopID;
                else
                    return 0;
            }
        }

        public void DeleteLineByID(int lineID)
        {
            using (var db = new BusStopDataContext())
                db.DeleteLineByID(lineID);

        }
        /// <summary>
        /// 
        /// </summary>
        public void RemoveEmptyLines()
        {
            using (var db = new BusStopDataContext())
                db.RemoveEmptyLines();
        }
        #region not implemented
        ///// <summary>
        ///// Remove high-delta bad rating stops
        ///// </summary>
        //public void RemoveBadRatingStops()
        //{
        //    var dt = new DsBusWeb.StopsDataTable();
        //    FillDt("SELECT s.*, sr.RatingGood, sr.RatingBad FROM Stops s INNER JOIN StopRating sr ON s.StopID = sr.StopID WHERE sr.RatingBad > (sr.RatingGood + 1)", dt);
        //    StringBuilder sb = new StringBuilder("There're following stops going to be removed:\r\n");
        //    StringBuilder arrayStringBuilder = new StringBuilder("(");
        //    bool firstElem = true;
        //    foreach (var dr in dt)
        //    {
        //        sb.AppendFormat("StopID={0} / StopName={1} / RatingGood={2} / RatingBad={3} / Owner={4} / DateCreated={5} / Culture={6}\r\n", dr.StopID, dr.StopName, dr["RatingGood"], dr["RatingBad"], dr.Owner, dr.DateCreated.ToString("yyyy-MM-dd HH:mm:ss"), dr.Culture);
        //        if (firstElem) arrayStringBuilder.Append(dr.StopID); else arrayStringBuilder.Append(", " + dr.StopID);
        //        firstElem = false;
        //    }
        //    arrayStringBuilder.Append(")");
        //    _log.Debug(sb);
        //    try
        //    {
        //        _db.BeginTrans();
        //        _db.ExecuteCommand.CommandText = "DELETE FROM StopRating WHERE StopID IN " + arrayStringBuilder.ToString();
        //        _db.Execute();
        //        _db.ExecuteCommand.CommandText = "DELETE FROM Line2Stop WHERE StopID IN " + arrayStringBuilder.ToString();
        //        _db.Execute();
        //        _db.ExecuteCommand.CommandText = "DELETE FROM Stops WHERE StopID IN " + arrayStringBuilder.ToString();
        //        _db.Execute();
        //        _db.CommitTrans();
        //    }
        //    catch
        //    {
        //        _db.RollBack();
        //        throw;
        //    }
        //    finally
        //    {
        //        _db.Close();
        //    }
        //}
        ///// <summary>
        ///// Merge if 2 lines have the same stop inside.
        ///// </summary>
        //public void MergeLines()
        //{
        //    // 1. Get all duplicate line in detail
        //    DsBusWeb.vwDupLineDetailDataTable dt = new DsBusWeb.vwDupLineDetailDataTable();
        //    FillDt("SELECT * FROM vwDupLineDetail", dt);

        //    List<DsBusWeb.vwDupLineDetailRow> dupLines = new List<DsBusWeb.vwDupLineDetailRow>();

        //    #region Scanning for merging process
        //    for (int i = 0; i < dt.Count; i++)
        //    {
        //        if (dt[i].RowState != DataRowState.Deleted) // If the row has not been merged...
        //        {
        //            DsBusWeb.vwDupLineDetailRow primaryLine = dt[i];
        //            dupLines.Clear();
        //            for (int j = i + 1; j < dt.Count; j++)
        //            {
        //                if (dt[j].RowState != DataRowState.Deleted)
        //                {
        //                    var currLine = dt[j];
        //                    if (currLine.LineName == primaryLine.LineName)
        //                    {
        //                        if (isOverlap(primaryLine, currLine)) dupLines.Add(currLine);
        //                    }
        //                    else
        //                        break;
        //                }
        //            }
        //            DoMerging(dupLines, primaryLine);
        //        }
        //    }
        //    #endregion

        //    MergeStops();
        //}
        //private static bool isOverlap(DsBusWeb.vwDupLineDetailRow line1, DsBusWeb.vwDupLineDetailRow line2)
        //{
        //    return !(line2.maxLatitude < (line1.minLatitude - DELTA_RADIUS) ||
        //        line2.minLatitude > (line1.maxLatitude + DELTA_RADIUS) ||
        //        line2.minLongitude > (line1.maxLongitude + DELTA_RADIUS) ||
        //        line2.maxLongitude < (line1.minLongitude - DELTA_RADIUS));
        //}
        ///// <summary>
        ///// Merge 2 lines by their ID. If their names are different to each other, the merging will fail and nothing change.
        ///// </summary>
        ///// <param name="lineId1">The ID of 1st line would be merged</param>
        ///// <param name="lineId2">The ID of 2nd line would be merged</param>
        ///// <returns>Success or failure</returns>
        //public bool MergeLines(int lineId1, int lineId2)
        //{
        //    bool success = false;
        //    int stopCountForLine1 = 0;
        //    stopCountForLine1 = Convert.ToInt32(ExecuteScalar(string.Format("SELECT COUNT(1) FROM Line2Stop WHERE LineID = {0}", lineId1)));
        //    int stopCountForLine2 = 0;
        //    stopCountForLine2 = Convert.ToInt32(ExecuteScalar(string.Format("SELECT COUNT(1) FROM Line2Stop WHERE LineID = {0}", lineId2)));
        //    int originalLineID = 0;
        //    int newLineID = 0;
        //    if (stopCountForLine2 > stopCountForLine1)
        //    {
        //        originalLineID = lineId1;
        //        newLineID = lineId2;
        //    }
        //    else
        //    {
        //        originalLineID = lineId2;
        //        newLineID = lineId1;
        //    }
        //    try
        //    {
        //        _db.BeginTrans();
        //        _db.ExecuteCommand.CommandText = string.Format("UPDATE Line2Stop SET LineID = {0} WHERE LineID = {1}", newLineID, originalLineID);
        //        _db.Execute(); // Move all stop under the line to the primary line.
        //        _db.ExecuteCommand.CommandText = string.Format("DELETE FROM Lines WHERE LineID = {0}", originalLineID);
        //        _db.Execute(); // Remove the empty line
        //        _db.CommitTrans();
        //        success = true;
        //    }
        //    catch
        //    {
        //        _db.RollBack();
        //        throw;
        //    }
        //    finally
        //    {
        //        _db.Close();
        //    }
        //    MergeStops();
        //    success = true;
        //    return success;
        //}
        ///// <summary>
        ///// Move all stops under these duplicated lines, then delete these duplicated lines.
        ///// </summary>
        ///// <param name="dupLines">List of duplicated lines</param>
        ///// <param name="primaryLine">The primary line which will be attached within these moving stops.</param>
        //private void DoMerging(List<DsBusWeb.vwDupLineDetailRow> dupLines, DsBusWeb.vwDupLineDetailRow primaryLine)
        //{
        //    if (dupLines.Count > 0)
        //    {
        //        #region do merging
        //        try
        //        {
        //            _db.BeginTrans();
        //            foreach (var line in dupLines)
        //            {
        //                _db.ExecuteCommand.CommandText = string.Format("UPDATE Line2Stop SET LineID = {0} WHERE LineID = {1}",
        //                    OleDbStrHelper.getParamStr(primaryLine.LineID),
        //                    OleDbStrHelper.getParamStr(line.LineID));
        //                _db.Execute(); // Move all stop under the line to the primary line.

        //                _db.ExecuteCommand.CommandText = string.Format("DELETE FROM Lines WHERE LineID = {0}",
        //                    OleDbStrHelper.getParamStr(line.LineID));
        //                _db.Execute(); // Delete the duplicate line.

        //                line.Delete();  // Marked as deleted row means it had been merged.
        //            }
        //            _db.CommitTrans();
        //        }
        //        catch
        //        {
        //            _db.RollBack();
        //            throw;
        //        }
        //        finally
        //        {
        //            _db.Close();
        //        }
        //        #endregion
        //    }
        //}
        ///// <summary>
        ///// Delete duplicate stop info
        ///// </summary>
        ///// <param name="dupStops">List of duplicated stops</param>
        //private void DoMerging(List<DsBusWeb.vwDupStopDetailRow> dupStops)
        //{
        //    if (dupStops.Count > 0)
        //    {
        //        #region do merging
        //        try
        //        {
        //            _db.BeginTrans();
        //            foreach (var stop in dupStops)
        //            {
        //                _db.ExecuteCommand.CommandText = string.Format("DELETE FROM Line2Stop WHERE StopID = {0}",
        //                    OleDbStrHelper.getParamStr(stop.StopID));
        //                _db.Execute(); // Move all stop under the line to the primary line.

        //                _db.ExecuteCommand.CommandText = string.Format("DELETE FROM StopRating WHERE StopID = {0}",
        //                    OleDbStrHelper.getParamStr(stop.StopID));
        //                _db.Execute(); // Delete the duplicate line.

        //                _db.ExecuteCommand.CommandText = string.Format("DELETE FROM Stops WHERE StopID = {0}",
        //                    OleDbStrHelper.getParamStr(stop.StopID));
        //                _db.Execute(); // Delete the duplicate line.
        //            }
        //            _db.CommitTrans();
        //        }
        //        catch
        //        {
        //            _db.RollBack();
        //            throw;
        //        }
        //        finally
        //        {
        //            _db.Close();
        //        }
        //        #endregion
        //    }
        //}
        ///// <summary>
        ///// Merge duplicate stops
        ///// </summary>
        //private void MergeStops()
        //{
        //    DsBusWeb.vwDupStopDetailDataTable dt = new DsBusWeb.vwDupStopDetailDataTable();
        //    FillDt("SELECT * FROM vwDupStopDetail", dt);

        //    List<DsBusWeb.vwDupStopDetailRow> dupStops = new List<DsBusWeb.vwDupStopDetailRow>();

        //    DsBusWeb.vwDupStopDetailRow primaryStop = null;

        //    #region Scanning for merging process
        //    foreach (var dr in dt)
        //    {
        //        if (null == primaryStop ||
        //            dr.LineID != primaryStop.LineID ||
        //            dr.StopName != primaryStop.StopName)
        //        {
        //            primaryStop = dr;
        //        }
        //        else
        //        {
        //            if (dr.Rating < primaryStop.Rating) dupStops.Add(dr);
        //        }
        //    }
        //    DoMerging(dupStops);
        //    dupStops.Clear();
        //    #endregion
        //}
        #endregion

        private static double GetDeltaRadius(double deltaKM)
        {
            const double R = 6371; // Radius of EARTH
            return deltaKM * 180 / (R * Math.PI);
        }
    }
}
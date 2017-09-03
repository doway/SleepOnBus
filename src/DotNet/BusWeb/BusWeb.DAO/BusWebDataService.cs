using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using log4net;

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

        public void LogAUsageCase(DsBusWeb.UsageStatisticRow dr)
        {
            try
            {
                ExecuteNonQuery(string.Format("INSERT INTO UsageStatistic(UserCode, Longitude, Latitude, Radius, Device) VALUES({0}, {1}, {2}, {3}, {4})",
                    OleDbStrHelper.getParamStr(dr.UserCode),
                    OleDbStrHelper.getParamStr(new decimal(dr.Longitude)),
                    OleDbStrHelper.getParamStr(new decimal(dr.Latitude)),
                    OleDbStrHelper.getParamStr(new decimal(dr.Radius)),
                    OleDbStrHelper.getParamStr(dr.Device)
                    ));
            }
            catch (Exception ex) { Trace.WriteLine(ex); }
        }

        public int InsertNewLine(DsBusWeb.LinesRow dr)
        {
            ExecuteNonQuery(string.Format("INSERT INTO Lines(LineName, Culture) VALUES({0}, {1})",
                OleDbStrHelper.getParamStr(dr.LineName),
                OleDbStrHelper.getParamStr(dr.Culture)));
            return Convert.ToInt32(ExecuteScalar("SELECT MAX(LineID) FROM Lines"));
        }

        public void UpdateLine(DsBusWeb.LinesRow dr)
        {
            ExecuteNonQuery(string.Format("UPDATE Lines SET LineName={0} WHERE LineID={1}",
                OleDbStrHelper.getParamStr(dr.LineName),
                OleDbStrHelper.getParamStr(dr.LineID)));
        }

        public int InsertNewStop(DsBusWeb.StopsRow dr)
        {
            _db.Open();
            _db.BeginTrans();
            int stopID = 0;
            try
            {
                _db.ExecuteCommand.CommandText = string.Format("INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner, Culture) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                    OleDbStrHelper.getParamStr(dr.StopName),
                    OleDbStrHelper.getParamStr(new decimal(dr.Longitude)),
                    OleDbStrHelper.getParamStr(new decimal(dr.Latitude)),
                    OleDbStrHelper.getParamStr(new decimal(dr.CreatorLongitude)),
                    OleDbStrHelper.getParamStr(new decimal(dr.CreatorLatitude)),
                    OleDbStrHelper.getParamStr(dr.Owner),
                    OleDbStrHelper.getParamStr(dr.Culture));
                _db.Execute();
                _db.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
                stopID = Convert.ToInt32(_db.ExecuteScalar());
                _db.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID) VALUES({0})",
                    OleDbStrHelper.getParamStr(stopID));
                _db.Execute();
                _db.CommitTrans();
            }
            catch
            {
                _db.RollBack();
                throw;
            }
            finally
            {
                _db.Close();
            }

            return stopID;
        }

        public void UpdateStop(DsBusWeb.StopsRow dr)
        {
            ExecuteNonQuery(string.Format("UPDATE Stops SET StopName={0} WHERE StopID={1}",
                OleDbStrHelper.getParamStr(dr.StopName),
                OleDbStrHelper.getParamStr(dr.StopID)));
        }

        public void InsertNewLine2StopRelation(DsBusWeb.Line2StopRow dr)
        {
            ExecuteNonQuery(string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})",
                OleDbStrHelper.getParamStr(dr.LineID),
                OleDbStrHelper.getParamStr(dr.StopID)));
        }

        public DsBusWeb.LinesDataTable GetLinesList(double Longitude, double Latitude, double radius, string owner)
        {
            DsBusWeb.LinesDataTable dt = new DsBusWeb.LinesDataTable();
            FillDt(string.Format("SELECT DISTINCT l.* FROM (Lines l INNER JOIN Line2Stop l2s ON l.LineID = l2s.LineID) INNER JOIN (SELECT StopID FROM Stops WHERE (Owner = {3}) OR (ABS(Longitude - {0}) < {2} AND ABS(Latitude - {1}) < {2})) tmp ON l2s.StopID = tmp.StopID ORDER BY LineName",
                OleDbStrHelper.getParamStr(new decimal(Longitude)),
                OleDbStrHelper.getParamStr(new decimal(Latitude)),
                OleDbStrHelper.getParamStr(new decimal(radius)),
                OleDbStrHelper.getParamStr(owner)), dt);
            return dt;
        }

        public DsBusWeb.LinesDataTable GetLinesList()
        {
            DsBusWeb.LinesDataTable dt = new DsBusWeb.LinesDataTable();
            FillDt("SELECT l.*, (SELECT COUNT(1) FROM Line2Stop l2s WHERE l2s.LineID = l.LineID) AS StopCount FROM Lines l ORDER BY l.LineName", dt);
            return dt;
        }

        public DsBusWeb.LinesDataTable GetLinesList(DateTime stpBgnDT, DateTime stpEndDT)
        {
            DsBusWeb.LinesDataTable dt = new DsBusWeb.LinesDataTable();
            FillDt(string.Format("SELECT l.*, (SELECT COUNT(1) FROM Line2Stop l2s WHERE l2s.LineID = l.LineID) AS StopCount FROM Lines l WHERE l.LineID IN (SELECT l2s.LineID FROM Line2Stop l2s INNER JOIN Stops s ON l2s.StopID = s.StopID WHERE s.DateCreated BETWEEN {0} AND {1}) ORDER BY l.LineName",
                OleDbStrHelper.getParamStr(stpBgnDT),
                OleDbStrHelper.getParamStr(stpEndDT)), dt);
            return dt;
        }

        public DsBusWeb.StopsDataTable GetStopsByLineID(int lineID)
        {
            return GetStopsByLineID(lineID, null);
        }

        public DsBusWeb.StopsDataTable GetStopsByLineID(int lineID, string owner)
        {
            DsBusWeb.StopsDataTable dt = new DsBusWeb.StopsDataTable();
            FillDt(string.Format("SELECT s.*, sr.RatingGood, sr.RatingBad FROM (Stops s INNER JOIN Line2Stop l2s ON s.StopID = l2s.StopID) INNER JOIN StopRating sr ON s.StopID = sr.StopID WHERE l2s.LineID={0} ORDER BY StopName",
                OleDbStrHelper.getParamStr(lineID)),
                dt);
            if (!string.IsNullOrEmpty(owner))
                foreach (var dr in dt)
                    if (dr.Owner == owner) dr.StopName = string.Format("{0}(*)", dr.StopName);
            return dt;
        }

        public void RateStopGood(int stopID)
        {
            ExecuteNonQuery(string.Format("UPDATE StopRating SET RatingGood=RatingGood + 1 WHERE StopID={0}", OleDbStrHelper.getParamStr(stopID)));
        }

        public void RateStopBad(int stopID)
        {
            ExecuteNonQuery(string.Format("UPDATE StopRating SET RatingBad=RatingBad + 1 WHERE StopID={0}", OleDbStrHelper.getParamStr(stopID)));
        }

        public void DeleteStopByID(int stopID)
        {
            try
            {
                _db.BeginTrans();
                _db.ExecuteCommand.CommandText = string.Format("DELETE FROM StopRating WHERE StopID = {0}", OleDbStrHelper.getParamStr(stopID));
                _db.Execute();
                _db.ExecuteCommand.CommandText = string.Format("DELETE FROM Line2Stop WHERE StopID = {0}", OleDbStrHelper.getParamStr(stopID));
                _db.Execute();
                _db.ExecuteCommand.CommandText = string.Format("DELETE FROM Stops WHERE StopID = {0}", OleDbStrHelper.getParamStr(stopID));
                _db.Execute();
                _db.CommitTrans();
            }
            catch
            {
                _db.RollBack();
                throw;
            }
            finally
            {
                _db.Close();
            }
        }

        public int GetStopIDByOwnerAndStopID(int stopID, string owner)
        {
            _db.ExecuteCommand.CommandText = string.Format("SELECT StopID FROM Stops WHERE StopID = {0} AND Owner = {1}",
                OleDbStrHelper.getParamStr(stopID),
                OleDbStrHelper.getParamStr(owner));
            return Convert.ToInt32(_db.ExecuteScalar());
        }

        public void DeleteLineByID(int lineID)
        {
            try
            {
                _db.BeginTrans();
                _db.ExecuteCommand.CommandText = string.Format("DELETE FROM StopRating WHERE StopID IN (SELECT StopID FROM Line2Stop WHERE LineID = {0})", OleDbStrHelper.getParamStr(lineID));
                _db.Execute();
                _db.ExecuteCommand.CommandText = string.Format("DELETE FROM Line2Stop WHERE LineID = {0}", OleDbStrHelper.getParamStr(lineID));
                _db.Execute();
                _db.ExecuteCommand.CommandText = "DELETE FROM Stops WHERE StopID NOT IN (SELECT StopID FROM Line2Stop)";
                _db.Execute();
                _db.ExecuteCommand.CommandText = string.Format("DELETE FROM Lines WHERE LineID = {0}", OleDbStrHelper.getParamStr(lineID));
                _db.Execute();
                _db.CommitTrans();
            }
            catch
            {
                _db.RollBack();
                throw;
            }
            finally
            {
                _db.Close();
            }
        }
        /// <summary>
        /// Remove high-delta bad rating stops
        /// </summary>
        public void RemoveBadRatingStops()
        {
            var dt = new DsBusWeb.StopsDataTable();
            FillDt("SELECT s.*, sr.RatingGood, sr.RatingBad FROM Stops s INNER JOIN StopRating sr ON s.StopID = sr.StopID WHERE sr.RatingBad > (sr.RatingGood + 1)", dt);
            StringBuilder sb = new StringBuilder("There're following stops going to be removed:\r\n");
            StringBuilder arrayStringBuilder = new StringBuilder("(");
            bool firstElem = true;
            foreach (var dr in dt)
            {
                sb.AppendFormat("StopID={0} / StopName={1} / RatingGood={2} / RatingBad={3} / Owner={4} / DateCreated={5} / Culture={6}\r\n", dr.StopID, dr.StopName, dr["RatingGood"], dr["RatingBad"], dr.Owner, dr.DateCreated.ToString("yyyy-MM-dd HH:mm:ss"), dr.Culture);
                if (firstElem) arrayStringBuilder.Append(dr.StopID);  else arrayStringBuilder.Append(", " + dr.StopID);
                firstElem = false;
            }
            arrayStringBuilder.Append(")");
            _log.Debug(sb);
            try
            {
                _db.BeginTrans();
                _db.ExecuteCommand.CommandText = "DELETE FROM StopRating WHERE StopID IN " + arrayStringBuilder.ToString();
                _db.Execute();
                _db.ExecuteCommand.CommandText = "DELETE FROM Line2Stop WHERE StopID IN " + arrayStringBuilder.ToString();
                _db.Execute();
                _db.ExecuteCommand.CommandText = "DELETE FROM Stops WHERE StopID IN " + arrayStringBuilder.ToString();
                _db.Execute();
                _db.CommitTrans();
            }
            catch
            {
                _db.RollBack();
                throw;
            }
            finally
            {
                _db.Close();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void RemoveEmptyLines()
        {
            ExecuteNonQuery("DELETE FROM Lines WHERE LineID NOT IN (SELECT DISTINCT LineID FROM Line2Stop)");
        }
        /// <summary>
        /// Merge if 2 lines have the same stop inside.
        /// </summary>
        public void MergeLines()
        {
            // 1. Get all duplicate line in detail
            DsBusWeb.vwDupLineDetailDataTable dt = new DsBusWeb.vwDupLineDetailDataTable();
            FillDt("SELECT * FROM vwDupLineDetail", dt);

            List<DsBusWeb.vwDupLineDetailRow> dupLines = new List<DsBusWeb.vwDupLineDetailRow>();

            #region Scanning for merging process
            for (int i = 0; i < dt.Count; i++)
            {
                if (dt[i].RowState != DataRowState.Deleted) // If the row has not been merged...
                {
                    DsBusWeb.vwDupLineDetailRow primaryLine = dt[i];
                    dupLines.Clear();
                    for (int j = i + 1; j < dt.Count; j++)
                    {
                        if (dt[j].RowState != DataRowState.Deleted)
                        {
                            var currLine = dt[j];
                            if (currLine.LineName == primaryLine.LineName)
                            {
                                if (isOverlap(primaryLine, currLine)) dupLines.Add(currLine);
                            }
                            else
                                break;
                        }
                    }
                    DoMerging(dupLines, primaryLine);
                }
            }
            #endregion

            MergeStops();
        }
        private static bool isOverlap(DsBusWeb.vwDupLineDetailRow line1, DsBusWeb.vwDupLineDetailRow line2)
        {
            return !(line2.maxLatitude < (line1.minLatitude - DELTA_RADIUS) ||
                line2.minLatitude > (line1.maxLatitude + DELTA_RADIUS) ||
                line2.minLongitude > (line1.maxLongitude + DELTA_RADIUS) ||
                line2.maxLongitude < (line1.minLongitude - DELTA_RADIUS));
        }
        /// <summary>
        /// Merge 2 lines by their ID. If their names are different to each other, the merging will fail and nothing change.
        /// </summary>
        /// <param name="lineId1">The ID of 1st line would be merged</param>
        /// <param name="lineId2">The ID of 2nd line would be merged</param>
        /// <returns>Success or failure</returns>
        public bool MergeLines(int lineId1, int lineId2)
        {
            bool success = false;
            int stopCountForLine1 = 0;
            stopCountForLine1 = Convert.ToInt32(ExecuteScalar(string.Format("SELECT COUNT(1) FROM Line2Stop WHERE LineID = {0}", lineId1)));
            int stopCountForLine2 = 0;
            stopCountForLine2 = Convert.ToInt32(ExecuteScalar(string.Format("SELECT COUNT(1) FROM Line2Stop WHERE LineID = {0}", lineId2)));
            int originalLineID = 0;
            int newLineID = 0;
            if (stopCountForLine2 > stopCountForLine1)
            {
                originalLineID = lineId1;
                newLineID = lineId2;
            }
            else
            {
                originalLineID = lineId2;
                newLineID = lineId1;
            }
            try
            {
                _db.BeginTrans();
                _db.ExecuteCommand.CommandText = string.Format("UPDATE Line2Stop SET LineID = {0} WHERE LineID = {1}", newLineID, originalLineID);
                _db.Execute(); // Move all stop under the line to the primary line.
                _db.ExecuteCommand.CommandText = string.Format("DELETE FROM Lines WHERE LineID = {0}", originalLineID);
                _db.Execute(); // Remove the empty line
                _db.CommitTrans();
                success = true;
            }
            catch
            {
                _db.RollBack();
                throw;
            }
            finally
            {
                _db.Close();
            }
            MergeStops();
            success = true;
            return success;
        }
        /// <summary>
        /// Move all stops under these duplicated lines, then delete these duplicated lines.
        /// </summary>
        /// <param name="dupLines">List of duplicated lines</param>
        /// <param name="primaryLine">The primary line which will be attached within these moving stops.</param>
        private void DoMerging(List<DsBusWeb.vwDupLineDetailRow> dupLines, DsBusWeb.vwDupLineDetailRow primaryLine)
        {
            if (dupLines.Count > 0)
            {
                #region do merging
                try
                {
                    _db.BeginTrans();
                    foreach (var line in dupLines)
                    {
                        _db.ExecuteCommand.CommandText = string.Format("UPDATE Line2Stop SET LineID = {0} WHERE LineID = {1}",
                            OleDbStrHelper.getParamStr(primaryLine.LineID),
                            OleDbStrHelper.getParamStr(line.LineID));
                        _db.Execute(); // Move all stop under the line to the primary line.

                        _db.ExecuteCommand.CommandText = string.Format("DELETE FROM Lines WHERE LineID = {0}",
                            OleDbStrHelper.getParamStr(line.LineID));
                        _db.Execute(); // Delete the duplicate line.

                        line.Delete();  // Marked as deleted row means it had been merged.
                    }
                    _db.CommitTrans();
                }
                catch
                {
                    _db.RollBack();
                    throw;
                }
                finally
                {
                    _db.Close();
                }
                #endregion
            }
        }
        /// <summary>
        /// Delete duplicate stop info
        /// </summary>
        /// <param name="dupStops">List of duplicated stops</param>
        private void DoMerging(List<DsBusWeb.vwDupStopDetailRow> dupStops)
        {
            if (dupStops.Count > 0)
            {
                #region do merging
                try
                {
                    _db.BeginTrans();
                    foreach (var stop in dupStops)
                    {
                        _db.ExecuteCommand.CommandText = string.Format("DELETE FROM Line2Stop WHERE StopID = {0}",
                            OleDbStrHelper.getParamStr(stop.StopID));
                        _db.Execute(); // Move all stop under the line to the primary line.

                        _db.ExecuteCommand.CommandText = string.Format("DELETE FROM StopRating WHERE StopID = {0}",
                            OleDbStrHelper.getParamStr(stop.StopID));
                        _db.Execute(); // Delete the duplicate line.

                        _db.ExecuteCommand.CommandText = string.Format("DELETE FROM Stops WHERE StopID = {0}",
                            OleDbStrHelper.getParamStr(stop.StopID));
                        _db.Execute(); // Delete the duplicate line.
                    }
                    _db.CommitTrans();
                }
                catch
                {
                    _db.RollBack();
                    throw;
                }
                finally
                {
                    _db.Close();
                }
                #endregion
            }
        }
        /// <summary>
        /// Merge duplicate stops
        /// </summary>
        private void MergeStops()
        {
            DsBusWeb.vwDupStopDetailDataTable dt = new DsBusWeb.vwDupStopDetailDataTable();
            FillDt("SELECT * FROM vwDupStopDetail", dt);

            List<DsBusWeb.vwDupStopDetailRow> dupStops = new List<DsBusWeb.vwDupStopDetailRow>();

            DsBusWeb.vwDupStopDetailRow primaryStop = null;

            #region Scanning for merging process
            foreach (var dr in dt)
            {
                if (null == primaryStop ||
                    dr.LineID != primaryStop.LineID ||
                    dr.StopName != primaryStop.StopName)
                {
                    primaryStop = dr;
                }
                else
                {
                    if (dr.Rating < primaryStop.Rating) dupStops.Add(dr);
                }
            }
            DoMerging(dupStops);
            dupStops.Clear();
            #endregion
        }
        /// <summary>
        /// Execute any command
        /// </summary>
        /// <param name="cmdText">Command text</param>
        /// <returns>Effect row count</returns>
        public int ExecuteAnything(string cmdText)
        {
            return ExecuteNonQuery(cmdText);
        }
        /// <summary>
        /// Query any data
        /// </summary>
        /// <param name="cmdText">Query statement</param>
        /// <returns></returns>
        public DataTable QueryAnything(string cmdText)
        {
            if (cmdText.ToUpper().StartsWith("SELECT "))
            {
                DataTable dt = new DataTable();
                FillDt(cmdText, dt);
                return dt;
            }
            else
            {
                throw new ArgumentException("The query statement is not begin with [SELECT]!");
            }
        }

        private static double GetDeltaRadius(double deltaKM)
        {
            const double R = 6371; // Radius of EARTH
            return deltaKM * 180 / (R * Math.PI);
        }
    }
}
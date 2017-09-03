using System;
using System.Data;
using System.Threading;
using BusWeb.DAO;
using BusWeb.DAO.DataSet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BusWeb.UnitTest
{
    /// <summary>
    ///This is a test class for BusWebDataServiceTest and is intended
    ///to contain all BusWebDataServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BusWebDataServiceTest : UnitTestBase
    {
        /// <summary>
        ///A test for InsertNewStop
        ///</summary>
        [TestMethod()]
        public void InsertNewStopTest()
        {
            BusWebDataService target = BusWebDataService.GetServiceInstance();
            DsBusWeb.StopsRow dr = new DsBusWeb.StopsDataTable().NewStopsRow();
            dr.StopName = "DOWILL";
            dr.Longitude = 121.543615;
            dr.Latitude = 25.033369;
            dr.CreatorLongitude = 121.543615;
            dr.CreatorLatitude = 25.033096;
            dr.Culture = "zh-TW";
            dr.Owner = "DOWILL";
            int actual = target.InsertNewStop(dr);

            #region verifying
            Assert.AreNotEqual<int>(0, actual, "The StopID should not be zero!");
            Thread.Sleep(CONST_SLEEP_TIME);
            DataTable dt = new DataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Stops";
            _coreDb.Fill(dt);
            Assert.AreEqual<int>(1, dt.Rows.Count, "We should have 1 stop in DB now!");
            Assert.AreEqual(actual, dt.Rows[0]["StopID"], "StopID is incorrect!");
            Assert.AreEqual(dr.Latitude, dt.Rows[0]["Latitude"], "Latitude is incorrect!");
            Assert.AreEqual(dr.Longitude, dt.Rows[0]["Longitude"], "Longitude is incorrect!");
            Assert.AreEqual(dr.CreatorLatitude, dt.Rows[0]["CreatorLatitude"], "CreatorLatitude is incorrect!");
            Assert.AreEqual(dr.CreatorLongitude, dt.Rows[0]["CreatorLongitude"], "CreatorLongitude is incorrect!");
            Assert.AreEqual(dr.Owner, dt.Rows[0]["Owner"], "Owner is incorrect!");
            Assert.AreEqual(dr.Culture, dt.Rows[0]["Culture"], "Culture is incorrect!");
            #endregion
        }
        /// <summary>
        ///A test for InsertNewLine2StopRelation
        ///</summary>
        [TestMethod()]
        public void InsertNewLine2StopRelationTest()
        {
            #region prepare testing data
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Lines(LineName) VALUES('286')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(LineID) FROM Lines";
            int lineID = Convert.ToInt32(_coreDb.ExecuteScalar());

            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude) VALUES('DOWILL', 121.543615, 25.033369)";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            int stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            #endregion

            Thread.Sleep(CONST_SLEEP_TIME);

            BusWebDataService target = BusWebDataService.GetServiceInstance();
            DsBusWeb.Line2StopRow dr = new DsBusWeb.Line2StopDataTable().NewLine2StopRow();
            dr.LineID = lineID;
            dr.StopID = stopID;
            target.InsertNewLine2StopRelation(dr);

            Thread.Sleep(CONST_SLEEP_TIME);

            #region verifying
            DsBusWeb.Line2StopDataTable dt = new DsBusWeb.Line2StopDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Line2Stop";
            _coreDb.Fill(dt);
            Assert.AreEqual<int>(1, dt.Count, "The row count of line2stop is not expected!");
            Assert.AreEqual<int>(lineID, dt[0].LineID, "The LineID is not expected!");
            Assert.AreEqual<int>(stopID, dt[0].StopID, "The StopID is not expected!");
            #endregion
        }
        /// <summary>
        ///A test for InsertNewLine
        ///</summary>
        [TestMethod()]
        public void InsertNewLineTest()
        {
            BusWebDataService target = BusWebDataService.GetServiceInstance();
            DsBusWeb.LinesRow dr = new DsBusWeb.LinesDataTable().NewLinesRow();
            dr.LineName = "286";
            dr.Culture = "zh-TW";
            int actual = target.InsertNewLine(dr);

            #region verifying
            Assert.AreNotEqual<int>(0, actual, "The LineID should not be zero!");
            Thread.Sleep(CONST_SLEEP_TIME);
            DataTable dt = new DataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Lines";
            _coreDb.Fill(dt);
            Assert.AreEqual<int>(1, dt.Rows.Count, "We should have 1 line in DB now!");
            Assert.AreEqual(actual, dt.Rows[0]["LineID"], "StopID is incorrect!");
            Assert.AreEqual(dr.LineName, dt.Rows[0]["LineName"], "LineName is incorrect!");
            Assert.AreEqual(dr.Culture, dt.Rows[0]["Culture"], "Culture is incorrect!");
            #endregion
        }
        /// <summary>
        ///A test for GetStopsByLineID
        ///</summary>
        [TestMethod()]
        public void GetStopsByLineIDTest()
        {
            #region prepare testing data
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Lines(LineName) VALUES('286')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(LineID) FROM Lines";
            int lineID = Convert.ToInt32(_coreDb.ExecuteScalar());

            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude) VALUES('DOWILL', 121.543615, 25.033369)";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            int stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 2, 5)", stopID);
            _coreDb.Execute();

            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineID, stopID);
            _coreDb.Execute();
            #endregion

            Thread.Sleep(CONST_SLEEP_TIME);

            BusWebDataService target = BusWebDataService.GetServiceInstance();
            DsBusWeb.StopsDataTable actual = target.GetStopsByLineID(lineID);

            Assert.AreEqual<int>(1, actual.Count, "The stop row count is not expected!");
            Assert.AreEqual<int>(2, Convert.ToInt32(actual[0]["RatingGood"]), "The rating good value is not expected!");
            Assert.AreEqual<int>(5, Convert.ToInt32(actual[0]["RatingBad"]), "The rating bad value is not expected!");
        }
        /// <summary>
        ///A test for GetStopsByLineID
        ///</summary>
        [TestMethod()]
        public void GetStopsByLineIDByOwnerTest()
        {
            #region prepare testing data
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Lines(LineName) VALUES('286')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(LineID) FROM Lines";
            int lineID = Convert.ToInt32(_coreDb.ExecuteScalar());

            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, Owner) VALUES('DOWILL', 121.543615, 25.033369, 'dowill.service@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            int stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 2, 5)", stopID);
            _coreDb.Execute();

            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineID, stopID);
            _coreDb.Execute();
            #endregion

            Thread.Sleep(CONST_SLEEP_TIME);

            BusWebDataService target = BusWebDataService.GetServiceInstance();
            DsBusWeb.StopsDataTable actual = target.GetStopsByLineID(lineID, "dowill.service@gmail.com");

            Assert.AreEqual<int>(1, actual.Count, "The stop row count is not expected!");
            Assert.AreEqual<int>(2, Convert.ToInt32(actual[0]["RatingGood"]), "The rating good value is not expected!");
            Assert.AreEqual<int>(5, Convert.ToInt32(actual[0]["RatingBad"]), "The rating bad value is not expected!");
            Assert.AreEqual<string>("DOWILL(*)", actual[0].StopName, "The stop name should be marked as ownership!");
        }
        /// <summary>
        ///A test for GetLinesList
        ///</summary>
        [TestMethod()]
        public void GetLinesListTest()
        {
            #region prepare testing data

            #region line-1
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Lines(LineName) VALUES('286')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(LineID) FROM Lines";
            int lineID = Convert.ToInt32(_coreDb.ExecuteScalar());

            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude) VALUES('DOWILL', 121.543615, 25.033369)";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            int stopID = Convert.ToInt32(_coreDb.ExecuteScalar());

            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineID, stopID);
            _coreDb.Execute();

            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude) VALUES('DOWILL-1', 119.543615, 24.033369)";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());

            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineID, stopID);
            _coreDb.Execute();
            #endregion

            #region line-2
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Lines(LineName) VALUES('287')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(LineID) FROM Lines";
            lineID = Convert.ToInt32(_coreDb.ExecuteScalar());

            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude) VALUES('DOWILL-2', 110.543615, 35.033369)";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());

            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineID, stopID);
            _coreDb.Execute();

            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude) VALUES('DOWILL-3', 100.543615, 30.033369)";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());

            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineID, stopID);
            _coreDb.Execute();
            #endregion

            #endregion

            Thread.Sleep(CONST_SLEEP_TIME);

            BusWebDataService target = BusWebDataService.GetServiceInstance();
            double Longitude = 121;
            double Latitude = 25;
            double radius = 1;
            DsBusWeb.LinesDataTable actual = target.GetLinesList(Longitude, Latitude, radius, null);
            Assert.AreEqual<int>(1, actual.Count, "The row count of line list is not expected!");
            Assert.AreEqual<string>("286", actual[0].LineName, "The line name is not expected!");
        }
        /// <summary>
        ///A test for RateStopBad
        ///</summary>
        [TestMethod()]
        public void RateStopBadTest()
        {
            #region prepare testing data
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude) VALUES('DOWILL', 121.543615, 25.033369)";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            int stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID) VALUES({0})", stopID);
            _coreDb.Execute();
            #endregion

            Thread.Sleep(CONST_SLEEP_TIME);

            BusWebDataService target = BusWebDataService.GetServiceInstance();
            target.RateStopBad(stopID);

            Thread.Sleep(CONST_SLEEP_TIME);

            #region verifying
            DsBusWeb.StopRatingDataTable dt = new DsBusWeb.StopRatingDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM StopRating";
            _coreDb.Fill(dt);
            Assert.AreEqual<int>(1, dt.Count, "Row count is not expeceted!");
            Assert.AreEqual<int>(0, dt[0].RatingGood, "RatingGood is not expected!");
            Assert.AreEqual<int>(1, dt[0].RatingBad, "RatingBad is not expected!");
            #endregion
        }
        /// <summary>
        ///A test for RateStopGood
        ///</summary>
        [TestMethod()]
        public void RateStopGoodTest()
        {
            #region prepare testing data
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude) VALUES('DOWILL', 121.543615, 25.033369)";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            int stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID) VALUES({0})", stopID);
            _coreDb.Execute();
            #endregion

            Thread.Sleep(CONST_SLEEP_TIME);

            BusWebDataService target = BusWebDataService.GetServiceInstance();
            target.RateStopGood(stopID);

            Thread.Sleep(CONST_SLEEP_TIME);

            #region verifying
            DsBusWeb.StopRatingDataTable dt = new DsBusWeb.StopRatingDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM StopRating";
            _coreDb.Fill(dt);
            Assert.AreEqual<int>(1, dt.Count, "Row count is not expeceted!");
            Assert.AreEqual<int>(1, dt[0].RatingGood, "RatingGood is not expected!");
            Assert.AreEqual<int>(0, dt[0].RatingBad, "RatingBad is not expected!");
            #endregion
        }
        /// <summary>
        ///A test for MergeLines
        ///</summary>
        [TestMethod()]
        public void MergeLinesTest()
        {
            #region prepare testing data
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Lines(LineName) VALUES('207')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(LineID) FROM Lines";
            int lineId1 = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Lines(LineName) VALUES('207')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(LineID) FROM Lines";
            int lineId2 = Convert.ToInt32(_coreDb.ExecuteScalar());

            int stopID = 0;

            #region line1
            #region 周美里
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('周美里', 121.583377, 25.057018, 0, 0, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 9, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 後山埤站
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('後山埤站', 121.582911, 25.04487, 0, 0, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 3, 1)", stopID);
            _coreDb.Execute();
            #endregion

            #region 秀朗國小
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('秀朗國小', 121.521262, 24.999231, 121.58084978, 25.05816265, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 3, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 週美里
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('週美里', 121.583377, 25.057018, 121.56014515, 25.03309135, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 松山家商
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('松山家商', 121.580912, 25.035887, 121.5546072, 25.0299129, 'a0203666@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 1, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 永和市永元路
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('永和市永元路', 121.522975, 25.001647, 121.5525412, 25.0270202, 'samjsck@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 2, 0)", stopID);
            _coreDb.Execute();
            #endregion
            #endregion

            #region line2
            #region 永和路
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('永和路', 120.635709, 24.21521, 121.5614992, 25.0326352, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 永和市永元路
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('永和市永元路', 121.522638, 25.000503, 121.5528976, 25.024357, 'samjsck@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 新湖舊宗路口
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('新湖舊宗路口', 121.578653, 25.062323, 121.5777363, 25.0634112, 'denniswa@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 新湖一路口
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('新湖一路口', 121.579208, 25.060522, 121.57881499, 25.06057842, 'joycewang914@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 1, 0)", stopID);
            _coreDb.Execute();
            #endregion
            #endregion
            #endregion

            Thread.Sleep(CONST_SLEEP_TIME);

            BusWebDataService target = BusWebDataService.GetServiceInstance();
            target.MergeLines();

            Thread.Sleep(CONST_SLEEP_TIME);

            #region verifying
            DsBusWeb.LinesDataTable dt = new DsBusWeb.LinesDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Lines";
            _coreDb.Fill(dt);

            Assert.AreEqual<int>(1, dt.Count, "Line count should be only 1 left!");
            DsBusWeb.Line2StopDataTable dt2 = new DsBusWeb.Line2StopDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Line2Stop";
            _coreDb.Fill(dt2);
            Assert.AreEqual<int>(9, dt2.Count, "Stops should be only 9 links left!");
            foreach (var dr in dt2) Assert.AreEqual<int>(lineId1, dr.LineID, "All LineID should point to the 1st line!");

            DsBusWeb.StopsDataTable dt3 = new DsBusWeb.StopsDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Stops";
            _coreDb.Fill(dt3);
            Assert.AreEqual<int>(9, dt3.Count, "Stops should be 9 stops left!");
            #endregion
        }
        /// <summary>
        ///A test for DeleteLineByID
        ///</summary>
        [TestMethod()]
        public void DeleteLineByIDTest()
        {
            #region prepare testing data
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Lines(LineName) VALUES('207')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(LineID) FROM Lines";
            int lineId1 = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Lines(LineName) VALUES('207')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(LineID) FROM Lines";
            int lineId2 = Convert.ToInt32(_coreDb.ExecuteScalar());

            int stopID = 0;

            #region line1
            #region 周美里
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('周美里', 121.583377, 25.057018, 0, 0, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 9, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 後山埤站
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('後山埤站', 121.582911, 25.04487, 0, 0, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 3, 1)", stopID);
            _coreDb.Execute();
            #endregion

            #region 秀朗國小
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('秀朗國小', 121.521262, 24.999231, 121.58084978, 25.05816265, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 3, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 週美里
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('週美里', 121.583377, 25.057018, 121.56014515, 25.03309135, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 松山家商
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('松山家商', 121.580912, 25.035887, 121.5546072, 25.0299129, 'a0203666@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 1, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 永和市永元路
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('永和市永元路', 121.522975, 25.001647, 121.5525412, 25.0270202, 'samjsck@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 2, 0)", stopID);
            _coreDb.Execute();
            #endregion
            #endregion

            #region line2
            #region 永和路
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('永和路', 120.635709, 24.21521, 121.5614992, 25.0326352, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 永和市永元路
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('永和市永元路', 121.522638, 25.000503, 121.5528976, 25.024357, 'samjsck@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 新湖舊宗路口
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('新湖舊宗路口', 121.578653, 25.062323, 121.5777363, 25.0634112, 'denniswa@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 新湖一路口
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('新湖一路口', 121.579208, 25.060522, 121.57881499, 25.06057842, 'joycewang914@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 1, 0)", stopID);
            _coreDb.Execute();
            #endregion
            #endregion
            #endregion

            Thread.Sleep(CONST_SLEEP_TIME);
            
            BusWebDataService target = BusWebDataService.GetServiceInstance();
            int lineID = lineId1;
            target.DeleteLineByID(lineID);

            Thread.Sleep(CONST_SLEEP_TIME);

            #region verifying
            DsBusWeb.LinesDataTable dt = new DsBusWeb.LinesDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Lines";
            _coreDb.Fill(dt);

            Assert.AreEqual<int>(1, dt.Count, "Line count should be only 1 left!");
            DsBusWeb.Line2StopDataTable dt2 = new DsBusWeb.Line2StopDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Line2Stop";
            _coreDb.Fill(dt2);
            Assert.AreEqual<int>(4, dt2.Count, "Stops should be only 4 links left!");
            foreach (var dr in dt2) Assert.AreEqual<int>(lineId2, dr.LineID, "All LineID should point to the 2nd line!");

            DsBusWeb.StopsDataTable dt3 = new DsBusWeb.StopsDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Stops";
            _coreDb.Fill(dt3);
            Assert.AreEqual<int>(4, dt3.Count, "Stops should be 4 stops left!");
            #endregion
        }
        /// <summary>
        ///A test for DeleteStopByID
        ///</summary>
        [TestMethod()]
        public void DeleteStopByIDTest()
        {
            #region prepare testing data
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Lines(LineName) VALUES('207')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(LineID) FROM Lines";
            int lineId1 = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Lines(LineName) VALUES('207')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(LineID) FROM Lines";
            int lineId2 = Convert.ToInt32(_coreDb.ExecuteScalar());

            int stopID = 0;

            #region line1
            #region 周美里
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('周美里', 121.583377, 25.057018, 0, 0, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 9, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 後山埤站
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('後山埤站', 121.582911, 25.04487, 0, 0, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 3, 1)", stopID);
            _coreDb.Execute();
            #endregion

            #region 秀朗國小
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('秀朗國小', 121.521262, 24.999231, 121.58084978, 25.05816265, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 3, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 週美里
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('週美里', 121.583377, 25.057018, 121.56014515, 25.03309135, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 松山家商
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('松山家商', 121.580912, 25.035887, 121.5546072, 25.0299129, 'a0203666@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 1, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 永和市永元路
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('永和市永元路', 121.522975, 25.001647, 121.5525412, 25.0270202, 'samjsck@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 2, 0)", stopID);
            _coreDb.Execute();
            #endregion
            #endregion

            #region line2
            #region 永和路
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('永和路', 120.635709, 24.21521, 121.5614992, 25.0326352, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 永和市永元路
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('永和市永元路', 121.522638, 25.000503, 121.5528976, 25.024357, 'samjsck@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 新湖舊宗路口
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('新湖舊宗路口', 121.578653, 25.062323, 121.5777363, 25.0634112, 'denniswa@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 新湖一路口
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('新湖一路口', 121.579208, 25.060522, 121.57881499, 25.06057842, 'joycewang914@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 1, 0)", stopID);
            _coreDb.Execute();
            #endregion
            #endregion
            #endregion

            Thread.Sleep(CONST_SLEEP_TIME);

            BusWebDataService target = BusWebDataService.GetServiceInstance();
            target.DeleteStopByID(stopID);

            Thread.Sleep(CONST_SLEEP_TIME);

            #region verifying
            DsBusWeb.LinesDataTable dt = new DsBusWeb.LinesDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Lines";
            _coreDb.Fill(dt);

            Assert.AreEqual<int>(2, dt.Count, "Line count should be only 2 left!");
            DsBusWeb.Line2StopDataTable dt2 = new DsBusWeb.Line2StopDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Line2Stop";
            _coreDb.Fill(dt2);
            Assert.AreEqual<int>(9, dt2.Count, "Stops should be only 9 links left!");
            foreach (var dr in dt2) Assert.AreNotEqual<int>(stopID, dr.StopID, "There should not be any stopID in link existing as the deleted one!");

            DsBusWeb.StopsDataTable dt3 = new DsBusWeb.StopsDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Stops";
            _coreDb.Fill(dt3);
            Assert.AreEqual<int>(9, dt3.Count, "Stops should be 9 stops left!");
            foreach (var dr in dt3) Assert.AreNotEqual<int>(stopID, dr.StopID, "There should not be any stopID in stop info existing as the deleted one!");
            #endregion
        }
        /// <summary>
        ///A test for GetStopIDByOwnerAndStopID
        ///</summary>
        [TestMethod()]
        public void GetStopIDByOwnerAndStopIDTest()
        {
            #region prepare testing data
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Lines(LineName) VALUES('207')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(LineID) FROM Lines";
            int lineId1 = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Lines(LineName) VALUES('207')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(LineID) FROM Lines";
            int lineId2 = Convert.ToInt32(_coreDb.ExecuteScalar());

            int stopID = 0;

            #region line1
            #region 周美里
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('周美里', 121.583377, 25.057018, 0, 0, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 9, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 後山埤站
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('後山埤站', 121.582911, 25.04487, 0, 0, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 3, 1)", stopID);
            _coreDb.Execute();
            #endregion

            #region 秀朗國小
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('秀朗國小', 121.521262, 24.999231, 121.58084978, 25.05816265, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 3, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 週美里
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('週美里', 121.583377, 25.057018, 121.56014515, 25.03309135, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 松山家商
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('松山家商', 121.580912, 25.035887, 121.5546072, 25.0299129, 'a0203666@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 1, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 永和市永元路
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('永和市永元路', 121.522975, 25.001647, 121.5525412, 25.0270202, 'samjsck@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 2, 0)", stopID);
            _coreDb.Execute();
            #endregion
            #endregion

            #region line2
            #region 永和路
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('永和路', 120.635709, 24.21521, 121.5614992, 25.0326352, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 永和市永元路
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('永和市永元路', 121.522638, 25.000503, 121.5528976, 25.024357, 'samjsck@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 新湖舊宗路口
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('新湖舊宗路口', 121.578653, 25.062323, 121.5777363, 25.0634112, 'denniswa@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 新湖一路口
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('新湖一路口', 121.579208, 25.060522, 121.57881499, 25.06057842, 'joycewang914@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 1, 0)", stopID);
            _coreDb.Execute();
            #endregion
            #endregion
            #endregion

            Thread.Sleep(CONST_SLEEP_TIME);

            BusWebDataService target = BusWebDataService.GetServiceInstance();
            string owner = "joycewang914@gmail.com";
            int expected = stopID;
            int actual = target.GetStopIDByOwnerAndStopID(stopID, owner);
            Assert.AreEqual<int>(expected, actual,"We should get the return value as the expect stopID.");
        }
        /// <summary>
        ///A test for GetStopIDByOwnerAndStopID
        ///</summary>
        [TestMethod()]
        public void GetStopIDByOwnerAndStopIDButOwnerNotMatchTest()
        {
            #region prepare testing data
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Lines(LineName) VALUES('207')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(LineID) FROM Lines";
            int lineId1 = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Lines(LineName) VALUES('207')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(LineID) FROM Lines";
            int lineId2 = Convert.ToInt32(_coreDb.ExecuteScalar());

            int stopID = 0;

            #region line1
            #region 周美里
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('周美里', 121.583377, 25.057018, 0, 0, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 9, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 後山埤站
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('後山埤站', 121.582911, 25.04487, 0, 0, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 3, 1)", stopID);
            _coreDb.Execute();
            #endregion

            #region 秀朗國小
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('秀朗國小', 121.521262, 24.999231, 121.58084978, 25.05816265, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 3, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 週美里
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('週美里', 121.583377, 25.057018, 121.56014515, 25.03309135, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 松山家商
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('松山家商', 121.580912, 25.035887, 121.5546072, 25.0299129, 'a0203666@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 1, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 永和市永元路
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('永和市永元路', 121.522975, 25.001647, 121.5525412, 25.0270202, 'samjsck@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 2, 0)", stopID);
            _coreDb.Execute();
            #endregion
            #endregion

            #region line2
            #region 永和路
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('永和路', 120.635709, 24.21521, 121.5614992, 25.0326352, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 永和市永元路
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('永和市永元路', 121.522638, 25.000503, 121.5528976, 25.024357, 'samjsck@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 新湖舊宗路口
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('新湖舊宗路口', 121.578653, 25.062323, 121.5777363, 25.0634112, 'denniswa@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 新湖一路口
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('新湖一路口', 121.579208, 25.060522, 121.57881499, 25.06057842, 'joycewang914@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 1, 0)", stopID);
            _coreDb.Execute();
            #endregion
            #endregion
            #endregion

            Thread.Sleep(CONST_SLEEP_TIME);

            BusWebDataService target = BusWebDataService.GetServiceInstance();
            string owner = "a0203666@gmail.com";
            int expected = 0;
            int actual = target.GetStopIDByOwnerAndStopID(stopID, owner);
            Assert.AreEqual<int>(expected, actual, "We should get zero as invalid stopID!");
        }
        /// <summary>
        ///A test for GetStopIDByOwnerAndStopID
        ///</summary>
        [TestMethod()]
        public void GetStopIDByOwnerAndStopIDButStopIDNotMatchTest()
        {
            #region prepare testing data
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Lines(LineName) VALUES('207')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(LineID) FROM Lines";
            int lineId1 = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Lines(LineName) VALUES('207')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(LineID) FROM Lines";
            int lineId2 = Convert.ToInt32(_coreDb.ExecuteScalar());

            int stopID = 0;

            #region line1
            #region 周美里
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('周美里', 121.583377, 25.057018, 0, 0, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 9, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 後山埤站
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('後山埤站', 121.582911, 25.04487, 0, 0, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 3, 1)", stopID);
            _coreDb.Execute();
            #endregion

            #region 秀朗國小
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('秀朗國小', 121.521262, 24.999231, 121.58084978, 25.05816265, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 3, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 週美里
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('週美里', 121.583377, 25.057018, 121.56014515, 25.03309135, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 松山家商
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('松山家商', 121.580912, 25.035887, 121.5546072, 25.0299129, 'a0203666@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 1, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 永和市永元路
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('永和市永元路', 121.522975, 25.001647, 121.5525412, 25.0270202, 'samjsck@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 2, 0)", stopID);
            _coreDb.Execute();
            #endregion
            #endregion

            #region line2
            #region 永和路
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('永和路', 120.635709, 24.21521, 121.5614992, 25.0326352, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 永和市永元路
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('永和市永元路', 121.522638, 25.000503, 121.5528976, 25.024357, 'samjsck@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 新湖舊宗路口
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('新湖舊宗路口', 121.578653, 25.062323, 121.5777363, 25.0634112, 'denniswa@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 新湖一路口
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('新湖一路口', 121.579208, 25.060522, 121.57881499, 25.06057842, 'joycewang914@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 1, 0)", stopID);
            _coreDb.Execute();
            #endregion
            #endregion
            #endregion

            Thread.Sleep(CONST_SLEEP_TIME);

            BusWebDataService target = BusWebDataService.GetServiceInstance();
            string owner = "joycewang914@gmail.com";
            int expected = 0;
            int actual = target.GetStopIDByOwnerAndStopID(stopID - 1, owner);
            Assert.AreEqual<int>(expected, actual, "We should get zero as invalid stopID!");
        }
        /// <summary>
        ///A test for MergeLines manually
        ///</summary>
        [TestMethod()]
        public void MergeLinesByManualTest()
        {
            #region prepare testing data
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Lines(LineName) VALUES('207')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(LineID) FROM Lines";
            int lineId1 = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Lines(LineName) VALUES('207')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(LineID) FROM Lines";
            int lineId2 = Convert.ToInt32(_coreDb.ExecuteScalar());

            int stopID = 0;

            #region line1
            #region 周美里
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('周美里', 121.583377, 25.057018, 0, 0, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 9, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 後山埤站
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('後山埤站', 121.582911, 25.04487, 0, 0, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 3, 1)", stopID);
            _coreDb.Execute();
            #endregion

            #region 秀朗國小
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('秀朗國小', 121.521262, 24.999231, 121.58084978, 25.05816265, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 3, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 週美里
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('週美里', 121.583377, 25.057018, 121.56014515, 25.03309135, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 松山家商
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('松山家商', 121.580912, 25.035887, 121.5546072, 25.0299129, 'a0203666@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 1, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 永和市永元路
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('永和市永元路', 121.522975, 25.001647, 121.5525412, 25.0270202, 'samjsck@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId1, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 2, 0)", stopID);
            _coreDb.Execute();
            #endregion
            #endregion

            #region line2
            #region 永和路
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('永和路', 120.635709, 24.21521, 121.5614992, 25.0326352, 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 永和市永元路
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('永和市永元路', 121.522638, 25.000503, 121.5528976, 25.024357, 'samjsck@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 新湖舊宗路口
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('新湖舊宗路口', 121.578653, 25.062323, 121.5777363, 25.0634112, 'denniswa@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 0, 0)", stopID);
            _coreDb.Execute();
            #endregion

            #region 新湖一路口
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, Owner) VALUES('新湖一路口', 121.579208, 25.060522, 121.57881499, 25.06057842, 'joycewang914@gmail.com')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineId2, stopID);
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 1, 0)", stopID);
            _coreDb.Execute();
            #endregion
            #endregion
            #endregion

            Thread.Sleep(CONST_SLEEP_TIME);
            
            BusWebDataService target = BusWebDataService.GetServiceInstance();
            bool expected = true;
            bool actual = target.MergeLines(lineId1, lineId2);
            Assert.AreEqual<bool>(expected, actual, "The merge result should be success!");

            Thread.Sleep(CONST_SLEEP_TIME);

            #region verifying
            DsBusWeb.LinesDataTable dt = new DsBusWeb.LinesDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Lines";
            _coreDb.Fill(dt);

            Assert.AreEqual<int>(1, dt.Count, "Line count should be only 1 left!");
            DsBusWeb.Line2StopDataTable dt2 = new DsBusWeb.Line2StopDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Line2Stop";
            _coreDb.Fill(dt2);
            Assert.AreEqual<int>(9, dt2.Count, "Stops should be only 9 links left!");
            foreach (var dr in dt2) Assert.AreEqual<int>(lineId1, dr.LineID, "All LineID should point to the 1st line!");

            DsBusWeb.StopsDataTable dt3 = new DsBusWeb.StopsDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Stops";
            _coreDb.Fill(dt3);
            Assert.AreEqual<int>(9, dt3.Count, "Stops should be 9 stops left!");
            #endregion
        }
        /// <summary>
        ///A test for LogAUsageCase
        ///</summary>
        [TestMethod()]
        public void LogAUsageCaseTest()
        {
            BusWebDataService target = BusWebDataService.GetServiceInstance();
            DsBusWeb.UsageStatisticRow dr = new DsBusWeb.UsageStatisticDataTable().NewUsageStatisticRow();
            dr.Device = 1;
            dr.Latitude = 25.0;
            dr.Longitude = 121.0;
            dr.Radius = 500;
            dr.UserCode = "tomtang0406@gmail.com";
            target.LogAUsageCase(dr);

            Thread.Sleep(CONST_SLEEP_TIME);

            #region verifying
            DsBusWeb.UsageStatisticDataTable dt = new DsBusWeb.UsageStatisticDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM UsageStatistic";
            _coreDb.Fill(dt);
            Assert.AreEqual<int>(1, dt.Count, "Row count is not expeceted!");
            Assert.AreEqual<string>(dr.UserCode, dt[0].UserCode, "UserCode is not expected!");
            Assert.AreEqual<int>(dr.Device, dt[0].Device, "Device is not expected!");
            Assert.AreEqual<double>(dr.Latitude, dt[0].Latitude, "Latitude is not expected!");
            Assert.AreEqual<double>(dr.Longitude, dt[0].Longitude, "Longitude is not expected!");
            Assert.AreEqual<double>(dr.Radius, dt[0].Radius, "Radius is not expected!");
            #endregion
        }
        /// <summary>
        ///A test for LogAUsageCase without any exception even there's an error inside
        ///</summary>
        [TestMethod()]
        public void LogAUsageCaseNoExceptionTest()
        {
            BusWebDataService target = BusWebDataService.GetServiceInstance();
            DsBusWeb.UsageStatisticRow dr = new DsBusWeb.UsageStatisticDataTable().NewUsageStatisticRow();
            dr.Device = 1;
            dr.Latitude = 25.0;
            dr.Longitude = 121.0;
            dr.Radius = 512;
            dr.UserCode = "tomtang0406@gmail.comxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"; // Length is out of range
            target.LogAUsageCase(dr);

            Thread.Sleep(CONST_SLEEP_TIME);

            #region verifying
            DsBusWeb.UsageStatisticDataTable dt = new DsBusWeb.UsageStatisticDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM UsageStatistic";
            _coreDb.Fill(dt);
            Assert.AreEqual<int>(0, dt.Count, "Row count is not expeceted!");
            #endregion
        }
        /// <summary>
        ///A test for RemoveBadRatingStops
        ///</summary>
        [TestMethod()]
        public void RemoveBadRatingStopsTest()
        {
            #region prepare testing data
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Lines(LineName) VALUES('286')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(LineID) FROM Lines";
            int lineID = Convert.ToInt32(_coreDb.ExecuteScalar());

            #region bad rating stop. will be deleted
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, Culture, Owner) VALUES('BAD', 121.543615, 25.033369, 'zh-TW', 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            int stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 2, 4)", stopID);
            _coreDb.Execute();

            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineID, stopID);
            _coreDb.Execute();
            #endregion

            #region other rating. won't be deleted
            _coreDb.ExecuteCommand.CommandText = "INSERT INTO Stops(StopName, Longitude, Latitude, Culture, Owner) VALUES('NORMAL', 121.543615, 25.033369, 'zh-TW', 'SYSTEM')";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "SELECT MAX(StopID) FROM Stops";
            stopID = Convert.ToInt32(_coreDb.ExecuteScalar());
            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO StopRating(StopID, RatingGood, RatingBad) VALUES({0}, 2, 3)", stopID);
            _coreDb.Execute();

            _coreDb.ExecuteCommand.CommandText = string.Format("INSERT INTO Line2Stop(LineID, StopID) VALUES({0}, {1})", lineID, stopID);
            _coreDb.Execute();
            #endregion

            #endregion

            Thread.Sleep(CONST_SLEEP_TIME);

            var target = BusWebDataService.GetServiceInstance();
            target.RemoveBadRatingStops();

            Thread.Sleep(CONST_SLEEP_TIME);

            #region verifying
            DsBusWeb.StopsDataTable dt = new DsBusWeb.StopsDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Stops";
            _coreDb.Fill(dt);
            Assert.AreEqual<int>(1, dt.Count, "Row count is not expeceted!");
            Assert.AreEqual<string>("NORMAL", dt[0].StopName, "StopName is not expeceted!");
            #endregion
        }
    }
}

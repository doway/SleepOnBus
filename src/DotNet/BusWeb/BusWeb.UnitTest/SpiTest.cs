using System;
using System.Threading;
using Bus.Model;
using BusWeb.DAO.DataSet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BusWeb.UnitTest
{
    /// <summary>
    ///This is a test class for SpiTest and is intended
    ///to contain all SpiTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SpiTest : UnitTestBase
    {
#if(DEBUG)
        /// <summary>
        ///A test for deleteStop
        ///</summary>
        [TestMethod()]
        public void deleteStopTest()
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

            BusWeb.Spi.Spi target = new BusWeb.Spi.Spi();
            string owner = "joycewang914@gmail.com";
            bool expected = true;
            bool actual = target.deleteStop(stopID, owner);

            Thread.Sleep(CONST_SLEEP_TIME);

            #region verifying
            Assert.AreEqual<bool>(expected, actual, "We shall got return value TRUE as success result.");
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
        ///A test for deleteStop but owner not match.
        ///</summary>
        [TestMethod()]
        public void deleteStopButOwnerNotMatchTest()
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

            BusWeb.Spi.Spi target = new BusWeb.Spi.Spi();
            string owner = "SYSTEM";
            bool expected = false;
            bool actual = target.deleteStop(stopID, owner);

            #region verifying
            Assert.AreEqual<bool>(expected, actual, "We shall got return value FALSE as success result.");
            DsBusWeb.LinesDataTable dt = new DsBusWeb.LinesDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Lines";
            _coreDb.Fill(dt);

            Assert.AreEqual<int>(2, dt.Count, "Line count should be only 2 left!");
            DsBusWeb.Line2StopDataTable dt2 = new DsBusWeb.Line2StopDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Line2Stop";
            _coreDb.Fill(dt2);
            Assert.AreEqual<int>(10, dt2.Count, "Stops should be only 10 links left!");

            DsBusWeb.StopsDataTable dt3 = new DsBusWeb.StopsDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Stops";
            _coreDb.Fill(dt3);
            Assert.AreEqual<int>(10, dt3.Count, "Stops should be 10 stops left!");
            #endregion
        }
        /// <summary>
        ///A test for insertNewStop
        ///</summary>
        [TestMethod()]
        public void insertNewStopTest()
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
            #endregion

            Thread.Sleep(CONST_SLEEP_TIME);

            BusWeb.Spi.Spi target = new BusWeb.Spi.Spi();
            int lineID = lineId1;
            string stopName = "測試路線";
            double longitude = 123F;
            double latitude = 25F;
            double curLongitude = 124F;
            double curLatitude = 26F;
            string owner = "DOWILL";
            string culture = "zh-TW";
            target.insertNewStop(lineID, stopName, longitude, latitude, curLongitude, curLatitude, owner, culture);

            Thread.Sleep(CONST_SLEEP_TIME);

            #region verifying
            DsBusWeb.LinesDataTable dt = new DsBusWeb.LinesDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Lines";
            _coreDb.Fill(dt);
            Assert.AreEqual<int>(2, dt.Count, "Line count should be 2!");

            DsBusWeb.Line2StopDataTable dt2 = new DsBusWeb.Line2StopDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Line2Stop";
            _coreDb.Fill(dt2);
            Assert.AreEqual<int>(1, dt2.Count, "Stops should be only 1 links!");

            DsBusWeb.StopsDataTable dt3 = new DsBusWeb.StopsDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Stops";
            _coreDb.Fill(dt3);
            Assert.AreEqual<int>(1, dt3.Count, "Stops should be 1 stops!");

            var dr = dt3[0];
            Assert.AreEqual<string>(stopName, dr.StopName, "StopName is incorrect!");
            Assert.AreEqual<double>(longitude, dr.Longitude, "Longitude is incorrect!");
            Assert.AreEqual<double>(latitude, dr.Latitude, "Latitude is incorrect!");
            Assert.AreEqual<double>(curLongitude, dr.CreatorLongitude, "CreatorLongitude is incorrect!");
            Assert.AreEqual<double>(curLatitude, dr.CreatorLatitude, "CreatorLatitude is incorrect!");
            Assert.AreEqual<string>(owner, dr.Owner, "Owner is incorrect!");
            Assert.AreEqual(culture, dr["Culture"], "Culture is incorrect!");
            #endregion
        }
        /// <summary>
        ///A test for insertNewStop with double submitting
        ///</summary>
        [TestMethod()]
        public void insertNewStopWithDoubleSubmittingTest()
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
            #endregion

            Thread.Sleep(CONST_SLEEP_TIME);

            BusWeb.Spi.Spi target = new BusWeb.Spi.Spi();
            int lineID = lineId1;
            string stopName = "測試路線";
            double longitude = 123F;
            double latitude = 25F;
            double curLongitude = 124F;
            double curLatitude = 26F;
            string owner = "DOWILL";
            string culture = "zh-TW";

            target.insertNewStop(lineID, stopName, longitude, latitude, curLongitude, curLatitude, owner, culture);

            Thread.Sleep(CONST_SLEEP_TIME);

            target.insertNewStop(lineID, stopName, longitude, latitude, curLongitude, curLatitude, owner, culture);

            Thread.Sleep(CONST_SLEEP_TIME);

            #region verifying
            DsBusWeb.LinesDataTable dt = new DsBusWeb.LinesDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Lines";
            _coreDb.Fill(dt);
            Assert.AreEqual<int>(2, dt.Count, "Line count should be 2!");

            DsBusWeb.Line2StopDataTable dt2 = new DsBusWeb.Line2StopDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Line2Stop";
            _coreDb.Fill(dt2);
            Assert.AreEqual<int>(1, dt2.Count, "Stops should be only 1 links!");

            DsBusWeb.StopsDataTable dt3 = new DsBusWeb.StopsDataTable();
            _coreDb.SelectCommand.CommandText = "SELECT * FROM Stops";
            _coreDb.Fill(dt3);
            Assert.AreEqual<int>(1, dt3.Count, "Stops should be 1 stops!");

            var dr = dt3[0];
            Assert.AreEqual<string>(stopName, dr.StopName, "StopName is incorrect!");
            Assert.AreEqual<double>(longitude, dr.Longitude, "Longitude is incorrect!");
            Assert.AreEqual<double>(latitude, dr.Latitude, "Latitude is incorrect!");
            Assert.AreEqual<double>(curLongitude, dr.CreatorLongitude, "CreatorLongitude is incorrect!");
            Assert.AreEqual<double>(curLatitude, dr.CreatorLatitude, "CreatorLatitude is incorrect!");
            Assert.AreEqual<string>(owner, dr.Owner, "Owner is incorrect!");
            Assert.AreEqual(culture, dr["Culture"], "Culture is incorrect!");
            #endregion
        }

        /// <summary>
        ///A test for getLineList
        ///</summary>
        [TestMethod()]
        public void getLineListTest()
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
            BusWeb.Spi.Spi target = new BusWeb.Spi.Spi();
            double Longitude = 121F;
            double Latitude = 25F;
            double radius = 1F;
            string owner = "joanna.910522@gmail.com";
            LineInfo[] actual = target.getLineList(Longitude, Latitude, radius, owner, 1);
            Thread.Sleep(CONST_SLEEP_TIME);

            #region verifying
            Assert.AreEqual<int>(1, actual.Length, "The row Length of line list is not expected!");
            Assert.AreEqual<string>("286", actual[0].LineName, "The line name is not expected!");

            DsBusWeb.UsageStatisticRow dr = new DsBusWeb.UsageStatisticDataTable().NewUsageStatisticRow();
            dr.Device = 1;
            dr.Latitude = 25.0;
            dr.Longitude = 121.0;
            dr.Radius = 1;
            dr.UserCode = "joanna.910522@gmail.com";

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
#endif
    }
}

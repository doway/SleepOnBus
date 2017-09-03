using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using DOWILL.DBAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BusWeb.UnitTest
{
    [TestClass]
    public class UnitTestBase
    {
        protected const int CONST_SLEEP_TIME = 5 * 1000;
        protected const string CONST_CONNECTION_KEY = "BusWeb.DAO.Properties.Settings.ConnectionString";
        protected static readonly string _currentAssemblyLocation = null;
        protected static DBOperatorBase _coreDb = null;

        static UnitTestBase()
        {
            FileInfo file = new FileInfo(Assembly.GetExecutingAssembly().Location);
            _currentAssemblyLocation = file.DirectoryName;
            Trace.WriteLine(string.Format("_currentAssemblyCodebase={0}", _currentAssemblyLocation));
            _coreDb = DBOperatorBase.GetDBOperatorObject(DBOperatorType.OleDBOperator,
                string.Format(ConfigurationManager.ConnectionStrings[CONST_CONNECTION_KEY].ConnectionString, _currentAssemblyLocation + @"\..\..\..\BusWeb.UnitTest\Database\"));
        }
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        /// <summary>
        /// Use TestInitialize to run code before running each test
        /// </summary>
        [TestInitialize()]
        public virtual void MyTestInitialize()
        {
            // Clean up all tables
            _coreDb.ExecuteCommand.CommandText = "DELETE FROM Line2Stop";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "DELETE FROM StopRating";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "DELETE FROM Stops";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "DELETE FROM Lines";
            _coreDb.Execute();
            _coreDb.ExecuteCommand.CommandText = "DELETE FROM UsageStatistic";
            _coreDb.Execute();
        }
        
        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            Thread.Sleep(CONST_SLEEP_TIME);
        }
        
        #endregion
    }
}

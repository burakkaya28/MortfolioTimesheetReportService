using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Reflection;
using log4net;
using static System.String;

namespace MortfolioTimesheetReportService
{
    public class Db
    {
        private static SqlConnection _connection;
        private static SqlDataReader _sqlDataReader;
        public const string ReportTitle = "OPTiiM Eksik Time Sheet Raporu";
        private static string _reportMessage = Empty;
        private readonly string _mainDirectory = AppDomain.CurrentDomain.BaseDirectory;

        private static readonly string MortfolioDbConString = ConfigurationManager.AppSettings["MFLO_CON_STRING"];
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string GetHtmlData()
        {
            var flag = false;

            _reportMessage =
                "<table border='1' style='width: 100%; border: 1px solid black;'><thead><tr><td colspan='5' style='text-align: center;'><b> " +
                ReportTitle +
                " </b></td></tr><tr><th>Ad Soyad</th><th>Takım</th><th>Periyod</th><th>Hafta Numarası</th><th>Durum</th></tr></thead><tbody style='text-align: center;'>";

            Console.WriteLine("Main Directory => " + AppDomain.CurrentDomain.BaseDirectory);
            Log.Info("Main Directory => " + AppDomain.CurrentDomain.BaseDirectory);

            var script = File.ReadAllText(_mainDirectory + @"\timesheet.sql");

            _connection = new SqlConnection(MortfolioDbConString);
            _connection.Open();
            _sqlDataReader = new SqlCommand(script, _connection).ExecuteReader();
            while (_sqlDataReader.Read())
            {
                flag = true;
                var userName = _sqlDataReader["USER_NAME"].ToString();
                var userEmail = new App().GetUserMail(userName);
                var fullName = _sqlDataReader["FULL_NAME"].ToString();
                var team = _sqlDataReader["TEAM"].ToString();
                var startDateFormatted = DateTime.Parse(_sqlDataReader["START_DATE"].ToString().ToString(CultureInfo.InvariantCulture).Replace(" 12:00:00 AM", "").Replace("00:00:00", "")).ToString("yyyy-MM-dd");
                var startDate = DateTime.Parse(_sqlDataReader["START_DATE"].ToString().ToString(CultureInfo.InvariantCulture).Replace(" 12:00:00 AM", "").Replace("00:00:00", "")).ToString("dd/MM/yyyy");
                var endDate = DateTime.Parse(_sqlDataReader["END_DATE"].ToString().ToString(CultureInfo.InvariantCulture).Replace(" 12:00:00 AM", "").Replace("00:00:00", "")).ToString("dd/MM/yyyy");
                var weekNumber = _sqlDataReader["WEEK_NUMBER"].ToString();
                var state = _sqlDataReader["STATE"].ToString();

                var leaveExist = new App().CheckLeaveIsExist(startDateFormatted, userEmail);

                //Log for Report
                Console.WriteLine(userEmail + " => " + startDateFormatted + " => " + leaveExist + " => " + weekNumber);

                //If user missing timesheet is not on Leave
                if (leaveExist == "0")
                {
                    _reportMessage += "<tr><td>" + fullName + "</td><td>" + team + "</td><td>" +
                                      startDate + " - " + endDate + "</td><td>" + weekNumber +
                                      "</td><td>" + state + "</td></tr>";
                }
            }
            _connection.Close();

            if (!flag)
                _reportMessage += "<tr><td colspan='5'>Eksik time sheet bulunmamaktadır.</td></tr>";

            return _reportMessage + "</tbody></table>";
        }
    }
}
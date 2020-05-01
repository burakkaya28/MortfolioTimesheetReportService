using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using log4net;

namespace MortfolioTimesheetReportService
{
    public class App
    {
        //Database Connection String
        public string MflodbConString = ConfigurationManager.AppSettings["MFLO_CON_STRING"];
        public string IzinAppConString = ConfigurationManager.AppSettings["IZINAPP_CON_STRING"];

        //Log4net main variable
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string GetUserMail(string username)
        {
            string value = null;
            try
            {
                using (var con = new SqlConnection(MflodbConString))
                {
                    
                    using (var cmd = new SqlCommand("select EMAIL from USERACCOUNT where USER_NAME = '"+username+"'", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        con.Open();
                        value = Convert.ToString(cmd.ExecuteScalar());
                        con.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.Error(ex);
            }
            return value;
        }

        public string CheckLeaveIsExist(string leaveStartDate, string userEmail)
        {
            string value = null;
            try
            {
                using (var con = new SqlConnection(IzinAppConString))
                {
                    
                    using (var cmd = new SqlCommand("select count(*) from LeaveRequests lr join Users u on u.UserId = lr.UserId where Day >= 5 and Status = 1 and '"+leaveStartDate+"' between lr.StartDate and lr.EndDate and DATEADD(DAY, 4, '"+leaveStartDate+"') <= EndDate and u.Email = '"+userEmail+"'", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        con.Open();
                        value = Convert.ToString(cmd.ExecuteScalar());
                        con.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Exception => " + ex);
                Log.Error(ex);
            }

            return value;
        }
    }
}

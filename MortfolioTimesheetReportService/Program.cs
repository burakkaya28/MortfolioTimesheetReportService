using System;
using System.Configuration;
using System.Reflection;
using System.Threading;
using log4net;
using log4net.Config;
using static System.String;

namespace MortfolioTimesheetReportService
{
    public class P
    {
        //Getting Mail Informations from Config File
        private readonly string _senderMail = ConfigurationManager.AppSettings["SENDER_MAIL"];
        private readonly string _senderMailPass = ConfigurationManager.AppSettings["SENDER_MAIL_PASSWORD"];
        private readonly string _senderHost = ConfigurationManager.AppSettings["SENDER_HOST"];
        private readonly bool _senderSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SENDER_SSL"]);
        private readonly int _senderPort = Convert.ToInt32(ConfigurationManager.AppSettings["SENDER_PORT"]);
        private string _tableHtml = Empty;

        //Defining log4net object
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static void Main()
        {
            //Configured Log4Net Logger
            XmlConfigurator.Configure();

            Console.WriteLine("Starting Mortfolio Missing Report Service...");
            Log.Info("Starting Mortfolio Missing Report Service...");

            var p = new P();
            var db = new Db();
            try
            {
                Console.WriteLine("Getting SQL Data from Mortfolio Database...");
                Log.Info("Getting SQL Data from Mortfolio Database...");

                p._tableHtml = db.GetHtmlData(); //Getting HTML data into variable

                Console.WriteLine("Getting SQL Data from Mortfolio Database has completed succesfullly.");
                Log.Info("Getting SQL Data from Mortfolio Database has completed succesfullly.");

                Console.WriteLine("Sending HTML Data to E-Mail System...");
                Log.Info("Sending HTML Data to E-Mail System...");

                //Sending Mail
                new Environment().SendAnEmail(true, p._tableHtml, p._senderMail, p._senderMailPass, p._senderHost,
                    p._senderSsl, p._senderPort);
            }
            catch (Exception ex)
            {
                Log.Error("ERROR: " + ex);
                Console.WriteLine("ERROR: " + ex);
                Console.ReadLine();
            }
            finally
            {
                Console.WriteLine("Mortfolio Missing Report Service has completed succesfullly.");
                Log.Info("Mortfolio Missing Report Service has completed succesfullly.");
                Thread.Sleep(2500);
            }
        }
    }
}

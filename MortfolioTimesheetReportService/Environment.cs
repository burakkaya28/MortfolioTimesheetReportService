using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using log4net;

namespace MortfolioTimesheetReportService
{
    public class Environment
    {
        //Defining log4net object
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //Getting Environment from Enum
        public static string IsProduction =  ConfigurationManager.AppSettings["IS_PRODUCTION"];

        /* PRODUCTION MAIL INFORMATION */
        public static string To1 = ConfigurationManager.AppSettings["RECEIVER_TO_1"];
        public static string To2 = ConfigurationManager.AppSettings["RECEIVER_TO_2"];
        public static string To3 = ConfigurationManager.AppSettings["RECEIVER_TO_3"];
        public static string To4 = ConfigurationManager.AppSettings["RECEIVER_TO_4"];
        public static string To5 = ConfigurationManager.AppSettings["RECEIVER_TO_5"];
        public static string To6 = ConfigurationManager.AppSettings["RECEIVER_TO_6"];
        public static string To7 = ConfigurationManager.AppSettings["RECEIVER_TO_7"];
        public static string To8 = ConfigurationManager.AppSettings["RECEIVER_TO_8"];
        public static string To9 = ConfigurationManager.AppSettings["RECEIVER_TO_9"];
        public static string To10 = ConfigurationManager.AppSettings["RECEIVER_TO_10"];

        public static string Bcc1 = ConfigurationManager.AppSettings["RECEIVER_BCC_1"];
        public static string Bcc2 = ConfigurationManager.AppSettings["RECEIVER_BCC_2"];
        public static string Bcc3 = ConfigurationManager.AppSettings["RECEIVER_BCC_3"];
        public static string Bcc4 = ConfigurationManager.AppSettings["RECEIVER_BCC_4"];
        public static string Bcc5 = ConfigurationManager.AppSettings["RECEIVER_BCC_5"];

        /* DEVELOPMENT MAIL INFORMATION */
        public static string DevTo1 = "burak.kaya@optiim.com";
       

        public void SendAnEmail(bool isHtml, string body, string sender, string senderPass, string host, bool enableSsl, int port)
        {
            try
            {
                Console.WriteLine("Sending E-Mail process is starting...");
                Log.Info("Sending E-Mail process is starting...");

                //Defining mail object
                var message = new MailMessage()
                {
                    From = new MailAddress(sender)
                };

                //If environment is production
                if (IsProduction == "1")
                {
                    //Getting to, cc and bcc list
                    if (To1 != "") message.To.Add(new MailAddress(To1));
                    if (To2 != "") message.To.Add(new MailAddress(To2));
                    if (To3 != "") message.To.Add(new MailAddress(To3));
                    if (To4 != "") message.To.Add(new MailAddress(To4));
                    if (To5 != "") message.To.Add(new MailAddress(To5));
                    if (To6 != "") message.To.Add(new MailAddress(To6));
                    if (To7 != "") message.To.Add(new MailAddress(To7));
                    if (To8 != "") message.To.Add(new MailAddress(To8));
                    if (To9 != "") message.To.Add(new MailAddress(To9));
                    if (To10 != "") message.To.Add(new MailAddress(To10));

                    if (Bcc1 != "") message.Bcc.Add(new MailAddress(Bcc1));
                    if (Bcc2 != "") message.Bcc.Add(new MailAddress(Bcc2));
                    if (Bcc3 != "") message.Bcc.Add(new MailAddress(Bcc3));
                    if (Bcc4 != "") message.Bcc.Add(new MailAddress(Bcc4));
                    if (Bcc5 != "") message.Bcc.Add(new MailAddress(Bcc5));
                }
                else //If environment is development
                {
                    if (DevTo1 != "")
                    {
                        message.To.Add(new MailAddress(DevTo1));
                    }
                }

                message.Subject = "Mortfolio Eksik Time Sheet Raporu " + DateTime.Now.ToString("dd.MM.yyy");
                message.Body = body;
                message.IsBodyHtml = isHtml;
                
                //Defining SMTP object
                var smtpClient = new SmtpClient()
                {
                    Credentials = new NetworkCredential(sender, senderPass),
                    Host = host,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = enableSsl,
                    Port = port
                };

                try //Sending Mail
                {
                    smtpClient.SendAsync(message, message);
                }
                catch (SmtpException ex)
                {
                    Log.Error(ex.ToString());
                    Console.WriteLine(ex.ToString());
                }
                Console.WriteLine("Sending E-Mail process has completed succesfullly.");
                Log.Info("Sending E-Mail process has completed succesfullly.");
            }
            catch (Exception ex)
            {
                Log.Error("ERROR: " + ex);
                Console.WriteLine("ERROR: " + ex);
                Console.ReadLine();
            }
        }
    }
}
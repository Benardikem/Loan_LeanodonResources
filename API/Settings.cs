using System;
using System.Configuration;
using System.Data.SqlClient;

namespace API.Settings
{
    /// <summary>
    /// Email SMTP Settings
    /// </summary>
    public class SMTP
    {
        /// <summary>
        /// SMTP Host
        /// </summary>
        public String Host
        {
            get;
            set;
        }

        /// <summary>
        /// SMTP Sender Name
        /// </summary>
        public String Name
        {
            get;
            set;
        }

        /// <summary>
        /// SMTP Address
        /// </summary>
        public String Address
        {
            get;
            set;
        }

        /// <summary>
        /// SMTP Password
        /// </summary>
        public String Password
        {
            get;
            set;
        }

        /// <summary>
        /// Send Mail Securely
        /// </summary>
        public Boolean Secure
        {
            get;
            set;
        }

        /// <summary>
        /// SMTP Sending Port
        /// </summary>
        public int Port
        {
            get;
            set;
        }

        /// <summary>
        /// SMTP Seding Timeout
        /// </summary>
        public int Timeout
        {
            get;
            set;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public SMTP()
        {
        }

        /// <summary>
        /// Initialize With Values
        /// </summary>
        /// <param name="WithValues">WithValues = True</param>
        public SMTP(Boolean WithValues = true)
        {
            if (WithValues)
            {
                Host = ConfigurationManager.AppSettings["SMTPHost"];
                Name = ConfigurationManager.AppSettings["SMTPName"];
                Address = ConfigurationManager.AppSettings["SMTPAddress"];
                Password = Utilities.Crypto.Decrypt(ConfigurationManager.AppSettings["SMTPPassword"]).ToString();
                Secure = Convert.ToBoolean(ConfigurationManager.AppSettings["SMTPSecure"]);
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPTimeout"]);
            }
        }

    }


    /// <summary>
    /// SMS Settings
    /// </summary>
    public class SMS
    {

        /// <summary>
        /// SMS Gateway User ID
        /// </summary>
        public String GateWayUserID
        {
            get;
            set;
        }

        /// <summary>
        /// SMS Gateway Password
        /// </summary>
        public String GateWayPassword
        {
            get;
            set;
        }

        /// <summary>
        /// SMS Gateway Sender
        /// </summary>
        public String GateWaySender
        {
            get;
            set;
        }

        /// <summary>
        /// SMS GateWay Base URL
        /// </summary>
        public String GatewayBaseURL
        {
            get;
            set;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public SMS()
        {
        }

        /// <summary>
        /// Initialize With Values
        /// </summary>
        /// <param name="WithValues">WithValues = True</param>
        public SMS(Boolean WithValues = true)
        {
            if (WithValues)
            {
                GateWayPassword = Utilities.Crypto.Decrypt(ConfigurationManager.AppSettings["GatewayPassword"]).ToString();
                GateWaySender = ConfigurationManager.AppSettings["GatewaySender"];
                GateWayUserID = ConfigurationManager.AppSettings["GatewayUserID"];
                GatewayBaseURL = ConfigurationManager.AppSettings["GatewayBaseURL"];
            }
        }

        ///// <summary>
        ///// Initialize with Values public to n Inheritor
        ///// </summary>
        ///// <param name="sms"></param>
        //public SMS(SMS sms) : this(true)
        //{
        //}

    }


    /// <summary>
    /// Application Specific Settings
    /// </summary>
    public class Application
    {
        /// <summary>
        /// Application Timezone
        /// </summary>
        public String TimeZone
        {
            get;
            set;
        }

        /// <summary>
        /// Default Date for Database
        /// </summary>
        public String DefaultDate
        {
            get;
            set;
        }

        /// <summary>
        /// Database Date Format
        /// </summary>
        public String DateFormat
        {
            get;
            set;
        }

        /// <summary>
        /// Database Time Format for 12-Hour Representation
        /// </summary>
        public String TimeFormat_12H
        {
            get;
            set;
        }

        /// <summary>
        /// Database Time Format for 24-Hour Representation
        /// </summary>
        public String TimeFormat_24H
        {
            get;
            set;
        }

        public static String Mode
        {
            get { return ConfigurationManager.AppSettings["AppMode"]; }
        }

        public static int AgentRoleId
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["AgentRoleId"]); }
        }

        public static String ProcessedFilePath
        {
            get { return ConfigurationManager.AppSettings["ProcessedFilePath"]; }
        }

        public static String PaymentFilePath
        {
            get { return ConfigurationManager.AppSettings["PaymentFilePath"]; }
        }
        /// <summary>
        /// Initialize
        /// </summary>
        public Application()
        {
        }

        /// <summary>
        /// Initialize With Values
        /// </summary>
        /// <param name="WithValues">WithValues = True</param>
        public Application(Boolean WithValues = true)
        {
            if (WithValues)
            {
                TimeZone = ConfigurationManager.AppSettings["Timezone"];
                DefaultDate = ConfigurationManager.AppSettings["DefaultDate"];
                DateFormat = ConfigurationManager.AppSettings["DateFormat"];
                TimeFormat_12H = ConfigurationManager.AppSettings["TimeFormat_12H"];
                TimeFormat_24H = ConfigurationManager.AppSettings["TimeFormat_24H"];
            }
        }

    }


    /// <summary>
    /// Database Settings
    /// </summary>
    public class Database
    {
        /// <summary>
        /// Database Server Address
        /// </summary>
        public String Server
        {
            get;
            set;
        }

        /// <summary>
        /// Database Name
        /// </summary>
        public String Name
        {
            get;
            set;
        }

        /// <summary>
        /// Database User
        /// </summary>
        public String User
        {
            get;
            set;
        }

        /// <summary>
        /// Database Password
        /// </summary>
        public String Password
        {
            get;
            set;
        }

        /// <summary>
        /// Database Backup Path
        /// </summary>
        public String BackupPath
        {
            get;
            set;
        }

        /// <summary>
        /// Database Restore Path
        /// </summary>
        public String RestorePath
        {
            get;
            set;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public Database()
        {
        }

        /// <summary>
        /// Initialize With Values
        /// </summary>
        /// <param name="WithValues">WithValues = true</param>
        public Database(Boolean WithValues = true)
        {
            Server = ConfigurationManager.AppSettings["Server"];
            Name = ConfigurationManager.AppSettings["Name"];
            User = ConfigurationManager.AppSettings["User"];
            Password = Utilities.Crypto.Decrypt(ConfigurationManager.AppSettings["Password"]).ToString();
        }

        /// <summary>
        /// Get Database Connection String
        /// </summary>
        /// <returns>Connection String</returns>
        public static String ConnectionString()
        {
            Database d = new Database(true);
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder["Server"] = d.Server;
            builder["uid"] = d.User;
            builder["pwd"] = d.Password;
            builder["Database"] = d.Name;
            builder["MultipleActiveResultSets"] = true;
            return builder.ConnectionString;
        }

        /// <summary>
        /// Database Connection
        /// </summary>
        public static SqlConnection Connection
        {
            get
            {

                return new SqlConnection(ConnectionString());
            }
        }
    }


    /// <summary>
    /// Site Settings
    /// </summary>
    public class Site
    {
        public static String SitePath
        {
            get { return ConfigurationManager.AppSettings["SitePath"]; }
        }

        /// <summary>
        /// Site URL
        /// </summary>
        public static String SiteURL
        {
            get { return ConfigurationManager.AppSettings["SiteURL"]; }
        }

        public static int AgentRoleId { get { return Convert.ToInt32(ConfigurationManager.AppSettings["AgentRoleId"]); } }
        public static string Phone { get { return ConfigurationManager.AppSettings["ContactPhone"]; } }
    }
}

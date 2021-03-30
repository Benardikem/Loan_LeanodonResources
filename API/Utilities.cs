using API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using System.Data;
using log4net;
using System.Globalization;
using System.Data.SqlClient;

namespace API.Utilities
{
    public class Constants
    {
        public const string DIVIDEND_PAYMENT = "DP";
        public const string LOCAL_CURRENCY = "NGN";
        public const int DOC_PROCESS_RETRIES = 10;
        public const int DOC_PROCESS_BATCH_COUNT = 1000;
        public static CultureInfo ci = new CultureInfo("en-GB");
        public static string[] formats = new string[] {
            "dd-MM-yyyy",
            "dd-MM-yy",
            "dd-MMM-yyyy",
            "dd-MMM-yy",
            "dd-MM-yyyy hh:mm:ss tt",
            "dd-MM-yy hh:mm:ss tt",
            "dd-MMM-yyyy hh:mm:ss tt",
            "dd-MMM-yy hh:mm:ss tt",
            "dd/MM/yy hh:mm:ss tt",
            "MM/dd/yy hh:mm:ss tt",
            "MM/dd/yyyy hh:mm:ss tt",
            "yyyy-MM-dd hh:mm:ss tt",
            "yyyy-MM-dd"
        };
    }
    #region ENUMS
    /// <summary>
    /// Top Level when calculating time length
    /// </summary>
    public enum TimeLengthTopLevel
    {
        /// <summary>
        /// Seconds
        /// </summary>
        Seconds,

        /// <summary>
        /// Minutes
        /// </summary>
        Minutes,

        /// <summary>
        /// Hours
        /// </summary>
        Hours,

        /// <summary>
        /// Days
        /// </summary>
        Days,

        /// <summary>
        /// Weeks
        /// </summary>
        Weeks,

        /// <summary>
        /// Months
        /// </summary>
        Months,

        /// <summary>
        /// Years
        /// </summary>
        Years
    }

    /// <summary>
    /// Application Event Type
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// Information Type Message
        /// </summary>
        Information = 0,

        /// <summary>
        /// Error Type Message
        /// </summary>
        Error = -1,

        /// <summary>
        /// Success Type Message
        /// </summary>
        Success = 1,

        /// <summary>
        /// Warning Type Message
        /// </summary>
        Warning = 2,

        /// <summary>
        /// General Type Message
        /// </summary>
        General = 99
    }

    /// <summary>
    /// Information Messagebox Types
    /// </summary>
    public enum MessageBoxType
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,

        /// <summary>
        /// Error
        /// </summary>
        Error = -1,

        /// <summary>
        /// Information
        /// </summary>
        Information = 2,

        /// <summary>
        /// Success
        /// </summary>
        Success = 1,

        /// <summary>
        /// Warning
        /// </summary>
        Warning = 3,

        /// <summary>
        /// General
        /// </summary>
        General = 99
    }

    /// <summary>
    /// Approval Status
    /// </summary>
    public enum Approval
    {
        Approved = 1,
        Unapproved = 0,
        Pending = 2
    }

    public enum ControlType
    {
        TEXTBOX = 0,
        LOOKUP = 1
    }

    public enum FieldType
    {
        Text,
        Email,
        Phone,
        Number,
        Date,
        Decimal
    }
    #endregion
    public static class Extensions
    {
        public static string StripHtmlTags(this string source)
        {
            return System.Text.RegularExpressions.Regex.Replace(source, "<.*?>|&.*?;", string.Empty);
        }
    }
    /// <summary>
    /// Date and Time Utilities Class
    /// </summary>
    public class Date
    {

        static Settings.Application a = new Settings.Application(true);

        #region PROPERTIES

        /// <summary>
        /// Integer Value of the Month
        /// </summary>
        public int MonthValue
        {
            get;
            set;
        }

        /// <summary>
        /// String Value of the Month
        /// </summary>
        public String MonthString
        {
            get;
            set;
        }

        #endregion


        /// <summary>
        /// Gets the length of time between a point in time and now.
        /// </summary>
        /// <param name="Time">The point in time</param>
        /// <returns>Length of time, expressed as a string of seconds, days, weeks, months or years.</returns>
        public static String CalculateTimeLength(DateTime Time)
        {
            int value = 0;
            String period = "";

            TimeSpan ts = GetDateTimeByTimeZone(DateTime.Now) - Time;
            int hrs = ts.Hours;
            int min = ts.Minutes;
            int sec = ts.Seconds;
            int day = ts.Days;
            int week;
            int month;
            int year;

            if (sec == 0)
            {
                period = "seconds";
                value = sec;
            }

            if (sec == 1)
            {
                period = "second";
                value = sec;
            }

            if (sec > 1)
            {
                period = "seconds";
                value = sec;
            }

            if (min == 1)
            {
                period = "minute";
                value = min;
            }

            if (min > 1)
            {
                period = "minutes";
                value = min;
            }

            if (hrs == 1)
            {
                period = "hour";
                value = hrs;
            }

            if (hrs > 1)
            {
                period = "hours";
                value = hrs;
            }

            if (day == 1)
            {
                period = "day";
                value = day;
            }

            if (day > 1)
            {
                period = "days";
                value = day;
            }

            if ((day >= 7))
            {
                week = (int)day / 7;
                if (week == 1)
                {
                    period = "week";
                    value = week;
                }

                if (week > 1)
                {
                    period = "weeks";
                    value = week;
                }

                if ((week >= 4))
                {
                    month = (int)week / 4;
                    if (month == 1)
                    {
                        period = "month";
                        value = month;
                    }

                    if (month > 1)
                    {
                        period = "months";
                        value = month;
                    }

                    if ((month >= 12))
                    {
                        year = (int)month / 12;
                        if (year == 1)
                        {
                            period = "year";
                            value = year;
                        }

                        if (year > 1)
                        {
                            period = "years";
                            value = year;
                        }
                    }
                }
            }

            return value + " " + period + " ago";

        }


        /// <summary>
        /// Gets the length of time between a point in time and now, guaged by the maximum amount of calculation allowed
        /// </summary>
        /// <param name="Time">The point in time</param>
        /// <param name="TopLevel">The maximum amount of calculation allowed.</param>
        /// <returns>Length of time, expressed as a string of seconds, days, weeks, months or years.</returns>
        public static String CalculateTimeLength(DateTime Time, TimeLengthTopLevel TopLevel)
        {
            int value = 0;
            String period = "";

            TimeSpan ts = DateTime.Now - Time;
            int hrs = ts.Hours;
            int min = ts.Minutes;
            int sec = ts.Seconds;
            int day = ts.Days;
            int week;
            int month;
            int year;

            if (sec == 0)
            {
                period = "seconds";
                value = sec;
            }

            if (sec == 1)
            {
                period = "second";
                value = sec;
            }

            if (sec > 1)
            {
                period = "seconds";
                value = sec;
            }

            //=========================================
            if (TopLevel == TimeLengthTopLevel.Seconds)
                return value + " " + period + " ago";
            //=========================================

            if (min == 1)
            {
                period = "minute";
                value = min;
            }

            if (min > 1)
            {
                period = "minutes";
                value = min;
            }

            //=========================================
            if (TopLevel == TimeLengthTopLevel.Minutes)
                return value + " " + period + " ago";
            //=========================================

            if (hrs == 1)
            {
                period = "hour";
                value = hrs;
            }

            if (hrs > 1)
            {
                period = "hours";
                value = hrs;
            }

            //=========================================
            if (TopLevel == TimeLengthTopLevel.Hours)
                return value + " " + period + " ago";
            //=========================================

            if (day == 1)
            {
                period = "day";
                value = day;
            }

            if (day > 1)
            {
                period = "days";
                value = day;
            }

            //=========================================
            if (TopLevel == TimeLengthTopLevel.Days)
                return value + " " + period + " ago";
            //=========================================

            if ((day >= 7))
            {
                week = (int)day / 7;
                if (week == 1)
                {
                    period = "week";
                    value = week;
                }

                if (week > 1)
                {
                    period = "weeks";
                    value = week;
                }

                //=========================================
                if (TopLevel == TimeLengthTopLevel.Weeks)
                    return value + " " + period + " ago";
                //=========================================

                if ((week >= 4))
                {
                    month = (int)week / 4;
                    if (month == 1)
                    {
                        period = "month";
                        value = month;
                    }

                    if (month > 1)
                    {
                        period = "months";
                        value = month;
                    }

                    //=========================================
                    if (TopLevel == TimeLengthTopLevel.Months)
                        return value + " " + period + " ago";
                    //=========================================

                    if ((month >= 12))
                    {
                        year = (int)month / 12;
                        if (year == 1)
                        {
                            period = "year";
                            value = year;
                        }

                        if (year > 1)
                        {
                            period = "years";
                            value = year;
                        }
                    }

                    //=========================================
                    if (TopLevel == TimeLengthTopLevel.Years)
                        return value + " " + period + " ago";
                    //=========================================
                }
            }

            return value + " " + period + " ago";

        }


        /// <summary>
        /// Gets the length of time between a point in time and now, guaged by the maximum amount of calculation allowed
        /// </summary>
        /// <param name="Time">The point in time</param>
        /// <returns>Length of time, expressed as a Timespan.</returns>
        public static TimeSpan CalculateTimeSpan(DateTime Time)
        {
            TimeSpan ts = DateTime.Now - Time;
            return ts;
        }


        /// <summary>
        /// Gets Month value based on Text String or Integer value of the Month
        /// </summary>
        /// <param name="Month">Month Value</param>
        /// <param name="Abbreviation">True, If Month Value is String and abbreviated; False, otherwise</param>
        /// <returns>Date and Time Object</returns>
        public static Date GetMonth(Object Month, Boolean Abbreviation = false)
        {
            Date D = new Date();
            Dictionary<Object, Object> Months = new Dictionary<Object, Object>();

            if (Month.GetType() == typeof(Int32))
            {
                if (!Abbreviation)
                {
                    Months.Add(1, "JANUARY");
                    Months.Add(2, "FEBRUARY");
                    Months.Add(3, "MARCH");
                    Months.Add(4, "APRIL");
                    Months.Add(5, "MAY");
                    Months.Add(6, "JUNE");
                    Months.Add(7, "JULY");
                    Months.Add(8, "AUGUST");
                    Months.Add(9, "SEPTEMBER");
                    Months.Add(10, "OCTOBER");
                    Months.Add(11, "NOVEMBER");
                    Months.Add(12, "DECEMBER");
                }
                else
                {
                    Months.Add(1, "JAN");
                    Months.Add(2, "FEB");
                    Months.Add(3, "MAR");
                    Months.Add(4, "APR");
                    Months.Add(5, "MAY");
                    Months.Add(6, "JUN");
                    Months.Add(7, "JUL");
                    Months.Add(8, "AUG");
                    Months.Add(9, "SEPT");
                    Months.Add(10, "OCT");
                    Months.Add(11, "NOV");
                    Months.Add(12, "DEC");
                }
            }
            else if (Month.GetType() == typeof(String))
            {
                if (!Abbreviation)
                {
                    Months.Add("JANUARY", 1);
                    Months.Add("FEBRUARY", 2);
                    Months.Add("MARCH", 3);
                    Months.Add("APRIL", 4);
                    Months.Add("MAY", 5);
                    Months.Add("JUNE", 6);
                    Months.Add("JULY", 7);
                    Months.Add("AUGUST", 8);
                    Months.Add("SEPTEMBER", 9);
                    Months.Add("OCTOBER", 10);
                    Months.Add("NOVEMBER", 11);
                    Months.Add("DECEMBER", 12);
                }
                else
                {
                    Months.Add("JAN", 1);
                    Months.Add("FEB", 2);
                    Months.Add("MAR", 3);
                    Months.Add("APR", 4);
                    Months.Add("MAY", 5);
                    Months.Add("JUN", 6);
                    Months.Add("JUL", 7);
                    Months.Add("AUG", 8);
                    Months.Add("SEPT", 9);
                    Months.Add("OCT", 10);
                    Months.Add("NOV", 11);
                    Months.Add("DEC", 12);
                }
            }

            if (Month.GetType() == typeof(Int32))
            {
                String Val = (String)Months[Month];
                D.MonthValue = (Int32)Month;
                D.MonthString = Val;
            }
            else if (Month.GetType() == typeof(String))
            {
                Int32 Val = (Int32)Months[Month];
                D.MonthValue = Val;
                D.MonthString = (String)Month;
            }
            return D;
        }


        /// <summary>
        /// Gets the number of days in a Month of a particular year
        /// </summary>
        /// <param name="Year">Year</param>
        /// <param name="Month">Month</param>
        /// <param name="Abbreviation">True, If Month Value is String and abbreviated; False, otherwise</param>
        /// <returns>Number of days in the month of a particular year</returns>
        public static int GetDays(int Year, Object Month, Boolean Abbreviation = false)
        {
            Date D = GetMonth(Month, Abbreviation);
            int NumberOfDays = DateTime.DaysInMonth(Year, D.MonthValue);
            return NumberOfDays;
        }


        /// <summary>
        /// Gets all available timezones.
        /// </summary>
        /// <returns></returns>
        private static ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones()
        {
            ReadOnlyCollection<TimeZoneInfo> TZIs = TimeZoneInfo.GetSystemTimeZones();
            return TZIs;
        }


        /// <summary>
        /// Gets all available timezones
        /// </summary>
        /// <returns></returns>
        public static List<TimeZoneInfo> GetTimeZones()
        {
            List<TimeZoneInfo> TZI = new List<TimeZoneInfo>(GetSystemTimeZones());
            return TZI;
        }


        /// <summary>
        /// Converts DateAndTime value to a corresponding value based on the configured Timezone.
        /// </summary>
        /// <param name="Value">DateAndTime Value</param>
        /// <returns>DateAndTime value based on configured timezone</returns>
        public static DateTime GetDateTimeByTimeZone(DateTime Value)
        {
            DateTime UTC = Value.ToUniversalTime();
            DateTime UserDateTime = TimeZoneInfo.ConvertTimeFromUtc(UTC, TimeZoneInfo.FindSystemTimeZoneById(a.TimeZone));
            return UserDateTime;
        }


        /// <summary>
        /// Gets date and time based on a timezone
        /// </summary>
        /// <param name="date">Date</param>
        /// <param name="TimeZone">Timezone</param>
        public static DateTime GetDateTimeByTimeZone(DateTime date, String TimeZone)
        {
            TimeZoneInfo estTimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZone);

            var SpecifiedDateTime = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
            var ConvertedDateTime = TimeZoneInfo.ConvertTime(SpecifiedDateTime, estTimeZone);

            return Convert.ToDateTime(ConvertedDateTime);
        }


        /// <summary>
        /// Get Timezone description by specified timezone set in the App.Config or Web.Config
        /// </summary>
        /// <returns></returns>
        public static TimeZoneInfo GetTimeZoneDescription()
        {
            return TimeZoneInfo.FindSystemTimeZoneById(a.TimeZone);
        }


        /// <summary>
        /// Get Timezone description by specified timezone
        /// </summary>
        /// <param name="TimeZone"></param>
        /// <returns></returns>
        public static TimeZoneInfo GetTimeZoneDescription(String TimeZone)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
        }
    }


    /// <summary>
    /// Generate Random Number
    /// </summary>
    public class Number
    {

        // Define default min and max number lengths.
        private static int DEFAULT_MIN_NUMBER_LENGTH = 8;
        private static int DEFAULT_MAX_NUMBER_LENGTH = 10;

        // Define supported number characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private static string PASSWORD_CHARS_LCASE = "1234567890";
        private static string PASSWORD_CHARS_UCASE = "98765432";
        private static string PASSWORD_CHARS_NUMERIC = "23456789";
        private static string PASSWORD_CHARS_SPECIAL = "0987654321";

        /// <summary>
        /// Generates a random number.
        /// </summary>
        /// <returns>
        /// Randomly generated number.
        /// </returns>
        /// <remarks>
        /// The length of the generated number will be determined at
        /// random. It will be no shorter than the minimum default and
        /// no longer than maximum default.
        /// </remarks>
        public static string Generate()
        {
            return Generate(DEFAULT_MIN_NUMBER_LENGTH,
                         DEFAULT_MAX_NUMBER_LENGTH);
        }

        /// <summary>
        /// Generates a random number of the exact length.
        /// </summary>
        /// <param name="length">
        /// Exact number length.
        /// </param>
        /// <returns>
        /// Randomly generated number.
        /// </returns>
        public static string Generate(int length)
        {
            return Generate(length, length);
        }

        /// <summary>
        /// Generates a random number.
        /// </summary>
        /// <param name="minLength">
        /// Minimum number length.
        /// </param>
        /// <param name="maxLength">
        /// Maximum number length.
        /// </param>
        /// <returns>
        /// Randomly generated number.
        /// </returns>
        /// <remarks>
        /// The length of the generated number will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        public static string Generate(int minLength,
                                int maxLength)
        {
            // Make sure that input parameters are valid.
            if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                return null;

            // Create a local array containing supported number characters
            // grouped by types. You can remove character groups from this
            // array, but doing so will weaken the number strength.
            char[][] charGroups = new char[][]
             {
          PASSWORD_CHARS_LCASE.ToCharArray(),
          PASSWORD_CHARS_UCASE.ToCharArray(),
          PASSWORD_CHARS_NUMERIC.ToCharArray(),
          PASSWORD_CHARS_SPECIAL.ToCharArray()
             };

            // Use this array to track the number of unused characters in each
            // character group.
            int[] charsLeftInGroup = new int[charGroups.Length];

            // Initially, all characters in each group are not used.
            for (int i = 0; i < charsLeftInGroup.Length; i++)
                charsLeftInGroup[i] = charGroups[i].Length;

            // Use this array to track (iterate through) unused character groups.
            int[] leftGroupsOrder = new int[charGroups.Length];

            // Initially, all character groups are not used.
            for (int i = 0; i < leftGroupsOrder.Length; i++)
                leftGroupsOrder[i] = i;

            // Because we cannot use the default randomizer, which is based on the
            // current time (it will produce the same "random" number within a
            // second), we will use a random number generator to seed the
            // randomizer.

            // Use a 4-byte array to fill it with random bytes and convert it then
            // to an integer value.
            byte[] randomBytes = new byte[4];

            // Generate 4 random bytes.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            // Convert 4 bytes into a 32-bit integer value.
            int seed = (randomBytes[0] & 0x7f) << 24 |
                      randomBytes[1] << 16 |
                      randomBytes[2] << 8 |
                      randomBytes[3];

            // Now, this is real randomization.
            Random random = new Random(seed);

            // This array will hold number characters.
            char[] number = null;

            // Allocate appropriate memory for the number.
            if (minLength < maxLength)
                number = new char[random.Next(minLength, maxLength + 1)];
            else
                number = new char[minLength];

            // Index of the next character to be added to number.
            int nextCharIdx;

            // Index of the next character group to be processed.
            int nextGroupIdx;

            // Index which will be used to track not processed character groups.
            int nextLeftGroupsOrderIdx;

            // Index of the last non-processed character in a group.
            int lastCharIdx;

            // Index of the last non-processed group.
            int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            // Generate password characters one at a time.
            for (int i = 0; i < number.Length; i++)
            {
                // If only one character group remained unprocessed, process it;
                // otherwise, pick a random character group from the unprocessed
                // group list. To allow a special character to appear in the
                // first position, increment the second parameter of the Next
                // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                if (lastLeftGroupsOrderIdx == 0)
                    nextLeftGroupsOrderIdx = 0;
                else
                    nextLeftGroupsOrderIdx = random.Next(0,
                                                  lastLeftGroupsOrderIdx);

                // Get the actual index of the character group, from which we will
                // pick the next character.
                nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                // Get the index of the last unprocessed characters in this group.
                lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                // If only one unprocessed character is left, pick it; otherwise,
                // get a random character from the unused character list.
                if (lastCharIdx == 0)
                    nextCharIdx = 0;
                else
                    nextCharIdx = random.Next(0, lastCharIdx + 1);

                // Add this character to the number.
                number[i] = charGroups[nextGroupIdx][nextCharIdx];

                // If we processed the last character in this group, start over.
                if (lastCharIdx == 0)
                    charsLeftInGroup[nextGroupIdx] =
                                         charGroups[nextGroupIdx].Length;
                // There are more unprocessed characters left.
                else
                {
                    // Swap processed character with the last unprocessed character
                    // so that we don't pick it until we process all characters in
                    // this group.
                    if (lastCharIdx != nextCharIdx)
                    {
                        char temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] =
                                  charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }
                    // Decrement the number of unprocessed characters in
                    // this group.
                    charsLeftInGroup[nextGroupIdx]--;
                }

                // If we processed the last group, start all over.
                if (lastLeftGroupsOrderIdx == 0)
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                // There are more unprocessed groups left.
                else
                {
                    // Swap processed group with the last unprocessed group
                    // so that we don't pick it until we process all groups.
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                  leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }
                    // Decrement the number of unprocessed groups.
                    lastLeftGroupsOrderIdx--;
                }
            }

            // Convert password characters into a string and return the result.
            return new string(number);
        }

        /// <summary>
        /// Generates a new random number that ranges between a Lower Limit and an Upper Limit
        /// </summary>
        /// <param name="LowerLimit">Lower Limit</param>
        /// <param name="UpperLimit">Upper Limit</param>
        /// <returns></returns>
        public static String GenerateFromRange(int LowerLimit, int UpperLimit)
        {
            Random rnd = new Random();
            int Num = rnd.Next(1, UpperLimit + 1);
            return Num.ToString();
        }


        /// <summary>
        /// Generates a List of Random Numbers between a Lower Limit and an Upper Limit
        /// </summary>
        /// <param name="Min">Lower Limit</param>
        /// <param name="Max">Upper Limit</param>
        /// <param name="ListCount">Number of Items to return</param>
        /// <returns>List of Random Integers</returns>
        public static List<Int32> GenerateList(int Min, int Max, int ListCount)
        {
            Random rand = new Random();
            List<int> result = new List<int>();
            HashSet<int> check = new HashSet<int>();

            if (Max == 0)
            {
                return result;
            }

            if (Max < ListCount)
                ListCount = Max;

            for (Int32 i = 0; i < ListCount; i++)
            {
                int curValue = rand.Next(Min, Max);
                while (check.Contains(curValue))
                {
                    curValue = rand.Next(Min, Max);
                }
                result.Add(curValue);
                check.Add(curValue);
            }

            return result;

        }

    }

    /// <summary>
    /// Random Password Class
    /// </summary>
    public class Password
    {
        // Define default min and max password lengths.
        private static int DEFAULT_MIN_PASSWORD_LENGTH = 8;
        private static int DEFAULT_MAX_PASSWORD_LENGTH = 10;

        // Define supported password characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private static string PASSWORD_CHARS_LCASE = "abcdefgijkmnpqrstwxyz";
        private static string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
        private static string PASSWORD_CHARS_NUMERIC = "23456789";
        private static string PASSWORD_CHARS_SPECIAL = "AQ";//@#$%^&+=

        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <returns>
        /// Randomly generated password.
        /// </returns>
        /// <remarks>
        /// The length of the generated password will be determined at
        /// random. It will be no shorter than the minimum default and
        /// no longer than maximum default.
        /// </remarks>
        public static string Generate()
        {
            return Generate(DEFAULT_MIN_PASSWORD_LENGTH,
                         DEFAULT_MAX_PASSWORD_LENGTH);
        }

        /// <summary>
        /// Generates a random password of the exact length.
        /// </summary>
        /// <param name="length">
        /// Exact password length.
        /// </param>
        /// <returns>
        /// Randomly generated password.
        /// </returns>
        public static string Generate(int length)
        {
            return Generate(length, length);
        }

        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <param name="minLength">
        /// Minimum password length.
        /// </param>
        /// <param name="maxLength">
        /// Maximum password length.
        /// </param>
        /// <returns>
        /// Randomly generated password.
        /// </returns>
        /// <remarks>
        /// The length of the generated password will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        public static string Generate(int minLength,
                                int maxLength)
        {
            // Make sure that input parameters are valid.
            if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                return null;

            // Create a local array containing supported password characters
            // grouped by types. You can remove character groups from this
            // array, but doing so will weaken the password strength.
            char[][] charGroups = new char[][]
             {
          PASSWORD_CHARS_LCASE.ToCharArray(),
          PASSWORD_CHARS_UCASE.ToCharArray(),
          PASSWORD_CHARS_NUMERIC.ToCharArray(),
          PASSWORD_CHARS_SPECIAL.ToCharArray()
             };

            // Use this array to track the number of unused characters in each
            // character group.
            int[] charsLeftInGroup = new int[charGroups.Length];

            // Initially, all characters in each group are not used.
            for (int i = 0; i < charsLeftInGroup.Length; i++)
                charsLeftInGroup[i] = charGroups[i].Length;

            // Use this array to track (iterate through) unused character groups.
            int[] leftGroupsOrder = new int[charGroups.Length];

            // Initially, all character groups are not used.
            for (int i = 0; i < leftGroupsOrder.Length; i++)
                leftGroupsOrder[i] = i;

            // Because we cannot use the default randomizer, which is based on the
            // current time (it will produce the same "random" number within a
            // second), we will use a random number generator to seed the
            // randomizer.

            // Use a 4-byte array to fill it with random bytes and convert it then
            // to an integer value.
            byte[] randomBytes = new byte[4];

            // Generate 4 random bytes.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            // Convert 4 bytes into a 32-bit integer value.
            int seed = (randomBytes[0] & 0x7f) << 24 |
                      randomBytes[1] << 16 |
                      randomBytes[2] << 8 |
                      randomBytes[3];

            // Now, this is real randomization.
            Random random = new Random(seed);

            // This array will hold password characters.
            char[] password = null;

            // Allocate appropriate memory for the password.
            if (minLength < maxLength)
                password = new char[random.Next(minLength, maxLength + 1)];
            else
                password = new char[minLength];

            // Index of the next character to be added to password.
            int nextCharIdx;

            // Index of the next character group to be processed.
            int nextGroupIdx;

            // Index which will be used to track not processed character groups.
            int nextLeftGroupsOrderIdx;

            // Index of the last non-processed character in a group.
            int lastCharIdx;

            // Index of the last non-processed group.
            int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            // Generate password characters one at a time.
            for (int i = 0; i < password.Length; i++)
            {
                // If only one character group remained unprocessed, process it;
                // otherwise, pick a random character group from the unprocessed
                // group list. To allow a special character to appear in the
                // first position, increment the second parameter of the Next
                // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                if (lastLeftGroupsOrderIdx == 0)
                    nextLeftGroupsOrderIdx = 0;
                else
                    nextLeftGroupsOrderIdx = random.Next(0,
                                                  lastLeftGroupsOrderIdx);

                // Get the actual index of the character group, from which we will
                // pick the next character.
                nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                // Get the index of the last unprocessed characters in this group.
                lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                // If only one unprocessed character is left, pick it; otherwise,
                // get a random character from the unused character list.
                if (lastCharIdx == 0)
                    nextCharIdx = 0;
                else
                    nextCharIdx = random.Next(0, lastCharIdx + 1);

                // Add this character to the password.
                password[i] = charGroups[nextGroupIdx][nextCharIdx];

                // If we processed the last character in this group, start over.
                if (lastCharIdx == 0)
                    charsLeftInGroup[nextGroupIdx] =
                                         charGroups[nextGroupIdx].Length;
                // There are more unprocessed characters left.
                else
                {
                    // Swap processed character with the last unprocessed character
                    // so that we don't pick it until we process all characters in
                    // this group.
                    if (lastCharIdx != nextCharIdx)
                    {
                        char temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] =
                                  charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }
                    // Decrement the number of unprocessed characters in
                    // this group.
                    charsLeftInGroup[nextGroupIdx]--;
                }

                // If we processed the last group, start all over.
                if (lastLeftGroupsOrderIdx == 0)
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                // There are more unprocessed groups left.
                else
                {
                    // Swap processed group with the last unprocessed group
                    // so that we don't pick it until we process all groups.
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                  leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }
                    // Decrement the number of unprocessed groups.
                    lastLeftGroupsOrderIdx--;
                }
            }

            // Convert password characters into a string and return the result.
            return new string(password);
        }
    }

    public class Crypto
    {
        private static string key = "shasgaiuhkjww-0212kj560-0121880ytYTBBB==1234*&%$";
        private static bool useHashing = true;
        public static string Encrypt(string toEncrypt)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            //AppSettingsReader settingsReader = new AppSettingsReader();
            // Get the key from config file
            //string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string cipherString)
        {
            byte[] keyArray;
            //get the byte code of the string
            //Console.Write("Cypher String" + cipherString);
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);
            //System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            //Get your key from config file to open the lock!
            //string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider
                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }

    public class Alphanumeric
    {
        // Define default min and max password lengths.
        private static int DEFAULT_MIN_PASSWORD_LENGTH = 8;
        private static int DEFAULT_MAX_PASSWORD_LENGTH = 10;

        // Define supported password characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private static string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
        private static string PASSWORD_CHARS_NUMERIC = "23456789";

        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <returns>
        /// Randomly generated password.
        /// </returns>
        /// <remarks>
        /// The length of the generated password will be determined at
        /// random. It will be no shorter than the minimum default and
        /// no longer than maximum default.
        /// </remarks>
        public static string Generate()
        {
            return Generate(DEFAULT_MIN_PASSWORD_LENGTH,
                            DEFAULT_MAX_PASSWORD_LENGTH);
        }

        /// <summary>
        /// Generates a random password of the exact length.
        /// </summary>
        /// <param name="length">
        /// Exact password length.
        /// </param>
        /// <returns>
        /// Randomly generated password.
        /// </returns>
        public static string Generate(int length)
        {
            return Generate(length, length);
        }

        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <param name="minLength">
        /// Minimum password length.
        /// </param>
        /// <param name="maxLength">
        /// Maximum password length.
        /// </param>
        /// <returns>
        /// Randomly generated password.
        /// </returns>
        /// <remarks>
        /// The length of the generated password will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        public static string Generate(int minLength,
                                      int maxLength)
        {
            //return "123";
            // Make sure that input parameters are valid.
            if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                return null;

            // Create a local array containing supported password characters
            // grouped by types. You can remove character groups from this
            // array, but doing so will weaken the password strength.
            char[][] charGroups = new char[][]
        {
            PASSWORD_CHARS_UCASE.ToCharArray(),
            PASSWORD_CHARS_NUMERIC.ToCharArray(),        };

            // Use this array to track the number of unused characters in each
            // character group.
            int[] charsLeftInGroup = new int[charGroups.Length];

            // Initially, all characters in each group are not used.
            for (int i = 0; i < charsLeftInGroup.Length; i++)
                charsLeftInGroup[i] = charGroups[i].Length;

            // Use this array to track (iterate through) unused character groups.
            int[] leftGroupsOrder = new int[charGroups.Length];

            // Initially, all character groups are not used.
            for (int i = 0; i < leftGroupsOrder.Length; i++)
                leftGroupsOrder[i] = i;

            // Because we cannot use the default randomizer, which is based on the
            // current time (it will produce the same "random" number within a
            // second), we will use a random number generator to seed the
            // randomizer.

            // Use a 4-byte array to fill it with random bytes and convert it then
            // to an integer value.
            byte[] randomBytes = new byte[4];

            // Generate 4 random bytes.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            // Convert 4 bytes into a 32-bit integer value.
            int seed = (randomBytes[0] & 0x7f) << 24 |
                        randomBytes[1] << 16 |
                        randomBytes[2] << 8 |
                        randomBytes[3];

            // Now, this is real randomization.
            Random random = new Random(seed);

            // This array will hold password characters.
            char[] password = null;

            // Allocate appropriate memory for the password.
            if (minLength < maxLength)
                password = new char[random.Next(minLength, maxLength + 1)];
            else
                password = new char[minLength];

            // Index of the next character to be added to password.
            int nextCharIdx;

            // Index of the next character group to be processed.
            int nextGroupIdx;

            // Index which will be used to track not processed character groups.
            int nextLeftGroupsOrderIdx;

            // Index of the last non-processed character in a group.
            int lastCharIdx;

            // Index of the last non-processed group.
            int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            // Generate password characters one at a time.
            for (int i = 0; i < password.Length; i++)
            {
                // If only one character group remained unprocessed, process it;
                // otherwise, pick a random character group from the unprocessed
                // group list. To allow a special character to appear in the
                // first position, increment the second parameter of the Next
                // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                if (lastLeftGroupsOrderIdx == 0)
                    nextLeftGroupsOrderIdx = 0;
                else
                    nextLeftGroupsOrderIdx = random.Next(0,
                                                         lastLeftGroupsOrderIdx);

                // Get the actual index of the character group, from which we will
                // pick the next character.
                nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                // Get the index of the last unprocessed characters in this group.
                lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                // If only one unprocessed character is left, pick it; otherwise,
                // get a random character from the unused character list.
                if (lastCharIdx == 0)
                    nextCharIdx = 0;
                else
                    nextCharIdx = random.Next(0, lastCharIdx + 1);

                // Add this character to the password.
                password[i] = charGroups[nextGroupIdx][nextCharIdx];

                // If we processed the last character in this group, start over.
                if (lastCharIdx == 0)
                    charsLeftInGroup[nextGroupIdx] =
                                              charGroups[nextGroupIdx].Length;
                // There are more unprocessed characters left.
                else
                {
                    // Swap processed character with the last unprocessed character
                    // so that we don't pick it until we process all characters in
                    // this group.
                    if (lastCharIdx != nextCharIdx)
                    {
                        char temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] =
                                    charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }
                    // Decrement the number of unprocessed characters in
                    // this group.
                    charsLeftInGroup[nextGroupIdx]--;
                }

                // If we processed the last group, start all over.
                if (lastLeftGroupsOrderIdx == 0)
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                // There are more unprocessed groups left.
                else
                {
                    // Swap processed group with the last unprocessed group
                    // so that we don't pick it until we process all groups.
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                    leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }
                    // Decrement the number of unprocessed groups.
                    lastLeftGroupsOrderIdx--;
                }
            }

            // Convert password characters into a string and return the result.
            return new string(password);
        }
    }

    public class General
    {
        public static readonly ILog LOGGER = LogManager.GetLogger("FILE_LOGGER");
        public static string getStatus(string _status)
        {
            string _statusname = "";
            switch (_status)
            {
                case "I":
                    _statusname = "Pending with Approver";
                    break;
                case "A":
                    _statusname = "Approved";
                    break;
                case "P":
                    _statusname = "Processed";
                    break;
                case "C":
                    _statusname = "Closed";
                    break;
                case "R":
                    _statusname = "Returned";
                    break;
                default:
                    _statusname = "Draft";
                    break;
            }
            return _statusname;
        }
        public class Utf8StringWriter : System.IO.StringWriter
        {
            public override System.Text.Encoding Encoding
            {
                get
                {
                    return System.Text.Encoding.UTF8;
                }
            }
        }
        public static string GetPageURL()
        {
            String PageURL = HttpContext.Current.Request.ServerVariables["URL"];
            if (PageURL.StartsWith("/")) { PageURL = PageURL.Remove(0, 1); }
            if (!String.IsNullOrEmpty(Settings.Site.SitePath))
            {
                PageURL = PageURL.ToLower().Replace(Settings.Site.SitePath, "");
            }
            return PageURL;
        }
        public static string generateXML(Object obj)
        {
            String ret = "";
            //try
            //{
            XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
            XmlSerializerNamespaces xns = new XmlSerializerNamespaces();
            xns.Add(string.Empty, string.Empty);
            StringWriter sw = new Utf8StringWriter();
            xmlSerializer.Serialize(sw, obj, xns);
            ret = sw.ToString();
            //}
            //catch (System.Exception) { }
            return ret;
        }
       
        /// <summary>
        /// Logs all Exceptions
        /// </summary>
        /// <param name="ex">Exception</param>
        public static void LogExceptions(Object User, Exception ex)
        {
            //Log.LogEvent(new Log(User, ex.InnerException == null ? ex.Message : ex.Message + "======" + ex.InnerException.StackTrace, EventType.Error));
            LOGGER.Error(User, ex);
        }

        private static void MakeRequest(HttpWebRequest request, Action<string> successAction, Action<Exception> errorAction)
        {
            try
            {
                using (var webResponse = (HttpWebResponse)request.GetResponse())
                {
                    using (var reader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        var objText = reader.ReadToEnd();
                        successAction(objText);
                    }
                }
            }
            catch (Exception ex)
            {
                errorAction(ex);
            }
        }
        public static T Deserialize<T>(string responseBody)
        {
            T toReturns = default(T);
            try
            {
                toReturns = JsonConvert.DeserializeObject<T>(responseBody);
            }
            catch (Exception ex)
            {
                LogExceptions("Deserialize", ex);
            }
            return toReturns;
        }
        public static string Serialize(Object obj)
        {
            String toReturns = "";
            try
            {
                toReturns = JsonConvert.SerializeObject(obj);
            }
            catch (Exception ex)
            {
                LogExceptions("Serialize", ex);
            }
            return toReturns;
        }
        public static byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        public static System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            using (MemoryStream ms = new MemoryStream(byteArrayIn))
            {
                System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
                return returnImage;
            }
        }

        public static string MaskString(string str, char mask, int iLeft, int iRight)
        {
            string ret = str;
            try
            {
                if (str.Length > iLeft)
                {
                    char[] chrs = str.ToArray();
                    int iEnd = chrs.Length > iLeft + iRight ? chrs.Length - iRight : chrs.Length;
                    for (int i = iLeft; i < iEnd; i++)
                    {
                        chrs[i] = mask;
                    }
                    ret = String.Join("", chrs);
                }
            }
            catch { }
            return ret;
        }

        public static string GetXString(Object value)
        {
            try
            {
                return value.ToString().Trim();
            }
            catch (System.Exception)
            {
                return string.Empty;
            }
        }

        public static Decimal GetXCurrency(Object value)
        {
            decimal ret = 0;
            try
            {
                if (value != null)
                {
                    string newValue = RemoveSpecialCharactersAndAlphabets(value.ToString());
                    Decimal.TryParse(newValue, out ret);
                }
            }
            catch (System.Exception)
            {
                //return 0;
            }
            return ret;
        }
        public static string RemoveSpecialCharactersAndAlphabets(String sText)
        {
            String ret = sText.ToUpper().Replace(" ", "");
            string SpecialCharacters = @"#@$%^*()-_=+,!~'[]{}|\/;:&""§£ABCDEFGHIJKLMNOPQRSTUVWXYZ ";
            Char[] chr = SpecialCharacters.ToCharArray();
            foreach (char c in chr)
            {
                ret = ret.Replace(c.ToString(), "");
            }
            return ret;
        }
    }
    public class TextToDataSet
    {
        public static DataSet Convert(FileInfo File, string TableName, string delimiter, bool HeaderRow)
        {
            DataSet set = new DataSet();
            int num = 1;
            StreamReader reader = File.OpenText();
            for (int i = 1; i < num; i++)
            {
                reader.ReadLine();
            }
            string[] values = reader.ReadLine().Split(delimiter.ToCharArray());
            set.Tables.Add(TableName);
            if (HeaderRow)
            {
                foreach (string str in values)
                {
                    bool flag = false;
                    string str2 = "";
                    int num3 = 0;
                    while (!flag)
                    {
                        string name = (str + str2).Replace("#", "").Replace("'", "").Replace("&", "");
                        if (!set.Tables[TableName].Columns.Contains(name))
                        {
                            set.Tables[TableName].Columns.Add(name);
                            flag = true;
                        }
                        else
                        {
                            num3++;
                            str2 = "_" + num3.ToString();
                        }
                    }
                }
            }
            else
            {
                for (int j = 0; j < values.Length; j++)
                {
                    string columnName = "Column_" + ((j + 1)).ToString();
                    set.Tables[TableName].Columns.Add(columnName);
                }
                set.Tables[TableName].Rows.Add(values);
            }
            foreach (string str6 in reader.ReadToEnd().Split("\r\n".ToCharArray()))
            {
                if (str6.Length > 0)
                {
                    string[] strArray3 = str6.Split(delimiter.ToCharArray());
                    set.Tables[TableName].Rows.Add(strArray3);
                }
            }
            reader.Close();
            reader.Dispose();
            return set;
        }

        public static DataSet Convert(string File, string TableName, string delimiter, int StartAtLine)
        {
            DataSet set = new DataSet();
            StreamReader reader = new StreamReader(File);
            for (int i = 1; i < StartAtLine; i++)
            {
                reader.ReadLine();
            }
            string[] strArray = reader.ReadLine().Split(delimiter.ToCharArray());
            set.Tables.Add(TableName);
            foreach (string str in strArray)
            {
                bool flag = false;
                string str2 = "";
                int num2 = 0;
                while (!flag)
                {
                    string name = (str + str2).Replace("#", "").Replace("'", "").Replace("&", "");
                    if (!set.Tables[TableName].Columns.Contains(name))
                    {
                        set.Tables[TableName].Columns.Add(name);
                        flag = true;
                    }
                    else
                    {
                        num2++;
                        str2 = "_" + num2.ToString();
                    }
                }
            }
            foreach (string str5 in reader.ReadToEnd().Split("\r\n".ToCharArray()))
            {
                if (str5.Length > 0)
                {
                    string[] values = str5.Split(delimiter.ToCharArray());
                    set.Tables[TableName].Rows.Add(values);
                }
            }
            reader.Close();
            reader.Dispose();
            return set;
        }

        public static DataTable Convert(String csvText, string TableName, string delimiter, bool HeaderRow, bool quotedString = false)
        {
            DataTable table = new DataTable(TableName);
            if (!String.IsNullOrEmpty(csvText))
            {
                String[] contentArray = csvText.Split("\r\n".ToCharArray());
                int num = 0;
                string[] values = contentArray[num].Split(delimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (HeaderRow)
                {
                    foreach (string str in values)
                    {
                        bool flag = false;
                        string str2 = "";
                        int num3 = 0;
                        while (!flag)
                        {
                            string name = (str + str2).Replace("#", "").Replace("'", "").Replace("&", "");
                            if (!table.Columns.Contains(name))
                            {
                                table.Columns.Add(name);
                                flag = true;
                            }
                            else
                            {
                                num3++;
                                str2 = "_" + num3.ToString();
                            }
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < values.Length; j++)
                    {
                        string columnName = "Column_" + ((j + 1)).ToString();
                        table.Columns.Add(columnName);
                    }
                    if (quotedString)
                        removeQuote(values);
                    table.Rows.Add(values);
                }

                num++;

                for (int i = num; i < contentArray.Length; i++)
                {
                    string sValues = contentArray[i];
                    if (sValues.Length > 0)
                    {
                        values = sValues.Split(delimiter.ToCharArray());
                        if (quotedString)
                            removeQuote(values);
                        table.Rows.Add(values);
                    }
                }
            }
            return table;
        }

        private static void removeQuote(String[] obj)
        {
            for (int i = 0; i < obj.Length; i++)
            {
                obj[i] = obj[i].Replace("\"", "").Trim();
            }
        }
    }

    public delegate void FooterEvent(GridViewRow row);

    /// <summary>
    /// A class to allow you to add summaries and groups to a GridView, easily!
    /// </summary>
    public class GridViewHelper
    {

        #region Fields

        private GridView mGrid;
        private GridViewSummaryList mGeneralSummaries;
        private GridViewGroupList mGroups;
        private bool useFooter;
        private SortDirection groupSortDir;

        #endregion

        public GridViewGroupList Groups
        {
            get { return mGroups; }
        }

        public GridViewSummaryList GeneralSummaries
        {
            get { return mGeneralSummaries; }
        }


        #region Messages

        private const string USE_ADEQUATE_METHOD_TO_REGISTER_THE_SUMMARY = "Use adequate method to register a summary with custom operation.";
        private const string GROUP_NOT_FOUND = "Group {0} not found. Please register the group before the summary.";
        private const string INVALID_SUMMARY = "Invalid summary.";
        private const string SUPPRESS_GROUP_ALREADY_DEFINED = "A suppress group is already defined. You can't define suppress AND summary groups simultaneously";
        private const string ONE_GROUP_ALREADY_REGISTERED = "At least a group is already defined. A suppress group can't coexist with other groups";

        #endregion


        #region Events

        /// <summary>
        /// Event triggered when a new group starts
        /// </summary>
        public event GroupEvent GroupStart;

        /// <summary>
        /// Event triggered when a group ends
        /// </summary>
        public event GroupEvent GroupEnd;

        /// <summary>
        /// Event triggered after a row for group header be inserted
        /// </summary>
        public event GroupEvent GroupHeader;

        /// <summary>
        /// Event triggered after a row for group summary be inserted
        /// </summary>
        public event GroupEvent GroupSummary;

        /// <summary>
        /// Event triggered after the general summaries be generated
        /// </summary>
        public event FooterEvent GeneralSummary;

        /// <summary>
        /// Event triggered when the footer is databound
        /// </summary>
        public event FooterEvent FooterDataBound;

        #endregion

        #region Constructors

        public GridViewHelper(GridView grd) : this(grd, false, SortDirection.Ascending) { }

        public GridViewHelper(GridView grd, bool useFooterForGeneralSummaries) : this(grd, useFooterForGeneralSummaries, SortDirection.Ascending) { }

        public GridViewHelper(GridView grd, bool useFooterForGeneralSummaries, SortDirection groupSortDirection)
        {
            this.mGrid = grd;
            this.useFooter = useFooterForGeneralSummaries;
            this.groupSortDir = groupSortDirection;
            this.mGeneralSummaries = new GridViewSummaryList();
            this.mGroups = new GridViewGroupList();
            this.mGrid.RowDataBound += new GridViewRowEventHandler(RowDataBoundHandler);
        }

        #endregion


        #region RegisterSummary overloads

        public GridViewSummary RegisterSummary(string column, SummaryOperation operation)
        {
            return this.RegisterSummary(column, String.Empty, operation);
        }

        public GridViewSummary RegisterSummary(string column, string formatString, SummaryOperation operation)
        {
            if (operation == SummaryOperation.Custom)
            {
                throw new Exception(USE_ADEQUATE_METHOD_TO_REGISTER_THE_SUMMARY);
            }

            // TO DO: Perform column validation...
            GridViewSummary s = new GridViewSummary(column, formatString, operation, null);
            mGeneralSummaries.Add(s);

            // if general summaries are displayed in the footer, it must be set to visible
            if (useFooter) mGrid.ShowFooter = true;

            return s;
        }

        public GridViewSummary RegisterSummary(string column, SummaryOperation operation, string groupName)
        {
            return this.RegisterSummary(column, String.Empty, operation, groupName);
        }

        public GridViewSummary RegisterSummary(string column, string formatString, SummaryOperation operation, string groupName)
        {
            if (operation == SummaryOperation.Custom)
            {
                throw new Exception(USE_ADEQUATE_METHOD_TO_REGISTER_THE_SUMMARY);
            }

            GridViewGroup group = mGroups[groupName];
            if (group == null)
            {
                throw new Exception(String.Format(GROUP_NOT_FOUND, groupName));
            }

            // TO DO: Perform column validation...
            GridViewSummary s = new GridViewSummary(column, formatString, operation, group);
            group.AddSummary(s);

            return s;
        }

        public GridViewSummary RegisterSummary(string column, CustomSummaryOperation operation, SummaryResultMethod getResult)
        {
            return RegisterSummary(column, String.Empty, operation, getResult);
        }

        public GridViewSummary RegisterSummary(string column, string formatString, CustomSummaryOperation operation, SummaryResultMethod getResult)
        {
            // TO DO: Perform column validation...
            GridViewSummary s = new GridViewSummary(column, formatString, operation, getResult, null);
            mGeneralSummaries.Add(s);

            // if general summaries are displayed in the footer, it must be set to visible
            if (useFooter) mGrid.ShowFooter = true;

            return s;
        }

        public GridViewSummary RegisterSummary(string column, CustomSummaryOperation operation, SummaryResultMethod getResult, string groupName)
        {
            return RegisterSummary(column, String.Empty, operation, getResult, groupName);
        }

        public GridViewSummary RegisterSummary(string column, string formatString, CustomSummaryOperation operation, SummaryResultMethod getResult, string groupName)
        {
            GridViewGroup group = mGroups[groupName];
            if (group == null)
            {
                throw new Exception(String.Format(GROUP_NOT_FOUND, groupName));
            }

            // TO DO: Perform column validation...
            GridViewSummary s = new GridViewSummary(column, formatString, operation, getResult, group);
            group.AddSummary(s);

            return s;
        }

        public GridViewSummary RegisterSummary(GridViewSummary s)
        {
            if (!s.Validate())
            {
                throw new Exception(INVALID_SUMMARY);
            }

            if (s.Group == null)
            {
                // if general summaries are displayed in the footer, it must be set to visible
                if (useFooter) mGrid.ShowFooter = true;

                mGeneralSummaries.Add(s);
            }
            else if (!s.Group.ContainsSummary(s))
            {
                s.Group.AddSummary(s);
            }

            return s;
        }

        #endregion


        #region RegisterGroup overloads

        public GridViewGroup RegisterGroup(string column, bool auto, bool hideGroupColumns)
        {
            string[] cols = new string[1] { column };
            return RegisterGroup(cols, auto, hideGroupColumns);
        }

        public GridViewGroup RegisterGroup(string[] columns, bool auto, bool hideGroupColumns)
        {
            if (HasSuppressGroup())
            {
                throw new Exception(SUPPRESS_GROUP_ALREADY_DEFINED);
            }

            // TO DO: Perform column validation...
            GridViewGroup g = new GridViewGroup(columns, auto, hideGroupColumns);
            mGroups.Add(g);

            if (hideGroupColumns)
            {
                for (int i = 0; i < mGrid.Columns.Count; i++)
                {
                    for (int j = 0; j < columns.Length; j++)
                    {
                        if (GetDataFieldName(mGrid.Columns[i]).ToLower() == columns[j].ToLower())
                        {
                            mGrid.Columns[i].Visible = false;
                        }
                    }
                }
            }

            return g;
        }

        #endregion


        #region SetSuppressGroup overloads

        public GridViewGroup SetSuppressGroup(string column)
        {
            string[] cols = new string[1] { column };
            return SetSuppressGroup(cols);
        }

        public GridViewGroup SetSuppressGroup(string[] columns)
        {
            if (mGroups.Count > 0)
            {
                throw new Exception(ONE_GROUP_ALREADY_REGISTERED);
            }

            // TO DO: Perform column validation...
            GridViewGroup g = new GridViewGroup(columns, true, false, false, false);
            mGroups.Add(g);

            // Disable paging because pager works in datarows that
            // will be suppressed
            mGrid.AllowPaging = false;

            return g;
        }

        #endregion


        #region Private Helper functions

        private string GetSequentialGroupColumns()
        {
            string ret = String.Empty;

            foreach (GridViewGroup g in mGroups)
            {
                ret += g.Name.Replace('+', ',') + ",";
            }
            return ret.Substring(0, ret.Length - 1);
        }

        /// <summary>
        /// Compares the actual group values with the values of the current dataitem
        /// </summary>
        /// <param name="g"></param>
        /// <param name="dataitem"></param>
        /// <returns></returns>
        private bool EvaluateEquals(GridViewGroup g, object dataitem)
        {
            // The values wasn't initialized
            if (g.ActualValues == null) return false;

            for (int i = 0; i < g.Columns.Length; i++)
            {
                if (g.ActualValues[i] == null && DataBinder.Eval(dataitem, g.Columns[i]) != null) return false;
                if (g.ActualValues[i] != null && DataBinder.Eval(dataitem, g.Columns[i]) == null) return false;
                if (!g.ActualValues[i].Equals(DataBinder.Eval(dataitem, g.Columns[i]))) return false;
            }

            return true;
        }

        private bool HasSuppressGroup()
        {
            foreach (GridViewGroup g in mGroups)
            {
                if (g.IsSuppressGroup) return true;
            }
            return false;
        }

        private bool HasAutoSummary(List<GridViewSummary> list)
        {
            foreach (GridViewSummary s in list)
            {
                if (s.Automatic) return true;
            }
            return false;
        }

        private object[] GetGroupRowValues(GridViewGroup g, object dataitem)
        {
            object[] values = new object[g.Columns.Length];

            for (int i = 0; i < g.Columns.Length; i++)
            {
                values[i] = DataBinder.Eval(dataitem, g.Columns[i]);
            }

            return values;
        }

        /// <summary>
        /// Inserts a grid row. Only cells required for the summary results
        /// will be created (except if GenerateAllCellsOnSummaryRow is true).
        /// The group will be checked for columns with summary
        /// </summary>
        /// <param name="beforeRow"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        private GridViewRow InsertGridRow(GridViewRow beforeRow, GridViewGroup g)
        {
            int colspan;
            TableCell cell;
            TableCell[] tcArray;
            int visibleColumns = this.GetVisibleColumnCount();

            Table tbl = (Table)mGrid.Controls[0];
            int newRowIndex = tbl.Rows.GetRowIndex(beforeRow);
            GridViewRow newRow = new GridViewRow(newRowIndex, newRowIndex, DataControlRowType.DataRow, DataControlRowState.Normal);

            if (g != null && (g.IsSuppressGroup || g.GenerateAllCellsOnSummaryRow))
            {
                // Create all the table cells
                tcArray = new TableCell[visibleColumns];
                for (int i = 0; i < visibleColumns; i++)
                {
                    cell = new TableCell();
                    cell.ApplyStyle(mGrid.Columns[GetRealIndexFromVisibleColumnIndex(i)].ItemStyle);
                    cell.Text = "&nbsp;";
                    tcArray[i] = cell;
                }
            }
            else
            {
                // Create only the required table cells
                colspan = 0;
                List<TableCell> tcc = new List<TableCell>();
                for (int i = 0; i < mGrid.Columns.Count; i++)
                {
                    if (ColumnHasSummary(i, g))
                    {
                        if (colspan > 0)
                        {
                            cell = new TableCell();
                            cell.Text = "&nbsp;";
                            cell.ColumnSpan = colspan;
                            tcc.Add(cell);
                            colspan = 0;
                        }

                        // insert table cell and copy the style
                        cell = new TableCell();
                        cell.ApplyStyle(mGrid.Columns[i].ItemStyle);
                        tcc.Add(cell);
                    }
                    else if (mGrid.Columns[i].Visible)
                    {
                        // A visible column that will have no cell because has
                        // no summary. So we increase the colspan...
                        colspan++;
                    }
                }

                if (colspan > 0)
                {
                    cell = new TableCell();
                    cell.Text = "&nbsp;";
                    cell.ColumnSpan = colspan;
                    tcc.Add(cell);
                    colspan = 0;
                }

                tcArray = new TableCell[tcc.Count];
                tcc.CopyTo(tcArray);
            }

            newRow.Cells.AddRange(tcArray);
            tbl.Controls.AddAt(newRowIndex, newRow);

            return newRow;
        }

        #endregion


        #region Core

        private void RowDataBoundHandler(object sender, GridViewRowEventArgs e)
        {
            foreach (GridViewGroup g in mGroups)
            {
                // The last group values are caught here
                if (e.Row.RowType == DataControlRowType.Footer)
                {
                    g.CalculateSummaries();
                    GenerateGroupSummary(g, e.Row);
                    if (GroupEnd != null)
                    {
                        GroupEnd(g.Name, g.ActualValues, e.Row);
                    }
                }
                else if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ProcessGroup(g, e);
                    if (g.IsSuppressGroup)
                    {
                        e.Row.Visible = false;
                    }
                }
                else if (e.Row.RowType == DataControlRowType.Pager)
                {
                    // Workaround to strange behavior (ColumnSpan not rendered)
                    // when AllowPaging=true
                    // Found at: http://aspadvice.com/blogs/joteke/archive/2006/02/11/15130.aspx
                    TableCell originalCell = e.Row.Cells[0];
                    TableCell newCell = new TableCell();
                    newCell.Visible = false;
                    e.Row.Cells.AddAt(0, newCell);
                    originalCell.ColumnSpan = this.GetVisibleColumnCount();
                }
            }

            // This will deal only with general summaries
            foreach (GridViewSummary s in mGeneralSummaries)
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    // Essentially this isn't required, but it prevents wrong calc
                    // in case of RowDataBound event be called twice (for each row)
                    s.Reset();
                }
                else if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    s.AddValue(DataBinder.Eval(e.Row.DataItem, s.Column));
                }
                else if (e.Row.RowType == DataControlRowType.Footer)
                {
                    s.Calculate();
                }
            }

            if (e.Row.RowType == DataControlRowType.Footer)
            {
                // Automatic generation of summary
                GenerateGeneralSummaries(e);

                // Triggers event footerdatabound
                if (FooterDataBound != null)
                {
                    FooterDataBound(e.Row);
                }
            }
        }

        private void ProcessGroup(GridViewGroup g, GridViewRowEventArgs e)
        {
            string groupHeaderText = String.Empty;

            // Check if it's still in the same group values
            if (!EvaluateEquals(g, e.Row.DataItem))
            {
                // Check if a group ends or if it is the first group values starting...
                if (g.ActualValues != null)
                {
                    g.CalculateSummaries();
                    GenerateGroupSummary(g, e.Row);

                    // Triggers event GroupEnd
                    if (GroupEnd != null)
                    {
                        GroupEnd(g.Name, g.ActualValues, e.Row);
                    }
                }

                // Another group values starts now
                g.Reset();
                g.SetActualValues(GetGroupRowValues(g, e.Row.DataItem));

                // If group is automatic inserts a group header
                if (g.Automatic)
                {
                    for (int v = 0; v < g.ActualValues.Length; v++)
                    {
                        if (g.ActualValues[v] == null) continue;
                        groupHeaderText += g.ActualValues[v].ToString();
                        if (g.ActualValues.Length - v > 1)
                        {
                            groupHeaderText += " - ";
                        }
                    }

                    GridViewRow newRow = InsertGridRow(e.Row);
                    newRow.Cells[0].Text = groupHeaderText;

                    // Triggers event GroupHeader
                    if (GroupHeader != null)
                    {
                        GroupHeader(g.Name, g.ActualValues, newRow);
                    }
                }

                // Triggers event GroupStart
                if (GroupStart != null)
                {
                    GroupStart(g.Name, g.ActualValues, e.Row);
                }
            }

            g.AddValueToSummaries(e.Row.DataItem);
        }

        private string GetFormatedString(string preferredFormat, string secondFormat, object value)
        {
            String format = preferredFormat;
            if (format.Length == 0)
            {
                format = secondFormat;
            }

            if (format.Length > 0)
                return String.Format(format, value);
            else
                return value.ToString();
        }

        private void GenerateGroupSummary(GridViewGroup g, GridViewRow row)
        {
            int colIndex;
            object colValue;

            if (!HasAutoSummary(g.Summaries) && !HasSuppressGroup()) return;

            // Inserts a new row 
            GridViewRow newRow = InsertGridRow(row, g);

            foreach (GridViewSummary s in g.Summaries)
            {
                if (s.Automatic)
                {
                    colIndex = GetVisibleColumnIndex(s.Column);
                    colIndex = ResolveCellIndex(newRow, colIndex);
                    newRow.Cells[colIndex].Text = this.GetFormatedString(s.FormatString, this.GetColumnFormat(GetColumnIndex(s.Column)), s.Value);
                }
            }

            // If it is a suppress group must set the grouped values in the cells
            // of the inserted row
            if (g.IsSuppressGroup)
            {
                for (int i = 0; i < g.Columns.Length; i++)
                {
                    colValue = g.ActualValues[i];
                    if (colValue != null)
                    {
                        colIndex = GetVisibleColumnIndex(g.Columns[i]);
                        colIndex = ResolveCellIndex(newRow, colIndex);
                        newRow.Cells[colIndex].Text = colValue.ToString();
                    }
                }
            }

            // Triggers event GroupSummary
            if (GroupSummary != null)
            {
                GroupSummary(g.Name, g.ActualValues, newRow);
            }

        }

        /// <summary>
        /// Generates the general summaries in the grid. 
        /// </summary>
        /// <param name="e">GridViewRowEventArgs</param>
        private void GenerateGeneralSummaries(GridViewRowEventArgs e)
        {
            int colIndex;
            GridViewRow row;

            if (!HasAutoSummary(this.mGeneralSummaries))
            {
                // Triggers event GeneralSummary
                if (GeneralSummary != null)
                {
                    GeneralSummary(e.Row);
                }

                return;
            }

            if (useFooter)
                row = e.Row;
            else
                row = InsertGridRow(e.Row, null);

            foreach (GridViewSummary s in mGeneralSummaries)
            {
                if (!s.Automatic) continue;

                if (useFooter)
                    colIndex = GetColumnIndex(s.Column);
                else
                    colIndex = GetVisibleColumnIndex(s.Column);

                colIndex = ResolveCellIndex(row, colIndex);
                row.Cells[colIndex].Text = this.GetFormatedString(s.FormatString, this.GetColumnFormat(GetColumnIndex(s.Column)), s.Value);
            }

            // Triggers event GeneralSummary
            if (GeneralSummary != null)
            {
                GeneralSummary(row);
            }

        }

        /// <summary>
        /// Identifies the equivalent index on a row that contains cells with colspan
        /// </summary>
        /// <param name="row"></param>
        /// <param name="colIndex"></param>
        /// <returns></returns>
        private int ResolveCellIndex(GridViewRow row, int colIndex)
        {
            int colspansum = 0;
            int realIndex;

            for (int i = 0; i < row.Cells.Count; i++)
            {
                realIndex = i + colspansum;
                if (realIndex == colIndex) return i;

                if (row.Cells[i].ColumnSpan > 1)
                {
                    colspansum = colspansum + row.Cells[i].ColumnSpan - 1;
                }
            }

            return -1;
        }

        private bool ColumnHasSummary(int colindex, GridViewGroup g)
        {
            List<GridViewSummary> list;
            string column = this.GetDataFieldName(mGrid.Columns[colindex]);

            if (g == null)
                list = this.mGeneralSummaries;
            else
                list = g.Summaries;

            foreach (GridViewSummary s in list)
            {
                if (column.ToLower() == s.Column.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        private bool ColumnHasSummary(string column, GridViewGroup g)
        {
            List<GridViewSummary> list;

            if (g == null)
                list = this.mGeneralSummaries;
            else
                list = g.Summaries;

            foreach (GridViewSummary s in list)
            {
                if (column.ToLower() == s.Column.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        #endregion


        #region Public Helper functions

        public int GetRealIndexFromVisibleColumnIndex(int visibleIndex)
        {
            int visibles = 0;
            for (int i = 0; i < mGrid.Columns.Count; i++)
            {
                if (mGrid.Columns[i].Visible)
                {
                    if (visibleIndex == visibles) return i;
                    visibles++;
                }
            }

            // Not found....
            return -1;
        }

        public int GetVisibleColumnIndex(string columnName)
        {
            int visibles = 0;
            for (int i = 0; i < mGrid.Columns.Count; i++)
            {
                if (GetDataFieldName(mGrid.Columns[i]).ToLower() == columnName.ToLower())
                {
                    return visibles;
                }

                if (mGrid.Columns[i].Visible) visibles++;
            }

            // Not found....
            return -1;
        }

        public int GetColumnIndex(string columnName)
        {
            for (int i = 0; i < mGrid.Columns.Count; i++)
            {
                if (GetDataFieldName(mGrid.Columns[i]).ToLower() == columnName.ToLower())
                {
                    return i;
                }
            }

            // Not found....
            return -1;
        }

        public string GetDataFieldName(DataControlField field)
        {
            // TO DO: Enable search in HyperLinkField, ButtonField...

            if (field is BoundField)
            {
                return (field as BoundField).DataField;
            }
            else
            {
                // It hopes that SortExpression is set (and it's equal to column name)
                return field.SortExpression;
            }
        }

        public string GetColumnFormat(int colIndex)
        {
            // TO DO: Enable search in HyperLinkField, ButtonField...

            if (mGrid.Columns[colIndex] is BoundField)
            {
                return (mGrid.Columns[colIndex] as BoundField).DataFormatString;
            }
            else
            {
                return String.Empty;
            }
        }

        public int GetVisibleColumnCount()
        {
            int ret = 0;

            for (int i = 0; i < mGrid.Columns.Count; i++)
            {
                if (mGrid.Columns[i].Visible) ret++;
            }

            return ret;
        }

        /// <summary>
        /// This method must be called to hide columns that doesn't 
        /// have any summary operation when we are using a suppress group
        /// </summary>
        public void HideColumnsWithoutGroupSummary()
        {
            string colname;
            bool colChecked;

            foreach (DataControlField dcf in mGrid.Columns)
            {
                colChecked = false;
                colname = GetDataFieldName(dcf).ToLower();

                foreach (GridViewGroup g in mGroups)
                {
                    // Check if it's part of the group columns
                    for (int j = 0; j < g.Columns.Length; j++)
                    {
                        if (colname == g.Columns[j].ToLower())
                        {
                            colChecked = true;
                            break;
                        }
                    }

                    if (colChecked) break;

                    // Check if it's part of a group summary
                    colChecked = ColumnHasSummary(colname, g);

                    if (colChecked) break;
                }

                if (colChecked) continue;

                dcf.Visible = false;

            }
        }


        /// <summary>
        /// Legacy name...
        /// </summary>
        public void SetInvisibleColumnsWithoutGroupSummary()
        {
            this.HideColumnsWithoutGroupSummary();
        }

        /// <summary>
        ///  Inserts a grid row with one cell only
        /// </summary>
        /// <param name="beforeRow"></param>
        /// <returns></returns>
        public GridViewRow InsertGridRow(GridViewRow beforeRow)
        {
            int visibleColumns = this.GetVisibleColumnCount();

            Table tbl = (Table)mGrid.Controls[0];
            int newRowIndex = tbl.Rows.GetRowIndex(beforeRow);
            GridViewRow newRow = new GridViewRow(newRowIndex, newRowIndex, DataControlRowType.DataRow, DataControlRowState.Normal);

            newRow.Cells.Add(new TableCell());
            if (visibleColumns > 1)
            {
                newRow.Cells[0].ColumnSpan = visibleColumns;
            }

            tbl.Controls.AddAt(newRowIndex, newRow);

            return newRow;
        }

        public void ApplyGroupSort()
        {
            mGrid.Sort(this.GetSequentialGroupColumns(), groupSortDir);
        }

        #endregion
    }

    public delegate void GroupEvent(string groupName, object[] values, GridViewRow row);

    /// <summary>
    /// A class that represents a group consisting of a set of columns
    /// </summary>
    public class GridViewGroup
    {
        #region Fields

        private string[] _columns;
        private object[] _actualValues;
        private int _quantity;
        private bool _automatic;
        private bool _hideGroupColumns;
        private bool _isSuppressGroup;
        private bool _generateAllCellsOnSummaryRow;
        private GridViewSummaryList mSummaries;

        #endregion

        #region Properties

        public string[] Columns
        {
            get { return _columns; }
        }

        public object[] ActualValues
        {
            get { return _actualValues; }
        }

        public int Quantity
        {
            get { return _quantity; }
        }

        public bool Automatic
        {
            get { return _automatic; }
            set { _automatic = value; }
        }

        public bool HideGroupColumns
        {
            get { return _hideGroupColumns; }
            set { _hideGroupColumns = value; }
        }

        public bool IsSuppressGroup
        {
            get { return _isSuppressGroup; }
        }

        public bool GenerateAllCellsOnSummaryRow
        {
            get { return _generateAllCellsOnSummaryRow; }
            set { _generateAllCellsOnSummaryRow = value; }
        }

        public string Name
        {
            get { return String.Join("+", this._columns); }
        }

        public GridViewSummaryList Summaries
        {
            get { return mSummaries; }
        }

        #endregion

        #region Constructors

        public GridViewGroup(string[] cols, bool isSuppressGroup, bool auto, bool hideGroupColumns, bool generateAllCellsOnSummaryRow)
        {
            this.mSummaries = new GridViewSummaryList();
            this._actualValues = null;
            this._quantity = 0;
            this._columns = cols;
            this._isSuppressGroup = isSuppressGroup;
            this._automatic = auto;
            this._hideGroupColumns = hideGroupColumns;
            this._generateAllCellsOnSummaryRow = generateAllCellsOnSummaryRow;
        }

        public GridViewGroup(string[] cols, bool auto, bool hideGroupColumns, bool generateAllCellsOnSummaryRow)
            : this(cols, false, auto, hideGroupColumns, generateAllCellsOnSummaryRow)
        {
        }

        public GridViewGroup(string[] cols, bool auto, bool hideGroupColumns)
            : this(cols, auto, hideGroupColumns, false)
        {
        }

        #endregion

        internal void SetActualValues(object[] values)
        {
            this._actualValues = values;
        }

        public bool ContainsSummary(GridViewSummary s)
        {
            return mSummaries.Contains(s);
        }

        public void AddSummary(GridViewSummary s)
        {
            if (this.ContainsSummary(s))
            {
                throw new Exception("Summary already exists in this group.");
            }

            if (!s.Validate())
            {
                throw new Exception("Invalid summary.");
            }

            ///s._group = this;
            s.SetGroup(this);
            this.mSummaries.Add(s);
        }

        public void Reset()
        {
            this._quantity = 0;

            foreach (GridViewSummary s in mSummaries)
            {
                s.Reset();
            }
        }

        public void AddValueToSummaries(object dataitem)
        {
            this._quantity++;

            foreach (GridViewSummary s in mSummaries)
            {
                s.AddValue(DataBinder.Eval(dataitem, s.Column));
            }
        }

        public void CalculateSummaries()
        {
            foreach (GridViewSummary s in mSummaries)
            {
                s.Calculate();
            }
        }
    }

    public class GridViewGroupList : List<GridViewGroup>
    {
        public GridViewGroup this[string name]
        {
            get { return this.FindGroupByName(name); }
        }

        public GridViewGroup FindGroupByName(string name)
        {
            foreach (GridViewGroup g in this)
            {
                if (g.Name.ToLower() == name.ToLower()) return g;
            }

            return null;
        }
    }
    public enum SummaryOperation { Sum, Avg, Count, Custom }
    public delegate void CustomSummaryOperation(string column, string groupName, object value);
    public delegate object SummaryResultMethod(string column, string groupName);

    /// <summary>
    /// A class that represents a summary operation defined to a column
    /// </summary>
    public class GridViewSummary
    {
        #region Fields

        private string _column;
        private SummaryOperation _operation;
        private CustomSummaryOperation _customOperation;
        private SummaryResultMethod _getSummaryMethod;
        private GridViewGroup _group;
        private object _value;
        private string _formatString;
        private int _quantity;
        private bool _automatic;
        private bool _treatNullAsZero;

        #endregion

        #region Properties

        public string Column
        {
            get { return _column; }
        }

        public SummaryOperation Operation
        {
            get { return _operation; }
        }

        public CustomSummaryOperation CustomOperation
        {
            get { return _customOperation; }
        }

        public SummaryResultMethod GetSummaryMethod
        {
            get { return _getSummaryMethod; }
        }

        public GridViewGroup Group
        {
            get { return _group; }
        }

        public object Value
        {
            get { return _value; }
        }

        public string FormatString
        {
            get { return _formatString; }
            set { _formatString = value; }
        }

        public int Quantity
        {
            get { return _quantity; }
        }

        public bool Automatic
        {
            get { return _automatic; }
            set { _automatic = value; }
        }

        public bool TreatNullAsZero
        {
            get { return _treatNullAsZero; }
            set { _treatNullAsZero = value; }
        }

        #endregion

        #region Constructors

        private GridViewSummary(string col, GridViewGroup grp)
        {
            this._column = col;
            this._group = grp;
            this._value = null;
            this._quantity = 0;
            this._automatic = true;
            this._treatNullAsZero = false;
        }

        public GridViewSummary(string col, string formatString, SummaryOperation op, GridViewGroup grp)
            : this(col, grp)
        {
            this._formatString = formatString;
            this._operation = op;
            this._customOperation = null;
            this._getSummaryMethod = null;
        }

        public GridViewSummary(string col, SummaryOperation op, GridViewGroup grp)
            : this(col, String.Empty, op, grp)
        {
        }

        public GridViewSummary(string col, string formatString, CustomSummaryOperation op, SummaryResultMethod getResult, GridViewGroup grp)
            : this(col, grp)
        {
            this._formatString = formatString;
            this._operation = SummaryOperation.Custom;
            this._customOperation = op;
            this._getSummaryMethod = getResult;
        }

        public GridViewSummary(string col, CustomSummaryOperation op, SummaryResultMethod getResult, GridViewGroup grp)
            : this(col, String.Empty, op, getResult, grp)
        {
        }

        #endregion

        internal void SetGroup(GridViewGroup g)
        {
            this._group = g;
        }

        public bool Validate()
        {
            if (this._operation == SummaryOperation.Custom)
            {
                return (this._customOperation != null && this._getSummaryMethod != null);
            }
            else
            {
                return (this._customOperation == null && this._getSummaryMethod == null);
            }
        }

        public void Reset()
        {
            this._quantity = 0;
            this._value = null;
        }

        public void AddValue(object newValue)
        {
            // Increment to (later) calc the Avg or for other calcs
            this._quantity++;

            // Built-in operations
            if (this._operation == SummaryOperation.Sum || this._operation == SummaryOperation.Avg)
            {
                if (this._value == null)
                    this._value = newValue;
                else
                    this._value = PerformSum(this._value, newValue);
            }
            else
            {
                // Custom operation
                if (this._customOperation != null)
                {
                    // Call the custom operation
                    if (this._group != null)
                        this._customOperation(this._column, this._group.Name, newValue);
                    else
                        this._customOperation(this._column, null, newValue);
                }
            }
        }

        public void Calculate()
        {
            if (this._operation == SummaryOperation.Avg)
            {
                this._value = PerformDiv(this._value, this._quantity);
            }
            if (this._operation == SummaryOperation.Count)
            {
                this._value = this._quantity;
            }
            else if (this._operation == SummaryOperation.Custom)
            {
                if (this._getSummaryMethod != null)
                {
                    this._value = this._getSummaryMethod(this._column, null);
                }
            }
            // if this.Operation == SummaryOperation.Avg
            // this.Value already contains the correct value
        }

        #region Built-in Summary Operations

        private object PerformSum(object a, object b)
        {
            object zero = 0;

            if (a == null)
            {
                if (_treatNullAsZero)
                    a = 0;
                else
                    return null;
            }

            if (b == null)
            {
                if (_treatNullAsZero)
                    b = 0;
                else
                    return null;
            }

            // Convert to proper type before add
            switch (a.GetType().FullName)
            {
                case "System.Int16": return Convert.ToInt16(a) + Convert.ToInt16(b);
                case "System.Int32": return Convert.ToInt32(a) + Convert.ToInt32(b);
                case "System.Int64": return Convert.ToInt64(a) + Convert.ToInt64(b);
                case "System.UInt16": return Convert.ToUInt16(a) + Convert.ToUInt16(b);
                case "System.UInt32": return Convert.ToUInt32(a) + Convert.ToUInt32(b);
                case "System.UInt64": return Convert.ToUInt64(a) + Convert.ToUInt64(b);
                case "System.Single": return Convert.ToSingle(a) + Convert.ToSingle(b);
                case "System.Double": return Convert.ToDouble(a) + Convert.ToDouble(b);
                case "System.Decimal": return Convert.ToDecimal(a) + Convert.ToDecimal(b);
                case "System.Byte": return Convert.ToByte(a) + Convert.ToByte(b);
                case "System.String": return a.ToString() + b.ToString();
            }

            return null;
        }

        private object PerformDiv(object a, int b)
        {
            object zero = 0;

            if (a == null)
            {
                return (_treatNullAsZero ? zero : null);
            }

            // Don't raise an exception, just return null
            if (b == 0)
            {
                return null;
            }

            // Convert to proper type before div
            switch (a.GetType().FullName)
            {
                case "System.Int16": return Convert.ToInt16(a) / b;
                case "System.Int32": return Convert.ToInt32(a) / b;
                case "System.Int64": return Convert.ToInt64(a) / b;
                case "System.UInt16": return Convert.ToUInt16(a) / b;
                case "System.UInt32": return Convert.ToUInt32(a) / b;
                case "System.Single": return Convert.ToSingle(a) / b;
                case "System.Double": return Convert.ToDouble(a) / b;
                case "System.Decimal": return Convert.ToDecimal(a) / b;
                case "System.Byte": return Convert.ToByte(a) / b;
                    // Operator '/' cannot be applied to operands of type 'ulong' and 'int'
                    //case "System.UInt64": return Convert.ToUInt64(a) / b;
            }

            return null;
        }

        #endregion

    }
    public class GridViewSummaryList : List<GridViewSummary>
    {
        public GridViewSummary this[string name]
        {
            get { return this.FindSummaryByColumn(name); }
        }

        public GridViewSummary FindSummaryByColumn(string columnName)
        {
            foreach (GridViewSummary s in this)
            {
                if (s.Column.ToLower() == columnName.ToLower()) return s;
            }

            return null;
        }
    }
    public class Messaging
    {

        #region EMAIL SMTP & MAIL MESSAGE CLASSES

        /// <summary>
        /// SMTP Client
        /// </summary>
        public class SMTPClient : System.Net.Mail.SmtpClient
        {
            /// <summary>
            /// Initialize
            /// </summary>
            public SMTPClient()
            {
                Host = ConfigurationManager.AppSettings["SMTPHost"];
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["SMTPPassword"]))
                {
                    //UseDefaultCredentials = true;
                    Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SMTPAddress"],
                        ConfigurationManager.AppSettings["SMTPPassword"].ToString().ToString());
                }
                //{
                //    //UseDefaultCredentials = false;
                //    Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SMTPAddress"], 
                //        Utilities.Crypto.Decrypt(ConfigurationManager.AppSettings["SMTPPassword"].ToString()).ToString());
                //}
                DeliveryFormat = System.Net.Mail.SmtpDeliveryFormat.International;
                DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SMTPSecure"]);
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPTimeout"]);
            }
        }


        /// <summary>
        /// Mail Message Class
        /// </summary>
        public class _MailMessage : System.Net.Mail.MailMessage
        {
            /// <summary>
            /// Initialize
            /// </summary>
            public _MailMessage()
            {
                From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["SMTPAddress"], ConfigurationManager.AppSettings["SMTPName"]);
                IsBodyHtml = true;
                Priority = System.Net.Mail.MailPriority.High;
            }
        }

        #endregion


        #region BULKSMSNIGERIA.NET SMS CLASS

        /// <summary>
        /// Bulk SMS Class
        /// </summary>
        public class BulkSMSNigeria : Settings.SMS
        {

            /// <summary>
            /// Recipient
            /// </summary>
            public String Recipient{get;set;}
            /// <summary>
            /// Message
            /// </summary>
            public String Message{ get; set;}

            //BulkSMSNigeria(Settings.SMS SMS) : base(SMS)
            //{
            //}

            /// <summary>
            /// Initialize
            /// </summary>
            public BulkSMSNigeria()
            {
                GatewayBaseURL = new Settings.SMS(true).GatewayBaseURL;
                GateWayPassword = new Settings.SMS(true).GateWayPassword;
                GateWaySender = new Settings.SMS(true).GateWaySender;
                GateWayUserID = new Settings.SMS(true).GateWayUserID;
            }

            /// <summary>
            /// SMS Respnse Class
            /// </summary>
            public class MessageResponse
            {

                /// <summary>
                /// Response Status
                /// </summary>
                public String Status{ get;set;}

                /// <summary>
                /// Total Units Used
                /// </summary>
                public Double UnitsUsed{ get;set;}

                /// <summary>
                /// List Of Numbers that failed
                /// </summary>
                public List<String> FailedNumbers{get; set; }


                /// <summary>
                /// Initialize, with Message Response
                /// </summary>
                /// <param name="Response">Message Response</param>
                public MessageResponse(String Response)
                {
                    String[] Res = Response.Split(new char[] { ' ' });
                    if (Res.Length >= 1)
                        Status = Res[0];

                    if (Res.Length >= 2)
                        UnitsUsed = Convert.ToDouble(Res[1]);

                    if (Res.Length == 3)
                    {
                        String[] Failed = Res[2].Split(new char[] { ',' });
                        List<String> FNums = new List<String>();
                        for (int i = 0; i < Failed.Length; i++)
                        {
                            if (!String.IsNullOrWhiteSpace(Failed[i]))
                                FNums.Add(Failed[i]);
                        }
                        FailedNumbers = FNums;
                    }
                }

            }

        }

        #endregion




        /// <summary>
        /// Send SMS Message
        /// </summary>
        /// <param name="B">Message Details</param>
        /// <returns>Message Response</returns>
        public static BulkSMSNigeria.MessageResponse SendSMS(BulkSMSNigeria B)
        {
            try
            {
                WebClient client = new WebClient();
                String baseurl = B.GatewayBaseURL;
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;");
                client.QueryString.Add("username", HttpUtility.UrlEncode(B.GateWayUserID));
                client.QueryString.Add("password", HttpUtility.UrlEncode(B.GateWayPassword));
                client.QueryString.Add("recipient", HttpUtility.UrlEncode(B.Recipient));
                client.QueryString.Add("message", HttpUtility.UrlEncode(B.Message));
                client.QueryString.Add("sender", HttpUtility.UrlEncode(B.GateWaySender));

                Stream data = client.OpenRead(baseurl);
                StreamReader reader = new StreamReader(data);

                BulkSMSNigeria.MessageResponse res = new BulkSMSNigeria.MessageResponse(reader.ReadToEnd());

                data.Close();
                reader.Close();

                return res;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// Log SMS Message
        /// </summary>
        /// <param name="M">Message Response</param>
        /// <param name="B">BulkSMS Details</param>
        public static void LogSMSMessage(BulkSMSNigeria.MessageResponse M, BulkSMSNigeria B)
        {
            String Q = "INSERT INTO SMS (Recipient, Message, Status, UnitsUsed) VALUES (@Rec, @Msg, @Stat, @Units)";
            SqlCommand cmd = new SqlCommand(Q, Settings.Database.Connection);

            cmd.Parameters.AddWithValue("@Rec", B.Recipient);
            cmd.Parameters.AddWithValue("@Msg", B.Message);
            cmd.Parameters.AddWithValue("@Stat", M.Status);
            cmd.Parameters.AddWithValue("@Units", M.UnitsUsed);

            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }


        /// <summary>
        /// Get SMS Account Balance
        /// </summary>
        /// <returns></returns>
        public static Double GetSMSBalance()
        {
            try
            {
                WebClient client = new WebClient();
                BulkSMSNigeria B = new BulkSMSNigeria();
                String baseurl = B.GatewayBaseURL;
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;");
                client.QueryString.Add("balance", HttpUtility.UrlEncode("true"));

                Stream data = client.OpenRead(baseurl);
                StreamReader reader = new StreamReader(data);

                String res = reader.ReadToEnd();

                data.Close();
                reader.Close();

                return Convert.ToDouble(res);
            }
            catch
            {
                return -1;
            }
        }


        /// <summary>
        /// Send Emails
        /// </summary>
        /// <param name="M">Mail Message Details</param>
        public static bool SendMail(_MailMessage M)
        {
            SMTPClient S = new SMTPClient();
            bool queued = false;
            try
            {
                S.Send(M);
                queued = true;
            }
            catch (Exception ex)
            {
                General.LogExceptions("", ex);
            }
            return queued;
        }
        public static void LogMail(_MailMessage M, string _module)
        {
            try
            {
                using (MyDbContext _db = new MyDbContext())
                {
                    if (M.To.Count > 0)
                    {
                        EmailLog _log = _db.EmailLogs.Create();
                        _log.To = String.Join(";", M.To.Select(a => a.Address).ToArray());
                        //if (M.Attachments != null && M.Attachments.Count > 0)
                        //    _log.Attachments = String.Join(";", M.Attachments.Select(a => a.Name).ToArray());
                        if (M.CC != null && M.CC.Count > 0)
                            _log.Cc = String.Join(";", M.CC.Select(a => a.Address).ToArray());
                        if (M.Bcc != null && M.Bcc.Count > 0)
                            _log.Bcc = String.Join(";", M.Bcc.Select(a => a.Address).ToArray());
                        _log.Body = M.Body;
                        _log.CreatedDate = Utilities.Date.GetDateTimeByTimeZone(DateTime.Now);
                        _log.Subject = M.Subject;
                        _log.Module = _module;
                        _db.EmailLogs.Add(_log);
                        _log.Sent = SendMail(M);
                        _db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                General.LogExceptions("", ex);
            }
        }
        public static void LogMail(EmailItem M, string _module = "")
        {
            try
            {
                using (MyDbContext _db = new MyDbContext())
                {
                    if (M.To.Count > 0)
                    {
                        EmailLog _log = _db.EmailLogs.Create();
                        _log.To = String.Join(";", M.To.Select(s => s).ToArray());
                        if (M.Attachments != null && M.Attachments.Count > 0)
                            _log.Attachments = String.Join(";", M.Attachments.Select(a => a).ToArray());
                        if (M.Cc != null && M.Cc.Count > 0)
                            _log.Cc = String.Join(";", M.Cc.Select(a => a).ToArray());
                        if (M.Bcc != null && M.Bcc.Count > 0)
                            _log.Bcc = String.Join(";", M.Bcc.Select(a => a).ToArray());
                        _log.Body = M.Body;
                        _log.CreatedDate = Utilities.Date.GetDateTimeByTimeZone(DateTime.Now);
                        _log.Subject = M.Title;
                        _log.Module = _module;
                        _db.EmailLogs.Add(_log);
                        _log.Sent = false;
                        _db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                General.LogExceptions("", ex);
            }
        }

        public static void SendPendingMails()
        {
            try
            {
                using (MyDbContext _db = new MyDbContext())
                {
                    var pendings = _db.EmailLogs.Where(a => !a.Sent);
                    foreach (var p in pendings)
                    {
                        _MailMessage M = new _MailMessage();
                        M.Subject = p.Subject;
                        M.Body = p.Body;
                        if (!string.IsNullOrEmpty(p.To))
                        {
                            string[] arr = p.To.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string s in arr)
                                M.To.Add(s);
                        }
                        if (!string.IsNullOrEmpty(p.Cc))
                        {
                            string[] arr = p.Cc.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string s in arr)
                                M.CC.Add(s);
                        }
                        if (!string.IsNullOrEmpty(p.Bcc))
                        {
                            string[] arr = p.Bcc.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string s in arr)
                                M.Bcc.Add(s);
                        }
                        if (!string.IsNullOrEmpty(p.Attachments))
                        {
                            string[] arr = p.Attachments.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string s in arr)
                                M.Attachments.Add(new System.Net.Mail.Attachment(s));
                        }
                        p.Sent = SendMail(M);
                        if (p.Sent)
                            p.SentDate = Utilities.Date.GetDateTimeByTimeZone(DateTime.Now);
                    }
                    _db.SaveChanges();
                }
        }
            catch (Exception ex)
            {
                General.LogExceptions("", ex);
            }
}
    }
    public class IPDFHelper
    {
        //protected Document pdfDoc;
        //protected PdfWriter writer;
        private MemoryStream _PDFStream = new MemoryStream();
        public MemoryStream PDFStream
        {
            get { return _PDFStream; }
            set
            {
                if (_PDFStream == value)
                    return;
                _PDFStream = value;

            }
        }

        public virtual byte[] generatePDF(String _reportPath)
        {
            //StringWriter sw = new StringWriter();
            //HtmlTextWriter hw = new HtmlTextWriter(sw);
            //control.RenderControl(hw);
            //string htmlContent = sw.ToString();
            //StringReader sr = new StringReader(htmlContent);
            //pdfDoc = new Document(PageSize.A4, 25, 25, 75, 25);
            //HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            //writer = PdfWriter.GetInstance(pdfDoc, _PDFStream);
            //pdfDoc.Open();
            //writer.PageEvent = this;
            //iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
            //pdfDoc.Close();
            //AddWaterMark();
            return PDFStream.GetBuffer();
        }

        //public bool CreatePDFFile(Control control, string filename)
        //{
        //    bool ret = false;
        //    StringWriter sw = new StringWriter();
        //    HtmlTextWriter hw = new HtmlTextWriter(sw);
        //    control.RenderControl(hw);
        //    string htmlContent = sw.ToString();
        //    StringReader sr = new StringReader(htmlContent);
        //    pdfDoc = new Document(PageSize.A4, 25, 25, 75, 25);
        //    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
        //    writer = PdfWriter.GetInstance(pdfDoc, _PDFStream);
        //    pdfDoc.Open();
        //    writer.PageEvent = this;
        //    iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
        //    pdfDoc.Close();

        //    AddWaterMark();
        //    return ret;
        //}

        public virtual void AddFooter()
        {
            //BaseColor grey = new BaseColor(128, 128, 128);
            //Font font = FontFactory.GetFont("Arial", 9, Font.NORMAL, grey);
            ////tbl footer
            //PdfPTable footerTbl = new PdfPTable(1);
            //footerTbl.TotalWidth = pdfDoc.PageSize.Width;
            //footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;

            ////page number
            //Chunk myFooter = new Chunk("Page " + (writer.CurrentPageNumber), FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8, grey));
            //PdfPCell footer = new PdfPCell(new Phrase(myFooter));
            //footer.Border = Rectangle.NO_BORDER;
            //footer.HorizontalAlignment = Element.ALIGN_CENTER;
            //footerTbl.AddCell(footer);

            //footerTbl.WriteSelectedRows(0, -1, 0, (pdfDoc.BottomMargin + 10), writer.DirectContent);
        }

        public virtual void AddHeader()
        {

        }

        public virtual void AddWaterMark()
        {

        }
    }

}
using System;
using System.Collections.Generic;
using System.Text;
using jiratimesheetsconsole.Model;
using jiratimesheetsconsole.Service;
using System.Linq;
using System.Globalization;

namespace jiratimesheetsconsole
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            const string BASEURL = "https://avid-ondemand.atlassian.net";
            string USERNAME = "denys.holovin@avid.com";
            string PASSWORD = "*******!";

            DateTime start = DateTime.ParseExact("2017/06/26", "yyyy/MM/dd", CultureInfo.InvariantCulture);
            DateTime end = DateTime.ParseExact("2017/07/01", "yyyy/MM/dd", CultureInfo.InvariantCulture);

            //Console.WriteLine("Enter your Jira username:");
            //string USERNAME = Console.ReadLine();

            //Console.WriteLine("Enter your Jira password:");
            //string PASSWORD = string.Empty;
            //ConsoleKeyInfo key;
            //do
            //{
            //    key = Console.ReadKey(true);
            //    if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            //    {
            //        PASSWORD += key.KeyChar;
            //        Console.Write("*");
            //    }
            //    else
            //    {
            //        if (key.Key == ConsoleKey.Backspace && PASSWORD.Length > 0)
            //        {
            //            PASSWORD = PASSWORD.Substring(0, (PASSWORD.Length - 1));
            //            Console.Write("\b \b");
            //        }
            //    }
            //}
            //while (key.Key != ConsoleKey.Enter);

            //Console.WriteLine();



            JiraTimecards myJiraIssuesWithWorkLogged = null;

            try
            {
                JiraTimesheetManager timesheetManager = new JiraTimesheetManager(BASEURL, USERNAME, PASSWORD);
                myJiraIssuesWithWorkLogged = timesheetManager.GetTimecards(start, end).GetAwaiter().GetResult();
            }
            catch (System.AggregateException exAggregate)
            {
                Console.WriteLine("Error: " + exAggregate.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }


            //Present output
            if (myJiraIssuesWithWorkLogged != null)
            {
                Console.WriteLine("My Jira Issues With Work Logged. Total Issues: {0}; Total Logged: {1}",
                    myJiraIssuesWithWorkLogged.Total,
                    ConvertSecondsToHumanReadableFormat(myJiraIssuesWithWorkLogged.Issues.SelectMany(many => many.Fields.Worklog.WorklogEntries).ToList(), start, end));


                Console.WriteLine("--------------------------------------");
                foreach (var item in myJiraIssuesWithWorkLogged.Issues)
                {
                    bool hasParent = false;
                    string tab = "";

                    //Display Parent
                    if (item.Fields.Parent != null)
                    {
                        hasParent = true;
                        Console.WriteLine("PARENT: {0} - [{1}] {2}",
                                item.Fields.Parent.Fields.Type.TypeName,
                                item.Fields.Parent.Key,
                                item.Fields.Parent.Fields.Summary);
                    }

                    if (hasParent)
                        tab = "\t";

                    //Displat Children
                    Console.WriteLine("{4}{0} - [{1}] {2}: {3}",
                        item.Fields.Type.TypeName,
                        item.Key,
                        item.Fields.Summary,
                        ConvertSecondsToHumanReadableFormat(item.Fields.Worklog.WorklogEntries, start, end),
                        tab);
                }

            }

            Console.ReadKey();
        }

        //TODO: redesign to wrap with Interface and make method testable - implement data driven tests
        private static string ConvertSecondsToHumanReadableFormat(List<WorklogEntry> worklogEntries, DateTime start, DateTime end)
        {
            double seconds = worklogEntries.Where(w => w.Started >= start && w.Started <= end).Sum(s => s.TimeSpentSeconds);

            TimeSpan totalTime = TimeSpan.FromSeconds(seconds);

            StringBuilder sb = new StringBuilder();
            if (totalTime.Days > 0)
                sb.Append(totalTime.Days + "d ");
            if (totalTime.Hours > 0)
                sb.Append(totalTime.Hours + "h ");
            if (totalTime.Minutes > 0)
                sb.Append(totalTime.Minutes + "m ");
            if (totalTime.Seconds > 0)
                sb.Append(totalTime.Seconds + "s ");

            return sb.ToString();

        }
    }
}

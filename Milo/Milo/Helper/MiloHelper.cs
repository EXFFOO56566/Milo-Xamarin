using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Milo.Model;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Milo.Helper
{
    public class MiloHelper
    {
        public static async Task Share(Meeting meeting,string meeting_link)
        {
            try
            {
                ShareTextRequest request = new ShareTextRequest
                    {
                        Title = $"{meeting.meeting_details.title}",
                        Subject = $"Start Time:{meeting.meeting_details.meeting_start_date} {meeting.meeting_details.starting_time} {System.Environment.NewLine}End Time:{meeting.meeting_details.meeting_end_date} {meeting.meeting_details.ending_time}",
                        Text = $"{meeting.meeting_details.title}{System.Environment.NewLine}Start Time:{meeting.meeting_details.meeting_start_date} {meeting.meeting_details.starting_time} {System.Environment.NewLine}End Time:{meeting.meeting_details.meeting_end_date} {meeting.meeting_details.ending_time} {System.Environment.NewLine}Meeting Agenda:{meeting.meeting_details.agenda} {System.Environment.NewLine}Meeting ID:{meeting.meeting_details.meeting_id} {System.Environment.NewLine}Join Link:{meeting_link}"
                    };
                    await Xamarin.Essentials.Share.RequestAsync(request);
            }
            catch (Exception) { }
        }
        public static string GetDurationInMins(string sstartDate, string sendDate, string sstartTime, string sendTime)
        {
            DateTime startDate = System.Convert.ToDateTime(sstartDate);
            DateTime endDateDate = System.Convert.ToDateTime(sendDate);

            TimeSpan startTime = TimeSpan.Parse(sstartTime);
            TimeSpan endTime = TimeSpan.Parse(sendTime);

            var startDateTime = MergeTime(startDate, startTime);
            var endDateTime = MergeTime(endDateDate, endTime);
            TimeSpan ts = endDateTime - startDateTime;
            return ts.TotalMinutes +"Mins.";
        }

        public static string FormatDateTime(string sdate, string stime)
        {
            DateTime date = System.Convert.ToDateTime(sdate);

            TimeSpan time = TimeSpan.Parse(stime);

            var dateTime = MergeTime(date, time);
            return dateTime.ToString("dd MMM yyy at HH:mm");
        }

        public static DateTime MergeTime(DateTime date, TimeSpan time)
        {
            return new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);
        }

        public static async Task<Plugin.Permissions.Abstractions.PermissionStatus> CheckPermissions(Permission permission)
        {
            var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);
            bool request = false;
            if (permissionStatus == Plugin.Permissions.Abstractions.PermissionStatus.Denied)
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    var title = $"{permission} Permission";
                    var question = $"To use this plugin the {permission} permission is required. Please go into Settings and turn on {permission} for the app.";
                    var positive = "Settings";
                    var negative = "Maybe Later";
                    var task = Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);
                    if (task == null)
                        return permissionStatus;

                    var result = await task;
                    if (result)
                    {
                        CrossPermissions.Current.OpenAppSettings();
                    }

                    return permissionStatus;
                }

                request = true;
            }

            if (request || permissionStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                var newStatus = await CrossPermissions.Current.RequestPermissionsAsync(permission);

                if (!newStatus.ContainsKey(permission))
                {
                    return permissionStatus;
                }

                permissionStatus = newStatus[permission];

                if (newStatus[permission] != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    permissionStatus = newStatus[permission];
                    var title = $"{permission} Permission";
                    var question = $"To use the plugin the {permission} permission is required.";
                    var positive = "Settings";
                    var negative = "Maybe Later";
                    var task = Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);
                    if (task == null)
                        return permissionStatus;

                    var result = await task;
                    if (result)
                    {
                        CrossPermissions.Current.OpenAppSettings();
                    }
                    return permissionStatus;
                }
            }

            return permissionStatus;
        }
        public static bool IsValidEmail(string email)
        {
            // Normalize the domain
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                RegexOptions.None, TimeSpan.FromMilliseconds(200));
            // Examines the domain part of the email and normalizes it.
            string DomainMapper(Match match)
            {
                try
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();
                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);
                    return match.Groups[1].Value + domainName;
                }
                catch
                {
                    return match.Value;
                }
            }
            return Regex.IsMatch(email,
                 @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z_])@))" +
                 @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                 RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
    }
}

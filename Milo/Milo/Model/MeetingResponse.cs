using System;
using System.Collections.Generic;
using Milo.Helper;

namespace Milo.Model
{
    public class MeetingDetails
    {
        public string title { get; set; }
        public string created_by { get; set; }
        public string meeting_status { get; set; }
        public string takeaways { get; set; }
        public string agenda { get; set; }
        public string created_by_name { get; set; }
        public string id { get; set; }
        public string password { get; set; }
        public string meeting_id { get; set; }
        public string meeting_start_date { get; set; }
        public string meeting_end_date { get; set; }
        public string starting_time { get; set; }
        public string ending_time { get; set; }
        public string DurationInMins
        {
            get
            {
                return MiloHelper.GetDurationInMins(meeting_start_date, meeting_end_date, starting_time, ending_time);
            }
        }
    }

    public class PerticipentName
    {
        public string email_id { get; set; }
        public string name { get; set; }
        public string status { get; set; }
    }

    public class MyStatus
    {
        public string status { get; set; }
    }

    public class Meeting
    {
        public MeetingDetails meeting_details { get; set; }
        public List<PerticipentName> perticipent_name { get; set; }
        public MyStatus my_status { get; set; }
        public string takeway { get; set; }

        public string CommaSeparatedParticipants
        {
            get
            {
                List<string> names = new List<string>();
                foreach (var participant in perticipent_name)
                {
                    names.Add(participant.name);
                }
                return String.Join(", ", names);
            }
        }
    }

    public class MeetingResponse
    {
        public int status { get; set; }
        public string msg { get; set; }
        public List<Meeting> data { get; set; }
    }
}

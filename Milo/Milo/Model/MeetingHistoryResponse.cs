using System;
using System.Collections.Generic;
using Milo.Helper;

namespace Milo.Model
{

    public class MeetingHistory
    {
        public string name { get; set; }
        public string image { get; set; }
        public string meeting_title { get; set; }
        public string meeting_id { get; set; }
        public string starting_date { get; set; }
        public string end_date { get; set; }
        
        public string strating_time { get; set; }
        public string agenda { get; set; }
        public string takeaways { get; set; }
        public string ending_time { get; set; }
        public string title { get; set; }

        public string ImageLink
        {
            get
            {
                return !string.IsNullOrEmpty(image)?AppSettings.BaseUrl+ "/uploads/"+image: "https://fakeimg.pl/70x70/ff0000%2C128/000%2C255/";
            }
        }

        public string DurationInMins
        {
            get
            {
                return MiloHelper.GetDurationInMins(starting_date, end_date, strating_time, ending_time);
            }
        }

        public string FormattedStartDateTime
        {
            get
            {
                return MiloHelper.FormatDateTime(starting_date, strating_time);
            }
        }
        public string FormattedEndDateTime
        {
            get
            {
                return MiloHelper.FormatDateTime(end_date, ending_time);
            }
        }

    }

    public class MeetingHistoryResponse
    {
        public int status { get; set; }
        public string msg { get; set; }
        public List<MeetingHistory> data { get; set; }
    }
}

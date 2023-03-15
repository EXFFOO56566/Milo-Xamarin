using System;
using System.Collections.Generic;

namespace Milo.Model
{
    public class Timezone
    {
        public string TimezoneName { get; set; }
        public string TimezoneValue { get; set; }

        public override string ToString()
        {
            return TimezoneValue;
        }
    }

    public class TimezoneResponse
    {
        public int status { get; set; }
        public string msg { get; set; }
        public List<Timezone> data { get; set; }
    }
}

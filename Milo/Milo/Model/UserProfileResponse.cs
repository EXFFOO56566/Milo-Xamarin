using System;
using Milo.Helper;

namespace Milo.Model
{
    public class UserProfile
{
        public string id { get; set; }
        public string is_registered { get; set; }
        public string role_id { get; set; }
        public string name { get; set; }
        public string user_id { get; set; }
        public string mobile_no { get; set; }
        public string email_id { get; set; }
        public string address { get; set; }
        public string pincode { get; set; }
        public string image { get; set; }
        public string password { get; set; }
        public string token_code { get; set; }
        public string registration_type { get; set; }
        public string status { get; set; }
        public string verified_email { get; set; }
        public string register_by { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string time_zone { get; set; }
        public string purchase_id { get; set; }
        public string platform_id { get; set; }
        public string platform_name { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }

        public string ImageLink
        {
            get
            {
                return !string.IsNullOrEmpty(image) ? AppSettings.BaseUrl + "/uploads/" + image : "https://fakeimg.pl/70x70/ff0000%2C128/000%2C255/";
            }
        }

    }

    public class UserProfileResponse
{
        public int status { get; set; }
        public string msg { get; set; }
        public UserProfile data { get; set; }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Milo.Helper;
using Milo.Model;

[assembly: Xamarin.Forms.Dependency(typeof(Milo.Services.MiloService))]
namespace Milo.Services
{
    public class MiloService : IMiloService
    {
        private string TimezonePath = "api/get-country";
        private string RegisterPath = "api/register";
        private string LoginPath = "api/login";
        private string ResetPasswordPath = "api/reset-password";
        private string SendVerifyLinkPath = "api/send-verification-link";
        private string MeetingsLink = "api/get-meeting";
        private string CreateMeetingLink = "api/create-new-meeting";
        private string UpdateMeetingLink = "api/update-meeting";
        private string MeetingHistoryLink = "api/get-meeting-history";
        private string AcceptMeetingLink = "api/accept-meeting";
        private string RejectMeetingLink = "api/reject-meeting";
        private string CancelMeetingLink = "api/cancel-meeting";
        private string ViewProfileLink = "api/view-profile";
        private string UpdateProfileLink = "api/update-profile";
        private string JoinMeetingLink = "api/join-meeting-bb";
        private string SearchUserLink = "api/search-user";

        private async Task<T> getDataAsync<T>(string path, List<KeyValuePair<string, string>> request = null, Stream fileStream = null, string picturePropertyName = "userfile", bool sessionRequired = true, string method = null, bool largeFileRequest = false) where T : class
        {
            using (var api = new APIService())
            {
                return await api.RequestAsync<T>(path, request, fileStream, picturePropertyName, sessionRequired, method,largeFileRequest);
            }
        }

        public  async Task<TimezoneResponse> GetTimezones()
        {
            return await getDataAsync<TimezoneResponse>(path: TimezonePath, largeFileRequest: false);
        }

        public async Task<StringResponse> Register(List<KeyValuePair<string, string>> request)
        {
            return await getDataAsync<StringResponse>(path: RegisterPath,request:request);
        }

        public async Task<StringResponse> Login(List<KeyValuePair<string, string>> request)
        {
            return await getDataAsync<StringResponse>(path: LoginPath, request: request);
        }

        public async Task<StringResponse> ResetPassword(List<KeyValuePair<string, string>> request)
        {
            return await getDataAsync<StringResponse>(path: ResetPasswordPath, request: request);
        }

        public async Task<StringResponse> SendVerifyLink(List<KeyValuePair<string, string>> request)
        {
            return await getDataAsync<StringResponse>(path: SendVerifyLinkPath, request: request);
        }

        public async Task<MeetingResponse> GetMeetings(List<KeyValuePair<string, string>> request)
        {
            return await getDataAsync<MeetingResponse>(path: MeetingsLink, request: request);
        }

        public async Task<StringResponse> CreateMeeting(List<KeyValuePair<string, string>> request)
        {
            return await getDataAsync<StringResponse>(path: CreateMeetingLink, request: request);
        }
        public async Task<StringResponse> UpdateMeeting(List<KeyValuePair<string, string>> request)
        {
            return await getDataAsync<StringResponse>(path: UpdateMeetingLink, request: request);
        }
        
        public async Task<MeetingHistoryResponse> GetMeetingHistory(List<KeyValuePair<string, string>> request)
        {
            return await getDataAsync<MeetingHistoryResponse>(path: MeetingHistoryLink, request: request);
        }

        public async Task<StringResponse> AcceptMeetingInvitation(List<KeyValuePair<string, string>> request)
        {
            return await getDataAsync<StringResponse>(path: AcceptMeetingLink, request: request);
        }

        public async Task<StringResponse> RejectMeetingInvitation(List<KeyValuePair<string, string>> request)
        {
            return await getDataAsync<StringResponse>(path: RejectMeetingLink, request: request);
        }

        public async Task<StringResponse> CancelMeeting(List<KeyValuePair<string, string>> request)
        {
            return await getDataAsync<StringResponse>(path: CancelMeetingLink, request: request);
        }

        public async Task<UserProfileResponse> ViewUserProfile(List<KeyValuePair<string, string>> request)
        {
            return await getDataAsync<UserProfileResponse>(path: ViewProfileLink, request: request);
        }

        public async Task<StringResponse> UpdateUserProfile(List<KeyValuePair<string, string>> request, Stream fileStream)
        {
            return await getDataAsync<StringResponse>(path: UpdateProfileLink, request: request, fileStream: fileStream);
        }

        public async Task<StringResponse> JoinMeeting(List<KeyValuePair<string, string>> request)
        {
            return await getDataAsync<StringResponse>(path: JoinMeetingLink, request: request);
        }
        public async Task<SearchUser> SearchUser(List<KeyValuePair<string, string>> request)
        {
            return await getDataAsync<SearchUser>(path: SearchUserLink, request: request);
        }
    }
}

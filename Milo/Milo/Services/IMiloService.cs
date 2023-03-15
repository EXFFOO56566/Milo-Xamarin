using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Milo.Model;

namespace Milo.Services
{
    public interface IMiloService
    {
        Task<TimezoneResponse> GetTimezones();

        Task<StringResponse> Register(List<KeyValuePair<string, string>> request);

        Task<StringResponse> Login(List<KeyValuePair<string, string>> request);

        Task<StringResponse> ResetPassword(List<KeyValuePair<string, string>> request);

        Task<StringResponse> SendVerifyLink(List<KeyValuePair<string, string>> request);

        Task<MeetingResponse> GetMeetings(List<KeyValuePair<string, string>> request);

        Task<StringResponse> CreateMeeting(List<KeyValuePair<string, string>> request);
        
        Task<StringResponse> UpdateMeeting(List<KeyValuePair<string, string>> request);

        Task<MeetingHistoryResponse> GetMeetingHistory(List<KeyValuePair<string, string>> request);

        Task<StringResponse> AcceptMeetingInvitation(List<KeyValuePair<string, string>> request);

        Task<StringResponse> RejectMeetingInvitation(List<KeyValuePair<string, string>> request);

        Task<StringResponse> CancelMeeting(List<KeyValuePair<string, string>> request);

        Task<UserProfileResponse> ViewUserProfile(List<KeyValuePair<string, string>> request);

        Task<StringResponse> UpdateUserProfile(List<KeyValuePair<string, string>> request, Stream fileStream);

        Task<StringResponse> JoinMeeting(List<KeyValuePair<string, string>> request);

        Task<SearchUser> SearchUser(List<KeyValuePair<string, string>> request);

    }
}

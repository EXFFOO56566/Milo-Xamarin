using System;
namespace Milo.Helper
{
    public class AppSettings
    {
#if DEBUG
        public const string BaseUrl = "https://admin.joinme.co.in/";
#elif RELEASE
        public const string BaseUrl = "https://admin.joinme.co.in/";
#endif
    }
}

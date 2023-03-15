using System.Globalization;
using System.Threading;

[assembly: Xamarin.Forms.Dependency(typeof(Milo.Helper.Localize))]
namespace Milo.Helper
{
    public class Localize : ILocalize
    {
        public CultureInfo GetCurrentCultureInfo()
        {
            return new System.Globalization.CultureInfo("en");
        }

        public void SetLocale(CultureInfo ci)
        {
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }
    }
}

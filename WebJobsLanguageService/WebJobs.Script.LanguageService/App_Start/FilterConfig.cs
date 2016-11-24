using System.Web;
using System.Web.Mvc;

namespace WebJobs.Script.LanguageService
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}

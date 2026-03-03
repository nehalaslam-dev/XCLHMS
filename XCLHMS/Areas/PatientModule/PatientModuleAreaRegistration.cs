using System.Web.Mvc;

namespace XCLHMS.Areas.PatientModule
{
    public class PatientModuleAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PatientModule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PatientModule_default",
                "PatientModule/{controller}/{action}/{id}",
                new { controller = "Pateint", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
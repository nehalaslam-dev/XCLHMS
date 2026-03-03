using System.Web.Mvc;

namespace XCLHMS.Areas.MLCModule
{
    public class MLCModuleAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "MLCModule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "MLCModule_default",
                "MLCModule/{controller}/{action}/{id}",
                new { action = "MLC", id = UrlParameter.Optional }
            );
        }
    }
}
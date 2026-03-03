using System.Web.Mvc;

namespace XCLHMS.Areas.EmployeeModule
{
    public class EmployeeModuleAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "EmployeeModule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "EmployeeModule_default",
                "EmployeeModule/{controller}/{action}/{id}",
                new { controller = "Employee", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}